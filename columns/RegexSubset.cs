using System.Collections;
using System.Text.RegularExpressions;

namespace OddsAndEnds;

// TODO: Dry
public class RegexSubset : IEnumerable<int>
{
    private string Declaration { get; set; }

    private string[] Columns { get; init; }

    private List<string> Segments { get; set; }

    private Index Start { get; set; }

    private Index End { get; set; }

    private int Step { get; set; }

    public RegexSubset(string declaration, string[] columns)
    {
        Declaration = declaration;

        Columns = columns;

        Step = (int)Options.App.ColumnStep;

        PreProcessDeclaration();

        ProcessSegments();
    }

    private void PreProcessDeclaration()
    {
        Segments = Declaration
            .Split('-', StringSplitOptions.RemoveEmptyEntries)
            .Select(d => d.Trim())
            .ToList();

        if (Declaration.StartsWith('-')) {
            Segments.Insert(0, "0");
        } else if (Declaration.EndsWith('-')) {
            Segments.Add(Segments[0].StartsWith('^') ? "0" : "^0");
        } else if (Segments.Count() == 1) {
            Segments.Add(Segments[0]);
        }
    }

    private void ProcessSegments()
    {
        if (Options.App.Debug) {
            Console.Error.WriteLine($"{Segments[0]} .. {Segments[1]}");
        }

        Start = new Index(Int32.Parse(Segments[0].Replace("^", String.Empty)), fromEnd: Segments[0].Contains('^'));

        End = new Index(Int32.Parse(Segments[1].Replace("^", String.Empty)), fromEnd: Segments[1].Contains('^'));
    }

    public void InitializeEnumerator(out uint start, out uint end, out int step, out int idx, out bool isSteppingForward)
    {
        start = (uint)(Start.IsFromEnd ? (Columns.Length - Start.Value) : Start.Value);
        end = (uint)(End.IsFromEnd ? (Columns.Length - End.Value) : End.Value);

        start = (uint)Math.Clamp(start, 0, Columns.Length - 1);
        end = (uint)Math.Clamp(end, 0, Columns.Length - 1);

        step = start > end ? -Step : Step;

        isSteppingForward = step > 0;

        idx = (int)start;
    }

    public bool GetIsProcessing(int idx, uint end, bool isSteppingForward)
    {
        return idx.CompareTo((int)end) switch
        {
            0 => true,

            > 0 => (isSteppingForward ? false : true),

            < 0 => (isSteppingForward ? true : false)
        };
    }

	public IEnumerator<int> GetEnumerator()
	{
        InitializeEnumerator(out uint start, out uint end, out int step, out int idx, out bool isSteppingForward);

        bool isProcessing = false;

        if (Options.App.Debug) {
            Console.Error.WriteLine($"{nameof(RegexSubset)}: Start GetEnumerator: {idx} {start} {end} {step} {isProcessing} {idx.CompareTo(isSteppingForward ? (int)end : (int)start)} {isSteppingForward}");
        }

        do
        {
            isProcessing = GetIsProcessing(idx, end, isSteppingForward);

            if (isProcessing) {
                yield return idx;

                idx += step;
            }

            if (Options.App.Debug) {
                Console.Error.WriteLine($"{nameof(RegexSubset)}: In GetEnumerator: {idx} {start} {end} {step} {idx.CompareTo(isSteppingForward ? (int)end : (int)start)} {isProcessing} {isSteppingForward}");
            }
        } while (isProcessing);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
