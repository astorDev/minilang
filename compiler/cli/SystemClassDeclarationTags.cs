public record SystemClassDeclarationTag(Tag Source)
{   
    public const string Data = "data";

    public static string[] All = new string [] {
        Data
    };

    public static bool TryParse(Tag tag, out SystemClassDeclarationTag declaration)
    {
        declaration = null!;
        if (All.Contains(tag.Name))
        {
            declaration = new(tag);
            return true;
        }

        return false;
    }
}