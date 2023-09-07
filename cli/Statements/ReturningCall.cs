public record ReturningCall(
    StringLiteral? StringLiteral = null, 
    FunctionCall? FunctionCall = null, 
    LocalVariableCall? LocalVariableCall = null,
    Lambda? Lambda = null,
    BoolLiteral? BoolLiteral = null,
    ArgumentCall? ArgumentCall = null,
    MaybeHeadlessBlock? Unparsed = null)
{
    public static ReturningCall Parse(MaybeHeadlessBlock block, FunctionContext context)
    {
        if (block.TryBeHeaded(out var headed))
        {
            var headedResult = ParsedHeadedOrNull(headed, context);
            if (headedResult != null) return headedResult;
        }

        if (FunctionCall.TryParse(block, context, out var functionCall)) return new (FunctionCall: functionCall);

        return new ReturningCall(Unparsed: block);
    }

    public static ReturningCall? ParsedHeadedOrNull(Block block, FunctionContext context)
    {
        if (ArgumentCall.TryParseForReturningCall(block, context, out var argumentCall)) return new (ArgumentCall: argumentCall);
        if (BoolLiteral.TryParse(block, out var boolLiteral)) return new (BoolLiteral: boolLiteral);
        if (StringLiteral.TryParse(block, context, out var stringLiteral)) return new (StringLiteral: stringLiteral);
        if (Lambda.TryParse(block, out var lambda)) return new (Lambda: lambda);
        if (LocalVariableCall.TryParse(block, context, out var localVariableCall)) return new (LocalVariableCall: localVariableCall);

        return null;
    }

    public object TheOne
     =>
        (object?)StringLiteral ??
        (object?)FunctionCall ??
        (object?)LocalVariableCall ??
        (object?)Lambda ??
        (object?)BoolLiteral ??
        (object?)ArgumentCall ??
        Unparsed!;

    public override string ToString()
    {
        return TheOne.ToString()!;
    }

    public void On(
        Action<StringLiteral> stringLiteral,
        Action<FunctionCall> functionCall,
        Action<LocalVariableCall> localVariableCall,
        Action<Lambda> lambda,
        Action<BoolLiteral> boolLiteral,
        Action<ArgumentCall> argumentCall
    )
    {
        if (StringLiteral != null) stringLiteral(StringLiteral);
        if (FunctionCall != null) functionCall(FunctionCall);
        if (LocalVariableCall != null) localVariableCall(LocalVariableCall);
        if (Lambda != null) lambda(Lambda);
        if (BoolLiteral != null) boolLiteral(BoolLiteral);
        if (ArgumentCall != null) argumentCall(ArgumentCall);
    }

    public bool TryBeFunctionCall(out FunctionCall functionCall)
    {
        functionCall = null!;
        if (FunctionCall == null) return false;
        functionCall = FunctionCall;
        return true;
    }

    public StringLiteral RequiredStringLiteral => StringLiteral ?? throw new Exception($"Returning call is not StringLiteral, but '{TheOne.GetType()}'");
    public FunctionCall AsFunctionCall => FunctionCall ?? throw new Exception($"Returning call is not {nameof(FunctionCall)}, but '{TheOne.GetType()}'");
    public LocalVariableCall AsLocalVariableCall => LocalVariableCall ?? throw new Exception($"Returning call is not {nameof(LocalVariableCall)}, but '{TheOne.GetType()}'");
    public Lambda AsLambda => Lambda ?? throw new Exception($"Returning call is not {nameof(Lambda)}, but '{TheOne.GetType()}'");


}

