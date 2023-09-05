public record Declaration(Block Block, string Filename)
{
    public static IEnumerable<Declaration> Read(FileData[] files) 
    {
        foreach (var file in files) 
        {
            foreach (var block in Block.Read(file.Content)) {
                yield return new Declaration(block, file.Name);
            }
        }
    }
}
    
public static class DeclarationPrinter
{
    public static Declaration[] Printed(this IEnumerable<Declaration> declarations)
    {
        var index = 0;
        Print.Header("Declarations:");

        foreach (var declaration in declarations)
        {
            Print.FileIndex(index++, declaration.Filename);
            Print.Line(declaration.Block.ToString());
        }

        return declarations.ToArray();
    }
}