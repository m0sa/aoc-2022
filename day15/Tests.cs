using System.Text.RegularExpressions;

namespace day15;

public class Input : Day15
{
    protected override int Part1Y => 2000000;
    public override long Part1Result { get; } = 5040643;
    protected override int Part2Max => 4000000;
    public override long Part2Result { get; } = 11016575214126;
}

public class Example : Day15
{
    protected override int Part1Y => 10;
    public override long Part1Result { get; } = 26;
    protected override int Part2Max => 20;
    public override long Part2Result { get; } = 56000011;
}

public abstract partial class Day15 : AOCDay
{
    protected ImmutableDictionary<Vec2D, Vec2D> ParseInput() =>
        Input
            .Select(line => Regex.Match(line, @"Sensor at x=(?<x1>[-0-9]+), y=(?<y1>[-0-9]+): closest beacon is at x=(?<x2>[-0-9]+), y=(?<y2>[-0-9]+)"))
            .ToImmutableDictionary(
                m => new Vec2D(int.Parse(m.Groups["x1"].Value), int.Parse(m.Groups["y1"].Value)),
                m => new Vec2D(int.Parse(m.Groups["x2"].Value), int.Parse(m.Groups["y2"].Value)));

    protected abstract int Part1Y { get; }
    public override long Part1()
    {
        var input = ParseInput();
        var rows = RowRanges(input);
        var sensorsAndBeaconsOnLine = input
            .SelectMany(kvp => new [] { kvp.Key, kvp.Value })
            .Where(b => b.Y == Part1Y)
            .Select(b => b.X)
            .ToHashSet();
        return RangeCount(rows[Part1Y], except: sensorsAndBeaconsOnLine);
    }

    protected abstract int Part2Max { get; }
    public override long Part2()
    {
        var input = ParseInput();
        var allRanges = RowRanges(input);
        var searchWindow = allRanges
            .Where(kvp => kvp.Key >= 0 && kvp.Key <= Part2Max)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value
                    .Select(x => new Range(Math.Max(0, x.From), Math.Min(Part2Max, x.To)))
                    .Where(x => x.From <= x.To)
                    .ToList());

        var row = searchWindow
            .Where(kvp => RangeCount(kvp.Value) == Part2Max)
            .Select(kvp => kvp.Key)
            .Single();

        var column = Enumerable
            .Range(0, Part2Max + 1)
            .Single(x => searchWindow[row].All(r => !r.Contains(x)));

        return column * 4000000L + row;
    }

    protected record struct Range(int From, int To)
    {
        public int Length => To - From + 1;
        public bool Contains(int x) => From <= x && x <= To;
    }
    protected static Dictionary<int, List<Range>> RowRanges(ImmutableDictionary<Vec2D, Vec2D> input)
    {
        var rows = new Dictionary<int, List<Range>>();
        void AddRowRange(Vec2D sensor, int distance, int offset)
        {
            var row = sensor.Y + offset;
            var ranges = rows.TryGetValue(row, out var existing) ? existing : rows[row] = new ();
            var range = new Range(
                sensor.X - distance + Math.Abs(offset),
                sensor.X + distance - Math.Abs(offset));
            ranges.Add(range);
        }
        foreach (var (sensor, beacon) in input)
        {
            var distance = ManhattanDistance(sensor, beacon);
            AddRowRange(sensor, distance, 0);
            for (var offset = 1; offset <= distance; offset++)
            {
                AddRowRange(sensor, distance, offset);
                AddRowRange(sensor, distance, offset * -1);
            }
        }
        return rows;
    }

    protected static int RangeCount(IEnumerable<Range> ranges, ISet<int>? except = null)
    {
        except = except ?? ImmutableHashSet<int>.Empty;
        var count = 0;
        var position = int.MinValue;
        foreach(var (from, to) in ranges.OrderBy(x => x.From))
        {
            Range r;
            if (position < from)
                r = new (from, to);
            else if (position <= to)
                r = new (position, to);
            else
                continue;

            count += r.Length - except.Count(r.Contains);
            position = to + 1;
        }
        return count;
    }

    public static int ManhattanDistance(Vec2D a, Vec2D b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
}
