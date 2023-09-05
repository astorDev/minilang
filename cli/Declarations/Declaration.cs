public interface IDeclaration
{
    Attributes Attributes { get; }
}

public record Declaration(
    FunctionDeclaration? Function = null,
    ClassDeclaration? Class = null
)
{
    public static implicit operator Declaration(FunctionDeclaration functionDeclaration) => new(Function: functionDeclaration);
    public static implicit operator Declaration(ClassDeclaration classDeclaration) => new(Class: classDeclaration);

    public FunctionDeclaration RequiredFunction => Function ?? throw new("Unable to use the declaration as function declaration");

    public static Declaration[] Read(IEnumerable<FileBlock> blocks)
    {
        return blocks.Select(Parse).ToArray();
    }
    
    public static Declaration Parse(FileBlock block)
    {
        if (FunctionDeclaration.TryParse(block, out var function)) return function;
        if (ClassDeclaration.TryParse(block, out var @class)) return @class;

        throw new($"Unable to parse declaration of '{block}'");
    }

    public IDeclaration TheOne => (IDeclaration?)Function ?? (IDeclaration?)Class ?? throw new Exception("The declaration is neither Function or Class declaration");
    public Attributes Attributes => TheOne.Attributes;

    public void On(
        Action<FunctionDeclaration> onFunction,
        Action<ClassDeclaration> onClass)
    {
        if (Function != null) onFunction(Function);
        if (Class != null) onClass(Class);
    }
}