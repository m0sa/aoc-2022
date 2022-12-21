using System.Text.RegularExpressions;

namespace day21;

public class Input : Day21
{
    public override long Part1Result { get; } = 63119856257960;
    public override long Part2Result { get; } = 3006709232464;
}

public class Example : Day21
{
    public override long Part1Result { get; } = 152;
    public override long Part2Result { get; } = 301;
}

public abstract class Day21 : AOCDay
{
    public interface IOperand { }
    public record Value(long Val) : IOperand;
    public record BinaryOperation(IOperand Left, char Operator, IOperand Right) : IOperand;
    public record Reference(string Name) : IOperand;
    private ImmutableDictionary<string, IOperand> Parse(IEnumerable<string> input)
    {
        var result = new Dictionary<string, IOperand>();
        foreach (var match in input.Select(line => Regex.Match(line, @"(?<Name>....): ((?<Value>\d+)|(?<Left>....) (?<Operator>.) (?<Right>....))")))
        {
            var name = match.Groups["Name"].Value;
            if (match.Groups.TryGetValue("Value", out var grp) && int.TryParse(grp.Value, out var value))
            {
                result[name] = new Value(value);
            }
            else
            {
                var leftName = match.Groups["Left"].Value;
                var rightName = match.Groups["Right"].Value;
                result[name] = new BinaryOperation(new Reference(leftName), match.Groups["Operator"].Value[0], new Reference(rightName));
            }
        }
        return result.ToImmutableDictionary();
    }

    public static ImmutableDictionary<string, IOperand> Simplify(ImmutableDictionary<string, IOperand> ops, ImmutableDictionary<string, IOperand> resolvedPartial = null)
    {
        var resolved = new Dictionary<string, IOperand>(resolvedPartial ?? ImmutableDictionary<string, IOperand>.Empty);
        var pending = ops.ToDictionary(x => x.Key, x => x.Value);
        while (true)
        {
            var solved = pending.Where(x => x.Value is Value).ToList();
            if (solved.Count == 0)
            {
                break;
            }
            solved.ForEach(x =>
            {
                pending.Remove(x.Key);
                resolved[x.Key] = x.Value;
            });
            pending = pending.ToDictionary(kvp => kvp.Key, kvp => ResolveNumerically(resolved, kvp.Value));
        }

        return resolved.Concat(pending).ToImmutableDictionary();
    }
    public static IOperand ResolveNumerically(IDictionary<string, IOperand> resolved, IOperand op) => op switch
    {
        Reference r => resolved.TryGetValue(r.Name, out var known) ? known : r,
        BinaryOperation b => new BinaryOperation(ResolveNumerically(resolved, b.Left), b.Operator, ResolveNumerically(resolved, b.Right)) switch
        {
            { Left: Value { Val: long l }, Operator: char o, Right: Value { Val: long r } } => new Value(
                o switch
                {
                    '+' => l + r,
                    '-' => l - r,
                    '*' => l * r,
                    '/' => l / r,
                }),
            BinaryOperation partial => partial,
        },
        _ => op,
    };

    public override long Part1() => Simplify(Parse(Input))["root"] is Value v ? v.Val : throw new InvalidOperationException();

    public static IOperand SolveEquation(string target, IOperand left, IOperand right, IDictionary<string, IOperand> solved)
    {
        left = ResolveNumerically(solved, ResolveSymbolically(solved, left));
        right = ResolveNumerically(solved, ResolveSymbolically(solved, right));

        return right switch
        {
            Value _ when left is Reference lr && lr.Name == target => right,
            Value _ => SolveEquation(target, right, left, solved),
            BinaryOperation { Left: Value l, Operator: char o, Right: IOperand r } =>
                SolveEquation(
                    target,
                    o switch
                    {
                        '+' => new BinaryOperation(left, '-', l),
                        '-' => new BinaryOperation(l, '-', left),
                        '*' => new BinaryOperation(left, '/', l),
                        '/' => new BinaryOperation(l, '/', left),
                    },
                    r,
                    solved),
            BinaryOperation { Left: IOperand l, Operator: char o, Right: Value r } =>
                SolveEquation(
                    target,
                    l,
                    o switch
                    {
                        '+' => new BinaryOperation(left, '-', r),
                        '-' => new BinaryOperation(left, '+', r),
                        '*' => new BinaryOperation(left, '/', r),
                        '/' => new BinaryOperation(left, '*', r),
                    },
                    solved),
        };
    }

    public static IOperand ResolveSymbolically(IDictionary<string, IOperand> resolved, IOperand op) => op switch
    {
        Value v => v,
        Reference r => resolved.TryGetValue(r.Name, out var known) ? ResolveSymbolically(resolved, known) : r,
        BinaryOperation b => new BinaryOperation(ResolveSymbolically(resolved, b.Left), b.Operator, ResolveSymbolically(resolved, b.Right)),
    };

    public override long Part2()
    {
        var ops = Parse(Input);
        var root = ops["root"] as BinaryOperation;
        var all = Simplify(ops.Remove("humn").Remove("root"));
        var result = SolveEquation("humn", root!.Left, root.Right, all);
        return result is Value v ? v.Val : throw new InvalidOperationException();
    }
}
