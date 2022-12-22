using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace day22;

public class Input : Day22
{
    public override long Part1Result { get; } = 65368;
    public override long Part2Result { get; } = 156166;
    protected override int CubeSize { get; } = 50;

    protected override State Part2Transform(State state)
    {
        var grid = ToGrid(state.Position);
        var off = SideOffset(state.Position);

        var nextState = (state.Facing, grid.X, grid.Y) switch
        {
            (Facing.Up,    1, 3) => new State(Facing.Right, new(Grid(2).Start,                Grid(2).Start + off.Normal.X)),
            (Facing.Up,    2, 1) => new State(Facing.Right, new(Grid(1).Start,                Grid(4).Start + off.Normal.X)),
            (Facing.Up,    3, 1) => new State(Facing.Up,    new(Grid(1).Start + off.Normal.X, Grid(4).End)),
            (Facing.Right, 3, 1) => new State(Facing.Left,  new(Grid(2).End,                  Grid(3).Start + off.Inverse.Y)),
            (Facing.Right, 2, 2) => new State(Facing.Up,    new(Grid(3).Start + off.Normal.Y, Grid(1).End)),
            (Facing.Right, 2, 3) => new State(Facing.Left,  new(Grid(3).End,                  Grid(1).Start + off.Inverse.Y)),
            (Facing.Right, 1, 4) => new State(Facing.Up,    new(Grid(2).Start + off.Normal.Y, Grid(3).End)),
            (Facing.Down,  1, 4) => new State(Facing.Down,  new(Grid(3).Start + off.Normal.X, Grid(1).Start)),
            (Facing.Down,  2, 3) => new State(Facing.Left,  new(Grid(1).End,                  Grid(4).Start + off.Normal.X)),
            (Facing.Down,  3, 1) => new State(Facing.Left,  new(Grid(2).End,                  Grid(2).Start + off.Normal.X)),
            (Facing.Left,  2, 1) => new State(Facing.Right, new(Grid(1).Start,                Grid(3).Start + off.Inverse.Y)),
            (Facing.Left,  2, 2) => new State(Facing.Down,  new(Grid(1).Start + off.Normal.Y, Grid(3).Start)),
            (Facing.Left,  1, 3) => new State(Facing.Right, new(Grid(2).Start,                Grid(1).Start + off.Inverse.Y)),
            (Facing.Left,  1, 4) => new State(Facing.Down,  new(Grid(2).Start + off.Normal.Y, Grid(1).Start)),
            _ => throw new Exception($"unexpected transition {grid} heading {state.Facing}"),
        };
        return nextState;
    }
}

public class Example : Day22
{
    public override long Part1Result { get; } = 6032;
    public override long Part2Result { get; } = 5031;

    protected override int CubeSize { get; } = 4;

    protected override State Part2Transform(State state)
    {
        var grid = ToGrid(state.Position);
        var offset = SideOffset(state.Position);

        switch (state.Position)
        {
            case (12, _) when state.Facing == Facing.Right && grid.Y == 2:
                return new State(Facing.Down, new(Grid(4).Start + offset.Inverse.Y, Grid(3).Start));
            case (_, 12) when state.Facing == Facing.Down && grid.X == 3:
                return new State(Facing.Up, new(Grid(1).Start + offset.Inverse.X, Grid(2).End));
            case (_, 5) when state.Facing == Facing.Up && grid.X == 2:
                return new State(Facing.Right, new(Grid(3).Start, Grid(1).Start + offset.Normal.X));
            default:
                throw new NotImplementedException();
        }
    }
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
        for (var position = 0; position < tape.Length;)
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

    public override long Part1() => Solve(Teleport_Part1);

    public enum Facing { Right = 0, Down = 1, Left = 2, Up = 3 }
    protected record struct State(Facing Facing, Vec2D Position);
    protected long Solve(Func<State, State> teleport)
    {
        var (facing, position) = Instructions()
            .Aggregate(new State(Facing.Right, StartingPoint), (agg, move) =>
                move switch
                {
                    (_, Turn turn) => new(
                        (Facing)((Directions.Length + (int)agg.Facing + (turn == Turn.Right ? 1 : -1)) % Directions.Length),
                        agg.Position
                    ),
                    (int steps, _) => Move(agg, teleport, steps),
                });

        return 1000 * position.Y + 4 * position.X + (int)facing;
    }

    private Vec2D? _bounds;
    protected Vec2D Max => _bounds ??= Map.Keys.Aggregate((agg, cur) => new Vec2D(Math.Max(agg.X, cur.X), Math.Max(agg.Y, cur.Y)));

    private State Teleport_Part1(State state)
    {
        var (facing, position) = state;
        var direction = Directions[(int)facing];
        do
        {
            position = new(
                ((Max.X + position.X + direction.X - 1) % Max.X) + 1,
                ((Max.Y + position.Y + direction.Y - 1) % Max.Y) + 1);
        } while (!Map.ContainsKey(position));

        return new State(facing, position);
    }

    private State Move(State state, Func<State, State> teleport, int steps)
    {
        if (steps == 0)
        {
            return state;
        }

        var (facing, position) = state;
        Tile nextTile;
        var nextPosition = position + Directions[(int)facing];
        if (!Map.TryGetValue(nextPosition, out nextTile))
        {
            (facing, nextPosition) = teleport(state);
            nextTile = Map[nextPosition];
        }

        return nextTile switch
        {
            Tile.Path => Move(new(facing, nextPosition), teleport, steps - 1),
            Tile.Wall => state,
        };
    }

    public override long Part2() => Solve(Part2Transform);

    protected abstract State Part2Transform(State state);
    protected abstract int CubeSize { get; }

    protected (Vec2D Normal, Vec2D Inverse) SideOffset(Vec2D position)
    {
        Vec2D offset = new(
            1 + (position.X - 1) % CubeSize,
            1 + (position.Y - 1) % CubeSize);
        return (
            Normal: offset - new Vec2D(1, 1),
            Inverse: new Vec2D(CubeSize, CubeSize) - offset);
    }
    protected Vec2D ToGrid(Vec2D position) =>
        new(1 + (position.X - 1) / CubeSize,
            1 + (position.Y - 1) / CubeSize);

    protected (int Start, int End) Grid(int gridCoordinate) =>
        (1 + (gridCoordinate - 1) * CubeSize, gridCoordinate * CubeSize);
}
