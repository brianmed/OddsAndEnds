// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

namespace OddsAndEnds
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("--help")) {
                Console.WriteLine(@"Description:
  Process regular expression(s) over lines in a file.

Usage:
  stream [options]

Options:
  -e        A regular expression.
  -f        A file with regular expressions.
  --help    Display this help.");

                Environment.Exit(0);
            }

            List<string> RegexesFind = new();
            List<string> RegexesReplace = new();

            foreach (string regex in GetRegexes(args))
            {
                char splitOn = regex[1];

                RegexesFind.Add(regex.Split(splitOn)[1]);
                RegexesReplace.Add(regex.Split(splitOn)[^2]);
            }

            using (StreamReader stdin = new(Console.OpenStandardInput()))
            {
                while(stdin.ReadLine() is string line && line is not null)
                {
                    foreach ((string Regex, int Index) yay in RegexesFind.Select((r, idx) => (r, idx)))
                    {
                        if (Regex.Match(line, yay.Regex.ToString()).Success) {
                            line = Regex.Replace(line, yay.Regex, RegexesReplace[yay.Index], RegexOptions.None);
                        }
                    }

                    Console.WriteLine(line);
                }
            }
        }

        static IEnumerable<string> GetRegexes(string[] args)
        {
            bool prevDashE = false;
            bool prevDashF = false;

            foreach (string arg in args)
            {
                if (prevDashE) {
                    prevDashE = false;

                    yield return arg;
                } else if (prevDashF) {
                    using (StreamReader stdin = new(File.OpenRead(arg)))
                    {    
                        while(stdin.ReadLine() is string line && line is not null)
                        {
                            yield return line;
                        }
                    }
                } else if (arg.Equals("-e")) {
                    prevDashE = true;
                } else if (arg.Equals("-f")) {
                    prevDashF = true;
                }
            }
        }
    }
}
