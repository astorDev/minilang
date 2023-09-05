public record ReturnStatement(Block Returned)
{
    public static ReturnStatement? IfParsed(Block block)
    {
        return block.After("return") is Block next ? new ReturnStatement(next) : null;
    }

    override public string ToString()
    {
        return $"ReturnStatement {{ {Returned} }}";
    }
}