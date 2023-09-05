var directoryPath = args[0];
var silent = args.Contains("--silent");
Print.Silent = silent;

Print.Line();

var files = FileData.Read(directoryPath, extension: "mini");
var fileBlocks = FileBlock.Read(files).Printed();
var declarations = Declaration.Read(fileBlocks);

var translator = new Translator(Path.Combine(directoryPath, "compiled"), new()
{
    { CsprojDialect.Key, new CsprojDialect() },
    { CsprogDialect.Key, new CsprogDialect() }
});

translator.PrepareTargetFolder();
translator.Translate(declarations);

//var translator = new Translator(directoryPath)


// var compileFunctionDeclaration = functionDeclarations.SingleOrDefault(d => d.Signature.Name == "compile") ?? throw new Exception("unable to find compile function");
// var compileFunctionCall = compileFunctionDeclaration.Statements.Single().AsFunctionCall;
// var dialectResolver = compilerDialects[compileFunctionCall.Path.Name];
// var dialect = dialectResolver(compileFunctionCall.Arguments);
// var compiler = Compiler.From(dialect, directoryPath);
// compiler.Printed();
//
// compiler.PrepareTargetFolder();
// compiler.CompileProjectFiles();
// var mainFunction = functionDeclarations.SingleOrDefault(d => d.Signature.Name == "go") ?? throw new Exception("unable to find go function");
// compiler.Compile(mainFunction);
// compiler.Compile(classDeclarations);