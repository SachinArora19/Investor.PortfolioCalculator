using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investor.PortfolioCalculator.Business.Contracts
{
    /// <summary>
    /// Interface for portfolio calculation logic, providing methods to calculate the value of an investor's portfolio.
    /// </summary>
    public interface IPortfolioCalculatorLogic
    {
        /// <summary>
        /// Calculates the total value of an investor's portfolio as of a specific reference date.
        /// </summary>
        /// <param name="investorId">The unique identifier of the investor.</param>
        /// <param name="referenceDate">The date for which the portfolio value should be calculated.</param>
        /// <returns>The total value of the portfolio as a decimal.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if there are issues with the investment data, such as unknown investment types or circular dependencies in fund investments.
        /// </exception>
        decimal CalculatePortfolioValue(string investorId, DateTime referenceDate);
    }
}
