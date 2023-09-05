public record Statement(
    FunctionCall? FunctionCall = null, 
    Assignment? Assignment = null,
    LocalVariableCall? LocalVariableCall = null
)
{
    public bool SpaceAfter => 
        (bool?)FunctionCall?.SpaceAfter ??
        (bool?)Assignment?.SpaceAfter ??
        (bool?)LocalVariableCall?.SpaceAfter ??
        throw new Exception("Unknown statement");


    public FunctionCall RequiredFunctionCall => FunctionCall ?? throw new Exception($"Statement is not function call, but {Concrete.GetType()}");
    public Assignment AsAssignment => Assignment ?? throw new Exception($"The statement is not assignment, but {Concrete.GetType()}");
    public LocalVariableCall AsLocalVariableCall => LocalVariableCall ?? throw new Exception($"The statement is not local variable call, but {Concrete.GetType()}");

    public static implicit operator Statement(FunctionCall functionCall) => new Statement(FunctionCall: functionCall);
    public static implicit operator Statement(Assignment assignment) => new Statement(Assignment: assignment);
    public static implicit operator Statement(LocalVariableCall localVariableCall) => new Statement(LocalVariableCall: localVariableCall);

    public static Statement Parse(Block block, FunctionContext context)
    {
        if (LocalVariableCall.TryParse(block, context, out var localVariableCall)) return localVariableCall;
        if (Assignment.TryParse(block, context, out var assignment)) return assignment;
        if (FunctionCall.TryParse(block, context, out var function)) return function;
        
        throw new Exception($"Unable to parse statement block {block}");
    }

    public void On
    (
        Action<FunctionCall> onFunctionCall,
        Action<Assignment> onAssignment,
        Action<LocalVariableCall> onLocalVariableCall
    )
    {
        if (FunctionCall != null) onFunctionCall(FunctionCall);
        if (Assignment != null) onAssignment(Assignment);
        if (LocalVariableCall != null) onLocalVariableCall(LocalVariableCall);
    }

    public object Concrete => 
        (object?)FunctionCall ??
        (object?)Assignment ??
        (object?)LocalVariableCall ??
        throw new Exception("Statement has no concrete implementation");

    public override string ToString()
    {
        return Concrete.ToString()!;
    }
}