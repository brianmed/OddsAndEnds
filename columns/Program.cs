// csv input and output
// json ?input? and output
namespace OddsAndEnds;

internal class Program
{
    static void Main(string[] args)
    {
        CommandLine.ParseOptions(args);

        Stream inputStream = Options.App.InputFile == "-" || Console.IsInputRedirected
            ? Console.OpenStandardInput()
            : File.OpenRead(Options.App.InputFile);

        using (StreamReader stdin = new(inputStream))
        {
            while(stdin.ReadLine() is string line && line is not null)
            {
                if (Options.App.Debug) {
                    Console.Error.WriteLine($"line: {line}");
                }

                string[] columnRangeDeclaraions = Options.App.Columns
                    .Split(',')
                    .Select(c => c.Trim())
                    .ToArray();

                string[] columns = line
                    .Split(Options.App.InputDelimiter);

                IEnumerable<string> declarations = columnRangeDeclaraions;

                foreach ((string Text, int Index) declaration in declarations.Select((c, idx) => (c, idx)))
                {
                    ColumnSubset columnsSubset = new(declaration.Text, columns);

                    foreach ((string Text, int Index) column in columnsSubset.Select((c, idx) => (c, idx)))
                    {
                        bool isAtEndOfDeclarations = declaration.Index == columnRangeDeclaraions.Length - 1;

                        bool isAtEndOfColumns = column.Index == columnsSubset.Count() - 1;

                        Console.Write($"{column.Text}{((isAtEndOfDeclarations && isAtEndOfColumns) ? String.Empty : Options.App.OutputDelimiter)}");
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
