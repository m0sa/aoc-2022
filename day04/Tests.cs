namespace day04;

public class Input : Day04
{
    public override long Part1Result { get; } = 498;
    public override long Part2Result { get; } = 859;
}

public class Example : Day04
{
    public override long Part1Result { get; } = 2;
    public override long Part2Result { get; } = 4;
}

public abstract class Day04
{
    public abstract long Part1Result { get; }
    public abstract long Part2Result { get; }
    protected virtual IEnumerable<string> Input => this.EmbeddedResourceLines();
    protected IEnumerable<(Range elf1, Range elf2)> ParsedInput()
    {
        return Input.Select(ParseLine);
        static (Range elf1, Range elf2) ParseLine(string line)
        {
            var split = line.Split(",");
            return (ParseRange(split[0]), ParseRange(split[1]));
        }
        static Range ParseRange(string range)
        {
            var split = range.Split("-");
            return new Range(new (int.Parse(split[0])), new (int.Parse(split[1])));
        }
    }

    [Fact]
    public void Part1()
    {
        var contains = ParsedInput().Count(x =>
            x.elf1.Start.Value <= x.elf2.Start.Value && x.elf1.End.Value >= x.elf2.End.Value ||
            x.elf2.Start.Value <= x.elf1.Start.Value && x.elf2.End.Value >= x.elf1.End.Value
        );
        Assert.Equal(Part1Result, contains);
    }


    [Fact]
    public void Part2()
    {
        var work = ParsedInput().ToArray();
        var hasIntersection = work.Count(x =>
        {
            var range1 = Enumerable.Range(x.elf1.Start.Value, x.elf1.End.Value - x.elf1.Start.Value + 1);
            var range2 = Enumerable.Range(x.elf2.Start.Value, x.elf2.End.Value - x.elf2.Start.Value + 1);
            return range1.Intersect(range2).Any();
        });
        Assert.Equal(Part2Result, hasIntersection);
    }
}

