public class CsharpBaseDialect
{
    public static readonly Dictionary<string, string> FunctionNamesMap = new()
    {
        { "print", "Console.WriteLine" },
        { "read", "Console.ReadLine" },
        { "webAppBuilder", "WebApplication.CreateBuilder"}
    };

    public static CsharpDialect Resolve(ArgumentAssignments arguments)
    {
        var ns = arguments.GetValueOrDefault("namespace")?.RequiredStringLiteral.OnlyString;
        var project = CsharpProject.OptionallyParse(arguments);
        return new CsharpDialect(ns, project);
    }

    public const string Key = "csharp";

    public void Append(CodeBuilder code, Statement statement)
    {
        statement.On(
            functionCall => Append(code, functionCall),
            assignment => Append(code, assignment),
            localVariableCall => Append(code, localVariableCall)
        );
        
        code.AppendLine();
        if (statement.SpaceAfter) code.AppendLine();
    }
    
    public void AppendDataClass(CodeBuilder code, ClassDeclaration declaration)
    {
        code.AppendLine($"public record {ClassSignature(declaration.Head.Me)}(");
        for (var i = 0; i < declaration.Body.Properties.Length; i++)
        {
            AppendDataClassProperty(code, declaration.Body.Properties[i], i == declaration.Body.Properties.Length - 1);
        }

        code.Append($"){Inheritors(declaration.Head.BaseClasses)};");
    }

    public static void AppendDataClassProperty(CodeBuilder code, PropertyDeclaration property, bool isLast)
    {
        var trailing = isLast ? "" : ",";
        code.AppendLine($"{Identation.One}{property.Type} {property.Name}{trailing}");
    }

    public void AppendRegularClass(CodeBuilder code, ClassDeclaration declaration)
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
        
        var (first, other) = path.SplitWithFirst();
        var all = new[] { first }.Union(other.Select(p => p.PascalCase()));
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
                literal => Append(code, literal),
                functionCall => Append(code, functionCall),
                localVariableCall => code.Append(localVariableCall.Variable.Name),
                lambda => Append(code, lambda)
            );
    }

    public void Append(CodeBuilder code, StringLiteral literal) 
    {
        if (literal.IsInterpolated) code.Append("$");
        code.Append("\"");
        foreach (var part in literal)
        {
            part.On(
                returningCall => {
                    code.Append("{");
                    Append(code, returningCall);
                    code.Append("}");
                },
                code.Append
            );
        }
        code.Append("\"");
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