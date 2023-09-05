using System.Collections;

public record InterpolatedStringPart(
    ReturningCall? ReturningCall = null,
    String? RawString = null
)
{
    public static implicit operator InterpolatedStringPart(ReturningCall returningCall) => new InterpolatedStringPart(ReturningCall: returningCall);
    public static implicit operator InterpolatedStringPart(string rawString) => new InterpolatedStringPart(RawString: rawString);

    public string RequiredString => RawString is not null ? RawString : throw new ("InterpolatedStringPart is not only string");

    public void On(
        Action<ReturningCall> returningCall,
        Action<String> rawString
    )
    {
        if (ReturningCall is not null) returningCall(ReturningCall);
        if (RawString is not null) rawString(RawString);
    }
}

public record StringLiteral(InterpolatedStringPart[] Parts) : IEnumerable<InterpolatedStringPart>
{
    public bool IsInterpolated => Parts.Length > 1;
    public string OnlyString => IsInterpolated ? throw new Exception("StringLiteral is not only string") : Parts[0].RequiredString;

    public static bool TryParse(Block block, FunctionContext context, out StringLiteral stringLiteral) {
        stringLiteral = null!;
        if (!block.TryGetOnlyMain(out var main)) return false;
        if (!main.StartsWith('\'')) return false;
        var raw = main.Trim('\'');
        var cleaned = raw.Replace('_', ' ');
        var parts = cleaned.Split('\'');
        var resultParts = new List<InterpolatedStringPart>();

        for (int i = 0; i < parts.Length; i++)
        {
            var isEven = i % 2 == 0;
            if (isEven)
            {
                resultParts.Add(parts[i]);
                continue;
            }
            else
            {
                var interpolatedBlock = Block.OfSingleElement(parts[i]);
                resultParts.Add(ReturningCall.Parse(interpolatedBlock, context));
            }
        }

        stringLiteral = new StringLiteral(resultParts.ToArray());
        return true;
    }

    public IEnumerator<InterpolatedStringPart> GetEnumerator()
    {
        return Parts.ToList().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}