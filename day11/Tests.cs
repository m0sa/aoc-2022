namespace day11;

public class Input : Day11
{
    public override long Part1Result { get; } = 120384;
    public override long Part2Result { get; } = 32059801242;
}

public class Example : Day11
{
    public override long Part1Result { get; } = 10605;
    public override long Part2Result { get; } = 2713310158;
}

public class Monkey
{
    public List<long> Items { get; set; }
    public Func<long, long> Operation { get; set; }
    public int DivisibleBy { get; set; }
    public int TrueMonkeyId { get; set; }
    public int FalseMonkeyId { get; set; }
    public long ItemsInspected { get; private set; }

    public void Turn(ImmutableList<Monkey> Monkeys, Func<long, long> worryProvider)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var worryLevel = worryProvider(Operation(Items[i]));
            var targetMonkey = worryLevel % DivisibleBy == 0 ? TrueMonkeyId : FalseMonkeyId;
            Monkeys[targetMonkey].Items.Add(worryLevel);
        }
        ItemsInspected += Items.Count;
        Items.Clear();
    }


    public static void Round(ImmutableList<Monkey> monkeys, Func<long, long> worryProvider) => monkeys.ForEach(x => x.Turn(monkeys, worryProvider));
}

public abstract class Day11 : AOCDay
{
    private ImmutableList<Monkey>? _monkeys;
    public ImmutableList<Monkey> Monkeys => _monkeys ??= ParseInput().ToImmutableList();

    private IEnumerable<Monkey> ParseInput()
    {
        Monkey? monkey = null;
        foreach (var line in Input)
        {
            if (string.IsNullOrEmpty(line) && monkey != null)
            {
                yield return monkey;
                monkey = null;
                continue;
            }
            else if (line.StartsWith("Monkey ") &&
                int.TryParse(line.Substring("Monkey ".Length).Trim(':'), out var monkeyId))
            {
                monkey = new Monkey { Items = new() };
            }
            else if (line.StartsWith("  Starting items:"))
            {
                monkey!.Items.AddRange(line.Substring("  Starting items:".Length).Split(", ").Select(long.Parse));
            }
            else if (line.StartsWith("  Operation: new = old "))
            {
                Func<long, long, long> op = line.Substring("  Operation: new = old ".Length, 1) switch
                {
                    "*" => (a, b) => checked(a * b),
                    "+" => (a, b) => checked(a + b),
                    "-" => (a, b) => checked(a - b),
                    _ => throw new Exception("Unexpected operation on line: " + line),
                };
                var operand = line.Substring("  Operation: new = old X ".Length);
                monkey!.Operation = old => op(old, long.TryParse(operand, out var operandLong) ? operandLong : old);
            }
            else if (line.StartsWith("  Test: divisible by ") && int.TryParse(line.Substring("  Test: divisible by ".Length), out var divisibleBy))
            {
                monkey!.DivisibleBy = divisibleBy;
            }
            else if (line.StartsWith("    If true: throw to monkey ") && int.TryParse(line.Substring("    If true: throw to monkey ".Length), out var trueMonkeyId))
            {
                monkey!.TrueMonkeyId = trueMonkeyId;
            }
            else if (line.StartsWith("    If false: throw to monkey ") && int.TryParse(line.Substring("    If true: throw to monkey ".Length), out var falseMonkeyId))
            {
                monkey!.FalseMonkeyId = falseMonkeyId;
            }
        }
        if (monkey != null)
        {
            yield return monkey;
        }
    }

    public override long Part1()
    {
        for (var i = 0; i < 20; i++) Monkey.Round(Monkeys, w => w / 3);
        return Monkeys.OrderByDescending(x => x.ItemsInspected).Take(2).Aggregate(1L, (agg, m) => agg * m.ItemsInspected);
    }

    public override long Part2()
    {
        var worryLevelDivisor = Monkeys.Select(x => x.DivisibleBy).Aggregate(1L, (agg, c) => agg * c);
        for (var i = 0; i < 10000; i++) Monkey.Round(Monkeys, w => w % worryLevelDivisor);
        return Monkeys.OrderByDescending(x => x.ItemsInspected).Take(2).Aggregate(1L, (agg, m) => agg * m.ItemsInspected);
    }

}
