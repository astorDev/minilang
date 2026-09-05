# Style Guide

## No method can exceed 10 lines. No class can exceed 50.

If any method you touch is more then 10 lines or if a class you touch exceeds 50 lines, refactor it. 
Use the following fixes priority:

1. Use latest C#: primary ctors, lambdas, etc.
2. Extract Extensions: If a functionality clearly attributes to an existing class - create an extension method.
3. Add Utility Method in dedicated Utility Class.

Passionately remove duplication: You can only do any other refactoring once the method doesn't have any duplications.

Note: For units that already exceeded the limit before you touched them you don't have to make them comply to the limit. BUT you must leave them with less line, then there was before you touched it.

## Never use `else`

All cases of `else` should be refactored to use either early `return`s or `?` operator.

## Don't nest methods; always use variables

Assign intermediate values to variables instead of nesting method calls.

**Bad:**
```csharp
action(CaseConverter.Parse(parseResult.GetValue(inputArg)!));
```

**Good:**
```csharp
var input = parseResult.GetValue(inputArg)!;
var words = CaseConverter.Parse(input);
action(words);
```

---

**Bad:**
```csharp
return Run(description, words => Console.WriteLine(convert(words)), args);
```

**Good:**
```csharp
return Run(description, words =>
{
    var result = convert(words);
    Console.WriteLine(result);
}, args);
```

## Don't create synthetic private methods; use properly formatted lambdas

Use a well-formatted multi-line lambda instead of extracting a private method solely to pass it as a callback.

**Bad:**
```csharp
return CaseCli.Run("Convert input string to all cases", Print, args);

void Print(string[] words)
{
    var results = CaseConverter.ToAll(words);
    foreach (var result in results)
        Console.WriteLine(result);
}
```

**Good:**
```csharp
return CaseCli.Run("Convert input string to all cases", words =>
{
    var results = CaseConverter.ToAll(words);
    foreach (var result in results)
        Console.WriteLine(result);
}, args);
```
