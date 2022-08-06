using System.Collections.Concurrent;

namespace OddsAndEnds
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Contains("--help")) {
                Console.WriteLine(@"Description:
  Line oriented sort, possibly uniquely with the sorted output displayed in
  sets, every s seconds.

Usage:
  psort [options]

Options:
  -s #      Number of seconds for each batch (default 5).
  -u        Sort uniquely.
  --help    Display this help.");

                Environment.Exit(0);
            }

            int delaySeconds = 5;
            bool distinct = false;

            if (args.Where(a => a.StartsWith("-s")).FirstOrDefault() is string argDelaySeconds && argDelaySeconds is not null)
            {
                delaySeconds = Int32.Parse(args.Where(a => a.StartsWith("-s")).Skip(1).FirstOrDefault() ?? "5");
            }

            if (args.Where(a => a.StartsWith("-u")).FirstOrDefault() is string argDistinct && argDistinct is not null)
            {
                distinct = true;
            }

            ConcurrentQueue<string> output = new();
            string overflow = String.Empty;

            Task.Run(() =>
            {
                using (StreamReader stdin = new(Console.OpenStandardInput()))
                {
                    while(stdin.ReadLine() is string line && line is not null)
                    {
                        output.Enqueue(line);
                    }
                }
            });

            DateTime finished = DateTime.Now.AddSeconds(delaySeconds);

            while (true)
            {
                if (finished >= DateTime.Now)
                {
                    System.Threading.Thread.Sleep(TimeSpan.FromMilliseconds(50));
                }
                else
                {
                    finished = DateTime.Now.AddSeconds(delaySeconds);

                    if (output.Any())
                    {
                        ICollection<string> sortedOutput = distinct ? new HashSet<string>() : new List<string>();

                        while (output.TryDequeue(out string s))
                        {
                            sortedOutput.Add(s);
                        }

                        foreach (string line in sortedOutput.OrderBy(o => o))
                        {
                            Console.WriteLine(line);
                        }

                    }
                }
            }
        }
    }
}
