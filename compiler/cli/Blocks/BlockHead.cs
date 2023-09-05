public record BlockHead(string Main, string[] Extras)
{
    public bool StartsWithLower => Main.StartsWithLower();
    public bool StartsWithUpper => Main.StartsWithUpper();

    public bool AnyExtras => Extras.Any();

    public static BlockHead Parse(string line) {
        var parts = line.Split(" ");

        return From(parts);
    }

    public static BlockHead From(string[] parts) {
        return new (
            parts[0],
            parts.Skip(1).ToArray()
        );
    }

    public bool TryPopTwo(out BlockHead resultHead, out (string First, string Second) poped) {
        resultHead = null!;
        poped = ("", "");
        
        if (Extras.Length < 2) return false;
        poped = (Main, Extras[0]);
        resultHead = new BlockHead(Extras[1], Extras.Skip(2).ToArray());

        return true;
    }

    public bool TryPopMain(string matcher, out BlockHead head) {
        head = null!;
        if (Main != matcher || !AnyExtras) return false;
        head = Next();
        return true;
    }

    public bool TryPop(int index, string poped, out BlockHead head) {
        if (index != 1) throw new NotImplementedException("TryPop only implemented for index 1");
        head = null!;

        var actual = Extras.ElementAtOrDefault(index);
        if (actual != poped) return false;
        head = new BlockHead(Main, Extras.Skip(1).ToArray());
        return true;
    }

    public BlockHead Pop(int index, out string poped) {
        if (index != 0) throw new NotImplementedException("Pop only implemented for index 0");
        poped = Main;
        return Next();
    }

    public override string ToString() => Main + (AnyExtras ? " " + String.Join(" ", Extras) : "");
    public string ToString(string identation) => identation + ToString();

    public BlockHead Next() {
        if (!AnyExtras) throw new Exception($"No next block head in '{this}'");
        return new (Extras[0], Extras.Skip(1).ToArray());
    }

    public BlockHead? After(string keyword) => Main == keyword ? Next() : null;
    public BlockHead? AfterMain() => AnyExtras ? From(Extras) : null;

    public (string First, string Second) DeconstructTwo() {
        if (Extras.Length != 1) throw new Exception($"Cannot deconstruct '{this}' into two parts");

        return (Main, Extras[0]);
    }
}