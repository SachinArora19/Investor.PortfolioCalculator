using Investor.PortfolioCalculator.Business.Contracts;
using Investor.PortfolioCalculator.ServiceBuilderDI;
using System.Globalization;

public class Program
{
    public static IPortfolioCalculatorLogic _portfolioCalculator;
    public static void LoadDependencies()
    {
        AppServiceProvider app = new AppServiceProvider();
        _portfolioCalculator = app.GetService<IPortfolioCalculatorLogic>()
            ?? throw new InvalidOperationException("Failed to resolve IPortfolioCalculatorLogic from the service provider.");
    }
    public static void Main(string[] args)
    {
        LoadDependencies();
        
        Console.WriteLine("Enter investorId and referenceDate (yyyy-MM-dd) separated by a semicolon. Press Enter to calculate the portfolio value or Ctrl+C to exit.");
        var line = Console.ReadLine();
        while (!string.IsNullOrWhiteSpace(line))
        {
            var input = line.Split(";");
            if (input.Length != 2)
            {
                Console.WriteLine("Invalid input. Please provide a valid investorId and a valid referenceDate (yyyy-MM-dd) separated by a semicolon.");
                line = Console.ReadLine();
                continue;
            }
            CalculatePortfolioByInvestorIdAndDate(input.First(), input.Last());
            Console.WriteLine(line);
            line = Console.ReadLine();
        }
    }

    private static void CalculatePortfolioByInvestorIdAndDate(string investorId, string referenceDate)
    {
        if (!DateTime.TryParseExact(referenceDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime investorReferenceDate))
        {
            Console.WriteLine("Invalid date format. Please use yyyy-MM-dd.");
            return;
        }

        try
        {
            decimal portfolioValue = _portfolioCalculator.CalculatePortfolioValue(investorId, investorReferenceDate);

            Console.WriteLine($"Portfolio value for {investorId} on {referenceDate:yyyy-MM-dd}: {portfolioValue:C}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}
