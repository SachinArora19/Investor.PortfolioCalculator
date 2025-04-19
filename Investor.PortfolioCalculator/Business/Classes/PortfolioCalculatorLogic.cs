using Investor.PortfolioCalculator.Business.Contracts;
using Investor.PortfolioCalculator.Extensions;

/// <summary>
/// Provides logic for calculating the value of an investor's portfolio.
/// </summary>
public class PortfolioCalculatorLogic : IPortfolioCalculatorLogic
{
    private readonly IEnumerable<Investment> _investments;
    private readonly IEnumerable<Transaction> _transactions;
    private readonly IEnumerable<Quote> _quotes;
    private readonly Dictionary<string, decimal> _cache = new Dictionary<string, decimal>();
    private readonly HashSet<string> _evaluating = new HashSet<string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="PortfolioCalculatorLogic"/> class.
    /// </summary>
    /// <param name="fileDataRepository">The data repository used to load investments, transactions, and quotes.</param>
    public PortfolioCalculatorLogic(IDataRepository fileDataRepository)
    {
        _investments = fileDataRepository.ParseFile<Investment>("Investments.csv", fileDataRepository.ParseInvestmentLine);
        _transactions = fileDataRepository.ParseFile<Transaction>("Transactions.csv", fileDataRepository.ParseTransactionLine);
        _quotes = fileDataRepository.ParseFile<Quote>("Quotes.csv", fileDataRepository.ParseQuoteLine);
    }

    /// <summary>
    /// Calculates the total value of an investor's portfolio as of a specific reference date.
    /// </summary>
    /// <param name="investorId">The unique identifier of the investor.</param>
    /// <param name="referenceDate">The date for which the portfolio value should be calculated.</param>
    /// <returns>The total value of the portfolio as a decimal.</returns>
    public decimal CalculatePortfolioValue(string investorId, DateTime referenceDate)
    {
        var investorInvestments = _investments.ByInvestorId(investorId).ToList();

        return investorInvestments.Sum(investment =>
            CalculateInvestmentValue(investment.InvestmentId, referenceDate));
    }

    /// <summary>
    /// Calculates the value of a specific investment as of a reference date.
    /// </summary>
    /// <param name="investmentId">The unique identifier of the investment.</param>
    /// <param name="referenceDate">The date for which the investment value should be calculated.</param>
    /// <returns>The value of the investment as a decimal.</returns>
    private decimal CalculateInvestmentValue(string investmentId, DateTime referenceDate)
    {
        var investment = _investments.FirstOrDefault(i => i.InvestmentId == investmentId);
        if (investment == null)
        {
            Console.WriteLine($"Investment doesn't exist for the ID: {investmentId}");
            return 0;
        }

        Func<Investment, DateTime, decimal> CalculateFunc;

        CalculateFunc = investment.InvestmentType switch
        {
            InvestmentType.Stock => CalculateStockValue,
            InvestmentType.RealEstate => CalculateRealEstateValue,
            InvestmentType.Fonds => CalculateFondsValue,
            _ => throw new InvalidOperationException($"Unknown investment type: {investment.InvestmentType}")
        };
        return CalculateFunc(investment, referenceDate);
    }

    /// <summary>
    /// Calculates the value of a stock investment as of a reference date.
    /// </summary>
    /// <param name="investment">The stock investment to calculate.</param>
    /// <param name="referenceDate">The date for which the stock value should be calculated.</param>
    /// <returns>The value of the stock investment as a decimal.</returns>
    private decimal CalculateStockValue(Investment investment, DateTime referenceDate)
    {
        var transactions = _transactions
            .Where(t => t.InvestmentId == investment.InvestmentId && t.Date <= referenceDate)
            .OrderBy(t => t.Date)
            .ToList();

        decimal units = transactions.Sum(t => t.Type == TransactionType.Shares ? t.Value : 0);
        var isin = _investments.First(i => i.InvestmentId == investment.InvestmentId).ISIN;

        var latestQuote = _quotes
            .Where(q => q.ISIN == isin && q.Date <= referenceDate)
            .OrderByDescending(q => q.Date)
            .FirstOrDefault();

        return units * (latestQuote?.PricePerShare ?? 0);
    }

    /// <summary>
    /// Calculates the value of a real estate investment as of a reference date.
    /// </summary>
    /// <param name="investment">The real estate investment to calculate.</param>
    /// <param name="referenceDate">The date for which the real estate value should be calculated.</param>
    /// <returns>The value of the real estate investment as a decimal.</returns>
    private decimal CalculateRealEstateValue(Investment investment, DateTime referenceDate)
    {
        var landValue = _transactions
            .Where(t => t.InvestmentId == investment.InvestmentId && t.Type == TransactionType.Estate && t.Date <= referenceDate)
            .Sum(t => t.Value);

        var buildingValue = _transactions
            .Where(t => t.InvestmentId == investment.InvestmentId && t.Type == TransactionType.Building && t.Date <= referenceDate)
            .Sum(t => t.Value);

        return landValue + buildingValue;
    }

    /// <summary>
    /// Calculates the value of a fund investment as of a reference date.
    /// </summary>
    /// <param name="investment">The fund investment to calculate.</param>
    /// <param name="referenceDate">The date for which the fund value should be calculated.</param>
    /// <returns>The value of the fund investment as a decimal.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a circular dependency is detected in fund investments.</exception>
    private decimal CalculateFondsValue(Investment investment, DateTime referenceDate)
    {
        if (_cache.TryGetValue(investment.InvestmentId, out var cachedValue))
            return cachedValue;

        if (_evaluating.Contains(investment.InvestmentId))
            throw new InvalidOperationException($"Cycle detected in fund investments: {investment.InvestmentId}");

        _evaluating.Add(investment.InvestmentId);

        var percentageTransactions = _transactions
            .Where(t => t.InvestmentId == investment.InvestmentId && t.Type == TransactionType.Percentage && t.Date <= referenceDate)
            .Sum(t => t.Value);

        var fondsInvestments = _investments
            .Where(i => i.FondsInvestor == investment.InvestmentId)
            .ToList();

        decimal fondsValue = fondsInvestments.Sum(fi =>
            CalculateInvestmentValue(fi.InvestmentId, referenceDate));

        decimal value = percentageTransactions * fondsValue;
        _cache[investment.InvestmentId] = value;
        _evaluating.Remove(investment.InvestmentId);

        return value;
    }
}
