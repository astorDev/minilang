public record Lambda(Statement Statement, FunctionContext Context)
{
    public static bool TryParse(Block block, out Lambda lambda) 
    {
        lambda = null!;
        if (!block.TryPopMain("=>", out var lambdaBodyBlock)) return false;

        var context = new FunctionContext();
        var statement = Statement.Parse(lambdaBodyBlock, context);
        lambda = new Lambda(statement, context);

        return true;
    }

    public override string ToString()
    {
        return $"Lambda {Context} = {Statement}";
    }
}