namespace day06;

public class Input : Day06
{
    public override long Part1Result { get; } = 1802;
    public override long Part2Result { get; } = 3551;
}

public class Example : Day06
{
    public override long Part1Result { get; } = 7;
    public override long Part2Result { get; } = 19;
}

public abstract class Day06 : AOCDay
{
    private int FindMarker(int len)
    {
        var input = Input.Single();
        for (int i = len - 1; i < input.Length; i++)
        {
            var marker = input.Substring(i - len + 1, len);
            if (marker.Distinct().Count() == len)
            {
                return i + 1;
            }
        }
        return -1;
    }
    public override long Part1() => FindMarker(4);
    public override long Part2() => FindMarker(14);
}
