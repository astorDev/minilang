using System.Collections.Immutable;
using System.Text;

public record CsharpProject(string Type, 
    Package[] Packages, 
    string[] Usings,
    string? ProjectName = null)
{
    public Dictionary<string, Action<StringBuilder>> Types => new ()
    {
        { "console", AppendConsoleStart },
        { "web", AppendWebStart }
    };

    public static CsharpProject? OptionallyParse(ArgumentAssignments assignments)
    {
        var call = assignments.GetValueOrDefault(PositionalArgument.Zero)?.AsFunctionCall;
        if (call == null) return null;

        return new(
            call.Path.Name,
            Packages: Extract.LiteralsOfOptionalArgument(assignments, "packages").Select(Package.Parse).ToArray(),
            Usings: Extract.LiteralsOfOptionalArgument(assignments, "usings").ToArray(),
            ProjectName : call.Arguments.GetValueOrDefault(PositionalArgument.Zero)?.RequiredStringLiteral.OnlyString
        );
    }

    public string Filename => $"{ProjectName}.csproj";

    public void Append(StringBuilder code)
    {
        var render = Types.GetValueOrDefault(Type) ?? throw new Exception($"Unknown project type `{Type}`");
        render(code);
        AppendPackages(code);
        AppendUsings(code);
        Close(code);
    }

    public void Close(StringBuilder code)
    {
        code.AppendLine("</Project>");
    }

    public void AppendConsoleStart(StringBuilder code)
    {
        code.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        code.AppendLine("  <PropertyGroup>");
        code.AppendLine($"    <OutputType>exe</OutputType>");
        code.AppendLine($"    <TargetFramework>net8.0</TargetFramework>");
        code.AppendLine("     <ImplicitUsings>enable</ImplicitUsings>");
        code.AppendLine("     <Nullable>enable</Nullable>");
        code.AppendLine("  </PropertyGroup>");
    }

    public void AppendWebStart(StringBuilder code)
    {
        code.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk.Web\">");
        code.AppendLine("  <PropertyGroup>");
        code.AppendLine($"    <TargetFramework>net8.0</TargetFramework>");
        code.AppendLine("     <ImplicitUsings>enable</ImplicitUsings>");
        code.AppendLine("     <Nullable>enable</Nullable>");
        code.AppendLine("  </PropertyGroup>");
    }

    public void AppendPackages(StringBuilder stringBuilder)
    {
        if (!Packages.Any()) return;

        stringBuilder.AppendLine();
        stringBuilder.AppendLine("  <ItemGroup>");
        foreach (var package in Packages)
        {
            stringBuilder.AppendLine($"    <PackageReference Include=\"{package.Name}\" Version=\"{package.Version}\" />");
        }

        stringBuilder.AppendLine("  </ItemGroup>");
    }

    public void AppendUsings(StringBuilder code)
    {
        if (!Usings.Any()) return;

        code.AppendLine();
        code.AppendLine("  <ItemGroup>");
        foreach (var usingStatement in Usings)
        {
            code.AppendLine($"    <Using Include=\"{usingStatement}\" />");
        }

        code.AppendLine("  </ItemGroup>");
    }
}