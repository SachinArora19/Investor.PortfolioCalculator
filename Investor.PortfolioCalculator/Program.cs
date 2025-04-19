using Investor.PortfolioCalculator.Business.Contracts;
using Investor.PortfolioCalculator.ServiceBuilderDI;
using System.Globalization;

/// <summary>
/// Entry point for the Portfolio Calculator application.
/// This program calculates the portfolio value for a given investor and reference date.
/// </summary>
public class Program
{
    /// <summary>
    /// Instance of the portfolio calculator logic, resolved via dependency injection.
    /// </summary>
    public static IPortfolioCalculatorLogic _portfolioCalculator;

    /// <summary>
    /// Loads dependencies required for the application using a service provider.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <see cref="IPortfolioCalculatorLogic"/> cannot be resolved from the service provider.
    /// </exception>
    public static void LoadDependencies()
    {
        AppServiceProvider app = new AppServiceProvider();
        _portfolioCalculator = app.GetService<IPortfolioCalculatorLogic>()
            ?? throw new InvalidOperationException("Failed to resolve IPortfolioCalculatorLogic from the service provider.");
    }

    /// <summary>
    /// Main entry point of the application.
    /// Prompts the user to input an investor ID and reference date, then calculates the portfolio value.
    /// </summary>
    /// <param name="args">Command-line arguments (not used).</param>
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
            Console.WriteLine("Enter investorId and referenceDate (yyyy-MM-dd) separated by a semicolon. Press Enter to calculate the portfolio value or Ctrl+C to exit.");
            line = Console.ReadLine();
        }
    }

    /// <summary>
    /// Calculates the portfolio value for a given investor ID and reference date.
    /// </summary>
    /// <param name="investorId">The unique identifier of the investor.</param>
    /// <param name="referenceDate">The reference date as a string in "yyyy-MM-dd" format.</param>
    /// <remarks>
    /// If the date format is invalid, an error message is displayed.
    /// If an exception occurs during calculation, it is caught and logged.
    /// </remarks>
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
