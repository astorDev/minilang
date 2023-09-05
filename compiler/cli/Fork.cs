public delegate bool TryParseDelegate<TSource, TResult>(TSource source, out TResult result);

public static class ForkExtensions
{
    public static (TParsed[] parsed, TSource[] unparsed) ForkParsed<TSource, TParsed>
    (
        this IEnumerable<TSource> sources,
        TryParseDelegate<TSource, TParsed> tryParse
    )
    {
        List<TParsed> parsed = new List<TParsed>();
        List<TSource> unparsed = new List<TSource>();

        foreach (var s in sources) {
            if (tryParse(s, out var p)) {
                parsed.Add(p);
            } else {
                unparsed.Add(s);
            }
        }

        return (parsed.ToArray(), unparsed.ToArray());
    }
}