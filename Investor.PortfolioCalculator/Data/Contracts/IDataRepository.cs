public interface IDataRepository
{
    List<T> ParseFile<T>(string fileName, Func<string[], T> parseLine);
    Investment ParseInvestmentLine(string[] parts);
    Quote ParseQuoteLine(string[] parts);
    Transaction ParseTransactionLine(string[] parts);

}