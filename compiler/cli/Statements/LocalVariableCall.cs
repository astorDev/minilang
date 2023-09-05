public record LocalVariableCall(Variable Variable)
{
    public static bool TryParse(Block block, FunctionContext context, out LocalVariableCall localVariableCall)
    {
        localVariableCall = null!;
        if (!block.TryGetOnlyMain(out var main)) return false;
        if (!context.TryGetLocalVariable(main, out var localVariable)) return false;
        
        localVariableCall = new LocalVariableCall(localVariable);
        return true;
    }

    public override string ToString()
    {
        return Variable.Name;
    }
}