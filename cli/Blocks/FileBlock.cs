public record FileBlock(Block Block, string Filename)
{
    public static IEnumerable<FileBlock> Read(FileData[] files) 
    {
        foreach (var file in files) 
        {
            foreach (var block in Block.Read(file.Content)) {
                yield return new(block, file.Name);
            }
        }
    }
}