public record Assignment(Variable Assignee, ReturningCall Assigned)
{
    public static bool TryParse(Block block, FunctionContext context, out Assignment assignment) {
        assignment = null!;
        if (!block.TryPopTwo(out var returningCallBlock, out var poped)) return false;
        if (poped.Second != "=") return false;

        var assignee = new Variable(poped.First);
        context.Add(assignee);
        assignment = new Assignment(assignee, ReturningCall.Parse(returningCallBlock, context));
        return true;
    }
}
