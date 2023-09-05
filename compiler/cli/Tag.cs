public record Tag(string Name)
{
    public static Tag Parse(string raw)
    {
        return new Tag(raw);
    }

    public override string ToString()
    {
        return Name;
    }
}