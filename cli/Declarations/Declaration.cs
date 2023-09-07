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

    public FunctionDeclaration RequiredFunction => Function ?? throw new($"Unable to use {this.TheOne} as function declaration");

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

    public bool TryGetClass(out ClassDeclaration classDeclaration) {
        classDeclaration = Class!;
        return Class != null;
    }

    public bool TryGetFunction(out FunctionDeclaration functionDeclaration) {
        functionDeclaration = Function!;
        return Function != null;
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

    public override string ToString()
    {
        return TheOne.ToString()!;
    }
}

public static class DeclarationExtensions
{
    public static (FunctionDeclaration[] Functions, ClassDeclaration[] Classes) Fork(this Declaration[] declarations)
    {
        var functions = new List<FunctionDeclaration>();
        var classes = new List<ClassDeclaration>();

        foreach (var declaration in declarations)
        {
            declaration.On(
                onFunction: functions.Add,
                onClass: classes.Add
            );
        }

        return (functions.ToArray(), classes.ToArray());
    }

    public static Declaration[] Printed(this Declaration[] declarations)
    {
        var (functionDeclarations, classDeclarations) = declarations.Fork();

        functionDeclarations.Printed();
        classDeclarations.Printed();

        return declarations;
    }
}