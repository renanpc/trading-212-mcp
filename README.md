# Trading 212 MCP Server

A remote MCP (Model Context Protocol) server for Trading 212, providing access to account summary, positions, orders, order history, dividends, transactions, instruments, and exchanges.

## Features

- **Account Management**: Get account summary with cash balance and investment metrics
- **Positions**: Fetch all open positions, filtered by ticker
- **Orders**: Place and manage market, limit, stop, and stop-limit orders
- **Order History**: Query historical orders with cursor-based pagination
- **Dividends**: Retrieve dividend history
- **Transactions**: View transaction history (deposits, withdrawals, fees)
- **Reports**: Request and download CSV reports
- **Instruments**: Browse all available tradable instruments
- **Exchanges**: View exchange working schedules

## Prerequisites

- .NET 10.0 SDK or later
- Trading 212 API credentials (API Key and API Secret)

## Getting API Credentials

1. Open the Trading 212 app
2. Go to Settings > API
3. Generate your API key and secret

For detailed instructions, visit the [Trading 212 Help Centre](https://helpcentre.trading212.com/hc/en-us/articles/14584770928157-Trading-212-API-key)

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `TRADING212_API_KEY` | Your Trading 212 API key | (required) |
| `TRADING212_API_SECRET` | Your Trading 212 API secret | (required) |
| `TRADING212_BASE_URL` | API endpoint | `https://demo.trading212.com` |
| `TRADING212_FAILURE_LOG_PATH` | Path for failure logs | `logs/trading212-api-failures.log` |
| `HTTP_PORT` | HTTP listen port | `8080` |

### Switching Between Demo and Live

- **Demo (Paper Trading)**: `TRADING212_BASE_URL=https://demo.trading212.com`
- **Live (Real Money)**: `TRADING212_BASE_URL=https://live.trading212.com`

## Disclaimer

This project is provided as a developer tool for integrating Trading 212 with MCP-compatible clients. It is not investment advice, not financial advice, and not a recommendation to buy, sell, or hold any instrument.

You are solely responsible for reviewing any actions taken through this server, including order placement, order cancellation, and any other account-affecting operations. Use of the live Trading 212 environment is entirely at your own risk.

Before using this project with a real account:

- test against the demo environment first
- verify all inputs and outputs independently
- review every order before execution
- understand the Trading 212 API limitations, rate limits, and account restrictions

By using this software, you acknowledge that the authors and contributors are not responsible for trading losses, missed opportunities, account restrictions, service interruptions, or other damages arising from its use, misuse, or unavailability. See [DISCLAIMER.md](./DISCLAIMER.md) for the full repository disclaimer.

## Running the Server

### Build and Run

```bash
dotnet build
dotnet run
```

### Build for Deployment

```bash
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

## Docker

Build locally:

```bash
docker build -t renanpcf/trading212-mcp:local .
```

Use the latest public image from Docker Hub:

```bash
docker pull renanpcf/trading212-mcp:latest
docker run --rm -p 8080:8080 \
  -e TRADING212_API_KEY=your-api-key \
  -e TRADING212_API_SECRET=your-api-secret \
  -e TRADING212_BASE_URL=https://demo.trading212.com \
  -e HTTP_PORT=8080 \
  renanpcf/trading212-mcp:latest
```

Run the container:

```bash
docker run --rm -p 8080:8080 \
  -e TRADING212_API_KEY=your-api-key \
  -e TRADING212_API_SECRET=your-api-secret \
  -e TRADING212_BASE_URL=https://demo.trading212.com \
  -e HTTP_PORT=8080 \
  renanpcf/trading212-mcp:local
```

Container environment variables:

- `TRADING212_API_KEY` is required.
- `TRADING212_API_SECRET` is required.
- `TRADING212_BASE_URL` is optional and defaults to `https://demo.trading212.com`.
- `TRADING212_FAILURE_LOG_PATH` is optional.
- `HTTP_PORT` is optional and defaults to `8080`.

## Available Tools

### Account Tools

- `get_account_summary` - Get account summary (cash, investments, total value)
- `get_positions` - Fetch open positions, optionally filtered by ticker

### Order Tools

- `get_orders` - Get all pending orders
- `get_order_by_id` - Get a specific pending order by ID
- `place_market_order` - Place a market order (positive qty = buy, negative = sell)
- `place_limit_order` - Place a limit order
- `place_stop_order` - Place a stop order
- `place_stop_limit_order` - Place a stop-limit order
- `cancel_order` - Cancel a pending order

### History Tools

- `get_historical_orders` - Get historical orders with pagination
- `get_dividends` - Get dividend history
- `get_transactions` - Get transaction history
- `get_reports` - List generated reports
- `request_report` - Request a new CSV report

### Instrument Tools

- `get_instruments` - Get all available instruments
- `get_exchanges` - Get exchange working schedules

## API Rate Limits

- Account summary: 1 req / 5s
- Positions: 1 req / 1s
- Orders (read): 1 req / 5s
- Orders (write): 1 req / 2s
- Order cancel: 50 req / 1m
- Market orders: 50 req / 1m
- Historical orders/dividends/transactions: 6 req / 1m
- Reports: 1 req / 30s (GET), 1 req / 30s (POST)
- Instruments: 1 req / 50s
- Exchanges: 1 req / 30s

## Important Notes

- Orders can only be executed in the **primary account currency**
- For sell orders, use a **negative** quantity value
- This API is only available for **Invest and Stocks ISA** account types
- The API is currently in **beta**
