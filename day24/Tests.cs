namespace day24;

public class Input : Day24
{
    public override long Part1Result { get; } = 332;
    public override long Part2Result { get; } = -1;
}

public class Example : Day24
{
    public override long Part1Result { get; } = 18;
    public override long Part2Result { get; } = -1;
}


public record Blizzard(Vec2D Position, Vec2D Direction);

public abstract class Day24 : AOCDay
{
    protected ImmutableList<Blizzard> InitialState { get; }
    protected Vec2D Start { get; }
    protected Vec2D Finish { get; }
    protected Vec2D Min => None;
    protected Vec2D Max { get; }
    protected Vec2D None { get; } = new Vec2D(0, 0);
    protected readonly static Vec2D Up = new (0, -1), Right = new (1, 0), Down = new (0, 1), Left = new (-1, 0);
    protected ImmutableDictionary<int, ImmutableHashSet<Vec2D>> StepToBlizzardLocations { get; }

    protected Day24()
    {
        InitialState = Input
            .Skip(1)
            .TakeWhile(l => !l.StartsWith("##"))
            .SelectMany((line, y) =>
                line
                    .Substring(1, line.Length - 2)
                    .Select((ch, x) => new Blizzard(
                        new (x, y),
                        ch switch {
                            '^' => Up,
                            '>' => Right,
                            'v' => Down,
                            '<' => Left,
                            _   => None
                        }))
                    .Where(x => x.Direction != None))
            .ToImmutableList();
        Max = InitialState
            .Select(x => x.Position)
            .Aggregate((agg, cur) => new Vec2D(Math.Max(cur.X, agg.X), Math.Max(cur.Y, agg.Y)));
        Start = new Vec2D(0, -1);
        Finish = Max + new Vec2D(0, 1);
    }

    private IEnumerable<Blizzard> WeatherForecast(IEnumerable<Blizzard> current, int step)
    {
        var modX = Max.X + 1;
        var modY = Max.Y + 1;
        foreach (var b in current)
        {
            yield return new Blizzard(
                new((b.Position.X + b.Direction.X * step + modX * step) % modX, (b.Position.Y + b.Direction.Y * step + modY * step) % modY),
                b.Direction);
        }
    }

    public record State(Vec2D Expedition, int Step);

    public long AStar()
    {
        var visited = new HashSet<State>();
        var weatherAt = new Dictionary<int, ImmutableDictionary<Vec2D, ImmutableHashSet<Vec2D>>>();
        var toExplore = new List<State>();
        var directions = new[] { Right, Down, Left, Up, None };
        toExplore.Add(new (Start, 0));

        var min = int.MaxValue;
        ImmutableList<Vec2D> best = ImmutableList<Vec2D>.Empty;
        while (toExplore.Any())
        {
            var current = toExplore[0]!;
            toExplore.RemoveAt(0);
            if (current.Step + Vec2D.ManhattanDistance(current.Expedition, Finish) >= min)
            {
                continue;
            }
            if (current.Expedition == Finish)
            {
                min = Math.Min(current.Step, min);
                continue;
            }
            var nextStep = current.Step + 1;
            ImmutableDictionary<Vec2D, ImmutableHashSet<Vec2D>> blizzardForecast;
            if (!weatherAt.TryGetValue(nextStep, out blizzardForecast))
            {
                blizzardForecast = weatherAt[nextStep] =
                    WeatherForecast(InitialState, nextStep)
                        .ToLookup(x => x.Position)
                        .ToImmutableDictionary(
                            x => x.Key,
                            x => x.Select(b => b.Direction).ToImmutableHashSet());
            }
            var nextStates = directions
                //.Where(d => !blizzardForecast.TryGetValue(current.Expedition, out var replacement) || !replacement.Contains(Rotate180(d)))
                .Select(d => current.Expedition + d)
                .Where(IsInBounds)
                .Where(p => !blizzardForecast.ContainsKey(p))
                .Select(p => new State(p, nextStep))
                .Where(visited.Add)
                .ToList();
            if (nextStates.Any())
            {
                toExplore.InsertRange(0, nextStates);
                toExplore.Sort((x, y) => Math.Sign(Score(x) - Score(y)));
            }
        }
        return min;

        bool IsInBounds(Vec2D position) => position == Start || position == Finish || (position.X >= Min.X && position.Y >= Min.Y && position.X <= Max.X && position.Y <= Max.Y);
        double Score(State state) => state.Step + Vec2D.ManhattanDistance(state.Expedition, Finish);
        Vec2D Rotate180(Vec2D normal) => new(normal.X * -1, normal.Y * -1);
    }

    public override long Part1() => AStar();

    public override long Part2() => -1;
}
