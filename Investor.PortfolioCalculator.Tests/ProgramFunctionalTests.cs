using System;
using System.IO;
using Xunit;

public class ProgramFunctionalTests
{
    [Fact]
    public void Main_ShouldCalculatePortfolioValue()
    {
        // Arrange
        string input = "Investor0;2017-02-23";
        string expectedOutput = "Portfolio value for Investor0 on 2017-02-23: $28,668,657.00";

        using (var inputReader = new StringReader(input))
        using (var outputWriter = new StringWriter())
        {
            Console.SetIn(inputReader);
            Console.SetOut(outputWriter);

            // Act
            Program.Main(Array.Empty<string>());

            // Assert
            string output = outputWriter.ToString();
            Assert.True(output.Split("\n").ToList().Find(x => x.Contains(expectedOutput)).Count() > 0);
            Assert.Contains(expectedOutput, output);
        }
    }
}
