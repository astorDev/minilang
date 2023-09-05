using System.Text;

public record CsharpDialect(string? Namespace, CsharpProject? Project) : ICompilerDialect
{
    public Dictionary<string, string> FunctionNamesMap = new Dictionary<string, string>()
    {
        { "print", "Console.WriteLine" },
        { "read", "Console.ReadLine" },
        { "webAppBuilder", "WebApplication.CreateBuilder"}
    };

    public static CsharpDialect Resolve(ArgumentAssignments arguments)
    {
        var ns = arguments.GetValueOrDefault("namespace")?.AsStringLiteral.ResultString;
        var project = CsharpProject.OptionallyParse(arguments);
        return new CsharpDialect(ns, project);
    }

    public const string Key = "csharp";
    public string TargetDir => "compiled";
    public string MainFilename => "Program.cs";

    public IEnumerable<string> ProjectFilenames
    {
        get
        {
            if (Project != null) yield return Project.Filename;
        }
    }

    public string Filename(string source) => $"{source}.cs";

    public void AppendPreClass(StringBuilder code)
    {
        code.AppendLine($"namespace {Namespace};");
        code.AppendLine();
    }

    public void AppendDataClass(StringBuilder code, ClassDeclaration declaration)
    {
        code.AppendLine($"public record {ClassSignature(declaration.Head.Me)}(");
        for (var i = 0; i < declaration.Body.Properties.Length; i++)
        {
            AppendDataClassProperty(code, declaration.Body.Properties[i], i == declaration.Body.Properties.Length - 1);
        }

        code.AppendLine($"){Inheritors(declaration.Head.BaseClasses)};");
    }

    public static void AppendDataClassProperty(StringBuilder code, PropertyDeclaration property, bool isLast)
    {
        var trailing = isLast ? "" : ",";
        code.AppendLine($"{Identation.One}{property.Type} {property.Name}{trailing}");
    }

    public void AppendRegularClass(StringBuilder code, ClassDeclaration declaration)
    {
        code.AppendLine($"public class {ClassSignature(declaration.Head.Me)}{Inheritors(declaration.Head.BaseClasses)}");
        code.AppendLine("{");
        code.AppendLine("}");
    }

    public string ClassSignature(ClassSignature signature) {
        var genericArgumentStrings = signature.GenericArguments.Any() ? signature.GenericArguments.Select(ClassSignature) : Array.Empty<string>();

        return signature.GenericArguments.Any() ? $"{signature.Name}<{String.Join(", ", genericArgumentStrings)}>" : signature.Name;
    }

    public string Inheritors(ClassSignature[] signatures) => signatures.Any() ? " : " + String.Join(", ", signatures.Select(ClassSignature)) : "";

    public override string ToString()
    {
        return $"CsharpCompiler (namespace:\"{Namespace}\")";
    }

    public void AppendMainStart(CodeBuilder code, FunctionSignature signature)
    {
    }

    public void AppendMainEnd(CodeBuilder code)
    {
    }

    public void Append(CodeBuilder code, FunctionCall call)
    {
        var csharpFunctionName = Name(call.Path);
        code.Append($"{csharpFunctionName}(");
        var argsCode = new CodeBuilder(", ");
        foreach (var argument in call.Arguments) {
            Append(argsCode.Sub(), argument.Value);
        }
        code.Append(argsCode.Build());
        code.Append(");");
    }

    public string Name(FunctionCallPath path) {
        if (path.TryGetNameOnly(out var name)) return FunctionNamesMap[name];
        
        (var steps, name) = path.SplitWithName();
        var all = steps.Append(name.PascalCase());
        return String.Join('.', all);
    }

    public void Append(CodeBuilder code, Assignment assignment)
    {
        code.Append($"var {assignment.Assignee.Name} = ");
        Append(code, assignment.Assigned.AsFunctionCall);
    }

    public void Append(CodeBuilder code, ReturningCall returningCall)
    {
        returningCall.On
            (
                literal => code.Append($"\"{returningCall.AsStringLiteral.ResultString}\""),
                functionCall => Append(code, functionCall),
                localVariableCall => code.Append(localVariableCall.Variable.Name),
                lambda => Append(code, lambda)
            );
    }

    public void AppendProjectFileBody(StringBuilder code, string filename)
    {
        if (filename == Project?.Filename) Project.Append(code); return;
    }

    public void Append(CodeBuilder code, LocalVariableCall localVariableCall)
    {
        code.Append(localVariableCall.Variable.Name);
    }

    public void Append(CodeBuilder code, Lambda lambda)
    {
        code.Append("() => ");
        Append(code, lambda.ReturningCall);
    }
}