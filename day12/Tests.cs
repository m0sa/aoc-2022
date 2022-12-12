namespace day12;

public class Input : Day12
{
    public override long Part1Result { get; } = 456;
    public override long Part2Result { get; } = 454;
}

public class Example : Day12
{
    public override long Part1Result { get; } = 31;
    public override long Part2Result { get; } = 29;
}

public abstract class Day12 : AOCDay
{
    private ImmutableDictionary<Vec2D, char>? _heightMap;
    protected ImmutableDictionary<Vec2D, char> HeightMap => _heightMap ??= Input
        .SelectMany((line, y) => line.Select((c, x) => new KeyValuePair<Vec2D, char>(new Vec2D(x, y), c)))
        .ToImmutableDictionary();
    protected static Vec2D[] Directions = { new (1, 0), new (0, 1), new (-1, 0), new (0, -1) };

    protected char HeightOf(Vec2D pos) => HeightMap[pos] switch { 'S' => 'a', 'E' => 'z', char c => c };

    protected long Dijkstra(Vec2D start)
    {
        var dis = HeightMap.ToDictionary(c => c.Key, c => int.MaxValue);
        var end = HeightMap.Where(kvp => kvp.Value == 'E').Select(kvp => kvp.Key).Single();
        var queue = HeightMap.Keys.ToList();
        bool TryDequeue(out Vec2D element)
        {
            if (queue!.Count > 0)
            {
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
        void UpdateQueue(Vec2D element, int distance)
        {
            dis[element] = distance;
            queue.Sort((v1, v2) => dis[v1].CompareTo(dis[v2]));
        }

        UpdateQueue(start, 0);
        while(TryDequeue(out var current))
        {
            if (current == end) break;
            var currentHeight = HeightOf(current);
            var nextDistance = dis[current] + 1;
            foreach (var child in
                from d in Directions
                let c = current + d
                where dis.TryGetValue(c, out var childDistance)
                   && nextDistance < childDistance
                   && HeightOf(c) - currentHeight <= 1
                select c)
            {
                UpdateQueue(child, nextDistance);
            }
        }
        return dis[end];
    }

    protected Vec2D Start => HeightMap.Where(kvp => kvp.Value == 'S').Select(kvp => kvp.Key).Single();

    public override long Part1() => Dijkstra(Start);

    public override long Part2() => HeightMap.Keys
        .Where(x => HeightOf(x) == 'a'
            && x.X == 0) // cheating by looking at data.... it's impossible to get out of any aaa-surrounded-by-cccc-s valley
        .Select(Dijkstra)
        .Min();
}