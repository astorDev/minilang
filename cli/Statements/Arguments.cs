public record ArgumentCall(
    ArgumentSignature Signature,
    CallContinuationPart[] Chain
)
{
    public static bool TryParseForAssignee(RawCallParts parts, FunctionContext context, out ArgumentCall argument)
    {
        argument = null!;
        if (!ArgumentSignature.TryParse(parts, out var signature, out var remaining))
        {
            return false;
        }

        argument = new ArgumentCall(
            context.AddOrGet(signature),
            CallContinuationPart.Parse(remaining, context)
        );

        return true;
    }

    public static bool TryParseForReturningCall(Block block, FunctionContext context, out ArgumentCall argument)
    {
        argument = null!;

        RawCallParts mainParts = RawCallParts.Parse(block.Main);
        if (!ArgumentSignature.TryParse(mainParts, out var signature, out var remaining))
        {
            return false;
        }

        argument = new ArgumentCall(
            context.AddOrGet(signature),
            CallContinuationPart.Parse(remaining, context)
        );

        return true;
    }

    public override string ToString()
    {
        var chainString = Chain.Any() ? " > " + string.Join(" > ", Chain.Select(x => x.ToString())) : "";
        return $"ArgumentCall ({Signature}){chainString}";
    }
}

public record ArgumentSignature(
    string Name,
    ClassSignature? Type
)
{
    public static bool TryParse(RawCallParts parts, out ArgumentSignature signature, out string[] remaining)
    {
        remaining = null!;
        signature = new ArgumentSignature(null!, null);
        if (!parts.TryPopFirstSymbol('@', out var active))
        {
            return false;
        }

        if (ClassSignature.TryParse(active.First, out var classSignature))
        {
            signature = signature with { Type = classSignature };
            active = active.Next();
        }

        signature = signature with { Name = active.First };
        remaining = active.Remaining;
        return true;
    }

    public override string ToString()
    {
        return $"{Type} {Name}";
    }
}