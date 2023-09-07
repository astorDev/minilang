var directoryPath = args[0];
var silent = args.Contains("--silent");
Print.Silent = silent;

Print.Line();

var files = FileData.Read(directoryPath, extension: "mini");
var fileBlocks = FileBlock.Read(files).Printed();
var declarations = Declaration.Read(fileBlocks).Printed();

var translator = new Translator(Path.Combine(directoryPath, "compiled"), new()
{
    { CsprojDialect.Key, new CsprojDialect() },
    { CsprogDialect.Key, new CsprogDialect() },
    { CsfileDialect.Key, new CsfileDialect() },
});

translator.PrepareTargetFolder();
translator.Translate(declarations);