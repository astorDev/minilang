public record CallContinuationPart(
    PropertyCall? PropertyCall = null,
    StringLiteral? StringIndexer = null
)
{
    public static CallContinuationPart Parse(string raw, FunctionContext context)
    {
        if (StringLiteral.TryParse(Block.OfSingleElement(raw), context, out var stringLiteral))
        {
            return new (StringIndexer: stringLiteral);
        }

        return new (PropertyCall: new(raw));
    }

    public void On(Action<PropertyCall> propertyCall, Action<StringLiteral> stringIndexer)
    {
        if (PropertyCall != null) propertyCall(PropertyCall);
        if (StringIndexer != null) stringIndexer(StringIndexer);
    }

    public static CallContinuationPart[] Parse(string[] rawCallParts, FunctionContext context)
    {
        return rawCallParts.Select(part => Parse(part, context)).ToArray();
    }

    public object TheOne
     =>
        (object?)PropertyCall ??
        (object?)StringIndexer!;

    public override string ToString()
    {
        return TheOne.ToString()!;
    }
}

public record PropertyCall(
    String PropertyName
);

public record RawCallParts(string First, string[] Remaining) : IEnumerable<string>
{
    public bool TryGetOnlyFirst(out string First)
    {
        First = this.First;
        return Remaining.Length == 0;
    }

    public string[] AsArray() => new[] {First}.Concat(Remaining).ToArray();

    public bool TryPopFirstSymbol(char symbol, out RawCallParts parts) 
    {
        parts = null!;
        if (First[0] != symbol) return false;
        parts = new RawCallParts(First[1..], Remaining);
        return true;
    }

    public static RawCallParts Parse(string raw)
    {
        var parts = raw.Split('.');
        return new RawCallParts(parts[0], parts.Skip(1).ToArray());
    }

    public bool TryGetNext(out RawCallParts next)
    {
        if (Remaining.Length > 0)
        {
            next = null!;
            return false;
        }

        next = new RawCallParts(Remaining[0], Remaining.Skip(1).ToArray());
        return true;
    }

    public RawCallParts Next()
    {
        if (Remaining.Length == 0) return new RawCallParts(String.Empty, Array.Empty<string>());
        return new RawCallParts(Remaining[0], Remaining.Skip(1).ToArray());
    }

    public IEnumerator<string> GetEnumerator()
    {
        return new[] {First}.Concat(Remaining).GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}