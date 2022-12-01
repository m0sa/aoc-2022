namespace day01;

public class Example : Day01
{
    public override int Part1Result { get; } = 24000;
    public override int Part2Result { get; } = 45000;
}

public class Input : Day01
{
    public override int Part1Result { get; } = 65912;
    public override int Part2Result { get; } = 195625;
}

public abstract class Day01
{
    public abstract int Part1Result { get; }
    public abstract int Part2Result { get; }

    [Fact]
    public void Part1()
    {
        Assert.Equal(Part1Result, SuppliesPerElve().Max());
    }

    [Fact]
    public void Part2()
    {
        Assert.Equal(Part2Result, SuppliesPerElve().OrderByDescending(x => x).Take(3).Sum());
    }

    protected record Line(int? value);
    protected IEnumerable<Line> Input => this
        .EmbeddedResourceLines()
        .Select(x => string.IsNullOrEmpty(x) ? new Line(null) : new (int.Parse(x)));

    public IEnumerable<int> SuppliesPerElve()
    {
        var current = 0;
        foreach (var line in Input)
        {
            switch (line) {
                case Line(null):
                    yield return current;
                    current = 0;
                    break;
                case Line(int x):
                    current += x;
                    break;
                default:
                    throw new UnreachableException();
            };
        }
        if (current > 0)
            yield return current;
    }
}

