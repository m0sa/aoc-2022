namespace day03;

public class Input : Day03
{
    public override long Part1Result { get; } = 7817;
    public override long Part2Result { get; } = 2444;
}

public class Example : Day03
{
    public override long Part1Result { get; } = 157;
    public override long Part2Result { get; } = 70;

    private const string example = """
        vJrwpWtwJgWrhcsFMMfFFhFp
        jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL
        PmmdzqPrVvPwwTWBwg
        wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn
        ttgJtRGJQctTZtZT
        CrZsJsPPZsGzwwsLwLmpwMDw
        """;

    protected override IEnumerable<string> Input { get; } = example.Split(Environment.NewLine);

    [Theory]
    [InlineData('a', 1)]
    [InlineData('z', 26)]
    [InlineData('A', 27)]
    [InlineData('Z', 52)]
    public void Priorities(char c, int expected) => Assert.Equal(expected, Priority(c));

    [Theory]
    [InlineData(0, 'p')]
    [InlineData(1, 'L')]
    [InlineData(2, 'P')]
    [InlineData(3, 'v')]
    [InlineData(4, 't')]
    [InlineData(5, 's')]
    public void Part1_Partials(int line, char expected) => Assert.Equal(expected, GetSharedItems(Input.ElementAt(line)).Single());
}

public abstract class Day03
{
    public abstract long Part1Result { get; }
    public abstract long Part2Result { get; }
    protected virtual IEnumerable<string> Input => this.EmbeddedResourceLines();
    protected static long Priority(char c)
    {
        var total = 0;
        if (char.IsUpper(c))
        {
            total += 26;
            c = char.ToLower(c);
        }
        total += ((int)c - 96);
        return total;
    }

    protected static IEnumerable<char> GetSharedItems(string line)
    {
        var half = line.Length / 2;
        var half1 = line.Substring(0, half);
        var half2 = line.Substring(half, line.Length - half1.Length);
        return GetSharedItems(new [] { half1, half2 });
    }

    protected static IEnumerable<char> GetSharedItems(IEnumerable<string> groups)
    {
        var common = groups.First().ToHashSet();
        foreach(var next in groups.Skip(1))
        {
            common.IntersectWith(next);
        }
        return common;
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(Part1Result, Input.SelectMany(GetSharedItems).Select(Priority).Sum());
    }

    protected static IEnumerable<IEnumerable<string>> GroupsOfThree(IEnumerable<string> lines)
    {
        var three = new List<string>(3);
        foreach(var line in lines)
        {
            three.Add(line);
            if (three.Count == 3)
            {
                yield return three.ToList();
                three.Clear();
            }
        }
    }


    [Fact]
    public void Part2()
    {
        Assert.Equal(Part2Result, GroupsOfThree(Input).Select(three => GetSharedItems(three).Single()).Select(Priority).Sum());
    }
}

