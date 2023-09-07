using System.Collections;

public record InterpolatedStringPart(
    ReturningCall? ReturningCall = null,
    String? RawString = null
)
{
    public bool IsReturningCall => ReturningCall is not null;

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

    public override string ToString()
    {
        if (RawString != null) return $"\"{RawString}\"";

        return ReturningCall!.ToString();
    }
}

public record StringLiteral(InterpolatedStringPart[] Parts) : IEnumerable<InterpolatedStringPart>
{
    public static Dictionary<string, string> Escapes = new Dictionary<string, string>() 
    {
        { "dot", "." },
        { "quote", "," }
    };

    public bool IsInterpolated => Parts.Any(p => p.IsReturningCall);
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
            var part = parts[i];
            var isEven = i % 2 == 0;
            if (isEven)
            {
                resultParts.Add(part);
                continue;
            }
            else
            {
                if (Escapes.TryGetValue(part, out var escape))
                {
                    resultParts.Add(escape);
                    continue;
                }

                var interpolatedBlock = Block.OfSingleElement(part);
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

    public override string ToString()
    {
        var partsString = string.Join<InterpolatedStringPart>("+", Parts);
        return IsInterpolated ? $"Interpolated string {partsString}" : $"Raw String \"{OnlyString}\"";
    }
}