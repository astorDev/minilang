public static class ClassSignatureString
{
    public static string Stringify(this ClassSignature signature) {
        var genericArgumentStrings = signature.GenericArguments.Any() ? signature.GenericArguments.Select(a => a.Stringify()) : Array.Empty<string>();

        return signature.GenericArguments.Any() ? $"{signature.Name}<{String.Join(", ", genericArgumentStrings)}>" : signature.Name;
    }
}