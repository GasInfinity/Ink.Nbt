namespace Ink.Nbt.Tags;

internal sealed class FloatTagData : ITagData
{
    public static readonly FloatTagData Shared = new();

    public NbtTagType Type
        => NbtTagType.Float;

    private FloatTagData()
    {
    }
}