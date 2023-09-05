global using ArgumentAssignments = System.Collections.Generic.Dictionary<string, ReturningCall>;

public static class ArgumentAssignmentExtensions
{
    public static ReturningCall RequiredSingle(this ArgumentAssignments assignments)
    {
        return assignments.Count == 1 ? assignments.Single().Value : throw new($"{assignments} has more then one element");
    }
}


public record FunctionCall(FunctionCallPath Path, ArgumentAssignments Arguments, bool SpaceAfter)
{
    public static bool TryParse(MaybeHeadlessBlock block, FunctionContext context, out FunctionCall functionCall) {
        functionCall = new FunctionCall(
            FunctionCallPath.Parse(block.Main),
            ArgumentAssingmentParser.Parse(block.AfterMain(), context),
            block.SpaceAfter
        );

        return true;
    }

    public override string ToString()
    {
        var argumentsString = Arguments.Any() 
            ? $"with arguments: {Environment.NewLine}{String.Join(Environment.NewLine, Arguments.Select(argument => $"{argument.Key} = {argument.Value}"))}"
            : "without arguments";

        return $"FunctionCall '{Path}' {argumentsString}" + (SpaceAfter ? " with space after" : "");
    }
}

public record FunctionCallPath(params string[] Parts)
{
    public static FunctionCallPath Parse(string? text) {
        if (text == null) return new FunctionCallPath("autoconstructor");
        return new FunctionCallPath(text.Split('.'));
    }

    public string OnlyName()
    {
        if (!TryGetNameOnly(out var name)) throw new InvalidOperationException($"{this} contains more then name");
        return name;
    }
    
    public bool TryGetNameOnly(out string name) {
        name = null!;
        if (Parts.Length != 1) return false;

        name = Parts.Single();
        return true;
    }

    public (string[] Prepath, string Name) SplitWithName() {
        var prepath = Parts.Take(Parts.Length - 1).ToArray();
        var name = Parts.Last();
        return (prepath, name);
    }

    public (string First, string[] Remaining) SplitWithFirst() {
        var first = Parts.First();
        var remaining = Parts.Skip(1).ToArray();
        return (first, remaining);
    }

    public string Name => Parts.Single();

    public override string ToString()
    {
        return String.Join('.', Parts);
    }
}

public record ArgumentAssingmentParser
{
    public static ArgumentAssignments Parse(MaybeHeadlessBlock block, FunctionContext context)
    {
        var headParts = block.HeadParts();
        var fromHead = headParts.Select(p => p.SplitInTwoLeftOptional(':')).Select(p => new Raw(Block.OfSingleElement(p.Right), p.Left));
        var fromInner = block.Inner.Select(b => { 
            if (!b.TryPopTwo(out var remaining, out var poped)){
                return new Raw(b);
            }

            if (poped.Second != "=") {
                return new Raw(b);
            }

            return new Raw(remaining, poped.First); 
        });

        var all = fromHead.Union(fromInner).ToArray();

        var result = new ArgumentAssignments();
        for (var i = 0; i < all.Length; i++)
        {
            var arg = all[i];
            result[arg.Name ?? PositionalArgument.Name(i)] = ReturningCall.Parse(arg.Block, context);
        }

        return result;
    }

    record Raw(MaybeHeadlessBlock Block, string? Name = null);
}

public record PositionalArgument
{
    public static string Name(int index) => $"Positional:{index}";
    public static string Zero = Name(0);
}