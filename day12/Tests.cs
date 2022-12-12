namespace day12;

public class Input : Day12
{
    public override long Part1Result { get; } = 456;
    public override long Part2Result { get; } = -1;
}

public class Example : Day12
{
    [Fact]
    public void Ex() {
        var path = Dijkstra();
        Assert.Equal(31, path);
    }
    public override long Part1Result { get; } = 31;
    public override long Part2Result { get; } = -1;
}

public abstract class Day12 : AOCDay
{
    private ImmutableDictionary<Vec2D, char>? _heightMap;
    protected ImmutableDictionary<Vec2D, char> HeightMap => _heightMap ??= Input
        .SelectMany((line, y) => line.Select((c, x) => new KeyValuePair<Vec2D, char>(new Vec2D(x, y), c)))
        .ToImmutableDictionary();
    protected static Vec2D[] Directions = { new (1, 0), new (0, 1), new (-1, 0), new (0, -1) };

    protected long Dijkstra()
    {
        var dis = HeightMap.ToDictionary(c => c.Key, c => int.MaxValue);
        var start = HeightMap.Where(kvp => kvp.Value == 'S').Select(kvp => kvp.Key).Single();
        var end = HeightMap.Where(kvp => kvp.Value == 'E').Select(kvp => kvp.Key).Single();
        dis[start] = 0;
        var queue = HeightMap.Keys.ToList();
        char HeightOf(Vec2D pos) => HeightMap[pos] switch { 'S' => 'a', 'E' => 'z', char c => c };
        bool TryDequeue(out Vec2D element)
        {
            if (queue!.Count > 0)
            {
                queue.Sort((v1, v2) => dis[v1].CompareTo(dis[v2]));
                element = queue[0];
                queue.RemoveAt(0);
                return true;
            }
            else
            {
                element = default;
                return false;
            }
        }
        while(TryDequeue(out var current))
        {
            if (current == end) continue;
            var currentHeight = HeightOf(current);
            var children =
                from d in Directions
                let child = current + d
                where HeightMap.ContainsKey(child)
                where HeightOf(child) - currentHeight <= 1
                select child;
            foreach (var child in children)
            {
                dis[child] = Math.Min(dis[current] + 1, dis[child]);
            }
        }
        var W = HeightMap.MaxBy(x => x.Key.X).Key.X;
        var H = HeightMap.MaxBy(x => x.Key.Y).Key.Y;
        var s = "";
        for (var y = 0; y <= H; y++)
        {
            for (var x = 0; x <= W; x++)
                s += $"{dis[new (x, y)],3}";
            s += Environment.NewLine;
        }
        return dis[end];
    }

    public override long Part1() => Dijkstra(); // first and last step don't count

    public override long Part2() => -1;
}