namespace Investor.PortfolioCalculator.Extensions
{
    public static class InvestmentExtensions
    {
        public static IEnumerable<Investment> ByInvestorId(this IEnumerable<Investment> investments, string investorId)
        {
            return investments.Where(investment => investment.InvestorId == investorId);
        }
    }
}
