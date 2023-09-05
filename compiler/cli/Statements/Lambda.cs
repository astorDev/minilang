public record Lambda(ReturningCall ReturningCall, FunctionContext LambdaContext)
{
    public static bool TryParse(Block block, out Lambda lambda) 
    {
        lambda = null!;
        if (!block.TryPopMain("=>", out var lambdaBodyBlock)) return false;

        var context = new FunctionContext();
        lambda = new Lambda(ReturningCall.Parse(lambdaBodyBlock, context), context);

        return true;
    }
}