using System.Text;

namespace day10;

public class Input : Day10
{
    public override long Part1Result { get; } = 14920;
    public override string Part2Result { get; } =
        """
        ###..#..#..##...##...##..###..#..#.####.
        #..#.#..#.#..#.#..#.#..#.#..#.#..#....#.
        ###..#..#.#....#..#.#....###..#..#...#..
        #..#.#..#.#....####.#....#..#.#..#..#...
        #..#.#..#.#..#.#..#.#..#.#..#.#..#.#....
        ###...##...##..#..#..##..###...##..####.
        """;
}

public class Example : Day10
{
    public override long Part1Result { get; } = 13140;
    public override string Part2Result { get; } =
        """
        ##..##..##..##..##..##..##..##..##..##..
        ###...###...###...###...###...###...###.
        ####....####....####....####....####....
        #####.....#####.....#####.....#####.....
        ######......######......######......####
        #######.......#######.......#######.....
        """;
}


public abstract class Day10 : AOCDay<long, string>
{
    protected record struct State(int Cycle, long X, long X_durring);
    protected static Func<long, long> Noop = x => x;

    private ImmutableList<State>? _states;
    protected ImmutableList<State> States => _states ??= base
        .Input
        .SelectMany(line => line switch {
            "noop" => Enumerable.Repeat(Noop, 1),
            string addx when addx.StartsWith("addx ") && int.TryParse(addx.Substring("addx ".Length), out var toAdd) => new [] { Noop, x => x + toAdd },
        })
        .Select((op, i) => new { operation = op, cycle = i+1 })
        .Aggregate(
            new [] { new State(0, 1, 1) }.ToImmutableList(),
            (agg, cur) => agg.Add(new State(cur.cycle, cur.operation(agg[agg.Count - 1].X), agg[agg.Count - 1].X)));

    public override long Part1()
    {
        var signalStrenght = 0L;
        var next = 20;
        foreach (var state in States)
        {
            if (state.Cycle == next)
            {
                signalStrenght += state.X_durring * state.Cycle;
                next += 40;
            }
        }

        return signalStrenght;
    }

    public override string Part2()
    {
        var output = new StringBuilder();
        foreach (var state in States.Skip(1))
        {
            var spritePixelsStart = state.X_durring - 1;
            var spritePixelsEnd = spritePixelsStart + 3;
            var pixelPos = (state.Cycle - 1) % 40;

            output.Append(pixelPos >= spritePixelsStart && pixelPos < spritePixelsEnd ? '#' : '.');
            if (pixelPos == 39) {
                output.AppendLine();
            }
        }
        return output.ToString().Trim();
    }
}
