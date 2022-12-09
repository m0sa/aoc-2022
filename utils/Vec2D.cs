namespace utils;

public record struct Vec2D(int X, int Y)
{
    public static Vec2D operator+(Vec2D a, Vec2D b) => new Vec2D(a.X + b.X, a.Y + b.Y);
    public static Vec2D operator-(Vec2D a, Vec2D b) => new Vec2D(a.X - b.X, a.Y - b.Y);
}

