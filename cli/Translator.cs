public interface IDialect
{
    void Write(CodeBuilder code, Declaration declaration);
    string GetFilename(Declaration declaration);
}

public record Translator(string TargetDir, Dictionary<string, IDialect> Dialects)
{
    public void PrepareTargetFolder()
    {
        if (Directory.Exists(TargetDir)) Directory.Delete(TargetDir, true);
        Directory.CreateDirectory(TargetDir);
    }

    public void Translate(IEnumerable<Declaration> declarations)
    {
        foreach (var x in declarations.GroupBy(DialectAndFilename))
        {
            Print.Line($"Writing {x.Key.Filename} with {x.Key.Dialect}");
            var code = new CodeBuilder();
            foreach (var declaration in x) x.Key.Dialect.Write(code, declaration);
            Save(code, x.Key.Filename);
        }
    }

    (IDialect Dialect, string Filename) DialectAndFilename(Declaration declaration)
    {
        foreach (var attribute in declaration.Attributes)
        {
            if (Dialects.TryGetValue(attribute.Key, out var dialect))
            {
                return (dialect, dialect.GetFilename(declaration));
            }
        }

        throw new($"Dialect not found for {declaration}");
    }
    
    void Save(CodeBuilder code, string filename)
    {
        var fullPath = Path.Combine(TargetDir, filename);
        var directory = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(directory);

        File.AppendAllText(fullPath, code.Build());
        Print.Line($"Saved {fullPath}");
    }
}