namespace day17;

public class Input : Day17
{
    public override long Part1Result { get; } = 3141;
    public override long Part2Result { get; } = -1;
}

public class Example : Day17
{
    private readonly ITestOutputHelper _output;
    public Example(ITestOutputHelper output) => _output = output;

    public override long Part1Result { get; } = 3068;
    public override long Part2Result { get; } = -1;

    [Fact]
    public void Part1Visual()
    {
        Print = currentHeight =>
        {
            if (StoppedRocks.Count % 10 != 0) return;
            _output.WriteLine("         ");
            for (var y = currentHeight - 1; y >= 0; y--)
            {
                var line = "|";
                for (var x = 0; x < 7; x++)
                {
                    if (StoppedCoordinates.Contains(new (x, y)))
                    {
                        line += '#';
                    }
                    else {
                        line += '.';
                    }
                }
                _output.WriteLine(line + "|");
            }
            _output.WriteLine("+-------+");
        };
        Assert.Equal(17, SolvePart1(10));
    }
}

public abstract class Day17 : AOCDay
{
    private static Vec2D[][] Shapes =
    {
        // @0@@
        new Vec2D[] { new(-1, 0), new(0, 0), new(1, 0), new(2, 0) } ,
        //  @
        // @@@
        //  0
        new Vec2D[] { new(0, 0), new(-1, 1), new(0, 1), new(1, 1), new(0, 2)},
        //   @
        //   @
        // @0@
        new Vec2D[] { new(-1, 0), new(0, 0), new(1, 0), new(1, 1), new(1, 2)},
        // @
        // @
        // @
        // @0
        new Vec2D[] { new(-1, 0), new(-1, 1), new(-1, 2), new(-1, 3) },
        // @@
        // @0
        new Vec2D[] { new(-1, 0), new(0, 0), new(-1, 1), new(0, 1) },
    };


    protected List<ImmutableHashSet<Vec2D>> StoppedRocks = new ();
    protected HashSet<Vec2D> StoppedCoordinates = new ();
    private static Vec2D FallingTransform = new Vec2D(0, -1);

    public override long Part1() => SolvePart1(2022);

    protected int SolvePart1(int rockCount)
    {
        var commands = Input.Single();

        var command = 0;
        var fallingRock = SpawnRock(0);
        var inputTransform = new Vec2D(0, 0);
        var currentHeight = int.MinValue;

        while (StoppedRocks.Count < rockCount)
        {
            processInput();
            updateState();
        }
        return currentHeight;

        void processInput()
        {
            var input = commands[command % commands.Length];
            inputTransform = input switch
                {
                    '>' => new Vec2D(1, 0),
                    '<' => new Vec2D(-1, 0),
                    _ => throw new InvalidOperationException($"Unexpected input: {input}")
                };
            command++;
        }

        void updateState()
        {
            fallingRock = TryMove(fallingRock, inputTransform, out var afterLR) ? afterLR : fallingRock;
            if (TryMove(fallingRock, FallingTransform, out var afterFall))
            {
                fallingRock = afterFall;
            }
            else
            {
                StoppedRocks.Add(fallingRock);
                StoppedCoordinates.UnionWith(fallingRock);
                currentHeight = Math.Max(currentHeight, fallingRock.Select(c => c.Y).Max() + 1);
                fallingRock = SpawnRock(currentHeight);
                Print(currentHeight);
            }
        }
    }

    protected Action<int> Print { get; set; } = _ => {};

    protected bool TryMove(ImmutableHashSet<Vec2D> fallingRock, Vec2D transform, out ImmutableHashSet<Vec2D> next)
    {
        next = fallingRock.Select(x => x + transform).ToImmutableHashSet();

        var bounds = next.Aggregate(
            (min: new Vec2D(int.MaxValue, int.MaxValue), max: new Vec2D(int.MinValue, int.MinValue), overlap: false),
            (agg, c) => (
                min: new Vec2D(Math.Min(agg.min.X, c.X), Math.Min(agg.min.Y, c.Y)),
                max: new Vec2D(Math.Max(agg.max.X, c.X), Math.Max(agg.max.Y, c.Y)),
                overlap: agg.overlap || StoppedCoordinates.Contains(c)));
        var isOOB = bounds.min.X < 0 || bounds.min.Y < 0 || bounds.max.X > 6;
        return !isOOB && !bounds.overlap;
    }

    private ImmutableHashSet<Vec2D> SpawnRock(int currentHeight)
    {
        Vec2D offset =  new (3, currentHeight + 3);
        return Shapes[StoppedRocks.Count % Shapes.Length].Select(c => c + offset).ToImmutableHashSet();
    }

    public override long Part2() => -1;
}
