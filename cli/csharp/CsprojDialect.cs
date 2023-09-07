public class CsprojDialect : IDialect
{
    public Dictionary<string, Action<CodeBuilder>> Types => new ()
    {
        { "console", AppendConsoleStart },
        { "web", AppendWebStart },
        { "lib", AppendLibStart },
    };

    public const string Key = "csproj";
    
    public void Write(CodeBuilder code, Declaration declaration)
    {
        if (!declaration.TryGetFunction(out var functionDeclaration)) throw new ("can only has function declaration");
        if (!functionDeclaration.TryGetSingleStatement(out var statement)) throw new ("only single statement supported");
        if (!statement.TryGetFunctionCall(out var call)) throw new ("the only statement must be function call");
        if (!call.Path.TryGetNameOnly(out var calledFunctionKey)) throw new ("the only function call must be just by name");
        if (!Types.TryGetValue(calledFunctionKey, out var begin)) throw new($"unable to find project type '{calledFunctionKey}'");

        begin(code);
        
        Append(code, call.Arguments.LiteralsOfOptionalArgument("packages").Select(Package.Parse).ToArray());
        Append(code, call.Arguments.LiteralsOfOptionalArgument("usings").ToArray());
        AppendReferences(code, call.Arguments.LiteralsOfOptionalArgument("references").ToArray());

        Close(code);
    }

    public string GetFilename(Declaration declaration)
    {
        var projectName = declaration.Attributes[Key];
        return $"{projectName}.csproj";
    }

    public void AppendReferences(CodeBuilder code, string[] projectReferences)
    {
        if (!projectReferences.Any()) return;

        code.AppendLine();
        code.AppendLine("  <ItemGroup>");
        foreach (var projectReference in projectReferences)
        {
            code.AppendLine($"    <ProjectReference Include=\"{projectReference}.csproj\" />");
        }

        code.AppendLine("  </ItemGroup>");
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

    public void AppendLibStart(CodeBuilder code)
    {
        code.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        code.AppendLine("  <PropertyGroup>");
        code.AppendLine($"    <TargetFramework>net8.0</TargetFramework>");
        code.AppendLine("     <ImplicitUsings>enable</ImplicitUsings>");
        code.AppendLine("     <Nullable>enable</Nullable>");
        code.AppendLine("  </PropertyGroup>");
    }
    
    public void Append(CodeBuilder code, Package[] packages)
    {
        if (!packages.Any()) return;

        code.AppendLine();
        code.AppendLine("  <ItemGroup>");
        foreach (var package in packages)
        {
            code.AppendLine($"    <PackageReference Include=\"{package.Name}\" Version=\"{package.Version}\" />");
        }

        code.AppendLine("  </ItemGroup>");
    }

    public void Append(CodeBuilder code, string[] usings)
    {
        if (!usings.Any()) return;

        code.AppendLine();
        code.AppendLine("  <ItemGroup>");
        foreach (var usingStatement in usings)
        {
            code.AppendLine($"    <Using Include=\"{usingStatement}\" />");
        }

        code.AppendLine("  </ItemGroup>");
    }

    public void Close(CodeBuilder code)
    {
        code.AppendLine("</Project>");
    }
}

public record Package(string Name, string Version)
{
    public static Package Parse(string text)
    {
        var parts = text.Split(':');
        if (parts.Length != 2) throw new($"package must be in format 'name:version', but was '{text}'");
        return new Package(parts[0], parts[1]);
    }

    public static Package[] Parse(ArgumentAssignments assignments)
    {
        return Extract.LiteralsOfOptionalArgument(assignments, "packages")
            .Select<string, Package>(Parse)
            .ToArray();
    }
}

public class Extract
{
    public static IEnumerable<string> LiteralsOfOptionalArgument(ArgumentAssignments assignments, string argumentName)
    {
        if (!assignments.TryGetValue(argumentName, out var assignment))
            return Array.Empty<string>();

        return assignment
            .AsFunctionCall
            .Arguments
            .Values.Select(a => a.RequiredStringLiteral.OnlyString)
            .ToArray();
    }
}
