using System.Text;

public record CsharpProject(string Type, string? ProjectName = null)
{
    public Dictionary<string, Action<StringBuilder>> Types => new ()
    {
        { "console", AppendConsole },
        { "web", AppendWeb }
    };

    public static CsharpProject? OptionallyParse(ArgumentAssignments assignments)
    {
        var call = assignments.GetValueOrDefault(PositionalArgument.Zero)?.AsFunctionCall;
        if (call == null) return null;

        return new CsharpProject(
            call.Path.Name,
            ProjectName : call.Arguments.GetValueOrDefault(PositionalArgument.Zero)?.AsStringLiteral.ResultString
        );
    }

    public string Filename => $"{ProjectName}.csproj";

    public void Append(StringBuilder code)
    {
        var render = Types.GetValueOrDefault(Type) ?? throw new Exception($"Unknown project type `{Type}`");
        render(code);
    }

    public void AppendConsole(StringBuilder code)
    {
        code.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
        code.AppendLine("  <PropertyGroup>");
        code.AppendLine($"    <OutputType>exe</OutputType>");
        code.AppendLine($"    <TargetFramework>net8.0</TargetFramework>");
        code.AppendLine("     <ImplicitUsings>enable</ImplicitUsings>");
        code.AppendLine("     <Nullable>enable</Nullable>");
        code.AppendLine("  </PropertyGroup>");
        code.AppendLine("</Project>");
    }

    public void AppendWeb(StringBuilder code)
    {
        code.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk.Web\">");
        code.AppendLine("  <PropertyGroup>");
        code.AppendLine($"    <TargetFramework>net8.0</TargetFramework>");
        code.AppendLine("     <ImplicitUsings>enable</ImplicitUsings>");
        code.AppendLine("     <Nullable>enable</Nullable>");
        code.AppendLine("  </PropertyGroup>");
        code.AppendLine("</Project>");
    }
}