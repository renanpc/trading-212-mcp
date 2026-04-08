using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Trading212Mcp.Clients;
using Trading212Mcp.Configuration;
using Trading212Mcp.Hosting;
using Trading212Mcp.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService();

if (!HasConfiguredUrls(builder.Configuration))
{
    var httpPort = ResolvePort(builder.Configuration);
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(httpPort);
    });
}

builder.Services
    .AddOptions<Trading212Options>()
    .Configure<IConfiguration>(
        (options, configuration) =>
        {
            options.ApiKey = configuration[Trading212Options.ApiKeyEnvironmentVariable];
            options.ApiSecret = configuration[Trading212Options.ApiSecretEnvironmentVariable];
            options.BaseUrl = configuration[Trading212Options.BaseUrlEnvironmentVariable]
                ?? Trading212Options.DefaultDemoBaseUrl;
            options.FailureLogPath = Trading212Options.ResolveFailureLogPath(
                configuration[Trading212Options.FailureLogPathEnvironmentVariable]);
        });

builder.Services.AddHttpClient<Trading212ApiClient>(
    (serviceProvider, httpClient) =>
    {
        var options = serviceProvider.GetRequiredService<IOptions<Trading212Options>>().Value;
        httpClient.BaseAddress = new Uri(Trading212Options.NormalizeBaseUrl(options.BaseUrl));
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("trading212-mcp/0.1.0");
    });

builder.Services.AddHostedService<StartupDiagnosticsHostedService>();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithTools<Trading212AccountTools>()
    .WithTools<Trading212OrderTools>()
    .WithTools<Trading212HistoryTools>()
    .WithTools<Trading212InstrumentTools>();

var app = builder.Build();

app.MapMcp();

app.Run();

static bool HasConfiguredUrls(IConfiguration configuration) =>
    !string.IsNullOrWhiteSpace(configuration["urls"]) ||
    !string.IsNullOrWhiteSpace(configuration["URLS"]) ||
    !string.IsNullOrWhiteSpace(configuration["ASPNETCORE_URLS"]);

static int ResolvePort(IConfiguration configuration)
{
    foreach (var key in new[] { "HTTP_PORT", "PORT", "port" })
    {
        if (int.TryParse(configuration[key], out var port) && port > 0 && port <= 65535)
        {
            return port;
        }
    }

    return Trading212Options.DefaultPort;
}
