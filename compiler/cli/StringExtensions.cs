using System.Reflection.PortableExecutable;

public static class StringExtensions
{
    public static bool StartsWithLower(this string s) => Char.IsLower(s[0]);
    public static bool StartsWithUpper(this string s) => Char.IsUpper(s[0]);

    public static bool StartsWithSpace(this string s) => s.StartsWith(" ");

    public static (string Left, string Right) SplitInTwo(this string s, char separator)
    {
        var parts = s.Split(separator);
        return (parts[0], parts[1]);
    }

    public static (string? Left, string Right) SplitInTwoLeftOptional(this string s, char separator)
    {
        var parts = s.Split(separator);
        return parts.Length == 2 ? (parts[0], parts[1]) : (null, parts[0]);
    }

    public static string PascalCase(this string s) => s[0].ToString().ToUpper() + s[1..];
}

public static class Identation
{
    public const int Length = 4;

    public static string Unindented(this string line)
    {
        return line[Length..];
    }

    public static string[] Cut(this string[] lines)
    {
        var result = new string[lines.Length];
        for (var i = 0; i < lines.Length; i++)
        {
            result[i] = lines[i].Unindented();
        }

        return result;
    }

    public static string One = "    ";
    public const string MinusOne = "   ";
}