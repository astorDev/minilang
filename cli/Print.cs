public static class Print
{
    public static bool Silent = true;

    public static void Header(string header)
    {
        if (Silent) return;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(header);
        Console.WriteLine();
        Console.ResetColor();
    }

    public static void Line(string text = "")
    {
        if (Silent) return;

        Console.WriteLine(text);
    }

    public static void Subheader(string subheader)
    {
        if (Silent) return;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(subheader);
        Console.ResetColor();
    }

    public static void FileIndex(int index, string filename, string extra = "") {
        if (Silent) return;

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write($"{index}. ");
        extra = extra != "" ? $" {extra}" : "";
        Console.Write($"(file: \"{filename}\"{extra})");
        Console.ResetColor();
        Console.WriteLine();
    }

    public static void Index(int index, string body) {
        if (Silent) return;

        Console.WriteLine($"{index}. {body}");
    }

    public static void LineByLine(IEnumerable<object> source) {
        if (Silent) return;

        foreach (var item in source)
        {
            Console.WriteLine(item);
        }
    }
}
