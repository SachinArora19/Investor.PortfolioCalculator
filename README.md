# Portfolio Valuation Console Application

## Overview

This project is a C# console application that calculates the value of an investor's portfolio on a specific date. The application is designed for portfolio managers who need to determine the historical or current value of client portfolios, which may include stocks, real estate, and funds (including nested funds).

## Features

- Calculates the portfolio value for any investor on any given date.
- Supports multiple investment types:
  - **Stocks:** Value determined by the number of shares held multiplied by the latest available share price.
  - **Real Estate:** Value is the sum of land value and building value.
  - **Funds:** Value is the percentage held in the fund multiplied by the total value of the fund's underlying investments, which can include other funds, stocks, or real estate.
- Handles recursive fund structures, allowing funds to contain other funds.
- Reads input data from three CSV files:
  - **Investments:** Assigns investments to investors and defines fund compositions.
  - **Transactions:** Records all transactions for each investment.
  - **Quotes:** Contains the historical price data for stocks.

## Input Files

- **Investments.csv:** Lists all investments, their types, and ownership.
- **Transactions.csv:** Contains all transactions (purchases, sales, value changes) for each investment.
- **Quotes.csv:** Provides historical price data for stocks (may not have prices for every day).

## How It Works

1. **Startup:**  
   The application prompts for the investor ID and the reference date (the date for which the portfolio value is to be calculated).

2. **Data Processing:**  
   - Reads and parses the CSV files.
   - Aggregates all relevant transactions up to the reference date.
   - For stocks, finds the most recent available quote on or before the reference date.
   - For funds, recursively calculates the value based on the underlying investments.

3. **Output:**  
   Displays the total portfolio value for the specified investor on the given date.

## Example Usage

```bash
PortfolioCalculator Investor1 2024-03-31
```

_Output:_
```
Portfolio value for Investor1 on 2024-03-31: $123,456.78
```

## Project Structure

- **Business Layer:** Contains the logic for portfolio calculation and investment valuation.
- **Data Layer:** Handles reading and parsing of CSV input files.
- **Dependency Injection:** The application uses dependency injection for loose coupling and testability, making it easy to swap out data sources or add unit tests.

## Installation & Running

1. Clone the repository.
3. Build the project using your preferred .NET build tool.
4. Run the application from the command line, providing the investor ID and reference date as arguments.

## Extensibility

- Easily add support for new investment types by extending the business logic.
- Swap out the data layer for a database or API by implementing the relevant interfaces.

## License

MIT License

---

**Task Reference:**  
This application was developed according to a client specification requiring the calculation of investor portfolio values based on various investment types and historical data.
