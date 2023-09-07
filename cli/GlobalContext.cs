public record GlobalContext
{
    private readonly Dictionary<string, ClassDeclaration> _constructors = new();

    public void Register(Declaration[] declarations)
    {
        foreach (var declaration in declarations)
        {
            declaration.On(
                functionDeclaration => {},
                Register
            );
        }
    }

    public void Register(ClassDeclaration classDeclaration)
    {
        _constructors.Add(classDeclaration.Head.Me.Name.CamelCase(), classDeclaration);
    }

    public bool TryUseAsConstructor(string globalFunctionName, out ClassDeclaration classDeclaration) => _constructors.TryGetValue(globalFunctionName, out classDeclaration!);
}