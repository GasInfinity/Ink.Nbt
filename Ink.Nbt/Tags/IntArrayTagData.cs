namespace Ink.Nbt.Tags;

internal sealed class IntArrayTagData : ITagData
{
    public readonly int[] Value = Array.Empty<int>();

    public NbtTagType Type
        => NbtTagType.IntArray;

    public IntArrayTagData(int[] value)
        => Value = value;
}
