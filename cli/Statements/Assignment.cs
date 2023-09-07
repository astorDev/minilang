public record Assignment(Assignee Target, ReturningCall Assigned, bool SpaceAfter)
{
    public static bool TryParse(Block block, FunctionContext context, out Assignment assignment) {
        assignment = null!;
        if (!block.TryPopTwoAndRemainHeaded(out var returningCallBlock, out var poped)) return false;
        if (poped.Second != "=") return false;

        assignment = new Assignment(
            Assignee.Parse(poped.First, context), 
            ReturningCall.Parse(returningCallBlock, context), 
            block.SpaceAfter
        );

        return true;
    }
}

public record VariableCall(
    Variable Variable,
    bool IsNew,
    CallContinuationPart[] Chain
)
{
    public static VariableCall Parse(RawCallParts parts, FunctionContext context)
    {
        var (isNew, variable) = context.GetOrAddLocalVariable(parts.First);
        return new VariableCall(
            variable,
            isNew,
            CallContinuationPart.Parse(parts.Remaining, context)
        );
    }
}

public record Assignee(
    ArgumentCall? Argument = null,
    VariableCall? Variable = null
)
{
    public static Assignee Parse(string raw, FunctionContext context)
    {
        var parts = RawCallParts.Parse(raw);
        if (ArgumentCall.TryParseForAssignee(parts, context, out var argumentCall))
        {
            return new Assignee(Argument: argumentCall);
        }

        return new Assignee(Variable : VariableCall.Parse(parts, context));
    }

    public void On(Action<ArgumentCall> argument, Action<VariableCall> variable)
    {
        if (Argument != null) argument(Argument);
        if (Variable != null) variable(Variable);
    }

    public object TheOne => (object?)Argument ?? (object?)Variable ?? throw new Exception("Assignee has no concrete implementation");

    public override string ToString()
    {
        return TheOne.ToString()!;
    }
}