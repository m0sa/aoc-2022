namespace day02;
using static Day02.Shape;
using static Day02.Outcome;

public class Input : Day02
{
    public override long Part1Result { get; } = 9177;
    public override long Part2Result { get; } = 12111;
}

public class Example : Day02
{
    public override long Part1Result { get; } = 15;
    public override long Part2Result { get; } = 12;

    private const string Line0 = "A Y";
    private const string Line1 = "B X";
    private const string Line2 = "C Z";

    protected override IEnumerable<string> Input { get; } = new[] { Line0, Line1, Line2 };

    [Theory]
    [InlineData(Line0, 8)]
    [InlineData(Line1, 1)]
    [InlineData(Line2, 6)]
    public void Part1_Partials(string line, int expected)
    {
        var round = ParseRound(line);
        Assert.Equal(expected, Points(round));
    }

    [Theory]
    [InlineData(Line0, 4)]
    [InlineData(Line1, 1)]
    [InlineData(Line2, 7)]
    public void Part2_Partials(string line, int expected)
    {
        var strategy = ParseStrategy(line);
        var round = StrategyToRound(strategy);
        Assert.Equal(expected, Points(round));
    }
}

public abstract class Day02
{
    protected virtual IEnumerable<string> Input => this.EmbeddedResourceLines();
    public abstract long Part1Result { get; }
    public abstract long Part2Result { get; }

    public enum Shape : byte { Rock, Paper, Scissors }
    protected static byte Points(Shape shape) => shape switch { Rock => 1, Paper => 2, Scissors => 3, _ => throw new ArgumentException() };

    public enum Outcome : byte { Loose, Draw, Win }
    protected static byte Points(Outcome outcome) => outcome switch { Loose => 0, Draw => 3, Win => 6, _ => throw new ArgumentException() };

    protected record Round(Shape Them, Shape Me);
    protected static long Points(Round round) => Points(round.Me) + Points(round switch
    {
        (Rock, Rock) => Draw,
        (Rock, Paper) => Win,
        (Rock, Scissors) => Loose,
        (Paper, Rock) => Loose,
        (Paper, Paper) => Draw,
        (Paper, Scissors) => Win,
        (Scissors, Rock) => Win,
        (Scissors, Paper) => Loose,
        (Scissors, Scissors) => Draw,
        _ => throw new ArgumentException(),
    });

    [Fact]
    public void Part1()
    {
        Assert.Equal(Part1Result, Input.Select(ParseRound).Select(Points).Sum());
    }

    protected static Round ParseRound(string line) => new Round(
        line[0] switch { 'A' => Rock, 'B' => Paper, 'C' => Scissors, char e => throw new Exception($"Unexpected shape: {e}") },
        line[2] switch { 'X' => Rock, 'Y' => Paper, 'Z' => Scissors, char e => throw new Exception($"Unexpected shape: {e}") });


    [Fact]
    public void Part2()
    {
        Assert.Equal(Part2Result, Input.Select(ParseStrategy).Select(StrategyToRound).Select(Points).Sum());
    }


    protected record Strategy(Shape them, Outcome outcome);

    protected static Strategy ParseStrategy(string line) => new Strategy(
        line[0] switch { 'A' => Rock, 'B' => Paper, 'C' => Scissors, char e => throw new Exception($"Unexpected shape: {e}") },
        line[2] switch { 'X' => Loose, 'Y' => Draw, 'Z' => Win, char e => throw new Exception($"Unexpected strategy: {e}") });

    protected Round StrategyToRound(Strategy strategy) => strategy switch
    {
        (Rock, Loose) => new Round(Rock, Scissors),
        (Rock, Draw) => new Round(Rock, Rock),
        (Rock, Win) => new Round(Rock, Paper),
        (Paper, Loose) => new Round(Paper, Rock),
        (Paper, Draw) => new Round(Paper, Paper),
        (Paper, Win) => new Round(Paper, Scissors),
        (Scissors, Loose) => new Round(Scissors, Paper),
        (Scissors, Draw) => new Round(Scissors, Scissors),
        (Scissors, Win) => new Round(Scissors, Rock),
        _ => throw new ArgumentException(),
    };
}

