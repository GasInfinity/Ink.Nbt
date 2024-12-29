namespace Ink.Nbt.Tags;

internal sealed class SByteArrayTagData : ITagData
{
    public readonly sbyte[] Value = Array.Empty<sbyte>();

    public NbtTagType Type
        => NbtTagType.SByteArray;

    public SByteArrayTagData(sbyte[] value)
        => Value = value;
}
