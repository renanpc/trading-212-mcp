using System.Text.Json.Serialization;

namespace Trading212Mcp.Models.History;

public sealed record PaginatedHistoricalOrders(
    [property: JsonPropertyName("items")] IReadOnlyList<HistoricalOrderRecord> Items,
    [property: JsonPropertyName("nextPagePath")] string? NextPagePath);

public sealed record HistoricalOrderRecord(
    [property: JsonPropertyName("order")] OrderRecord Order,
    [property: JsonPropertyName("fill")] Fill Fill);

public sealed record OrderRecord(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("instrument")] HistoricalInstrument? Instrument,
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

public sealed record HistoricalInstrument(
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("isin")] string? Isin = null);

public sealed record Fill(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("filledAt")] DateTimeOffset FilledAt,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("quantity")] decimal Quantity,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("tradingMethod")] string TradingMethod,
    [property: JsonPropertyName("walletImpact")] FillWalletImpact WalletImpact);

public sealed record FillWalletImpact(
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("fxRate")] decimal FxRate,
    [property: JsonPropertyName("netValue")] decimal NetValue,
    [property: JsonPropertyName("realisedProfitLoss")] decimal RealisedProfitLoss,
    [property: JsonPropertyName("taxes")] IReadOnlyList<Tax> Taxes);

public sealed record Tax(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("quantity")] decimal Quantity,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("chargedAt")] DateTimeOffset ChargedAt);

public sealed record PaginatedDividends(
    [property: JsonPropertyName("items")] IReadOnlyList<HistoryDividendItem> Items,
    [property: JsonPropertyName("nextPagePath")] string? NextPagePath);

public sealed record HistoryDividendItem(
    [property: JsonPropertyName("reference")] string Reference,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("amountInEuro")] decimal? AmountInEuro,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("tickerCurrency")] string TickerCurrency,
    [property: JsonPropertyName("quantity")] decimal Quantity,
    [property: JsonPropertyName("grossAmountPerShare")] decimal GrossAmountPerShare,
    [property: JsonPropertyName("instrument")] HistoricalInstrument Instrument,
    [property: JsonPropertyName("paidOn")] DateTimeOffset PaidOn,
    [property: JsonPropertyName("type")] string Type);

public sealed record PaginatedTransactions(
    [property: JsonPropertyName("items")] IReadOnlyList<HistoryTransactionItem> Items,
    [property: JsonPropertyName("nextPagePath")] string? NextPagePath);

public sealed record HistoryTransactionItem(
    [property: JsonPropertyName("reference")] string Reference,
    [property: JsonPropertyName("dateTime")] DateTimeOffset DateTime,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("currency")] string Currency);

public sealed record ReportResponse(
    [property: JsonPropertyName("reportId")] long ReportId,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("downloadLink")] string? DownloadLink,
    [property: JsonPropertyName("timeFrom")] DateTimeOffset? TimeFrom,
    [property: JsonPropertyName("timeTo")] DateTimeOffset? TimeTo,
    [property: JsonPropertyName("dataIncluded")] ReportDataIncluded? DataIncluded);

public sealed record ReportDataIncluded(
    [property: JsonPropertyName("includeOrders")] bool IncludeOrders,
    [property: JsonPropertyName("includeDividends")] bool IncludeDividends,
    [property: JsonPropertyName("includeTransactions")] bool IncludeTransactions,
    [property: JsonPropertyName("includeInterest")] bool IncludeInterest);

public sealed record PublicReportRequest(
    [property: JsonPropertyName("timeFrom")] DateTimeOffset TimeFrom,
    [property: JsonPropertyName("timeTo")] DateTimeOffset TimeTo,
    [property: JsonPropertyName("dataIncluded")] ReportDataIncluded DataIncluded);

public sealed record EnqueuedReportResponse(
    [property: JsonPropertyName("reportId")] long ReportId);