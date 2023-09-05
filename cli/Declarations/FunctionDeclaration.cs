public record FunctionDeclaration(FunctionSignature Signature, Statement[] Statements, string Filename) : IDeclaration
{
    public Statement SingleStatement => Statements.Length == 1 ? Statements.Single() : throw new($"{this} has more then one statement");
    
    public static bool TryParse(FileBlock declaration, out FunctionDeclaration functionDeclaration)
    {
        functionDeclaration = null!;
        if (!FunctionSignature.TryParse(declaration.Block.Head, out var signature)) return false;
        var context = new FunctionContext();

        functionDeclaration = new(
            signature, 
            declaration.Block.Inner.Select(b => Statement.Parse(b, context)).ToArray(), 
            declaration.Filename
        );
        return true;
    }

    public Attributes Attributes => Signature.Attributes;
}

public static class FunctionDeclarationPrinter
{
    public static FunctionDeclaration[] Printed(this IEnumerable<FunctionDeclaration> declarations)
    {
        var index = 0;
        Print.Header("Function Declarations:");

        foreach (var declaration in declarations)
        {
            Print.FileIndex(index++, declaration.Filename);
            Print.Line(declaration.Signature.ToString());

            GoPrint(declaration.Statements);
            Print.Line();
        }

        return declarations.ToArray();
    }

    public static void GoPrint(Statement[] statements) 
    {
        Print.Subheader("Statements:");
        var index = 0;

        foreach (var statement in statements) {
            Print.Index(index++, statement.ToString());
        }
    }
}

public record FunctionSignature(string Name, Attributes Attributes)
{
    public static bool TryParse(BlockHead head, out FunctionSignature signature)
    {
        signature = null!;

        if (!head.StartsWithLower)
        {
            return false;
        }

        signature = new(head.Main, new (head.Extras.Select(AttributeParser.Parse)));

        return true;
    }

    public override string ToString()
    {
        var attributesString = Attributes.Count > 0 ? $" with attributes {String.Join<Attribute>(", ", Attributes)}" : "";
        return $"Function '{Name}'{attributesString}";
    }
}