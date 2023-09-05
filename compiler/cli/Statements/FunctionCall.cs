global using ArgumentAssignments = System.Collections.Generic.Dictionary<string, ReturningCall>;

public record FunctionCall(FunctionCallPath Path, ArgumentAssignments Arguments)
{
    public static bool TryParse(Block block, FunctionContext context, out FunctionCall functionCall) {
        functionCall = new FunctionCall(
            FunctionCallPath.Parse(block.Main!),
            ArgumentAssingmentParser.Parse(block.AfterMain(), context)
        );

        return true;
    }

    public override string ToString()
    {
        var argumentsString = Arguments.Any() 
            ? $"with arguments: {Environment.NewLine}{String.Join(Environment.NewLine, Arguments.Select(argument => $"{argument.Key} = {argument.Value}"))}"
            : "without arguments";

        return $"FunctionCall '{Path}' {argumentsString}";
    }
}

public record FunctionCallPath(string[] Parts)
{
    public static FunctionCallPath Parse(string text) {
        return new FunctionCallPath(text.Split('.'));
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
        var fromHead = headParts.Select(p => p.SplitInTwoLeftOptional(':')).Select(p => new { Name = p.Left, Block = Block.OfSingleElement(p.Right) });
        var fromInner = block.Inner.Select(b => new { Name = (String?)null, Block = b });

        var all = fromHead.Union(fromInner).ToArray();

        var result = new ArgumentAssignments();
        for (var i = 0; i < all.Length; i++)
        {
            var arg = all[i];
            result[arg.Name ?? PositionalArgument.Name(i)] = ReturningCall.Parse(arg.Block, context);
        }

        return result;
    }
}

public record PositionalArgument
{
    public static string Name(int index) => $"Positional:{index}";
    public static string Zero = Name(0);
}