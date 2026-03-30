using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Trading212Mcp.Configuration;

namespace Trading212Mcp.Hosting;

internal sealed class StartupDiagnosticsHostedService(
    IOptions<Trading212Options> options,
    ILogger<StartupDiagnosticsHostedService> logger) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var apiKeyConfigured = !string.IsNullOrWhiteSpace(options.Value.ApiKey);
        var apiSecretConfigured = !string.IsNullOrWhiteSpace(options.Value.ApiSecret);
        var isLiveEnvironment = options.Value.BaseUrl.Contains("live.trading212.com", StringComparison.OrdinalIgnoreCase);

        if (!apiKeyConfigured)
        {
            logger.LogWarning(
                "Trading 212 API key is not configured. Set the {EnvironmentVariable} environment variable.",
                Trading212Options.ApiKeyEnvironmentVariable);
        }

        if (!apiSecretConfigured)
        {
            logger.LogWarning(
                "Trading 212 API secret is not configured. Set the {EnvironmentVariable} environment variable.",
                Trading212Options.ApiSecretEnvironmentVariable);
        }

        if (apiKeyConfigured && apiSecretConfigured)
        {
            logger.LogInformation(
                "Trading 212 MCP server configured for {Environment} environment ({BaseUrl}).",
                isLiveEnvironment ? "LIVE" : "DEMO",
                options.Value.BaseUrl);
        }
        else
        {
            logger.LogInformation(
                "Trading 212 MCP server started in configuration mode. API credentials are required for full functionality.");
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}