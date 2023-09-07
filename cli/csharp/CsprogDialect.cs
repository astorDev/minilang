public class CsprogDialect : IDialect
{
    public const string Key = "csprog";
    
    public void Write(CodeBuilder code, Declaration declaration)
    {
        foreach (var statement in declaration.RequiredFunction.Statements) CsfileDialect.Append(code, statement);
    }

    public string GetFilename(Declaration declaration)
    {
        var folder = declaration.Attributes[Key];
        return Path.Combine(folder, "Program.cs");
    }
}