namespace day07;

public class Input : Day07
{
    public override long Part1Result { get; } = 1792222;
    public override long Part2Result { get; } = 1112963;
}

public class Example : Day07
{
    public override long Part1Result { get; } = 95437;
    public override long Part2Result { get; } = 24933642;

    [Fact]
    public void ParseInputTest()
    {
        var result = ParseInput();

        Assert.Equal(10, result.Count(x => x.Type == NodeType.File));
        Assert.Equal(4, result.Count(x => x.Type == NodeType.Directory));
        Assert.Equal(48381165, result.Single(d => d.Path.IsEmpty).Size);
    }
}

public abstract class Day07 : AOCDay
{
    protected enum NodeType { Directory, File }
    protected record Node(ImmutableList<string> Path, NodeType Type, long Size = 0);

    private static bool StartsWith(ImmutableList<string> path, ImmutableList<string> startsWith)
    {
        if (path.Count < startsWith.Count) return false;
        return startsWith.Zip(path.Take(startsWith.Count)).All(x => x.First == x.Second);
    }
    protected ImmutableList<Node> ParseInput()
    {
        var input = ParseInput_Initial().ToImmutableList();
        var files = input.Where(x => x.Type == NodeType.File).ToImmutableList();

        return input.Select(x => x.Type switch {
            NodeType.File => x,
            NodeType.Directory => new Node(x.Path, x.Type, files.Where(f => StartsWith(f.Path, x.Path)).Sum(f => f.Size))
        }).ToImmutableList();

        IEnumerable<Node> ParseInput_Initial()
        {
            var cd = new List<string>();
            yield return new Node(cd.ToImmutableList(), NodeType.Directory); // root dir

            var input = base.Input.ToArray();
            for (var i = 1; i < input.Length; i++) // i=0 -> $ cd /
            {
                var line = input[i];
                if (line == "$ cd ..")
                {
                    cd.RemoveAt(cd.Count - 1);
                    continue;
                }
                else if (line.StartsWith("$ cd "))
                {
                    cd.Add(line.Substring("$ cd ".Length));
                    continue;
                }
                else if (line == "$ ls")
                {
                    for (i++; i < input.Length && !input[i].StartsWith("$") ; i++)
                    {
                        var basePath = cd.ToImmutableList();
                        var lsLine = input[i];
                        if (lsLine.StartsWith("dir "))
                        {
                            yield return new Node(basePath.Add(lsLine.Substring("dir ".Length)), NodeType.Directory);
                        }
                        else
                        {
                            var split = lsLine.Split(' ');
                            yield return new Node(basePath.Add(split[1]), NodeType.File, long.Parse(split[0]));
                        }
                    }
                    i--;
                }
            }
        }
    }

    public override long Part1() => ParseInput().Where(x => x.Type == NodeType.Directory && x.Size <= 100000).Sum(x => x.Size);

    public override long Part2()
    {
        var diskSize = 70000000;
        var requiredFree = 30000000;
        var fileSystem = ParseInput();

        var currentUsed = fileSystem.Single(d => d.Path.IsEmpty).Size;
        var currentFree = diskSize - currentUsed;
        var minimalToFree = requiredFree - currentFree;

        var directoryToRemove = fileSystem.Where(x => x.Type == NodeType.Directory).OrderBy(x => x.Size).First(d => d.Size >= minimalToFree);
        return directoryToRemove.Size;
    }
}
