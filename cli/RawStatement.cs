// public record RawStatement(RawBlock Block)
// {
//     public static bool TryParse(RawBlock block, out RawStatement statement) {
//         statement = null!;
//         if (Char.IsLower(block.Content.Top[0]) && !block.Content.Top.Contains(":")) {
//             statement = new RawStatement(block);
//             return true;
//         }

//         return false;
//     }
// }

// public static class RawStatementUtil
// {
//     public static RawStatement[] Printed(this IEnumerable<RawStatement> statements) {
//         var index = 0;
//         foreach (var statement in statements)
//         {
//             Print.Header("Statements:");
//             Print.Index(index++, statement.Block.Filename);
//             //Console.WriteLine($"{index++}. (file: \"{statement.Block.Filename}\")");
//             Console.WriteLine(statement.Block.Content.Top);
//             Console.WriteLine(String.Join(Environment.NewLine, statement.Block.Content.Others));
//             Console.WriteLine();
//         }

//         return statements.ToArray();
//     }

//     public static IEnumerable<RawStatement> Read(RawBlock[] blocks) {
//         foreach (var block in blocks)
//             if (RawStatement.TryParse(block, out var statement)) yield return statement;
//     }
// }