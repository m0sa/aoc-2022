namespace Day09;

public class Input : Day09
{
    public override long Part1Result { get; } = -1;
    public override long Part2Result { get; } = -1;
}

public class Example : Day09
{
    public override long Part1Result { get; } = 13;
    public override long Part2Result { get; } = -1;
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

    public override long Part1()
    {
        Vec2D h = new ();
        Vec2D t = new ();
        var posiTions = new HashSet<Vec2D>() { t };
        foreach (var instruction in ParseInput())
        {
            for(var step = 0; step < instruction.Steps; step++)
            {
                h += instruction.Direction;
                t += GetPart1HDirection(h, t);
                posiTions.Add(t);
            }
        }
        return posiTions.Count;
    }
    private static Vec2D GetPart1HDirection(Vec2D h, Vec2D t)
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

    public override long Part2() => -1;
}
