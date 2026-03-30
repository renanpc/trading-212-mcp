using System.ComponentModel;
using ModelContextProtocol.Server;
using Trading212Mcp.Clients;
using Trading212Mcp.Models.Api;
using Trading212Mcp.Models.History;

namespace Trading212Mcp.Tools;

internal sealed class Trading212HistoryTools(Trading212ApiClient apiClient)
{
    private const int MaxLimit = 50;

    [McpServerTool(Name = "get_historical_orders")]
    [Description("Get historical orders with pagination. Use nextPagePath to fetch subsequent pages.")]
    public async Task<PaginatedHistoricalOrders> GetHistoricalOrders(
        [Description("Pagination cursor (order ID).")] long? cursor = null,
        [Description("Filter by ticker (e.g., AAPL_US_EQ).")] string? ticker = null,
        [Description("Number of items per page (max 50).")] int limit = 20)
    {
        try
        {
            ValidateLimit(limit);

            return await apiClient.GetHistoricalOrdersAsync(cursor, ticker, limit);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }

    [McpServerTool(Name = "get_dividends")]
    [Description("Get dividend history with pagination.")]
    public async Task<PaginatedDividends> GetDividends(
        [Description("Pagination cursor (timestamp).")] long? cursor = null,
        [Description("Filter by ticker (e.g., AAPL_US_EQ).")] string? ticker = null,
        [Description("Number of items per page (max 50).")] int limit = 20)
    {
        try
        {
            ValidateLimit(limit);

            return await apiClient.GetDividendsAsync(cursor, ticker, limit);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }

    [McpServerTool(Name = "get_transactions")]
    [Description("Get transaction history (deposits, withdrawals, fees, transfers).")]
    public async Task<PaginatedTransactions> GetTransactions(
        [Description("Pagination cursor.")] string? cursor = null,
        [Description("Filter by start time.")] DateTimeOffset? time = null,
        [Description("Number of items per page (max 50).")] int limit = 20)
    {
        try
        {
            ValidateLimit(limit);

            return await apiClient.GetTransactionsAsync(cursor, time, limit);
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }

    [McpServerTool(Name = "get_reports")]
    [Description("Get previously requested CSV reports and their status.")]
    public async Task<IReadOnlyList<ReportResponse>> GetReports()
    {
        try
        {
            return await apiClient.GetReportsAsync();
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }

    [McpServerTool(Name = "request_report")]
    [Description("Request a new CSV report with specified date range and data types.")]
    public async Task<EnqueuedReportResponse> RequestReport(
        [Description("Start of the reporting period.")] DateTimeOffset timeFrom,
        [Description("End of the reporting period.")] DateTimeOffset timeTo,
        [Description("Include historical orders in the report.")] bool includeOrders = true,
        [Description("Include dividends in the report.")] bool includeDividends = true,
        [Description("Include transactions in the report.")] bool includeTransactions = true,
        [Description("Include interest in the report.")] bool includeInterest = false)
    {
        try
        {
            if (timeTo <= timeFrom)
            {
                throw new ArgumentException("timeTo must be after timeFrom.");
            }

            return await apiClient.RequestReportAsync(
                new PublicReportRequest(timeFrom, timeTo, new ReportDataIncluded(includeOrders, includeDividends, includeTransactions, includeInterest)));
        }
        catch (ArgumentException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }

    private static void ValidateLimit(int limit)
    {
        if (limit is < 1 or > MaxLimit)
        {
            throw new ArgumentException($"limit must be between 1 and {MaxLimit}.");
        }
    }
}