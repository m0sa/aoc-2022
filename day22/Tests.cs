namespace day22;

public class Input : Day22
{
    public override long Part1Result { get; } = 65368;
    public override long Part2Result { get; } = -1;
}

public class Example : Day22
{
    public override long Part1Result { get; } = 6032;
    public override long Part2Result { get; } = -1;

    [Fact]
    public void CheckStartingPoint() => Assert.Equal(new Vec2D(9, 1), StartingPoint);
}

public abstract class Day22 : AOCDay
{
    public enum Tile { Path, Wall }
    private ImmutableDictionary<Vec2D, Tile>? _map;
    protected ImmutableDictionary<Vec2D, Tile> Map => _map ??= Input
        .TakeWhile(x => x != "")
        .SelectMany((lineStr, lineNum) => lineStr
            .Select((columnChar, columnNum) => (columnChar, columnNum))
            .Where(c => c is not (' ', _))
            .Select(c => new KeyValuePair<Vec2D, Tile>(
                new(c.columnNum + 1, lineNum + 1),
                c.columnChar switch { '.' => Tile.Path, '#' => Tile.Wall }))
        ).ToImmutableDictionary();

    protected Vec2D StartingPoint => new(Map.Keys.MinBy(v => v is (int x, 1) ? x : int.MaxValue).X, 1);
    protected static Vec2D[] Directions =
    {
        new Vec2D(1, 0),  // >
        new Vec2D(0, 1),  // v
        new Vec2D(-1, 0), // <
        new Vec2D(0, -1), // ^
    };
    protected enum Turn { Left, Right }
    protected record struct Instruction(int? Moves = null, Turn? Turn = null);
    protected IEnumerable<Instruction> Instructions()
    {
        var tape = Input.Last();
        char[] lr = { 'L', 'R' };
        for (var position = 0; position < tape.Length; )
        {
            if (tape[position] switch { 'L' => Turn.Left, 'R' => Turn.Right, _ => (Turn?)null } is Turn turn)
            {
                yield return new Instruction(Turn: turn);
                position++;
            }
            else
            {
                var nextTurn = tape.IndexOfAny(lr, position) is int i && i != -1 ? i : tape.Length;
                var stepsLength = nextTurn - position;
                var steps = int.Parse(tape.AsSpan(position, stepsLength));
                yield return new Instruction(steps);
                position += stepsLength;
            }
        }
    }

    public override long Part1()
    {
        var (facing, position) = Instructions()
            .Aggregate((facing: 0, position: StartingPoint), (agg, move) =>
                move switch
                {
                    (_, Turn turn) => (
                        facing: (Directions.Length + agg.facing + (turn == Turn.Right ? 1 : -1)) % Directions.Length,
                        position: agg.position
                    ),
                    (int steps, _) => (
                        facing: agg.facing,
                        position: Move(agg.position, Directions[agg.facing], steps)
                    ),
                });

        return 1000 * position.Y + 4 * position.X + facing;
    }

    private Vec2D? _bounds;
    protected Vec2D Max => _bounds ??= Map.Keys.Aggregate((agg, cur) => new Vec2D(Math.Max(agg.X, cur.X), Math.Max(agg.Y, cur.Y)));

    Vec2D Move(Vec2D position, Vec2D direction, int steps)
    {
        if (steps == 0)
        {
            return position;
        }

        Tile nextTile;
        var nextPosition = position;
        do
        {
            nextPosition = new(
                ((Max.X + nextPosition.X + direction.X - 1) % Max.X) + 1,
                ((Max.Y + nextPosition.Y + direction.Y - 1) % Max.Y) + 1);
        } while (!Map.TryGetValue(nextPosition, out nextTile));

        return nextTile switch
        {
            Tile.Path => Move(nextPosition, direction, steps - 1),
            Tile.Wall => position,
        };
    }

    public override long Part2() => -1;
}
