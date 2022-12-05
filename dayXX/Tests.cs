namespace dayXX;

public class Input : DayXX
{
    public override long Part1Result { get; } = -1;
    public override long Part2Result { get; } = -1;
}

public class Example : DayXX
{
    public override long Part1Result { get; } = -1;
    public override long Part2Result { get; } = -1;
}

public abstract class DayXX : AOCDay
{

    public override long Part1() => -1;

    public override long Part2() => -1;
}
