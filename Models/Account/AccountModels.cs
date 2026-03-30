using System.Text.Json.Serialization;

namespace Trading212Mcp.Models.Account;

public sealed record AccountSummary(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("cash")] Cash Cash,
    [property: JsonPropertyName("investments")] Investments Investments,
    [property: JsonPropertyName("totalValue")] decimal TotalValue);

public sealed record Cash(
    [property: JsonPropertyName("availableToTrade")] decimal AvailableToTrade,
    [property: JsonPropertyName("inPies")] decimal InPies,
    [property: JsonPropertyName("reservedForOrders")] decimal ReservedForOrders);

public sealed record Investments(
    [property: JsonPropertyName("currentValue")] decimal CurrentValue,
    [property: JsonPropertyName("totalCost")] decimal TotalCost,
    [property: JsonPropertyName("realizedProfitLoss")] decimal RealizedProfitLoss,
    [property: JsonPropertyName("unrealizedProfitLoss")] decimal UnrealizedProfitLoss);

public sealed record Position(
    [property: JsonPropertyName("instrument")] Instrument Instrument,
    [property: JsonPropertyName("quantity")] decimal Quantity,
    [property: JsonPropertyName("quantityAvailableForTrading")] decimal QuantityAvailableForTrading,
    [property: JsonPropertyName("quantityInPies")] decimal QuantityInPies,
    [property: JsonPropertyName("averagePricePaid")] decimal AveragePricePaid,
    [property: JsonPropertyName("currentPrice")] decimal CurrentPrice,
    [property: JsonPropertyName("createdAt")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("walletImpact")] PositionWalletImpact WalletImpact);

public sealed record Instrument(
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("isin")] string? Isin = null);

public sealed record PositionWalletImpact(
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("currentValue")] decimal CurrentValue,
    [property: JsonPropertyName("totalCost")] decimal TotalCost,
    [property: JsonPropertyName("unrealizedProfitLoss")] decimal UnrealizedProfitLoss,
    [property: JsonPropertyName("fxImpact")] decimal FxImpact);