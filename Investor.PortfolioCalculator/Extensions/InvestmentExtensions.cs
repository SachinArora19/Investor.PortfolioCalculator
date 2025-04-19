namespace Investor.PortfolioCalculator.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="Investment"/> class.
    /// </summary>
    public static class InvestmentExtensions
    {
        /// <summary>
        /// Filters a collection of investments to return only those belonging to a specific investor.
        /// </summary>
        /// <param name="investments">The collection of <see cref="Investment"/> objects to filter.</param>
        /// <param name="investorId">The unique identifier of the investor.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing the investments that belong to the specified investor.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the <paramref name="investments"/> collection is null.
        /// </exception>
        public static IEnumerable<Investment> ByInvestorId(this IEnumerable<Investment> investments, string investorId)
        {
            if (investments == null)
                throw new ArgumentNullException(nameof(investments), "The investments collection cannot be null.");

            return investments.Where(investment => investment.InvestorId == investorId);
        }
    }
}
