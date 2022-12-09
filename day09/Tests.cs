namespace Day09;

public class Input : Day09
{
    public override long Part1Result { get; } = 6271;
    public override long Part2Result { get; } = 2458;
}

public class Example : Day09
{
    public override long Part1Result { get; } = 13;
    public override long Part2Result { get; } = 1;
}
public class Example2 : Day09
{
    public override long Part1Result { get; } = 88;
    public override long Part2Result { get; } = 36;
}

public abstract class Day09 : AOCDay
{
    protected record Instruction(Vec2D Direction, int Steps);
    protected IEnumerable<Instruction> ParseInput() => Input.Select(line =>
    {
        var split = line.Split(' ');
        Vec2D direction = split[0] switch
        {
            "U" => new (0, 1),
            "D" => new (0, -1),
            "L" => new (-1, 0),
            "R" => new (1, 0),
        };
        var steps = int.Parse(split[1]);
        return new Instruction(direction, steps);
    });

    public override long Part1() => TailVisitCount(2);

    private long TailVisitCount(int knotCount)
    {
        var knots = new Vec2D[knotCount];
        var posiTions = new HashSet<Vec2D>() { new() };
        foreach (var instruction in ParseInput())
        {
            for(var step = 0; step < instruction.Steps; step++)
            {
                knots[0] += instruction.Direction;
                for (var k = 1; k < knots.Length; k++)
                {
                    knots[k] += GetDirection(knots[k - 1], knots[k]);
                }
                posiTions.Add(knots[knots.Length - 1]);
            }
        }
        return posiTions.Count;
    }

    private static Vec2D GetDirection(Vec2D h, Vec2D t)
    {
        var d = h - t;
        var dx = Math.Abs(d.X);
        var dy = Math.Abs(d.Y);
        if (dx <= 1 && dy <= 1)  // no change, they're neighbours (at least diagonally)
        {
            return new();
        }
        return new (dx == 0 ? 0 : d.X / dx, dy == 0 ? 0 : d.Y / dy);
    }

    public override long Part2() => TailVisitCount(10);
}
