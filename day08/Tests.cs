namespace Day08;
using Map = ImmutableDictionary<Vec2D, byte>;

public class Input : Day08
{
    public override long Part1Result { get; } = 1711;
    public override long Part2Result { get; } = 301392;
}

public class Example : Day08
{
    public override long Part1Result { get; } = 21;
    public override long Part2Result { get; } = 8;
}

public abstract class Day08 : AOCDay
{
    private static Vec2D[] Directions = { new (1, 0), new (0, 1), new (0, -1), new (-1, 0) };

    private Map? _map;
    protected Map Map => _map ??= Input
        .SelectMany((line, y) => line.Select((h, x) => new
        {
            coordinate = new Vec2D(x, y),
            height = byte.Parse(line.AsSpan(x, 1)),
        }))
        .ToImmutableDictionary(x => x.coordinate, x => x.height);

    protected bool IsVisible(Vec2D pov) => Directions
        .Select(direction => LookInDirection(pov, direction))
        .Any(lineOfSight => lineOfSight.All(h => h < Map[pov]));

    protected IEnumerable<byte> LookInDirection(Vec2D pov, Vec2D direction)
    {
        var current = pov + direction;
        while (Map.TryGetValue(current, out var neighbour))
        {
            yield return neighbour;
            current += direction;
        }
    }

    protected long ScenicScore(Vec2D pov)
    {
        var hPov = Map[pov];
        var mul = 1;
        foreach (var d in Directions)
        {
            var cnt = 0;
            foreach (var h in LookInDirection(pov, d))
            {
                cnt++;
                if (h >= hPov) break;
            }
            mul *= cnt;
        }
        return mul;
    }

    public override long Part1() => Map.Keys.Count(IsVisible);

    public override long Part2() => Map.Keys.Max(ScenicScore);
}
