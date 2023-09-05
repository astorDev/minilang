public class CsprogDialect : IDialect
{
    readonly CsharpBaseDialect _csharp = new();
    public const string Key = "csprog";
    
    public void Write(CodeBuilder code, Declaration declaration)
    {
        foreach (var statement in declaration.RequiredFunction.Statements) _csharp.Append(code, statement);
    }

    public string GetFilename(Declaration declaration)
    {
        return "Program.cs";
    }
}