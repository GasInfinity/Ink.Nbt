namespace Ink.Nbt.Tags;

internal sealed class LongArrayTagData : ITagData
{
    public readonly long[] Value = Array.Empty<long>();

    public NbtTagType Type
        => NbtTagType.LongArray;

    public LongArrayTagData(long[] value)
        => Value = value;
}
