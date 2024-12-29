namespace Ink.Nbt.Tags;

internal sealed class IntTagData : ITagData
{
    public static readonly IntTagData Shared = new();

    public NbtTagType Type
        => NbtTagType.Int;

    private IntTagData()
    {
    }
}
