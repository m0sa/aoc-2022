namespace Day08;
using Map = ImmutableDictionary<Vec2D, byte>;

public class Input : Day08
{
    public override long Part1Result { get; } = 1711;
    public override long Part2Result { get; } = -1;
}

public class Example : Day08
{
    public override long Part1Result { get; } = 21;
    public override long Part2Result { get; } = -1;
}

public record struct Vec2D(int X, int Y)
{
    public static Vec2D operator+(Vec2D a, Vec2D b) => new Vec2D(a.X + b.X, a.Y + b.Y);
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

    public override long Part1() => Map.Keys.Count(IsVisible);

    public override long Part2() => -1;
}
