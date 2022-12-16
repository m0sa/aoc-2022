using System.Text.RegularExpressions;

namespace day16;

public class Input : Day16
{
    public override long Part1Result { get; } = 1991;
    public override long Part2Result { get; } = -1;
}

public class Example : Day16
{
    public override long Part1Result { get; } = 1651;
    public override long Part2Result { get; } = 1707;
}

public abstract class Day16 : AOCDay
{
    protected record Valve(string Name, ImmutableList<string> LeadsTo, int FlowRate);

    private ImmutableList<Valve>? _map;
    protected ImmutableList<Valve> Map => _map ??= Input
            .Select(l => Regex.Match(l,
                "Valve (?<Name>[A-Z]+) has flow rate=(?<FlowRate>[0-9]+); tunnels? leads? to valves? ((?<LeadsTo>[A-Z]+)(, )?)+"))
            .Select(m => new Valve(
                m.Groups["Name"].Value,
                m.Groups["LeadsTo"].Captures.Cast<Capture>().Select(c => c.Value).ToImmutableList(),
                int.Parse(m.Groups["FlowRate"].Value)))
            .ToImmutableList();
    private ImmutableDictionary<string, Valve>? _lookupByName;
    protected Valve FindValve(string name) => (_lookupByName ??= Map.ToImmutableDictionary(v=>v.Name, v => v))[name];

    private ImmutableDictionary<Valve, ImmutableDictionary<Valve, int>>? _distances;
    protected ImmutableDictionary<Valve, ImmutableDictionary<Valve, int>> Distances => _distances ??= Map.ToImmutableDictionary(v => v, v => ShortestPaths(v));

    protected ImmutableDictionary<Valve, int> ShortestPaths(Valve from)
    {
        var dis = Map.ToDictionary(c => c, c => int.MaxValue);
        var queue = Map.ToList();
        bool TryDequeue(out Valve element)
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
        void UpdateQueue(Valve element, int distance)
        {
            dis[element] = distance;
            queue.Sort((v1, v2) => dis[v1].CompareTo(dis[v2]));
        }

        UpdateQueue(from, 0);
        while(TryDequeue(out var current))
        {
            var nextDistance = dis[current] + 1;
            foreach (var child in
                from c in current.LeadsTo.Select(FindValve)
                where dis.TryGetValue(c, out var childDistance)
                   && nextDistance < childDistance
                select c)
            {
                UpdateQueue(child, nextDistance);
            }
        }
        return dis.ToImmutableDictionary();
    }

    protected ImmutableHashSet<Valve> ClosedValves() => Map.Where(x => x.FlowRate > 0).ToImmutableHashSet();

    public override long Part1() => SolvePart1(0, ClosedValves(), new (30, FindValve("AA")));
    private const int ValveOpeningTime = 1;
    protected int SolvePart1(int gain, ImmutableHashSet<Valve> closedValves, State current)
    {
        var potentialMoves = FindPotentialMoves(current, closedValves);
        return potentialMoves.IsEmpty
            ? gain
            : potentialMoves.Max(m => SolvePart1(gain + m.Gain, closedValves.Remove(m.Valve), m));
    }

    public override long Part2() => SolvePart2(0, ClosedValves(), new (26, FindValve("AA")), new (26, FindValve("AA")));

    protected record State(int RemainingTime, Valve Valve)
    {
        public int Gain => RemainingTime * Valve.FlowRate;
    }

    protected ImmutableList<State> FindPotentialMoves(State player, ImmutableHashSet<Valve> closedValves) =>
        (
            from nextClosed in closedValves
            let TravelTime = Distances[player.Valve][nextClosed]
            let RemainingTime = player.RemainingTime - TravelTime - ValveOpeningTime
            where RemainingTime > 0
            select new State(RemainingTime, nextClosed)
        ).ToImmutableList();

    protected int SolvePart2(int gain, ImmutableHashSet<Valve> closedValves, State me, State other)
    {
        var potentialMoves = FindPotentialMoves(me, closedValves);
        return potentialMoves.IsEmpty
            ? SolvePart1(gain, closedValves, other)
            : potentialMoves.Max(m => SolvePart2(gain + m.Gain, closedValves.Remove(m.Valve), other, m));
    }
}
