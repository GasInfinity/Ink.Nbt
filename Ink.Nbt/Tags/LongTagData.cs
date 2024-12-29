namespace Ink.Nbt.Tags;

internal sealed class LongTagData : ITagData
{
    public static readonly LongTagData Shared = new();

    public NbtTagType Type
        => NbtTagType.Long;

    private LongTagData()
    {
    }
}
