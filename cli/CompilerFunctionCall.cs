// public record CompilerFunctionCall(FunctionCall call)
// {
//     public static Func<FunctionCall, ICompilerDialect?>[] CompilersResolutions = new Func<FunctionCall, ICompilerDialect?>[]
//     {
//         CsharpDialect.TryParse
//     };

//     public static bool TryParse(FunctionCall call, out ICompilerDialect dialect)
//     {
//         dialect = null!;

//         foreach (var compilerResolution in CompilersResolutions) {
//             var maybeCompiler = compilerResolution(call);
//             if (maybeCompiler != null) {
//                 dialect = maybeCompiler;
//                 return true;
//             }
//         }
    
//         return false;
//     }

//     public static Compiler Read(IEnumerable<FunctionCall> calls, string sourcePath)
//     {
//         foreach (var call in calls)
//             if (TryParse(call, out var compiler)) return Compiler.From(compiler, sourcePath);

//         throw new Exception("No compiler found");
//     }
// }