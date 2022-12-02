namespace day02;

public class Example : Day02
{
    public override long Part1Result { get; } = 15;
    public override long Part2Result { get; } = 12;
}

public class Input : Day02
{
    public override long Part1Result { get; } = 9177;
    public override long Part2Result { get; } = -1;
}

public abstract class Day02
{
    public abstract long Part1Result { get; }
    public abstract long Part2Result { get; }

    [Fact]
    public void Part1()
    {
        Assert.Equal(Part1Result, Input.Select(RoundScore).Sum());
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(Part2Result, Input.Select(GetOutcome).Select(RoundScore).Sum());

        static string GetOutcome(string strategy)
        {
            switch (strategy)
            {
                case "A X": return "A Y"; // L
                case "A Y": return "A X"; // D EX
                case "A Z": return "A Z"; // W
                case "B X": return "B X"; // L EX
                case "B Y": return "B Y"; // D
                case "B Z": return "B Z"; // W
                case "C X": return "C Y"; // L
                case "C Y": return "C Z"; // D
                case "C Z": return "C X"; // W EX
                default:
                    throw new UnreachableException($"Unexpected strategy: {strategy}");
            };
        }
    }

    protected IEnumerable<string> Input => this
        .EmbeddedResourceLines();

    public static long RoundScore(string outcome)
    {
        switch (outcome)
        {
            case "A X": return 1 + 3; // OK
            case "A Y": return 2 + 6; // EX
            case "A Z": return 3 + 0; // OK
            case "B X": return 1 + 0; // EX
            case "B Y": return 2 + 3; // OK
            case "B Z": return 3 + 6; // OK
            case "C X": return 1 + 6; // OK
            case "C Y": return 2 + 0; // OK
            case "C Z": return 3 + 3; // EX
            default:
                throw new UnreachableException($"Unexpected round: {outcome}");
        };
    }
}

