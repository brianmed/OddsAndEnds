using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;

namespace OddsAndEnds;

public class Options
{
    public static AppOptions App { get; set; }
}

public class AppOptions
{
    public string Columns { get; init; }

    public string ColumnsIncludedInRegex { get; init; }

    public string[] ColumnRegex { get; init; }

    public int ColumnStep { get; init; }

    public bool Debug { get; init; }

    public string InputDelimiter { get; init; }

    public string InputFile { get; init; }

    public string OutputDelimiter { get; init; }
}

public class CommandLine
{
    public static void ParseOptions(string[] args)
    {
        try
        {
            RootCommand rootCommand = new()
            {
                new Option<string>("--columns", description: "List of Columns to Extract")
                {
                    IsRequired = true
                },
                new Option<bool>("--debug", description: "Debug Output")
                {
                    IsRequired = false
                },
                new Option<string>("--ifs", description: "Delimiter for Input Columns")
                {
                    IsRequired = false
                },
                new Option<string>("--input", description: "Input File")
                {
                    IsRequired = false
                },
                new Option<string>("--ofs", description: "Delimiter for Output Columns")
                {
                    IsRequired = false
                },
                new Option<string>("--regex", description: "Regex Substitute Pattern for the Extracted Columns")
                {
                    IsRequired = false
                },
                new Option<string>("--regexColumns", description: "List of Columns included in Regex Substitution")
                {
                    IsRequired = false
                },
                new Option<int>("--step", description: "Column Traversal Step")
                {
                    IsRequired = false
                }
            };

            rootCommand.Name = "columns";
            rootCommand.Description = "Extract columns from lines in a file.";

            rootCommand.Handler = CommandHandler.Create<string, bool, string, string, string, string, string, int?>((columns, debug, ifs, input, ofs, regexColumns, regex, step) =>
            {
                List<string> findAndReplace = new();

                if (regex is not null) {
                    char splitOn = regex[1];

                    findAndReplace.Add(regex.Split(splitOn)[1]);
                    findAndReplace.Add(regex.Split(splitOn)[^2]);
                }

                Options.App = new()
                {
                    Columns = columns,
                    ColumnsIncludedInRegex = String.IsNullOrEmpty(regexColumns) ? String.Empty : regexColumns,
                    ColumnRegex = findAndReplace.ToArray(),
                    ColumnStep = step.GetValueOrDefault(1),
                    Debug = debug,
                    InputDelimiter = ifs is null ? "," : ifs,
                    InputFile = String.IsNullOrEmpty(input) ? "-" : input,
                    OutputDelimiter = ofs is null ? "," : ofs
                };
            });

            rootCommand.Invoke(args);

            // Not sure if there is a way to handle these via the library
            if (args.Any(a => new[] { "--version", "--help", "-h", "-?" }.Contains(a.Trim()))) {
                Environment.Exit(0);
            }

            if (Options.App is null) {
                Environment.Exit(0);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine($"Try '{System.Diagnostics.Process.GetCurrentProcess().ProcessName} --help' for more information.");

            Environment.Exit(1);
        }   
    }
}
