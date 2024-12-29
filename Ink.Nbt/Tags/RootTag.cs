namespace Ink.Nbt.Tags;

public readonly struct RootTag(NbtTag Tag, string? Name = null)
{
    public readonly string? Name = Name;
    public readonly NbtTag Tag = Tag;

    public void WriteTo<T>(NbtWriter<T> writer)
        where T : struct, INbtDatatypeWriter<T>
    {
        if(Name == null)
            writer.WriteUnnamedProperty(Tag.Type);
        else
            writer.WriteProperty(Tag.Type, Name);
        Tag.WriteTo(writer);
    }
}
