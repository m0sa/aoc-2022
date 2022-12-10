namespace utils;

public interface IAOCDay { }
public abstract class AOCDay : AOCDay<long> {}
public abstract class AOCDay<T> : AOCDay<T, T> {}
public abstract class AOCDay<T1, T2> : IAOCDay
{
    public abstract T1 Part1Result { get; }
    public abstract T2 Part2Result { get; }

    public abstract T1 Part1();
    public abstract T2 Part2();

    protected virtual IEnumerable<string> Input => this.EmbeddedResourceLines();

    [Fact]
    public void TestPart1() => Assert.Equal(Part1Result, Part1());
    [Fact]
    public void TestPart2() => Assert.Equal(Part2Result, Part2());
}
