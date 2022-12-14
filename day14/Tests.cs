namespace day14;

public class Input : Day14
{
    public override long Part1Result { get; } = 961;
    public override long Part2Result { get; } = 26375;
}

public class Example : Day14
{
    [Fact]
    public void ScanSanityCheck() => Assert.Equal(20, Rocks.Count());
    public override long Part1Result { get; } = 24;
    public override long Part2Result { get; } = 93;
}

public abstract class Day14 : AOCDay
{
    private ImmutableHashSet<Vec2D>? _rocks;
    protected ImmutableHashSet<Vec2D> Rocks => _rocks ??= ParseScan();
    public ImmutableHashSet<Vec2D> ParseScan()
    {
        List<Vec2D> scan = new();
        foreach (var line in Input)
        {
            var points = line.Split(" -> ").Select(v => v.Split(",").Select(int.Parse).ToList()).Select(s => new Vec2D(s[0], s[1])).ToList();
            for (var i = 0; i < points.Count; i++)
            {
                var from = points[i];
                var to = points.Count == i+1 ? from : points[i+1];
                var direction = to - from;
                var step = new Vec2D(Math.Sign(direction.X), Math.Sign(direction.Y));
                var current = from;
                do
                {
                    scan.Add(current);
                    current += step;
                } while (current != to + step);
            }
        }
        return scan.ToImmutableHashSet();
    }

    protected int MaxY => Rocks.Select(x => x.Y).Max();
    protected static Vec2D SandSource { get; } = new(500, 0);
    protected static Vec2D[] SandSteps = { new (0, 1), new (-1, 1), new (1, 1) }; // down, down-left, down-right

    public override long Part1() => SandCount(abbyss: MaxY + 1);

    protected int SandCount(int abbyss = int.MaxValue, int virtualGround = int.MaxValue)
    {
        var sand = new HashSet<Vec2D>();
        Vec2D? inFlightSand = null;
        while ((inFlightSand?.Y ?? 0) < abbyss && !sand.Contains(SandSource))
        {
            var current = inFlightSand ??= SandSource;
            var next = SandSteps
                .Select(step => current + step)
                .Where(p => p.Y < virtualGround && !Rocks.Contains(p) && !sand.Contains(p))
                .ToList();
            if (next.Count > 0)
            {
                inFlightSand = next[0];
            }
            else
            {
                sand.Add(current);
                inFlightSand = null;
            }
        }
        return sand.Count;
    }

    public override long Part2() => SandCount(virtualGround: MaxY + 2);
}
