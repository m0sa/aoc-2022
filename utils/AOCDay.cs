namespace utils;

public abstract class AOCDay
{
    public abstract long Part1Result { get; }
    public abstract long Part2Result { get; }

    public abstract long Part1();
    public abstract long Part2();

    protected virtual IEnumerable<string> Input => this.EmbeddedResourceLines();

    [Fact]
    public void TestPart1() => Assert.Equal(Part1Result, Part1());
    [Fact]
    public void TestPart2() => Assert.Equal(Part2Result, Part2());
}