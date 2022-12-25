namespace day25;

public class Input : Day25
{
    public override string Part1Result { get; } = "2011-=2=-1020-1===-1";
    public override string Part2Result { get; } = "";
}

public class Example : Day25
{
    public override string Part1Result { get; } = "2=-1=0";
    public override string Part2Result { get; } = "";

    [Theory]
    [InlineData("1", 1)]
    [InlineData("2", 2)]
    [InlineData("1=", 3)]
    [InlineData("1-", 4)]
    [InlineData("10", 5)]
    [InlineData("11", 6)]
    [InlineData("12", 7)]
    [InlineData("2=", 8)]
    [InlineData("2-", 9)]
    [InlineData("20", 10)]
    [InlineData("1=0", 15)]
    [InlineData("1-0", 20)]
    [InlineData("1=11-2", 2022)]
    [InlineData("1-0---0", 12345)]
    public void SnafuTest(string snafu, long @decimal)
    {
        Assert.Equal(@decimal, Decode(snafu));
        Assert.Equal(snafu, Encode(@decimal));
    }
}

public abstract class Day25 : AOCDay<string>
{
    public static long Decode(string snafu) => snafu
        .ToCharArray()
        .Reverse()
        .Select((ch, index) => (factor: ch switch { '2' => 2, '1' => 1, '0' => 0, '-' => -1, '=' => -2 }, index))
        .Aggregate(0L, (agg, cur) => agg + (long)Math.Pow(5, cur.index) * cur.factor);

    public static string Encode(long value)
    {
        var powerOf5 = 5L;
        var sb = new System.Text.StringBuilder();
        while (value > 0L)
        {
            var offset = value + 2;
            var mul = offset / powerOf5;
            var rem = offset % powerOf5;
            sb.Insert(0, rem switch { 0L => '=', 1L => '-', 2L => '0', 3L => '1', 4L => '2', });
            value = mul;
        }
        return sb.ToString();
    }
    public override string Part1() => Encode(Input.Select(line => Decode(line)).Sum());

    public override string Part2() => "";
}
