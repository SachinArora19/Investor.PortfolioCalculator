/// <summary>
/// Interface for data repository operations, providing methods to parse data from files
/// and convert them into domain-specific objects.
/// </summary>
public interface IDataRepository
{
    /// <summary>
    /// Parses a CSV file into a list of objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to parse each line into.</typeparam>
    /// <param name="fileName">The name of the CSV file to parse.</param>
    /// <param name="parseLine">A function to parse a line into an object of type <typeparamref name="T"/>.</param>
    /// <returns>A list of parsed objects of type <typeparamref name="T"/>.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the specified file is not found.</exception>
    /// <exception cref="FormatException">Thrown if the file contains invalid data.</exception>
    List<T> ParseFile<T>(string fileName, Func<string[], T> parseLine);

    /// <summary>
    /// Parses a line of data into an <see cref="Investment"/> object.
    /// </summary>
    /// <param name="parts">An array of strings representing the parts of the line split by a delimiter.</param>
    /// <returns>An <see cref="Investment"/> object parsed from the line.</returns>
    /// <exception cref="FormatException">Thrown if the line format is invalid.</exception>
    Investment ParseInvestmentLine(string[] parts);

    /// <summary>
    /// Parses a line of data into a <see cref="Quote"/> object.
    /// </summary>
    /// <param name="parts">An array of strings representing the parts of the line split by a delimiter.</param>
    /// <returns>A <see cref="Quote"/> object parsed from the line.</returns>
    /// <exception cref="FormatException">Thrown if the line format is invalid.</exception>
    Quote ParseQuoteLine(string[] parts);

    /// <summary>
    /// Parses a line of data into a <see cref="Transaction"/> object.
    /// </summary>
    /// <param name="parts">An array of strings representing the parts of the line split by a delimiter.</param>
    /// <returns>A <see cref="Transaction"/> object parsed from the line.</returns>
    /// <exception cref="FormatException">Thrown if the line format is invalid.</exception>
    Transaction ParseTransactionLine(string[] parts);
}
