public record ClassDeclaration(
    ClassDeclarationHead Head, 
    ClassDeclarationBody Body,
    string Filename
) : IDeclaration
{
    public static bool TryParse(FileBlock declaration, out ClassDeclaration classDeclaration) {
        classDeclaration = null!;
        if (!ClassDeclarationHead.TryParse(declaration.Block.Head, out var head)) return false;

        classDeclaration = new(
            head,
            ClassDeclarationBody.Parse(declaration.Block.Inner),
            declaration.Filename
        );

        return true;
    }

    public Attributes Attributes => Head.Attributes;
}

public record ClassDeclarationBody(PropertyDeclaration[] Properties)
{
    public static ClassDeclarationBody Parse(Block[] blocks) {
        (var properties, blocks) = blocks.ForkParsed<Block, PropertyDeclaration>(PropertyDeclaration.TryParse);

        return new ClassDeclarationBody(
            Properties : properties
        );
    }
}

public record ClassDeclarationHead(ClassSignature Me, ClassSignature[] BaseClasses, Attributes Attributes) {
    public static bool TryParse(BlockHead head, out ClassDeclarationHead result) {
        result = null!;
        if (!ClassSignature.TryParse(head.Main, out var me)) return false;
        var (bases, remaining) = head.Extras.ForkParsed<string, ClassSignature>(ClassSignature.TryParse);
        
        result = new(
            me,
            bases,
            new(remaining.Select(AttributeParser.Parse))
        );  

        return true;
    }

    public override string ToString()
    {
        var baseClassesString = BaseClasses.Any() ? $" BaseClasses = {String.Join(", ", BaseClasses.Select(bc => bc.ToString()))}" : "";
        var tagsString = "";
        return $"{{ Me = {Me}" + baseClassesString + tagsString + " }";
    }
}

public record ClassSignature(string Name, ClassSignature[] GenericArguments)
{
    public static bool TryParse(string raw, out ClassSignature signature) 
    {
        signature = null!;
        if (!raw.StartsWithUpper()) return false;
        signature = Parse(raw);
        return true;
    }

    private static ClassSignature Parse(string raw, char splitSymbol = '_')
    {
        string[] split = raw.Split(splitSymbol);
        var n = split[0];
        var gArgs = split.Skip(1).Select(s => Parse(s, '~')).ToArray();
        return new(n, gArgs);
    }

    public override string ToString()
    {
        var genericsArgsString = GenericArguments.Any() ? $", GenericArguments = {String.Join(", ", GenericArguments.Select(a => a.ToString()))}" : "";
        return $"{{ Name = {Name}" + genericsArgsString + " }";
    }
}

public static class RawClassDeclarationUtil {
    public static ClassDeclaration[] Printed(this IEnumerable<ClassDeclaration> declarations) {
        var index = 0;
        Print.Header("Class declarations:");

        foreach (var declaration in declarations)
        {
            Print.FileIndex(index++, declaration.Filename);
            Console.WriteLine(declaration.Head);
            GoPrint(declaration.Body.Properties);  
        }
        return declarations.ToArray();
    }

    public static void GoPrint(IEnumerable<PropertyDeclaration> properties) {
        var index = 0;
        Print.Subheader("Properties:");

        foreach (var property in properties) {
            Print.Index(index++, property.ToString());
        }

        Console.WriteLine();
    }
}

public record PropertyDeclaration(string Type, string Name)
{
    public static bool TryParse(Block block, out PropertyDeclaration propertyDeclaration) {
        propertyDeclaration = null!;
        if (!block.StartsWithUpper) return false;
        if (block.AnyInner) throw new Exception($"Property declaration '{block.Head}' cannot have inner blocks");
        var (type, name) = block.Head.DeconstructTwo();
        propertyDeclaration = new PropertyDeclaration(type, name);

        return true;
    }

    public override string ToString() => $"{Type} {Name}";
}