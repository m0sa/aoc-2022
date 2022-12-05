using System.Text.RegularExpressions;

namespace day05;

public class Input : Day05
{
    public override string Part1Result { get; } = "FWSHSPJWM";
    public override string Part2Result { get; } = "PWPWHGFZS";
}

public class Example : Day05
{
    private readonly ITestOutputHelper _output;
    public Example(ITestOutputHelper output) => _output = output;
    public override string Part1Result { get; } = "CMZ";
    public override string Part2Result { get; } = "MCD";

    [Fact]
    public void ParseTest()
    {
        var game = ParsedInput();
        _output.WriteLine($"stackInstructions:\n{string.Join("\n", game.Instructions)}");
        for (var i = 0; i < game.Stacks.Count; i++)
        {
            _output.WriteLine($"stack[{i + 1}]: {string.Join(",", game.Stacks[i])}");
        }

        Assert.Equal(4, game.Instructions.Count);
        Assert.Equal(3, game.Stacks.Count);
        Assert.Equal(3, game.Stacks.Max(s => s.Count));
    }
}

public record Instruction(int Move, int From, int To);
public record CraneGame(List<Stack<char>> Stacks, List<Instruction> Instructions)
{
    public List<Stack<char>> Part1()
    {
        foreach (var instruction in Instructions)
        {
            ExecuteInstruction(instruction.Move, Stacks[instruction.From - 1], Stacks[instruction.To - 1]);
        }
        static void ExecuteInstruction(int move, Stack<char> from, Stack<char> to)
        {
            while (move > 0)
            {
                to.Push(from.Pop());
                move--;
            }
        }
        return Stacks;
    }
    public List<Stack<char>> Part2()
    {
        foreach (var instruction in Instructions)
        {
            ExecuteInstruction(instruction.Move, Stacks[instruction.From - 1], Stacks[instruction.To - 1]);
        }
        static void ExecuteInstruction(int move, Stack<char> from, Stack<char> to)
        {
            var stackTmp = new Stack<char>();
            while (move > 0)
            {
                stackTmp.Push(from.Pop());
                move--;
            }
            while (stackTmp.Count > 0)
            {
                to.Push(stackTmp.Pop());
            }
        }
        return Stacks;
    }
}
public abstract class Day05 : AOCDay<string>
{
    public CraneGame ParsedInput()
    {
        var stacks = new List<Stack<char>>();
        var stackInstructions = Input.TakeWhile(x => !x.StartsWith(" 1 ")).ToList();
        var stackInstructionsMax = stackInstructions[0].Length;
        for (var i = 1; i < stackInstructionsMax; i = i + 4)
        {
            var stack = new Stack<char>();
            for (var l = stackInstructions.Count - 1; l >= 0; l--) // from the buttom up
            {
                var line = stackInstructions[l];
                var ch = line[i];
                if (ch != ' ')
                {
                    stack.Push(ch);
                }
            }
            stacks.Add(stack);
        }

        var instructions = new List<Instruction>();
        var instructionRegex = new Regex(@"move (\d+) from (\d+) to (\d+)");
        foreach (var line in Input.Skip(stackInstructions.Count + 2))
        {
            var match = instructionRegex.Match(line);
            instructions.Add(new Instruction(
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value)));
        }
        return new(stacks, instructions);
    }

    public override string Part1() => new string(ParsedInput().Part1().Select(st => st.Peek()).ToArray());

    public override string Part2() => new string(ParsedInput().Part2().Select(st => st.Peek()).ToArray());
}
