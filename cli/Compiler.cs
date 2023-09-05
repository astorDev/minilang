using System.Text;

public interface ICompilerDialect
{
    string TargetDir { get; }
    string MainFilename { get; }
    string Filename(string source);
    IEnumerable<string> ProjectFilenames { get; }

    void AppendProjectFileBody(StringBuilder code, string filename);
    void AppendMainStart(CodeBuilder code, FunctionSignature signature);
    void AppendMainEnd(CodeBuilder code);
    void Append(CodeBuilder code, FunctionCall functionCall);
    void Append(CodeBuilder code, Assignment assignment);
    void Append(CodeBuilder code, LocalVariableCall localVariableCall);
    void Append(CodeBuilder code, Lambda lambda);
    void AppendPreClass(StringBuilder code);
    void AppendDataClass(StringBuilder code, ClassDeclaration declaration);
    void AppendRegularClass(StringBuilder code, ClassDeclaration declaration);
}

// public record Compiler(ICompilerDialect Dialect, string TargetDir)
// {
//     private void Save(StringBuilder code, string filename)
//     {
//         File.AppendAllText(Path.Combine(TargetDir, filename), code.ToString());
//     }
//
//         private void Save(CodeBuilder code, string filename)
//     {
//         File.AppendAllText(Path.Combine(TargetDir, filename), code.Build());
//     }
//
//     public void CompileProjectFiles()
//     {
//         foreach (var filename in Dialect.ProjectFilenames) {
//             var code = new StringBuilder();
//             Dialect.AppendProjectFileBody(code, filename);
//
//             Save(code, filename);
//         }
//     }
//
//     public void Compile(ClassDeclaration[] declarations)
//     {
//         foreach (var fileDeclarations in declarations.GroupBy(d => d.Filename == "" ? Dialect.MainFilename : d.Filename))
//         {
//             var code = new StringBuilder();
//
//             Dialect.AppendPreClass(code);
//             AppendToFile(code, fileDeclarations.ToArray());
//
//             Save(code, Dialect.Filename(fileDeclarations.Key));
//         }
//     }
//
//     public void Compile(FunctionDeclaration mainFunction)
//     {
//         var code = new CodeBuilder();
//
//         Dialect.AppendMainStart(code, mainFunction.Signature);
//         foreach (var statement in mainFunction.Statements)
//         {
//             statement.On(
//                 functionCall => Dialect.Append(code, functionCall),
//                 assignment => Dialect.Append(code, assignment),
//                 localVariableCall => Dialect.Append(code, localVariableCall)
//             );
//             
//             if (statement.SpaceAfter) code.AppendLine();
//             code.AppendLine();
//         }
//         Dialect.AppendMainEnd(code);
//
//         Save(code, Dialect.Filename(Dialect.MainFilename));
//     }
//
//     public void AppendToFile(StringBuilder code, IEnumerable<ClassDeclaration> classes)
//     {
//         foreach (var declaration in classes)
//         {
//             if (declaration.Head.Tags.Any(t => t.Name == SystemClassDeclarationTag.Data)) {
//                 Dialect.AppendDataClass(code, declaration);
//             } else {
//                 Dialect.AppendRegularClass(code, declaration);
//             }
//         }
//     }
//
//     public void Compile(ClassDeclaration declaration)
//     {
//         var code = new StringBuilder();
//
//         Dialect.AppendPreClass(code);
//         
//         if (declaration.Head.Tags.Any(t => t.Name == SystemClassDeclarationTag.Data)) {
//             Dialect.AppendDataClass(code, declaration);
//         } else {
//             Dialect.AppendRegularClass(code, declaration);
//         }
//
//         var outPath = Path.Combine(TargetDir, Dialect.Filename(declaration.Filename));
//         Save(code, Dialect.Filename(declaration.Filename));
//     }
//
//     public void PrepareTargetFolder()
//     {
//         if (Directory.Exists(TargetDir)) Directory.Delete(TargetDir, true);
//         Directory.CreateDirectory(TargetDir);
//     }
//
//     public static Compiler From(ICompilerDialect Dialect, string SourcePath)
//     {
//         return new Compiler(Dialect, Path.Combine(SourcePath, Dialect.TargetDir));
//     }
//
//     public override string ToString()
//     {
//         return $"Dialect:\"{Dialect}\")";
//     }
// }
//
// public static class CompilerUtil {
//     public static Compiler Printed(this Compiler compiler) {
//         Print.Header("Compiler:");
//         Print.Line(compiler.ToString());
//         Print.Line();
//         return compiler;
//     }
//
//     public static void CompileClassDeclarationsLoud(this Compiler compiler, ClassDeclaration[] classDeclarations) {
//         Print.Header("Compiling class declarations:");
//         var index = 0;
//
//         foreach (var classDeclaration in classDeclarations)
//         {
//             Print.FileIndex(index++, classDeclaration.Filename, $"class: \"{classDeclaration.Head.Me}\"");
//         }
//
//         compiler.Compile(classDeclarations);
//     }
//}