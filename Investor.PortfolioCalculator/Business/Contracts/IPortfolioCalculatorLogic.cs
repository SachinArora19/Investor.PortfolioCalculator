using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investor.PortfolioCalculator.Business.Contracts
{
    public interface IPortfolioCalculatorLogic
    {
        decimal CalculatePortfolioValue(string investorId, DateTime referenceDate);
    }
}
