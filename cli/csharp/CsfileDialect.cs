public record CsfileDialect(GlobalContext Context) : IDialect
{
    public static readonly Dictionary<string, string> FunctionNamesMap = new()
    {
        { "print", "Console.WriteLine" },
        { "read", "Console.ReadLine" },
        { "webAppBuilder", "WebApplication.CreateBuilder"}
    };

    readonly CsharpBaseDialect _csharp = new();
    public const string Key = "csfile";

    public string GetFilename(Declaration declaration)
    {
        var filename = declaration.Attributes[Key];
        return filename + ".cs";
    }

    public void Write(CodeBuilder code, Declaration declaration)
    {
        declaration.On(
            onFunction: f => throw new NotImplementedException("Unable to append function declaration in csfile"),
            onClass: c => _csharp.AppendDataClass(code, c)
        );
    }

    public void Append(CodeBuilder code, Statement statement, bool inLambda = false)
    {
        statement.On(
            functionCall => Append(code, functionCall, skipSemicolon: inLambda),
            assignment => Append(code, assignment, skipSemicolon: inLambda),
            localVariableCall => Append(code, localVariableCall)
        );
        
        if (statement.SpaceAfter) code.AppendLine();
        if (!inLambda) code.AppendLine();
    }

    public void Append(CodeBuilder code, Assignment assignment, bool skipSemicolon = false)
    {
        Append(code, assignment.Target);
        code.Append(" = ");
        Append(code, assignment.Assigned);
        if (!skipSemicolon) code.Append(";");
    }

    public void Append(CodeBuilder code, Assignee assignee)
    {
        assignee.On(
            argument: x => Append(code, x),
            variable: x => Append(code, x)
        );
    }

    public void Append(CodeBuilder code, ArgumentCall argumentCall)
    {
        code.Append(argumentCall.Signature.Name);
        Append(code, argumentCall.Chain);
    }

    public void Append(CodeBuilder code, VariableCall variableCall)
    {
        if (variableCall.IsNew) code.Append("var ");
        code.Append(variableCall.Variable.Name);
        Append(code, variableCall.Chain);
    }

    public void Append(CodeBuilder code, CallContinuationPart[] assigneeChain)
    {
        foreach (var part in assigneeChain)
        {
            Append(code, part);
        }
    }

    public void Append(CodeBuilder code, CallContinuationPart part)
    {
        part.On(
            propertyCall: x => code.Append($".{x.PropertyName.PascalCase()}"),
            stringIndexer: x => {
                var sub = code.Sub();
                sub.Append("[");
                Append(sub, x);
                sub.Append("]");
            }
        );
    }

    public void Append(CodeBuilder code, FunctionCall call, bool skipSemicolon = false)
    {
        var csharpFunctionName = Name(call.Path);
        code.Append($"{csharpFunctionName}(");
        var argsCode = new CodeBuilder(", ");
        foreach (var argument in call.Arguments) {
            Append(argsCode.Sub(), argument.Value);
        }
        code.Append(argsCode.Build());
        code.Append(")");
        if (!skipSemicolon) code.Append(";");
    }

    public string Name(FunctionCallPath path) 
    {
        if (path.TryGetNameOnly(out var name)) {
            return ResolveGlobalFunctionName(name);
        }
        
        var (first, other) = path.SplitWithFirst();
        var all = new[] { first }.Union(other.Select(p => p.PascalCase()));
        return String.Join('.', all);
    }

    public string ResolveGlobalFunctionName(string name) 
    {
        if (Context.TryUseAsConstructor(name, out var @class)) return "new " + @class.Head.Me.Name;
        if (FunctionNamesMap.TryGetValue(name, out var globalFunctionName)) return globalFunctionName;

        return name;
    }

    public void Append(CodeBuilder code, ReturningCall returningCall)
    {
        returningCall.On
            (
                literal => Append(code, literal),
                functionCall => Append(code, functionCall),
                localVariableCall => code.Append(localVariableCall.Variable.Name),
                lambda => Append(code, lambda),
                boolLiteral => code.Append(boolLiteral.Value.ToString().ToLowerInvariant()),
                argumentCall => Append(code, argumentCall)
            );
    }

    public void Append(CodeBuilder code, Lambda lambda)
    {
        var argsString = lambda.Context.Arguments.Any() ? String.Join(", ", lambda.Context.Arguments.Select(ArgumentString)) : "";
        code.Append($"({argsString}) => ");
        Append(code, lambda.Statement, inLambda: true);
    }

    public string ArgumentString(ArgumentSignature signature)
    {
        var code = new CodeBuilder();
        if (signature.Type != null) {
            code.Append(ClassSignature(signature.Type));
            code.Append(" ");
        }

        code.Append(signature.Name);
        return code.Build();
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

    public void AppendClass(CodeBuilder code, ClassDeclaration declaration)
    {
        if (!declaration.Attributes.ContainsKey("data")) throw new NotImplementedException("Non-data classes are not supported in csfile");
        _csharp.AppendDataClass(code, declaration);
    }

    public void Append(CodeBuilder code, LocalVariableCall localVariableCall)
    {
        code.Append(localVariableCall.Variable.Name);
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

    public void AppendDataClassProperty(CodeBuilder code, PropertyDeclaration property, bool isLast)
    {
        var trailing = isLast ? "" : ",";
        code.AppendLine($"{Identation.One}{property.Type} {property.Name}{trailing}");
    }

    public string ClassSignature(ClassSignature signature) {
        var genericArgumentStrings = signature.GenericArguments.Any() ? signature.GenericArguments.Select(ClassSignature) : Array.Empty<string>();

        return signature.GenericArguments.Any() ? $"{signature.Name}<{String.Join(", ", genericArgumentStrings)}>" : signature.Name;
    }

     public string Inheritors(ClassSignature[] signatures) => signatures.Any() ? " : " + String.Join(", ", signatures.Select(ClassSignature)) : "";
}