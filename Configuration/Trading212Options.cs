namespace Trading212Mcp.Configuration;

internal sealed class Trading212Options
{
    public const string ApiKeyEnvironmentVariable = "TRADING212_API_KEY";
    public const string ApiSecretEnvironmentVariable = "TRADING212_API_SECRET";
    public const string BaseUrlEnvironmentVariable = "TRADING212_BASE_URL";
    public const string FailureLogPathEnvironmentVariable = "TRADING212_FAILURE_LOG_PATH";
    public const string DefaultDemoBaseUrl = "https://demo.trading212.com";
    public const string DefaultLiveBaseUrl = "https://live.trading212.com";
    public const string DefaultFailureLogPath = "logs/trading212-api-failures.log";

    public string? ApiKey { get; set; }

    public string? ApiSecret { get; set; }

    public string BaseUrl { get; set; } = DefaultDemoBaseUrl;

    public string FailureLogPath { get; set; } = DefaultFailureLogPath;

    public static string NormalizeBaseUrl(string? baseUrl)
    {
        var value = string.IsNullOrWhiteSpace(baseUrl) ? DefaultDemoBaseUrl : baseUrl.Trim();
        return value.EndsWith('/') ? value : $"{value}/";
    }

    public static string ResolveFailureLogPath(string? failureLogPath)
    {
        var value = string.IsNullOrWhiteSpace(failureLogPath)
            ? DefaultFailureLogPath
            : failureLogPath.Trim();

        return Path.IsPathRooted(value)
            ? value
            : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, value));
    }
}