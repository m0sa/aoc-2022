using System.Text;

namespace day17;

public class Input : Day17
{
    public override long Part1Result { get; } = 3141;
    public override long Part2Result { get; } = 1561739130391;
}

public class Example : Day17
{
    private readonly ITestOutputHelper _output;
    public Example(ITestOutputHelper output) => _output = output;

    public override long Part1Result { get; } = 3068;
    public override long Part2Result { get; } = 1514285714288;

    [Fact]
    public void Part1Visual()
    {
        RunUntil(10);
        var sb = new StringBuilder().AppendLine();
        PrintTop(sb, 20);
        _output.WriteLine(sb.ToString());
        Assert.Equal(17, CurrentHeight);
    }
}

public abstract class Day17 : AOCDay
{
    private static Vec2DL[][] Shapes =
    {
        // @0@@
        new Vec2DL[] { new(-1, 0), new(0, 0), new(1, 0), new(2, 0) } ,
        //  @
        // @@@
        //  0
        new Vec2DL[] { new(0, 0), new(-1, 1), new(0, 1), new(1, 1), new(0, 2)},
        //   @
        //   @
        // @0@
        new Vec2DL[] { new(-1, 0), new(0, 0), new(1, 0), new(1, 1), new(1, 2)},
        // @
        // @
        // @
        // @0
        new Vec2DL[] { new(-1, 0), new(-1, 1), new(-1, 2), new(-1, 3) },
        // @@
        // @0
        new Vec2DL[] { new(-1, 0), new(0, 0), new(-1, 1), new(0, 1) },
    };

    protected long StoppedRockCount = 0;
    protected long Command = 0;
    protected long CurrentHeight = 0;
    protected List<ImmutableHashSet<Vec2DL>> StoppedRocks = new ();
    protected ImmutableHashSet<Vec2DL> StoppedCoordinates = ImmutableHashSet<Vec2DL>.Empty;
    protected ImmutableHashSet<Vec2DL> FallingRock = ImmutableHashSet<Vec2DL>.Empty;
    private static Vec2DL FallingTransform = new Vec2DL(0, -1);

    public override long Part1()
    {
        RunUntil(2022);
        return CurrentHeight;
    }
    public override long Part2()
    {
        RunUntil(1000000000000);
        return CurrentHeight;
    }

    protected void RunUntil(long rockCount)
    {
        var commands = Input.Single().ToCharArray();

        var inputTransform = new Vec2DL(0, 0);

        var sb = new StringBuilder();
        var cache = new Dictionary<string, (long rocks, long height)>();

        while (StoppedRockCount < rockCount)
        {
            processInput();
            updateState();
        }

        void processInput()
        {
            var input = commands[Command % commands.LongLength];
            inputTransform = input switch
                {
                    '>' => new Vec2DL(1, 0),
                    '<' => new Vec2DL(-1, 0),
                    _ => throw new InvalidOperationException($"Unexpected input: {input}")
                };
            Command++;
        }

        void updateState()
        {
            if (FallingRock.IsEmpty)
            {
                Vec2DL offset =  new (3, CurrentHeight + 3);
                FallingRock = Shapes[StoppedRockCount % Shapes.Length].Select(c => c + offset).ToImmutableHashSet();
            }
            FallingRock = TryMove(FallingRock, inputTransform, out var afterLR) ? afterLR : FallingRock;
            if (TryMove(FallingRock, FallingTransform, out var afterFall))
            {
                FallingRock = afterFall;
            }
            else
            {
                StoppedRockCount++;
                StoppedRocks.Add(FallingRock);
                StoppedCoordinates = StoppedRocks.SelectMany(x => x).ToImmutableHashSet();
                CurrentHeight = Math.Max(CurrentHeight, FallingRock.Select(c => c.Y).Max() + 1);
                FallingRock = ImmutableHashSet<Vec2DL>.Empty;

                if (StoppedRocks.Count > 400) StoppedRocks.RemoveAt(0);

                sb.Clear();
                sb.AppendFormat("Tape: {0}, Rock: {1}", Command % commands.LongLength, StoppedRockCount % Shapes.Length).AppendLine();
                PrintTop(sb, 200);
                var cacheKey = sb.ToString();
                if (cache.TryGetValue(cacheKey, out var seen))
                {
                    var deltaHeight = CurrentHeight - seen.height;
                    var deltaRocks = StoppedRockCount - seen.rocks;
                    var multiple = 0;
                    do
                    {
                        multiple++;
                    } while (StoppedRockCount + ((multiple + 1) * deltaRocks) < rockCount);
                    StoppedRockCount += multiple * deltaRocks;
                    var deltaY = multiple * deltaHeight;
                    var delta = new Vec2DL(0, deltaY);
                    CurrentHeight += deltaY;
                    StoppedRocks = StoppedRocks.Select(r => r.Select(c => c + delta).ToImmutableHashSet()).ToList();
                    StoppedCoordinates = StoppedRocks.SelectMany(x => x).ToImmutableHashSet();
                    cache.Clear();
                }

                cache[cacheKey] = (StoppedRockCount, CurrentHeight);
            }
        }
    }

    protected void PrintTop(StringBuilder printer, int topLines)
    {
        for (var t = 0; t < Math.Min(topLines, CurrentHeight); t++)
        {
            var y = CurrentHeight - t - 1;
            for (var x = 0; x < 7; x++)
            {
                if (StoppedCoordinates.Contains(new (x, y)))
                {
                    printer.Append('#');
                }
                else
                {
                    printer.Append('.');
                }
            }
            printer.AppendLine();
        }
    }

    protected bool TryMove(ImmutableHashSet<Vec2DL> fallingRock, Vec2DL transform, out ImmutableHashSet<Vec2DL> next)
    {
        next = fallingRock.Select(x => x + transform).ToImmutableHashSet();

        var bounds = next.Aggregate(
            (min: new Vec2DL(int.MaxValue, int.MaxValue), max: new Vec2DL(int.MinValue, int.MinValue), overlap: false),
            (agg, c) => (
                min: new Vec2DL(Math.Min(agg.min.X, c.X), Math.Min(agg.min.Y, c.Y)),
                max: new Vec2DL(Math.Max(agg.max.X, c.X), Math.Max(agg.max.Y, c.Y)),
                overlap: agg.overlap || StoppedCoordinates.Contains(c)));
        var isOOB = bounds.min.X < 0 || bounds.min.Y < 0 || bounds.max.X > 6;
        return !isOOB && !bounds.overlap;
    }
}
