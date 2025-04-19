using Moq;
using Xunit;

public class PortfolioCalculatorLogicUnitTests
{
    private readonly Mock<IDataRepository> _mockRepository;
    private readonly Mock<ILogger> _mockLogger; // Added mock for ILogger
    private readonly PortfolioCalculatorLogic _calculator;

    public PortfolioCalculatorLogicUnitTests()
    {
        _mockRepository = new Mock<IDataRepository>();
        _mockLogger = new Mock<ILogger>(); // Initialize mock logger

        // Mock data for investments
        _mockRepository.Setup(repo => repo.ParseFile<Investment>("Investments.csv", It.IsAny<Func<string[], Investment>>()))
            .Returns(new List<Investment>
            {
                new Investment { InvestorId = "1", InvestmentId = "INV1", InvestmentType = InvestmentType.Stock, ISIN = "ISIN1" },
                new Investment { InvestorId = "1", InvestmentId = "INV2", InvestmentType = InvestmentType.RealEstate }
            });

        // Mock data for transactions
        _mockRepository.Setup(repo => repo.ParseFile<Transaction>("Transactions.csv", It.IsAny<Func<string[], Transaction>>()))
            .Returns(new List<Transaction>
            {
                new Transaction { InvestmentId = "INV1", Type = TransactionType.Shares, Date = DateTime.Parse("2025-01-01"), Value = 100 },
                new Transaction { InvestmentId = "INV2", Type = TransactionType.Estate, Date = DateTime.Parse("2025-01-01"), Value = 200 }
            });

        // Mock data for quotes
        _mockRepository.Setup(repo => repo.ParseFile<Quote>("Quotes.csv", It.IsAny<Func<string[], Quote>>()))
            .Returns(new List<Quote>
            {
                new Quote { ISIN = "ISIN1", Date = DateTime.Parse("2025-01-01"), PricePerShare = 50 }
            });

        _calculator = new PortfolioCalculatorLogic(_mockRepository.Object, _mockLogger.Object); // Pass mock logger
    }

    [Fact]
    public void CalculatePortfolioValue_ShouldReturnCorrectValue()
    {
        // Arrange
        string investorId = "1";
        DateTime referenceDate = DateTime.Parse("2025-01-01");

        // Act
        decimal portfolioValue = _calculator.CalculatePortfolioValue(investorId, referenceDate);

        // Assert
        Assert.Equal(5200, portfolioValue); // (100 shares * 50 price) + 200 estate value
    }

    [Fact]
    public void CalculatePortfolioValue_InvalidInvestorId_ShouldReturnZero()
    {
        // Arrange
        string investorId = "999"; // Non-existent investor
        DateTime referenceDate = DateTime.Parse("2025-01-01");

        // Act
        decimal portfolioValue = _calculator.CalculatePortfolioValue(investorId, referenceDate);

        // Assert
        Assert.Equal(0, portfolioValue);
    }
}
