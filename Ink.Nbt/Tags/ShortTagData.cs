namespace Ink.Nbt.Tags;

internal sealed class ShortTagData : ITagData
{
    public static readonly ShortTagData Shared = new();

    public NbtTagType Type
        => NbtTagType.Short;

    private ShortTagData()
    {
    }
}
