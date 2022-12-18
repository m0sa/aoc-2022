namespace day18;

public class Input : Day18
{
    public override long Part1Result { get; } = 4192;
    public override long Part2Result { get; } = 2520;
}

public class Example : Day18
{
    public override long Part1Result { get; } = 64;
    public override long Part2Result { get; } = 58;
}

public abstract class Day18 : AOCDay
{

    public override long Part1() => Solve().Item1;

    public override long Part2() => Solve().Item2;
    protected (int, int) Solve()
    {
        var (part1, part2) = (0, 0);
        var all = Parse();
        part1 = all.Sum(x => Directions.Select(d => d + x).Count(n => !all.Contains(n)));

        var bounds = all.Aggregate(
            (min: new Vec3D(int.MaxValue, int.MaxValue, int.MaxValue), max: new Vec3D(int.MinValue, int.MinValue, int.MinValue)),
            (agg, cur) => (
                min: new Vec3D(Math.Min(agg.min.X, cur.X), Math.Min(agg.min.Y, cur.Y), Math.Min(agg.min.Z, cur.Z)),
                max: new Vec3D(Math.Max(agg.max.X, cur.X), Math.Max(agg.max.Y, cur.Y), Math.Max(agg.max.Z, cur.Z))));
        bounds = (min: bounds.min + new Vec3D(-1, -1, -1), max: bounds.max + new Vec3D(1, 1, 1));

        bool IsOutOufBounds(Vec3D v)
        {
            return v.X < bounds.min.X || v.Y < bounds.min.Y || v.Z < bounds.min.Z
                || v.X > bounds.max.X || v.Y > bounds.max.Y || v.Z > bounds.max.Z;
        }

        var visited = new HashSet<Vec3D>();
        var toExplore = new HashSet<Vec3D>();
        toExplore.Add(bounds.min);

        while (toExplore.Count > 0)
        {
            var current = toExplore.First();
            toExplore.Remove(current);
            if (!visited.Add(current)) continue;

            foreach (var n in Directions.Select(d => current + d))
            {
                if (IsOutOufBounds(n)) continue;
                else if (all.Contains(n)) part2++;
                else toExplore.Add(n);
            }
        }

        return (part1, part2);
    }

    protected ImmutableHashSet<Vec3D> Parse() => Input
        .Select(line => line.Split(',').Select(int.Parse).ToArray())
        .Select(s => new Vec3D(s[0], s[1], s[2]))
        .ToImmutableHashSet();

    protected static Vec3D[] Directions =
    {
        new (1, 0, 0),
        new (-1, 0, 0),
        new (0, 1, 0),
        new (0, -1, 0),
        new (0, 0, 1),
        new (0, 0, -1),
    };

    protected record struct Vec3D(int X, int Y, int Z)
    {
        public static Vec3D operator +(Vec3D a, Vec3D b) => new Vec3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
}
