namespace Ink.Nbt.Tags;

internal sealed class StringTagData : ITagData
{
    public string Value { get; init; } = string.Empty;

    public NbtTagType Type
        => NbtTagType.String;

    public StringTagData()
    {
    }

    public StringTagData(string value)
        => Value = value;
}
