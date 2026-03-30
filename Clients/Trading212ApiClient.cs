using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Trading212Mcp.Configuration;
using Trading212Mcp.Models.Account;
using Trading212Mcp.Models.Api;
using Trading212Mcp.Models.History;
using Trading212Mcp.Models.Instruments;
using Trading212Mcp.Models.Orders;

namespace Trading212Mcp.Clients;

internal sealed class Trading212ApiClient(
    HttpClient httpClient,
    IOptions<Trading212Options> options,
    ILogger<Trading212ApiClient> logger)
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly SemaphoreSlim FailureLogLock = new(1, 1);

    public async Task<AccountSummary> GetAccountSummaryAsync(CancellationToken cancellationToken = default) =>
        await SendAsync<AccountSummary>(
            HttpMethod.Get,
            "api/v0/equity/account/summary",
            body: null,
            cancellationToken);

    public async Task<IReadOnlyList<Position>> GetPositionsAsync(string? ticker = null, CancellationToken cancellationToken = default)
    {
        var path = string.IsNullOrWhiteSpace(ticker)
            ? "api/v0/equity/positions"
            : $"api/v0/equity/positions?ticker={Uri.EscapeDataString(ticker)}";

        return await SendAsync<IReadOnlyList<Position>>(
            HttpMethod.Get,
            path,
            body: null,
            cancellationToken) ?? [];
    }

    public async Task<IReadOnlyList<Order>> GetOrdersAsync(CancellationToken cancellationToken = default) =>
        await SendAsync<IReadOnlyList<Order>>(
            HttpMethod.Get,
            "api/v0/equity/orders",
            body: null,
            cancellationToken) ?? [];

    public async Task<Order?> GetOrderByIdAsync(long orderId, CancellationToken cancellationToken = default) =>
        await SendAsync<Order?>(
            HttpMethod.Get,
            $"api/v0/equity/orders/{orderId}",
            body: null,
            cancellationToken);

    public async Task<Order> PlaceMarketOrderAsync(MarketRequest request, CancellationToken cancellationToken = default) =>
        await SendAsync<Order>(
            HttpMethod.Post,
            "api/v0/equity/orders/market",
            request,
            cancellationToken);

    public async Task<Order> PlaceLimitOrderAsync(LimitRequest request, CancellationToken cancellationToken = default) =>
        await SendAsync<Order>(
            HttpMethod.Post,
            "api/v0/equity/orders/limit",
            request,
            cancellationToken);

    public async Task<Order> PlaceStopOrderAsync(StopRequest request, CancellationToken cancellationToken = default) =>
        await SendAsync<Order>(
            HttpMethod.Post,
            "api/v0/equity/orders/stop",
            request,
            cancellationToken);

    public async Task<Order> PlaceStopLimitOrderAsync(StopLimitRequest request, CancellationToken cancellationToken = default) =>
        await SendAsync<Order>(
            HttpMethod.Post,
            "api/v0/equity/orders/stop_limit",
            request,
            cancellationToken);

    public async Task CancelOrderAsync(long orderId, CancellationToken cancellationToken = default) =>
        await SendAsync<object>(
            HttpMethod.Delete,
            $"api/v0/equity/orders/{orderId}",
            body: null,
            cancellationToken);

    public async Task<PaginatedHistoricalOrders> GetHistoricalOrdersAsync(
        long? cursor = null,
        string? ticker = null,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string> { $"limit={limit}" };
        if (cursor.HasValue)
        {
            queryParams.Add($"cursor={cursor.Value}");
        }

        if (!string.IsNullOrWhiteSpace(ticker))
        {
            queryParams.Add($"ticker={Uri.EscapeDataString(ticker)}");
        }

        var path = $"api/v0/equity/history/orders?{string.Join("&", queryParams)}";
        return await SendAsync<PaginatedHistoricalOrders>(
            HttpMethod.Get,
            path,
            body: null,
            cancellationToken);
    }

    public async Task<PaginatedDividends> GetDividendsAsync(
        long? cursor = null,
        string? ticker = null,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string> { $"limit={limit}" };
        if (cursor.HasValue)
        {
            queryParams.Add($"cursor={cursor.Value}");
        }

        if (!string.IsNullOrWhiteSpace(ticker))
        {
            queryParams.Add($"ticker={Uri.EscapeDataString(ticker)}");
        }

        var path = $"api/v0/equity/history/dividends?{string.Join("&", queryParams)}";
        return await SendAsync<PaginatedDividends>(
            HttpMethod.Get,
            path,
            body: null,
            cancellationToken);
    }

    public async Task<PaginatedTransactions> GetTransactionsAsync(
        string? cursor = null,
        DateTimeOffset? time = null,
        int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string> { $"limit={limit}" };
        if (!string.IsNullOrWhiteSpace(cursor))
        {
            queryParams.Add($"cursor={Uri.EscapeDataString(cursor)}");
        }

        if (time.HasValue)
        {
            queryParams.Add($"time={Uri.EscapeDataString(time.Value.ToString("O"))}");
        }

        var path = $"api/v0/equity/history/transactions?{string.Join("&", queryParams)}";
        return await SendAsync<PaginatedTransactions>(
            HttpMethod.Get,
            path,
            body: null,
            cancellationToken);
    }

    public async Task<IReadOnlyList<ReportResponse>> GetReportsAsync(CancellationToken cancellationToken = default) =>
        await SendAsync<IReadOnlyList<ReportResponse>>(
            HttpMethod.Get,
            "api/v0/equity/history/exports",
            body: null,
            cancellationToken) ?? [];

    public async Task<EnqueuedReportResponse> RequestReportAsync(PublicReportRequest request, CancellationToken cancellationToken = default) =>
        await SendAsync<EnqueuedReportResponse>(
            HttpMethod.Post,
            "api/v0/equity/history/exports",
            request,
            cancellationToken);

    public async Task<IReadOnlyList<TradableInstrument>> GetInstrumentsAsync(CancellationToken cancellationToken = default) =>
        await SendAsync<IReadOnlyList<TradableInstrument>>(
            HttpMethod.Get,
            "api/v0/equity/metadata/instruments",
            body: null,
            cancellationToken) ?? [];

    public async Task<IReadOnlyList<Exchange>> GetExchangesAsync(CancellationToken cancellationToken = default) =>
        await SendAsync<IReadOnlyList<Exchange>>(
            HttpMethod.Get,
            "api/v0/equity/metadata/exchanges",
            body: null,
            cancellationToken) ?? [];

    private async Task<TResponse> SendAsync<TResponse>(
        HttpMethod method,
        string relativePath,
        object? body,
        CancellationToken cancellationToken)
    {
        EnsureCredentialsConfigured();

        var requestBody = body is null ? null : JsonSerializer.Serialize(body, JsonSerializerOptions);

        using var request = new HttpRequestMessage(method, relativePath);

        var credentials = $"{options.Value.ApiKey}:{options.Value.ApiSecret}";
        var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        request.Headers.Authorization = new("Basic", encodedCredentials);

        if (requestBody is not null)
        {
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        }

        using var response = await httpClient.SendAsync(request, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var requestUri = request.RequestUri ?? new Uri(httpClient.BaseAddress!, relativePath);
            await TryWriteFailureLogAsync(
                requestUri,
                method,
                request,
                requestBody,
                response,
                responseBody,
                cancellationToken);

            logger.LogWarning(
                "Trading 212 API returned {StatusCode} for {Method} {Path}. Failure details were appended to {LogPath}. Response body: {Body}",
                (int)response.StatusCode,
                method,
                relativePath,
                options.Value.FailureLogPath,
                responseBody);

            throw Trading212ApiException.Create(response.StatusCode, responseBody);
        }

        if (typeof(TResponse) == typeof(object))
        {
            return default!;
        }

        var result = JsonSerializer.Deserialize<TResponse>(responseBody, JsonSerializerOptions);
        if (result is null && typeof(TResponse) != typeof(string))
        {
            throw new InvalidOperationException("Trading 212 API returned an empty or invalid JSON response.");
        }

        return result!;
    }

    private void EnsureCredentialsConfigured()
    {
        if (string.IsNullOrWhiteSpace(options.Value.ApiKey))
        {
            throw new InvalidOperationException(
                $"Set the {Trading212Options.ApiKeyEnvironmentVariable} environment variable before calling Trading 212 tools.");
        }

        if (string.IsNullOrWhiteSpace(options.Value.ApiSecret))
        {
            throw new InvalidOperationException(
                $"Set the {Trading212Options.ApiSecretEnvironmentVariable} environment variable before calling Trading 212 tools.");
        }
    }

    private async Task TryWriteFailureLogAsync(
        Uri requestUri,
        HttpMethod method,
        HttpRequestMessage request,
        string? requestBody,
        HttpResponseMessage response,
        string responseBody,
        CancellationToken cancellationToken)
    {
        try
        {
            await AppendFailureLogAsync(
                requestUri,
                method,
                request,
                requestBody,
                response,
                responseBody,
                cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                exception,
                "Failed to append Trading 212 failure details to {LogPath}.",
                options.Value.FailureLogPath);
        }
    }

    private async Task AppendFailureLogAsync(
        Uri requestUri,
        HttpMethod method,
        HttpRequestMessage request,
        string? requestBody,
        HttpResponseMessage response,
        string responseBody,
        CancellationToken cancellationToken)
    {
        var logPath = options.Value.FailureLogPath;
        var directory = Path.GetDirectoryName(logPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var entry = BuildFailureLogEntry(
            requestUri,
            method,
            request,
            requestBody,
            response,
            responseBody);

        await FailureLogLock.WaitAsync(cancellationToken);
        try
        {
            await File.AppendAllTextAsync(logPath, entry, cancellationToken);
        }
        finally
        {
            FailureLogLock.Release();
        }
    }

    private static string BuildFailureLogEntry(
        Uri requestUri,
        HttpMethod method,
        HttpRequestMessage request,
        string? requestBody,
        HttpResponseMessage response,
        string responseBody)
    {
        var builder = new StringBuilder();
        builder.AppendLine(new string('=', 100));
        builder.Append("Timestamp (UTC): ").AppendLine(DateTimeOffset.UtcNow.ToString("O"));
        builder.Append("Request: ").Append(method).Append(' ').AppendLine(requestUri.ToString());
        builder.Append("Response Status: ").Append((int)response.StatusCode).Append(' ').AppendLine(response.StatusCode.ToString());
        builder.AppendLine("Request Headers:");
        AppendHeaders(builder, request.Headers, redactAuthorization: true);

        if (request.Content?.Headers is { } requestContentHeaders)
        {
            AppendHeaders(builder, requestContentHeaders, redactAuthorization: false);
        }

        builder.AppendLine("Request JSON Body:");
        builder.AppendLine(string.IsNullOrWhiteSpace(requestBody) ? "(none)" : requestBody);
        builder.AppendLine("Response Headers:");
        AppendHeaders(builder, response.Headers, redactAuthorization: false);

        if (response.Content?.Headers is { } responseContentHeaders)
        {
            AppendHeaders(builder, responseContentHeaders, redactAuthorization: false);
        }

        builder.AppendLine("Response Body:");
        builder.AppendLine(string.IsNullOrWhiteSpace(responseBody) ? "(empty)" : responseBody);
        builder.AppendLine();

        return builder.ToString();
    }

    private static void AppendHeaders(
        StringBuilder builder,
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers,
        bool redactAuthorization)
    {
        foreach (var header in headers)
        {
            var value = string.Join(", ", header.Value);
            if (redactAuthorization && header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
            {
                value = "Basic [redacted]";
            }

            builder.Append("  ").Append(header.Key).Append(": ").AppendLine(value);
        }
    }
}