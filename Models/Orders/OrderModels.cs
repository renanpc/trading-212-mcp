using System.Text.Json.Serialization;

namespace Trading212Mcp.Models.Orders;

public sealed record Order(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("instrument")] Instrument? Instrument,
    [property: JsonPropertyName("side")] string Side,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("quantity")] decimal? Quantity,
    [property: JsonPropertyName("filledQuantity")] decimal? FilledQuantity,
    [property: JsonPropertyName("value")] decimal? Value,
    [property: JsonPropertyName("filledValue")] decimal? FilledValue,
    [property: JsonPropertyName("limitPrice")] decimal? LimitPrice,
    [property: JsonPropertyName("stopPrice")] decimal? StopPrice,
    [property: JsonPropertyName("timeInForce")] string? TimeInForce,
    [property: JsonPropertyName("strategy")] string? Strategy,
    [property: JsonPropertyName("extendedHours")] bool? ExtendedHours,
    [property: JsonPropertyName("createdAt")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("initiatedFrom")] string? InitiatedFrom);

public sealed record Instrument(
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("isin")] string? Isin = null);

public sealed record MarketRequest(
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("quantity")] decimal Quantity,
    [property: JsonPropertyName("extendedHours")] bool ExtendedHours = false);

public sealed record LimitRequest(
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("quantity")] decimal Quantity,
    [property: JsonPropertyName("limitPrice")] decimal LimitPrice,
    [property: JsonPropertyName("timeValidity")] string TimeValidity = "DAY");

public sealed record StopRequest(
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("quantity")] decimal Quantity,
    [property: JsonPropertyName("stopPrice")] decimal StopPrice,
    [property: JsonPropertyName("timeValidity")] string TimeValidity = "DAY");

public sealed record StopLimitRequest(
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("quantity")] decimal Quantity,
    [property: JsonPropertyName("stopPrice")] decimal StopPrice,
    [property: JsonPropertyName("limitPrice")] decimal LimitPrice,
    [property: JsonPropertyName("timeValidity")] string TimeValidity = "DAY");