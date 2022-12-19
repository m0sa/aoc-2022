using System.Text.RegularExpressions;

namespace day19;

public class Input : Day19
{
    public override long Part1Result { get; } = 1395;
    public override long Part2Result { get; } = 2700;
}

public class Example : Day19
{
    public override long Part1Result { get; } = 33;
    public override long Part2Result { get; } = 52 * 62;

    [Theory]
    [InlineData(24, 1, 9)]
    [InlineData(24, 2, 12)]
    [InlineData(32, 1, 56)]
    [InlineData(32, 2, 62)]
    public void ExampleScenario(int time, int blueprintId, int max) => Assert.Equal(max, MaxFor(Blueprints[blueprintId], time));
}

public record Blueprint(
    int OreRobotOreCost,
    int ClayRobotOreCost,
    int ObsidianRobotOreCost,
    int ObsidianRobotClayCost,
    int GeodeRobotOreCost,
    int GeodeRobotObsidianCost);

public abstract class Day19 : AOCDay
{
    private ImmutableDictionary<int, Blueprint>? _blueprints;
    protected ImmutableDictionary<int, Blueprint> Blueprints => _blueprints ??=
        Input
            .Select(line => Regex.Match(line, @"Blueprint (?<Id>\d+): Each ore robot costs (?<OreRobot_OreCost>\d+) ore. Each clay robot costs (?<ClayRobot_OreCost>\d+) ore. Each obsidian robot costs (?<ObsidianRobot_OreCost>\d+) ore and (?<ObsidianRobot_ClayCost>\d+) clay. Each geode robot costs (?<GeodeRobot_OreCost>\d+) ore and (?<GeodeRobot_ObsidianCost>\d+) obsidian."))
            .ToImmutableDictionary(
                m => int.Parse(m.Groups["Id"].Value),
                m => new Blueprint(
                    OreRobotOreCost: int.Parse(m.Groups["OreRobot_OreCost"].Value),
                    ClayRobotOreCost: int.Parse(m.Groups["ClayRobot_OreCost"].Value),
                    ObsidianRobotOreCost: int.Parse(m.Groups["ObsidianRobot_OreCost"].Value),
                    ObsidianRobotClayCost: int.Parse(m.Groups["ObsidianRobot_ClayCost"].Value),
                    GeodeRobotOreCost: int.Parse(m.Groups["GeodeRobot_OreCost"].Value),
                    GeodeRobotObsidianCost: int.Parse(m.Groups["GeodeRobot_ObsidianCost"].Value)));

    protected record struct State(int OreRobots, int ClayRobots, int ObsidianRobots, int GeodeRobots, int OreReserve, int ClayReserve, int ObsidianReserve, int GeodeReserve, int MinutesRemaining);

    protected long MaxFor(Blueprint blueprint, int minutes = 24)
    {
        var toExplore = new List<State>
        {
            default(State) with { OreRobots = 1, MinutesRemaining = minutes }
        };
        var visited = new HashSet<State>();
        var max = 0L;
        var maxOreCost = new[]
        {
            blueprint.OreRobotOreCost,
            blueprint.ClayRobotOreCost,
            blueprint.ObsidianRobotOreCost,
            blueprint.GeodeRobotOreCost
        }.Max();
        var maxClayCost = blueprint.ObsidianRobotClayCost;
        var maxObsidianCost = blueprint.GeodeRobotObsidianCost;
        while (toExplore.Count > 0)
        {
            var current = toExplore[0];
            toExplore.RemoveAt(0);

            max = Math.Max(max, current.GeodeReserve);

            var time = current.MinutesRemaining;
            if (time == 0) continue;

            var nextTime = time - 1;
            current = current with
            {
                OreRobots = Math.Min(current.OreRobots, maxOreCost),
                ClayRobots = Math.Min(current.ClayRobots, maxClayCost),
                ObsidianRobots = Math.Min(current.ObsidianRobots, maxObsidianCost),
            };
            current = current with
            {
                OreReserve = Math.Min(current.OreReserve, time * maxOreCost - current.OreRobots * nextTime),
                ClayReserve = Math.Min(current.ClayReserve, time * maxClayCost - current.ClayRobots * nextTime),
                ObsidianReserve = Math.Min(current.ObsidianReserve, time * maxObsidianCost - current.ObsidianRobots * nextTime),
            };

            if (!visited.Add(current)) continue;

            var stockpile = current with
            {
                OreReserve = current.OreReserve + current.OreRobots,
                ClayReserve = current.ClayReserve + current.ClayRobots,
                ObsidianReserve = current.ObsidianReserve + current.ObsidianRobots,
                GeodeReserve = current.GeodeReserve + current.GeodeRobots,
                MinutesRemaining = nextTime,
            };
            toExplore.Insert(0, stockpile);

            if (current.OreReserve >= blueprint.OreRobotOreCost)
            {
                toExplore.Insert(0, stockpile with
                {
                    OreRobots = current.OreRobots + 1,
                    OreReserve = stockpile.OreReserve - blueprint.OreRobotOreCost,
                });
            }

            if (current.OreReserve >= blueprint.ClayRobotOreCost)
            {
                toExplore.Insert(0, stockpile with
                {
                    ClayRobots = current.ClayRobots + 1,
                    OreReserve = stockpile.OreReserve - blueprint.ClayRobotOreCost,
                });
            }

            if (current.OreReserve >= blueprint.ObsidianRobotOreCost && current.ClayReserve >= blueprint.ObsidianRobotClayCost)
            {
                toExplore.Insert(0, stockpile with
                {
                    ObsidianRobots = current.ObsidianRobots + 1,
                    OreReserve = stockpile.OreReserve - blueprint.ObsidianRobotOreCost,
                    ClayReserve = stockpile.ClayReserve - blueprint.ObsidianRobotClayCost,
                });
            }

            if (current.OreReserve >= blueprint.GeodeRobotOreCost && current.ObsidianReserve >= blueprint.GeodeRobotObsidianCost)
            {
                toExplore.Insert(0, stockpile with
                {
                    GeodeRobots = current.GeodeRobots + 1,
                    OreReserve = stockpile.OreReserve - blueprint.GeodeRobotOreCost,
                    ObsidianReserve = stockpile.ObsidianReserve - blueprint.GeodeRobotObsidianCost,
                });
            }
        }

        return max;
    }

    public override long Part1() => Blueprints
        .Select(kvp => kvp.Key * MaxFor(kvp.Value, 24))
        .Sum();

    public override long Part2() => Blueprints
        .OrderBy(kvp => kvp.Key)
        .Take(3)
        .AsParallel()
        .Select(kvp => MaxFor(kvp.Value, 32))
        .Aggregate(1L, (agg, cur) => agg * cur);
}
