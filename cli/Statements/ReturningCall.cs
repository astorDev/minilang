public record ReturningCall(
    StringLiteral? StringLiteral = null, 
    FunctionCall? FunctionCall = null, 
    LocalVariableCall? LocalVariableCall = null,
    Lambda? Lambda = null,
    MaybeHeadlessBlock? Unparsed = null)
{
    public static ReturningCall Parse(MaybeHeadlessBlock block, FunctionContext context)
    {
        if (block.TryBeHeaded(out var headed))
        {
            if (Lambda.TryParse(headed, out var lambda)) return lambda;
            if (StringLiteral.TryParse(headed, context, out var stringLiteral)) return stringLiteral;
            if (LocalVariableCall.TryParse(headed, context, out var localVariableCall)) return localVariableCall;
        }

        if (FunctionCall.TryParse(block, context, out var functionCall)) return functionCall;

        return new ReturningCall(Unparsed: block);
    }

    public static implicit operator ReturningCall(Lambda lambda) => new ReturningCall(Lambda: lambda);
    public static implicit operator ReturningCall(LocalVariableCall localVariableCall) => new ReturningCall(LocalVariableCall: localVariableCall);
    public static implicit operator ReturningCall(StringLiteral stringLiteral) => new ReturningCall(StringLiteral : stringLiteral);
    public static implicit operator ReturningCall(FunctionCall functionCall) => new ReturningCall(FunctionCall : functionCall);
    public static implicit operator ReturningCall(Block unparsed) => new ReturningCall(Unparsed : unparsed);

    public override string ToString()
    {
        return TheOne.ToString()!;
    }

    public void On(
        Action<StringLiteral> stringLiteral,
        Action<FunctionCall> functionCall,
        Action<LocalVariableCall> localVariableCall,
        Action<Lambda> lambda
    )
    {
        if (StringLiteral != null) stringLiteral(StringLiteral);
        if (FunctionCall != null) functionCall(FunctionCall);
        if (LocalVariableCall != null) localVariableCall(LocalVariableCall);
        if (Lambda != null) lambda(Lambda);
    }

    public StringLiteral RequiredStringLiteral => StringLiteral ?? throw new Exception($"Returning call is not StringLiteral, but '{TheOne.GetType()}'");
    public FunctionCall AsFunctionCall => FunctionCall ?? throw new Exception($"Returning call is not {nameof(FunctionCall)}, but '{TheOne.GetType()}'");
    public LocalVariableCall AsLocalVariableCall => LocalVariableCall ?? throw new Exception($"Returning call is not {nameof(LocalVariableCall)}, but '{TheOne.GetType()}'");
    public Lambda AsLambda => Lambda ?? throw new Exception($"Returning call is not {nameof(Lambda)}, but '{TheOne.GetType()}'");

    public object TheOne
     =>
        (object?)StringLiteral ??
        (object?)FunctionCall ??
        (object?)LocalVariableCall ??
        (object?)Lambda ??
        Unparsed!;
}

