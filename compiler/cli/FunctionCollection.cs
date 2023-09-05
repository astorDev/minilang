public class FunctionCollection
{
    private Dictionary<FunctionSignature, FunctionSignature> inner = new Dictionary<FunctionSignature, FunctionSignature>();

    public static FunctionCollection CreateFromKnown()
    {
        var collection = new FunctionCollection();
        collection.Add(new FunctionSignature("csharp"));
        return collection; 
    }

    public void Add(FunctionSignature signature)
    {
        inner.Add(signature, signature);
    }
}

// public record CallOfFunction(FunctionAccessor accessor, Dictionary<string, BlockContent> arguments)
// {
//     public bool TryParse(BlockContent block, FunctionCollection knownFunctions, out CallOfFunction call)
//     {
//         call = null!;
        
//     }
// }

public record FunctionAccessor(string path)
{

}

public record AccessorRow(string[] Pointer, string Tail)
{
    public AccessorRow Parse(string path)
    {
        var split = path.Split(".");
        var pointer = split.Take(split.Length - 1).ToArray();
        var tail = split.Last();
        return new AccessorRow(pointer, tail);
    }
}