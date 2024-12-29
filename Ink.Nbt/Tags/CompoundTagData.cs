namespace Ink.Nbt.Tags;

public sealed partial class CompoundTagData : ITagData
{
    public NbtTagType Type
        => NbtTagType.Compound;
}
