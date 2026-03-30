using System.ComponentModel;
using ModelContextProtocol.Server;
using Trading212Mcp.Clients;
using Trading212Mcp.Models.Api;
using Trading212Mcp.Models.Instruments;

namespace Trading212Mcp.Tools;

internal sealed class Trading212InstrumentTools(Trading212ApiClient apiClient)
{
    [McpServerTool(Name = "get_instruments")]
    [Description("Get all available tradable instruments (stocks, ETFs, etc.). Data is refreshed every 10 minutes.")]
    public async Task<IReadOnlyList<TradableInstrument>> GetInstruments()
    {
        try
        {
            return await apiClient.GetInstrumentsAsync();
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }

    [McpServerTool(Name = "get_exchanges")]
    [Description("Get all accessible exchanges and their working schedules. Data is refreshed every 10 minutes.")]
    public async Task<IReadOnlyList<Exchange>> GetExchanges()
    {
        try
        {
            return await apiClient.GetExchangesAsync();
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }
}