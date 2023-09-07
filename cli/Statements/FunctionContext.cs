public record MaybeClassSignatureKey(ClassSignature? Signature)
{
    public static implicit operator MaybeClassSignatureKey(ClassSignature? signature) => new(signature);
}

public class FunctionContext
{
    private readonly Dictionary<string, Variable> localVariables = new Dictionary<string, Variable>();
    private readonly Dictionary<string, ArgumentSignature> namedParameters = new();
    private readonly Dictionary<MaybeClassSignatureKey, ArgumentSignature> anonymousParameters = new();

    public ArgumentSignature[] Arguments => namedParameters.Values.Concat(anonymousParameters.Values).ToArray();
    
    public Variable? SearchLocalVariable(string name)
    {
        return localVariables.GetValueOrDefault(name);
    }

    public (bool isNew, Variable variable) GetOrAddLocalVariable(string name)
    {
        if (localVariables.TryGetValue(name, out var result))
        {
            return (false, result);
        }

        result = new Variable(name);
        localVariables.Add(name, result);
        return (true, result);
    }

    private ArgumentSignature GetOrAddAnonymous(ArgumentSignature signature)
    {   
        if (!anonymousParameters.TryGetValue(signature.Type, out var result))
        {
            result = signature with { Name = $"p{anonymousParameters.Count + 1}" };
            anonymousParameters.Add(signature.Type, result);
        }

        return result;
    }

    private ArgumentSignature GetOrAddNamed(ArgumentSignature signature)
    {
        if (!namedParameters.TryGetValue(signature.Name, out var result))
        {
            result = signature;
            namedParameters.Add(signature.Name, result);
        }

        return result;
    }

    public ArgumentSignature AddOrGet(ArgumentSignature signature)
    {
        if (signature.Name == "")
        {
            return GetOrAddAnonymous(signature);
        }

        return GetOrAddNamed(signature);
    }

    public string NextAnonymousParameterName(ClassSignature? type)
    {
        return "p1";
    }

    public bool TryGetLocalVariable(string name, out Variable variable)
    {
        return localVariables.TryGetValue(name, out variable!);
    }

    public bool HasLocalVariable(string name)
    {
        return localVariables.ContainsKey(name);
    }

    public Variable Add(Variable variable)
    {
        localVariables.Add(variable.Name, variable);
        return variable;
    }

    public override string ToString()
    {
        var localVariablesString = localVariables.Any() ? "LocalVariables: " + string.Join(", ", localVariables.Select(x => x.Key)) : "";
        var parametersString = Arguments.Any() ? "Parameters: " + string.Join(", ", Arguments.Select(x => x.ToString())) : "";
        return $"({localVariablesString} {parametersString})";
    }
}

public record Variable(string Name);