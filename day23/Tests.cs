namespace day23;

public class Input : Day23
{
    public override long Part1Result { get; } = 3931;
    public override long Part2Result { get; } = -1;
}

public class Example : Day23
{
    public override long Part1Result { get; } = 110;
    public override long Part2Result { get; } = -1;
}

public abstract class Day23 : AOCDay
{
    public override long Part1()
    {
        var state = InitialState;

        for (var i = 0; i < 10; i++)
        {
            state = Round(
                state,
                elf => Enumerable
                    .Range(0, proposals.Length)
                    .Select(j => proposals[(j + i) % proposals.Length].Invoke(state, elf))
                    .FirstOrDefault(x => x.HasValue));
        }

        var bounds = state.Aggregate(
            new { min = new Vec2D(int.MaxValue, int.MaxValue), max = new Vec2D(int.MinValue, int.MinValue)},
            (agg, cur) => new {
                min = new Vec2D(Math.Min(agg.min.X, cur.X), Math.Min(agg.min.Y, cur.Y)),
                max = new Vec2D(Math.Max(agg.max.X, cur.X), Math.Max(agg.max.Y, cur.Y))
            });
        var area = (bounds.max.X - bounds.min.X + 1) * (bounds.max.Y - bounds.min.Y + 1);
        return area - state.Count;
    }

    public override long Part2() => -1;

    private ImmutableHashSet<Vec2D>? _initialState;
    protected ImmutableHashSet<Vec2D> InitialState => _initialState ??=
        Input
            .SelectMany((line, y) => line
                .Select((ch, x) => (ch, new Vec2D(x, y)))
                .Where(i => i is ('#', _))
                .Select(i => i.Item2))
            .ToImmutableHashSet();

    protected static ImmutableHashSet<Vec2D> Round(ImmutableHashSet<Vec2D> elves, Func<Vec2D, Vec2D?> proposeMove)
    {
        // 1st half
        var moveCandidates = elves.Where(elf => !IsEmpty(compass, elf, elves));
        var proposedMoves = moveCandidates
            .Select(elf => new { elf, proposal = proposeMove(elf) })
            .Where(p => p.proposal.HasValue)
            .ToLookup(p => p.proposal!.Value, p => p.elf);

        // 2nd half
        var fromTo = proposedMoves.Where(x => x.Count() == 1).ToImmutableDictionary(x => x.Single(), x => x.Key);
        return elves.Except(fromTo.Keys).Union(fromTo.Values);
    }
    private static readonly Vec2D N = new(0, -1), NE = new(1, -1), E = new(1, 0), SE = new(1, 1), S = new(0, 1), SW = new(-1, 1), W= new(-1, 0), NW = new(-1, -1);
    private static readonly Vec2D[] compass = { N, NE, E, SE, S, SW, W, NW }, north = { NW, N, NE }, east = { NE, E, SE }, south = { SE, S, SW }, west = { SW, W, NW };
    private static bool IsEmpty(Vec2D[] direction, Vec2D elf, ISet<Vec2D> elves) => !direction.Select(x => x+elf).Any(elves.Contains);

    private delegate Vec2D? ProposeMove(ISet<Vec2D> others, Vec2D current);
    private static ProposeMove BuildProposal(Vec2D[] directionMustBeEmpty, Vec2D directionToMove)
    {
        return (others, current) => {
            if (IsEmpty(directionMustBeEmpty, current, others)) return current + directionToMove;
            else return null;
        };
    }
    private static readonly ProposeMove[] proposals = { BuildProposal(north, N), BuildProposal(south, S), BuildProposal(west, W), BuildProposal(east, E) };
}
