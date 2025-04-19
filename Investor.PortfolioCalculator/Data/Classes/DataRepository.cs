using System.Globalization;
using System.Reflection;

public class DataRepository : IDataRepository
{
    /// <summary>
    /// Parses a CSV file into a list of objects of type T.
    /// </summary>
    /// <typeparam name="T">The type of object to parse each line into.</typeparam>
    /// <param name="fileName">The name of the CSV file.</param>
    /// <param name="parseLine">A function to parse a line into an object of type T.</param>
    /// <returns>A list of parsed objects.</returns>
    public List<T> ParseFile<T>(string fileName, Func<string[], T> parseLine)
    {
        var filePath = Path.Combine(System.IO.Path.GetFullPath(@"..\..\..\Data\CSV Data sources"), fileName);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        return File.ReadAllLines(filePath)
            .Skip(1)
            .Select(line => line.Split(';'))
            .Where(parts => parts.Length > 0)
            .Select(parseLine)
            .ToList();
    }

    /// <summary>
    /// Parses a line into a Investment object.
    /// </summary>
    /// <param name="parts">The parts of the line split by the delimiter.</param>
    /// <returns>A Transaction object.</returns>
    public Investment ParseInvestmentLine(string[] parts)
    {
        if (parts.Length < 6) throw new FormatException("Invalid investment line format.");
        return new Investment
        {
            InvestorId = parts[0],
            InvestmentId = parts[1],
            InvestmentType = Enum.TryParse<InvestmentType>(parts[2], out var investmentType)
                ? investmentType
                : throw new FormatException($"Invalid InvestmentType value: {parts[2]}"),
            FondsInvestor = parts[5]
        };
    }

    /// <summary>
    /// Parses a line into a Quote object.
    /// </summary>
    /// <param name="parts">The parts of the line split by the delimiter.</param>
    /// <returns>A Quote object.</returns>
    public Quote ParseQuoteLine(string[] parts)
    {
        if (parts.Length < 3) throw new FormatException("Invalid quote line format.");
        return new Quote
        {
            ISIN = parts[0],
            Date = DateTime.ParseExact(parts[1], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            PricePerShare = decimal.Parse(parts[2], CultureInfo.InvariantCulture)
        };
    }

    /// <summary>
    /// Parses a line into a Transaction object.
    /// </summary>
    /// <param name="parts">The parts of the line split by the delimiter.</param>
    /// <returns>A Transaction object.</returns>
    public Transaction ParseTransactionLine(string[] parts)
    {
        if (parts.Length != 4) throw new FormatException("Invalid transaction line format.");
        return new Transaction
        {
            InvestmentId = parts[0],
            Type = Enum.TryParse<TransactionType>(parts[1], out var transactionType)
                ? transactionType
                : throw new FormatException($"Invalid TransactionType value: {parts[1]}"),
            Date = DateTime.ParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            Value = decimal.Parse(parts[3], CultureInfo.InvariantCulture)
        };
    }
}