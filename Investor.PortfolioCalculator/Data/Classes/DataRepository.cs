using System.Globalization;
using System.Reflection;

public class DataRepository : IDataRepository
{
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

    public Investment ParseInvestmentLine(string[] parts)
    {
        if (parts.Length < 6) throw new FormatException("Invalid investment line format.");
        return new Investment
        {
            InvestorId = parts[0],
            InvestmentId = parts[1],
            InvestmentType = parts[2],
            FondsInvestor = parts[5]
        };
    }

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

    public Transaction ParseTransactionLine(string[] parts)
    {
        if (parts.Length != 4) throw new FormatException("Invalid transaction line format.");
        return new Transaction
        {
            InvestmentId = parts[0],
            Type = parts[1],
            Date = DateTime.ParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            Value = decimal.Parse(parts[3], CultureInfo.InvariantCulture)
        };
    }
}