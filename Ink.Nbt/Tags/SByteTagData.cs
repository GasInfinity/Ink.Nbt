namespace Ink.Nbt.Tags;

internal sealed class SByteTagData : ITagData
{
    public static readonly SByteTagData Shared = new();

    public NbtTagType Type
        => NbtTagType.SByte;
    
    private SByteTagData()
    {
    }
}
