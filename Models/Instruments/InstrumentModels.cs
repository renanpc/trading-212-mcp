using System.Text.Json.Serialization;

namespace Trading212Mcp.Models.Instruments;

public sealed record TradableInstrument(
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("currencyCode")] string CurrencyCode,
    [property: JsonPropertyName("shortName")] string? ShortName,
    [property: JsonPropertyName("isin")] string? Isin,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("addedOn")] DateTimeOffset? AddedOn,
    [property: JsonPropertyName("extendedHours")] bool ExtendedHours,
    [property: JsonPropertyName("maxOpenQuantity")] decimal? MaxOpenQuantity,
    [property: JsonPropertyName("workingScheduleId")] long? WorkingScheduleId);

public sealed record Exchange(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("workingSchedules")] IReadOnlyList<WorkingSchedule> WorkingSchedules);

public sealed record WorkingSchedule(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("timeEvents")] IReadOnlyList<TimeEvent> TimeEvents);

public sealed record TimeEvent(
    [property: JsonPropertyName("date")] DateTimeOffset Date,
    [property: JsonPropertyName("type")] string Type);