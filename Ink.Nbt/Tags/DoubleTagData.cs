namespace Ink.Nbt.Tags;

internal sealed class DoubleTagData : ITagData
{
    public static readonly DoubleTagData Shared = new();

    public NbtTagType Type
        => NbtTagType.Double;

    private DoubleTagData()
    {
    }
}