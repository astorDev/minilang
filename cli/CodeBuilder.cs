using OneOf;

public record CodeBuilder(string Separator = "")
{
    public List<OneOf<string, CodeBuilder>> parts = new List<OneOf<string, CodeBuilder>>();

    public void Append(string text) {
        parts.Add(text);
    }

    public void AppendLine(string text = "") {
        parts.Add(text);
        parts.Add(Environment.NewLine);
    }

    public string Build() 
    {
        var partsAsStrings = parts.Select(x => x.Match(
            str => str,
            builder => builder.Build()
        )).Select(x => x!);

        return String.Join(Separator, partsAsStrings);
    }

    public override string ToString()
    {
        return Build();
    }

    public CodeBuilder Sub(string separator = "") {
        var sub = new CodeBuilder(separator);
        parts.Add(sub);
        return sub;
    }
}