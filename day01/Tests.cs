namespace day01;

public class Example : Day01
{
    public override long Part1Result { get; } = 24000;
    public override long Part2Result { get; } = 45000;
}

public class Input : Day01
{
    public override long Part1Result { get; } = 65912;
    public override long Part2Result { get; } = 195625;
}

public abstract class Day01 : AOCDay
{
    public override long Part1() => SuppliesPerElve().Max();
    public override long Part2() => SuppliesPerElve().OrderByDescending(x => x).Take(3).Sum();

    protected record Line(int? value);
    protected IEnumerable<Line> ParsedInput => Input
        .Select(x => string.IsNullOrEmpty(x) ? new Line(null) : new(int.Parse(x)));

    public IEnumerable<int> SuppliesPerElve()
    {
        var current = 0;
        foreach (var line in ParsedInput)
        {
            switch (line)
            {
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
