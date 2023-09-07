public record BoolLiteral(bool Value)
{
    public static Dictionary<string, bool> Literals = new()
    {
        {"TRUE", true},
        {"FALSE", false}
    };

    public static bool TryParse(Block block, out BoolLiteral literal)
    {
        literal = null!;
        if (!block.TryGetOnlyMain(out var main)) return false;
        if (!Literals.TryGetValue(main, out var value)) return false;
        
        literal = new BoolLiteral(value);
        return true;
    }
}