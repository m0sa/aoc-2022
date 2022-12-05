namespace utils;

public interface IAOCDay { }
public abstract class AOCDay : AOCDay<long> {}
public abstract class AOCDay<T> : IAOCDay
{
    public abstract T Part1Result { get; }
    public abstract T Part2Result { get; }

    public abstract T Part1();
    public abstract T Part2();

    protected virtual IEnumerable<string> Input => this.EmbeddedResourceLines();

    [Fact]
    public void TestPart1() => Assert.Equal(Part1Result, Part1());
    [Fact]
    public void TestPart2() => Assert.Equal(Part2Result, Part2());
}
