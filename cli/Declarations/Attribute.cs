global using Attributes = System.Collections.Generic.Dictionary<string, string?>;
global using Attribute = System.Collections.Generic.KeyValuePair<string, string?>;


public record AttributeParser
{
    public static Attribute Parse(string raw)
    {
        var (left, right) = raw.SplitInTwoRightOptional(':');
        return new(left, right);
    }
}