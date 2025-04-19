using Investor.PortfolioCalculator.Business.Contracts;
using Investor.PortfolioCalculator.Extensions;

public class PortfolioCalculatorLogic (IDataRepository fileDataRepository) : IPortfolioCalculatorLogic
{
    private readonly IEnumerable<Investment> _investments = fileDataRepository.ParseFile<Investment>("Investments.csv", fileDataRepository.ParseInvestmentLine);
    private readonly IEnumerable<Transaction> _transactions = fileDataRepository.ParseFile<Transaction>("Transactions.csv", fileDataRepository.ParseTransactionLine);
    private readonly IEnumerable<Quote> _quotes = fileDataRepository.ParseFile<Quote>("Quotes.csv", fileDataRepository.ParseQuoteLine);
    private readonly Dictionary<string, decimal> _cache = new Dictionary<string, decimal>();
    private readonly HashSet<string> _evaluating = new HashSet<string>();

    public decimal CalculatePortfolioValue(string investorId, DateTime referenceDate)
    {
        var investorInvestments = _investments.ByInvestorId(investorId).ToList();

        return investorInvestments.Sum(investment =>
            CalculateInvestmentValue(investment.InvestmentId, referenceDate));
    }

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
            "Stock" => CalculateStockValue,
            "RealEstate" => CalculateRealEstateValue,
            "Fonds" => CalculateFondsValue,
            _ => throw new InvalidOperationException($"Unknown investment type: {investment.InvestmentType}")
        };
        return CalculateFunc(investment, referenceDate);
    }

    private decimal CalculateStockValue(Investment investment, DateTime referenceDate)
    {
        var transactions = _transactions
            .Where(t => t.InvestmentId == investment.InvestmentId && t.Date <= referenceDate)
            .OrderBy(t => t.Date)
            .ToList();

        decimal units = transactions.Sum(t => t.Type == "Shares" ? t.Value : 0);
        var isin = _investments.First(i => i.InvestmentId == investment.InvestmentId).ISIN;

        var latestQuote = _quotes
            .Where(q => q.ISIN == isin && q.Date <= referenceDate)
            .OrderByDescending(q => q.Date)
            .FirstOrDefault();

        return units * (latestQuote?.PricePerShare ?? 0);
    }

    private decimal CalculateRealEstateValue(Investment investment, DateTime referenceDate)
    {
        var landValue = _transactions
            .Where(t => t.InvestmentId == investment.InvestmentId && t.Type == "Estate" && t.Date <= referenceDate)
            .Sum(t => t.Value);

        var buildingValue = _transactions
            .Where(t => t.InvestmentId == investment.InvestmentId && t.Type == "Building" && t.Date <= referenceDate)
            .Sum(t => t.Value);

        return landValue + buildingValue;
    }

    private decimal CalculateFondsValue(Investment investment, DateTime referenceDate)
    {
        if (_cache.TryGetValue(investment.InvestmentId, out var cachedValue))
            return cachedValue;

        if (_evaluating.Contains(investment.InvestmentId))
            throw new InvalidOperationException($"Cycle detected in fund investments: {investment.InvestmentId}");

        _evaluating.Add(investment.InvestmentId);

        var percentageTransactions = _transactions
            .Where(t => t.InvestmentId == investment.InvestmentId && t.Type == "Percentage" && t.Date <= referenceDate)
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