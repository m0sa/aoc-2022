using System.Linq;
using System.Runtime.CompilerServices;

namespace day20;

public class Input : Day20
{
    public override long Part1Result { get; } = 7004;
    public override long Part2Result { get; } = 17200008919529;
}

public class Example : Day20
{
    public override long Part1Result { get; } = 3;
    public override long Part2Result { get; } = 1623178306;

    [Fact]
    public void ExampleMixTest()
    {
        var mixed = Mix(1, Parsed);

        Assert.Equal(new long [] {1, 2, -3, 4, 0, 3, -2}, mixed);
    }

    [Fact]
    public void ExampleDecrepytedMixTest()
    {
        var initial = new long[] { 811589153, 1623178306, -2434767459, 2434767459, -1623178306, 0, 3246356612 }.ToImmutableList();

        Assert.Equal(new long[]  { 0, 2434767459, 1623178306, 3246356612, -2434767459, -1623178306, 811589153 }, Mix(2, initial));
    }
}

public abstract class Day20 : AOCDay
{
    private ImmutableList<long>? _parsed;
    public ImmutableList<long> Parsed => _parsed ??= Input.Select(long.Parse).ToImmutableList();

    public static ImmutableList<long> Mix(int times, ImmutableList<long> input)
    {
        var mod = input.Count - 1;
        var indices = Enumerable.Range(0, input.Count).ToList();
        var values = input.ToList();
        for (var t = 0; t < times; t++)
        {
            for (var originalIndex = 0; originalIndex < input.Count; originalIndex++)
            {
                var currentIndex = indices.IndexOf(originalIndex);
                var currentValue = values[currentIndex];
                if (currentValue == 0) continue;

                var targetPosition = (int)((currentIndex + currentValue) % mod);
                targetPosition = targetPosition <= 0 ? targetPosition + mod : targetPosition;

                indices.RemoveAt(currentIndex);
                values.RemoveAt(currentIndex);
                indices.Insert(targetPosition, originalIndex);
                values.Insert(targetPosition, currentValue);
            }
        }
        return values.ToImmutableList();
    }

    public override long Part1() => GroveCoordinates(Mix(1, Parsed));

    private const long DecryptionKey = 811589153;
    public override long Part2() => GroveCoordinates(Mix(10, Parsed.Select(x => x * DecryptionKey).ToImmutableList()));

    private static long GroveCoordinates(ImmutableList<long> mixed)
    {
        var index0 = mixed.IndexOf(0);
        long atOffsetFrom0(int offset) => mixed[(index0 + offset) % mixed.Count];
        return atOffsetFrom0(1000) + atOffsetFrom0(2000) + atOffsetFrom0(3000);
    }
}
