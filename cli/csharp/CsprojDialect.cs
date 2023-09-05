public class CsprojDialect : IDialect
{
    public Dictionary<string, Action<CodeBuilder>> Types => new ()
    {
        { "console", AppendConsoleStart },
        { "web", AppendWebStart }
    };

    public const string Key = "csproj";
    
    public void Write(CodeBuilder code, Declaration declaration)
    {
        var typeKey = declaration.RequiredFunction.SingleStatement.RequiredFunctionCall.Path.OnlyName();
        if (!Types.TryGetValue(typeKey, out var begin)) throw new($"unable to find project type '{typeKey}'");
        begin(code);
        Close(code);
    }

    public string GetFilename(Declaration declaration)
    {
        var projectName = declaration.Attributes[Key];
        return $"{projectName}.csproj";
    }

    public void AppendConsoleStart(CodeBuilder code)
    {
        code.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        code.AppendLine("  <PropertyGroup>");
        code.AppendLine($"    <OutputType>exe</OutputType>");
        code.AppendLine($"    <TargetFramework>net8.0</TargetFramework>");
        code.AppendLine("     <ImplicitUsings>enable</ImplicitUsings>");
        code.AppendLine("     <Nullable>enable</Nullable>");
        code.AppendLine("  </PropertyGroup>");
    }

    public void AppendWebStart(CodeBuilder code)
    {
        code.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk.Web\">");
        code.AppendLine("  <PropertyGroup>");
        code.AppendLine($"    <TargetFramework>net8.0</TargetFramework>");
        code.AppendLine("     <ImplicitUsings>enable</ImplicitUsings>");
        code.AppendLine("     <Nullable>enable</Nullable>");
        code.AppendLine("  </PropertyGroup>");
    }
    
    public void Close(CodeBuilder code)
    {
        code.AppendLine("</Project>");
    }
}