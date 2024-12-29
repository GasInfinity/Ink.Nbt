namespace Ink.Nbt.Tags;

public sealed partial class ListTagData : ITagData
{
    private readonly List<NbtTag> values = new();

    public NbtTagType ListType { get; private set; }

    public NbtTagType Type
        => NbtTagType.List;

    public ListTagData(int length, NbtTagType type)
    {
        values = new(length);
        ListType = type;
    }
}
