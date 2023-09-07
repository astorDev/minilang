public class CsharpBaseDialect
{
    public const string Key = "csharp";
    
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
}