using System.ComponentModel;
using ModelContextProtocol.Server;
using Trading212Mcp.Clients;
using Trading212Mcp.Models.Api;
using Trading212Mcp.Models.Orders;

namespace Trading212Mcp.Tools;

internal sealed class Trading212OrderTools(Trading212ApiClient apiClient)
{
    private static readonly HashSet<string> ValidTimeValidity = new(StringComparer.OrdinalIgnoreCase)
    {
        "DAY",
        "GOOD_TILL_CANCEL"
    };

    [McpServerTool(Name = "get_orders")]
    [Description("Get all pending (active) orders that are not yet filled, cancelled, or expired.")]
    public async Task<IReadOnlyList<Order>> GetOrders()
    {
        try
        {
            return await apiClient.GetOrdersAsync();
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }

    [McpServerTool(Name = "get_order_by_id")]
    [Description("Get a single pending order by its unique ID.")]
    public async Task<Order?> GetOrderById(
        [Description("The unique identifier of the order.")] long orderId)
    {
        try
        {
            return await apiClient.GetOrderByIdAsync(orderId);
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }

    [McpServerTool(Name = "place_market_order")]
    [Description("Place a Market order. Use positive quantity for buy, negative for sell.")]
    public async Task<Order> PlaceMarketOrder(
        [Description("Ticker symbol (e.g., AAPL_US_EQ).")] string ticker,
        [Description("Quantity: positive for buy, negative for sell.")] decimal quantity,
        [Description("Allow execution outside regular trading hours.")] bool extendedHours = false)
    {
        try
        {
            ValidateTicker(ticker);
            ValidateQuantity(quantity, isSell: quantity < 0);

            return await apiClient.PlaceMarketOrderAsync(
                new MarketRequest(ticker, quantity, extendedHours));
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

    [McpServerTool(Name = "place_limit_order")]
    [Description("Place a Limit order that executes at a specified price or better.")]
    public async Task<Order> PlaceLimitOrder(
        [Description("Ticker symbol (e.g., AAPL_US_EQ).")] string ticker,
        [Description("Quantity: positive for buy, negative for sell.")] decimal quantity,
        [Description("The limit price for the order.")] decimal limitPrice,
        [Description("Time validity: DAY or GOOD_TILL_CANCEL.")] string timeValidity = "DAY")
    {
        try
        {
            ValidateTicker(ticker);
            ValidateQuantity(quantity, isSell: quantity < 0);
            ValidateTimeValidity(timeValidity);

            return await apiClient.PlaceLimitOrderAsync(
                new LimitRequest(ticker, quantity, limitPrice, timeValidity));
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

    [McpServerTool(Name = "place_stop_order")]
    [Description("Place a Stop order that triggers a Market order when the stop price is reached.")]
    public async Task<Order> PlaceStopOrder(
        [Description("Ticker symbol (e.g., AAPL_US_EQ).")] string ticker,
        [Description("Quantity: positive for buy, negative for sell.")] decimal quantity,
        [Description("The trigger price (based on Last Traded Price).")] decimal stopPrice,
        [Description("Time validity: DAY or GOOD_TILL_CANCEL.")] string timeValidity = "DAY")
    {
        try
        {
            ValidateTicker(ticker);
            ValidateQuantity(quantity, isSell: quantity < 0);
            ValidateTimeValidity(timeValidity);

            return await apiClient.PlaceStopOrderAsync(
                new StopRequest(ticker, quantity, stopPrice, timeValidity));
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

    [McpServerTool(Name = "place_stop_limit_order")]
    [Description("Place a Stop-Limit order that triggers a Limit order when the stop price is reached.")]
    public async Task<Order> PlaceStopLimitOrder(
        [Description("Ticker symbol (e.g., AAPL_US_EQ).")] string ticker,
        [Description("Quantity: positive for buy, negative for sell.")] decimal quantity,
        [Description("The trigger price (based on Last Traded Price).")] decimal stopPrice,
        [Description("The limit price for the triggered order.")] decimal limitPrice,
        [Description("Time validity: DAY or GOOD_TILL_CANCEL.")] string timeValidity = "DAY")
    {
        try
        {
            ValidateTicker(ticker);
            ValidateQuantity(quantity, isSell: quantity < 0);
            ValidateTimeValidity(timeValidity);

            return await apiClient.PlaceStopLimitOrderAsync(
                new StopLimitRequest(ticker, quantity, stopPrice, limitPrice, timeValidity));
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

    [McpServerTool(Name = "cancel_order")]
    [Description("Cancel a pending order by its ID. Cancellation is not guaranteed if the order is already being filled.")]
    public async Task CancelOrder(
        [Description("The unique identifier of the order to cancel.")] long orderId)
    {
        try
        {
            await apiClient.CancelOrderAsync(orderId);
        }
        catch (Trading212ApiException exception)
        {
            throw new InvalidOperationException(exception.Message, exception);
        }
    }

    private static void ValidateTicker(string ticker)
    {
        if (string.IsNullOrWhiteSpace(ticker))
        {
            throw new ArgumentException("ticker is required.");
        }
    }

    private static void ValidateQuantity(decimal quantity, bool isSell)
    {
        if (quantity == 0)
        {
            throw new ArgumentException("quantity cannot be zero.");
        }

        if (isSell && quantity > 0)
        {
            throw new ArgumentException("To place a sell order, use a negative quantity value.");
        }
    }

    private static void ValidateTimeValidity(string timeValidity)
    {
        if (!ValidTimeValidity.Contains(timeValidity))
        {
            throw new ArgumentException("timeValidity must be DAY or GOOD_TILL_CANCEL.");
        }
    }
}