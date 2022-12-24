namespace utils;

public record struct Vec2D(int X, int Y)
{
    public static Vec2D operator+(Vec2D a, Vec2D b) => new Vec2D(a.X + b.X, a.Y + b.Y);
    public static Vec2D operator-(Vec2D a, Vec2D b) => new Vec2D(a.X - b.X, a.Y - b.Y);
    public static int ManhattanDistance(Vec2D a, Vec2D b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
}
public record struct Vec2DL(long X, long Y)
{
    public static Vec2DL operator+(Vec2DL a, Vec2DL b) => new Vec2DL(a.X + b.X, a.Y + b.Y);
    public static Vec2DL operator-(Vec2DL a, Vec2DL b) => new Vec2DL(a.X - b.X, a.Y - b.Y);
}

