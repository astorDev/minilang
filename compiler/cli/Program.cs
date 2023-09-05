var directoryPath = args[0];
var silent = args.Contains("--silent");
Print.Silent = silent;

var compilerDialects = new Dictionary<string, Func<ArgumentAssignments, ICompilerDialect>>()
{
    { CsharpDialect.Key, CsharpDialect.Resolve }
};

Print.Line();

var files = FileData.Read(directoryPath, extension: "mini");
var declarations = Declaration.Read(files).Printed();

(var classDeclarations, declarations) = declarations.ForkParsed<Declaration, ClassDeclaration>(ClassDeclaration.TryParse);
classDeclarations.Printed();

(var functionDeclarations, _) = declarations.ForkParsed<Declaration, FunctionDeclaration>(FunctionDeclaration.TryParse);
functionDeclarations.Printed();

var compileFunctionDeclaration = functionDeclarations.SingleOrDefault(d => d.Signature.Name == "compile") ?? throw new Exception("unable to find compile function");
var compileFunctionCall = compileFunctionDeclaration.Statements.Single().AsFunctionCall;
var dialectResolver = compilerDialects[compileFunctionCall.Path.Name];
var dialect = dialectResolver(compileFunctionCall.Arguments);
var compiler = Compiler.From(dialect, directoryPath);
compiler.Printed();

compiler.PrepareTargetFolder();
compiler.CompileProjectFiles();
var mainFunction = functionDeclarations.SingleOrDefault(d => d.Signature.Name == "go") ?? throw new Exception("unable to find go function");
compiler.Compile(mainFunction);
compiler.Compile(classDeclarations);