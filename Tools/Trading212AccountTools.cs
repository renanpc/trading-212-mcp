using System.ComponentModel;
using ModelContextProtocol.Server;
using Trading212Mcp.Clients;
using Trading212Mcp.Models.Account;
using Trading212Mcp.Models.Api;

namespace Trading212Mcp.Tools;

internal sealed class Trading212AccountTools(Trading212ApiClient apiClient)
{
    [McpServerTool(Name = "get_account_summary")]
    [Description("Get the account summary including cash balance, investments, and total account value.")]
    public async Task<AccountSummary> GetAccountSummary()
    {
        try
        {
            return await apiClient.GetAccountSummaryAsync();
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }

    [McpServerTool(Name = "get_positions")]
    [Description("Fetch all open positions for the account, optionally filtered by ticker.")]
    public async Task<IReadOnlyList<Position>> GetPositions(
        [Description("Optional ticker to filter positions (e.g., AAPL_US_EQ).")] string? ticker = null)
    {
        try
        {
            return await apiClient.GetPositionsAsync(ticker);
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }
}