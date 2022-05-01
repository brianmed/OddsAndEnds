// csv input and output
// json ?input? and output
namespace OddsAndEnds;

using System.Text.RegularExpressions;

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
            string[] regexRangeDeclaraions = String.IsNullOrEmpty(Options.App.ColumnsIncludedInRegex)
                ? Enumerable.Empty<string>().ToArray()
                : Options.App.ColumnsIncludedInRegex
                    .Split(',')
                    .Select(c => c.Trim())
                    .ToArray();

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

                RegexSubset[] regexSubsets = regexRangeDeclaraions
                    .Select(r => new RegexSubset(r, columns))
                    .ToArray();

                IEnumerable<string> declarations = columnRangeDeclaraions;

                foreach ((string Text, int Index) declaration in declarations.Select((c, idx) => (c, idx)))
                {
                    ColumnSubset columnsSubset = new(declaration.Text, columns);

                    foreach ((string Text, int Index) column in columnsSubset.Select((c, idx) => (c, idx)))
                    {
                        bool isAtEndOfDeclarations = declaration.Index == columnRangeDeclaraions.Length - 1;

                        bool isAtEndOfColumns = column.Index == columnsSubset.Count() - 1;

                        string text = column.Text;

                        if (Options.App.ColumnRegex.Length == 2) {
                            if (regexSubsets.Where(rs => rs.Where(s => s == column.Index).Any()).Any()) {
                                text = Regex.Replace(text, Options.App.ColumnRegex[0], Options.App.ColumnRegex[1]);
                            }
                        }

                        Console.Write($"{text}{((isAtEndOfDeclarations && isAtEndOfColumns) ? String.Empty : Options.App.OutputDelimiter)}");
                    }
                }

                Console.WriteLine();
            }
        }
    }
}
