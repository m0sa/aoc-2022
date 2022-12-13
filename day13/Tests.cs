namespace day13;
using PacketList = ImmutableList<PacketItem>;
public record struct PacketItem(byte? Number, PacketList? List);
public record struct PacketPair(PacketList Left, PacketList Right);

public class Input : Day13
{
    public override long Part1Result { get; } = 6395;
    public override long Part2Result { get; } = 24921;
}

public class Example : Day13
{
    public override long Part1Result { get; } = 13;
    public override long Part2Result { get; } = 140;
}

public abstract class Day13 : AOCDay
{
    private ImmutableList<PacketPair>? _packetPairs;
    protected ImmutableList<PacketPair> PacketPairs => _packetPairs ??= ParseInput().ToImmutableList();
    private IEnumerable<PacketPair> ParseInput()
    {
        var enumerator = Input.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var left = ParseLine(enumerator.Current);
            enumerator.MoveNext();
            var right = ParseLine(enumerator.Current);
            yield return new PacketPair(left, right);
            enumerator.MoveNext();
        }
    }

    protected static PacketList ParseLine(string line)
    {
        Stack<List<PacketItem>> stack = new();
        char[] trailingDelimiters = { ',', ']' };
        stack.Push(new());
        for (var i = 1; i < line.Length - 1; i++)
        {
            switch (line[i])
            {
                case ' ':
                case ',':
                    continue;
                case '[':
                    stack.Push(new());
                    break;
                case ']':
                    var list = stack.Pop().ToImmutableList();
                    stack.Peek().Add(new(null, list));
                    break;
                default: // number
                    var d = line.IndexOfAny(trailingDelimiters, i);
                    var l = d - i;
                    stack.Peek().Add(new(byte.Parse(line.AsSpan(i, d - i)), null));
                    i += l - 1;
                    break;
            }
        }
        return stack.Pop().ToImmutableList();
    }

    protected static bool? Compare(PacketList leftList, PacketList rightList)
    {
        var commonLength = Math.Min(leftList.Count, rightList.Count);
        for (var i = 0; i < commonLength; i++)
            if (Compare(leftList[i], rightList[i]) is bool decision) return decision;
        return leftList.Count == rightList.Count
            ? null
            : leftList.Count < rightList.Count;
    }

    protected static bool? Compare(PacketItem left, PacketItem right)
    {
        if (left is PacketItem(null, PacketList leftList) && right is PacketItem(null, PacketList rightList))
            return Compare(leftList, rightList);
        else if (left is PacketItem(byte leftByte, null) && right is PacketItem(byte rightByte, null))
            return leftByte == rightByte
                ? null
                : leftByte < rightByte;
        else if (left is PacketItem(byte _, null) && right is PacketItem(null, PacketList _))
            return Compare(WrapInList(left), right);
        else if (left is PacketItem(null, PacketList _) && right is PacketItem(byte _, null))
            return Compare(left, WrapInList(right));
        else throw new UnreachableException();
    }

    protected static PacketItem WrapInList(PacketItem item) => new(null, new[] { item }.ToImmutableList());

    public override long Part1() =>
        PacketPairs
            .Select((p, i) => new { p.Left, p.Right, N = i + 1 })
            .Where(p => Compare(p.Left, p.Right) == true)
            .Sum(p => p.N);

    public override long Part2()
    {
        var divider2 = WrapInList(WrapInList(new PacketItem(2, null))).List!;
        var divider6 = WrapInList(WrapInList(new PacketItem(6, null))).List!;
        var allPackets = PacketPairs.SelectMany(p => new[] { p.Left, p.Right }).ToList();
        allPackets.Add(divider2);
        allPackets.Add(divider6);
        allPackets.Sort((list1, list2) =>
            Compare(list1, list2) switch
            {
                true => -1,
                null => 0,
                false => 1,
            });
        return (allPackets.IndexOf(divider2) + 1) * (allPackets.IndexOf(divider6) + 1);
    }
}
