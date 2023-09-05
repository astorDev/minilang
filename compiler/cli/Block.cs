using System.Text;

public record MaybeHeadlessBlock(BlockHead? Head, Block[] Inner)
{
    public IEnumerable<string> HeadParts() {
        if (Head != null) {
            yield return Head.Main;

            foreach (var extra in Head.Extras){
                yield return extra;
            }
        }
    }
}

public record PopMatcher(int index, string? pattern);

public record Block(BlockHead Head, Block[] Inner)
{
    public bool StartsWithLower => Head?.StartsWithLower ?? false;
    public bool StartsWithUpper => Head?.StartsWithUpper ?? false;
    public string Main = Head.Main;
    public bool AnyInner => Inner.Any();
    public bool TryGetOnlyMain(out string main, Func<string, bool>? predicate = null) {
        main = null!;
        if (AnyInner && Head.AnyExtras && (predicate?.Invoke(Main) ?? true)) return false;
        main = Head.Main;
        return true;
    }

    public bool TryPopTwo(out Block resultBlock, out (string First, string Second) poped) {
        var success = Head.TryPopTwo(out var popedHead, out poped);
        resultBlock = success ? new Block(popedHead, Inner) : null!;
        return success;
    }

    public bool TryPopMain(string matcher, out Block poped)
    {
        var success = Head.TryPopMain(matcher, out var popedHead);
        poped = success ? new Block(popedHead, Inner) : null!;
        return success;
    }

    public static Block OfSingleElement(string element) => new(new BlockHead(element, Array.Empty<string>()), Array.Empty<Block>());
    
    public static Block[] Read(string[] lines) {

        var finalizedBlocks = new List<Block>();
        var currentBlock = new List<string>();
        var firstLine = lines.Where(l => !String.IsNullOrWhiteSpace(l)).PopFirst(out var remainingLines);
        var blockWriter = new BlockWriter(BlockHead.Parse(firstLine));
        foreach (var line in remainingLines) blockWriter.Add(line);

        return blockWriter.Finish();
    }

    public void ThrowIfDeep(string reason) {
        if (AnyInner) throw new Exception($"Block {Head} cannot contain any inner element - {reason}");
    }

    public Block? After(string keyword) => Head?.After(keyword) is BlockHead head ? new Block(head, Inner) : null;
    public MaybeHeadlessBlock AfterMain() => new MaybeHeadlessBlock(Head.AfterMain(), Inner);

    public string ToString(string identation)
    {
        var sb = new StringBuilder();
        sb.AppendLine(Head?.ToString(identation));
        foreach (var block in Inner)
        {
            sb.AppendLine(block.ToString(identation + Identation.One));
        }
        return sb.ToString();
    }

    public override string ToString()
    {
        return ToString("");
    }
}

public class BlockWriter
{
    List<Block> blocks = new();
    BlockHead head;
    BlockWriter? sub;

    public BlockWriter(BlockHead head)
    {
        this.head = head;
    }

    public void Add(string line)
    {
        if (!line.StartsWithSpace())
        {
            Rollup();
            head = BlockHead.Parse(line);
            sub = null;
        }
        else
        {
            if (sub == null) sub = new BlockWriter(BlockHead.Parse(line.Unindented()));
            else sub.Add(line.Unindented());
        }
    }

    private void Rollup() {
        blocks.Add(new Block(head, sub?.Finish() ?? Array.Empty<Block>()));
    }

    public Block[] Finish() {
        Rollup();
        return blocks.ToArray();
    }
}

public static class IEnumerableExtensions
{
    public static T PopFirst<T>(this IEnumerable<T> source, out IEnumerable<T> rest)
    {
        rest = source.Skip(1);
        return source.First();
    }
}