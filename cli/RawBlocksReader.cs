public static class DeclarationPrinter
{
    public static FileBlock[] Printed(this IEnumerable<FileBlock> declarations)
    {
        var index = 0;
        Print.Header("File blocks:");

        foreach (var declaration in declarations)
        {
            Print.FileIndex(index++, declaration.Filename);
            Print.Line(declaration.Block.ToString());
        }

        return declarations.ToArray();
    }
}