public class FunctionContext
{
    private readonly Dictionary<string, Variable> localVariables = new Dictionary<string, Variable>();

    public Variable? SearchLocalVariable(string name)
    {
        return localVariables.GetValueOrDefault(name);
    }

    public bool TryGetLocalVariable(string name, out Variable variable)
    {
        if (name.StartsWith("@"))
        {
            variable = new Variable(new string(name.Skip(1).ToArray()));
            return true;
        }

        return localVariables.TryGetValue(name, out variable!);
    }

    public void Add(Variable variable)
    {
        localVariables.Add(variable.Name, variable);
    }
}

public record Variable(string Name);