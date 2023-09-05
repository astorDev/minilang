public record LocalVariableCall(Variable Variable, bool SpaceAfter = false)
{
    public static bool TryParse(Block block, FunctionContext context, out LocalVariableCall localVariableCall)
    {
        localVariableCall = null!;
        if (!block.TryGetOnlyMain(out var main)) return false;
        if (!context.TryGetLocalVariable(main, out var localVariable)) return false;
        
        localVariableCall = new LocalVariableCall(localVariable, block.SpaceAfter);
        return true;
    }

    public override string ToString()
    {
        return Variable.Name;
    }
}