namespace day24;

public class Input : Day24
{
    public override long Part1Result { get; } = 332;
    public override long Part2Result { get; } = 942;
}

public class Example : Day24
{
    public override long Part1Result { get; } = 18;
    public override long Part2Result { get; } = 54;

}


public record Blizzard(Vec2D Position, Vec2D Direction);

public abstract class Day24 : AOCDay
{
    [Fact]
    public void WeatherForecastAssumption()
    {
        var rand = new Random().Next(Max.X * Max.Y, Max.X * Max.Y * 100);

        var pattern1 = WeatherForecast(InitialState, rand).Select(x => x.Position);
        var pattern2 = StepToBlizzardLocations[rand % StepToBlizzardLocations.Count];

        Assert.Empty(pattern1.Except(pattern2));
    }
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

        StepToBlizzardLocations = Enumerable
            .Range(0, (Max.X + 1) * (Max.Y + 1))
            .ToImmutableDictionary(
                i => i,
                i => WeatherForecast(InitialState, i)
                    .Select(x => x.Position)
                    .ToImmutableHashSet());
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

    public int AStar(State initialState, Vec2D target)
    {
        var visited = new HashSet<State>();
        var toExplore = new List<State>();
        var directions = new[] { Right, Down, Left, Up, None };
        toExplore.Add(initialState);

        var min = int.MaxValue;
        ImmutableList<Vec2D> best = ImmutableList<Vec2D>.Empty;
        while (toExplore.Any())
        {
            var current = toExplore[0]!;
            toExplore.RemoveAt(0);

            if (current.Step + Vec2D.ManhattanDistance(current.Expedition, target) >= min)
            {
                continue;
            }
            if (current.Expedition == target)
            {
                min = Math.Min(current.Step, min);
                continue;
            }
            var nextStep = current.Step + 1;
            var blizzardForecast = StepToBlizzardLocations[nextStep % StepToBlizzardLocations.Count];
            var nextStates = directions
                .Select(d => current.Expedition + d)
                .Where(IsInBounds)
                .Where(p => !blizzardForecast.Contains(p))
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
        double Score(State state) => state.Step + Vec2D.ManhattanDistance(state.Expedition, target);
        Vec2D Rotate180(Vec2D normal) => new(normal.X * -1, normal.Y * -1);
    }

    public override long Part1() => AStar(new State(Start, 0), Finish);

    public override long Part2()
    {
        var first = AStar(new State(Start, 0), Finish);
        var snackTrip = AStar(new State(Finish, first), Start);
        return AStar(new State(Start, snackTrip), Finish);
    }
}
