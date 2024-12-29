namespace Ink.Nbt;

internal readonly struct NbtStackState
{
    const int ListFlagMask = 0b1;

    private readonly int data;
    private readonly byte extraData;

    public int ListLength
        => data;

    public bool InList
        => (extraData & ListFlagMask) == 1;

    public NbtTagType TagType
        => (NbtTagType)(extraData >>> 4);

    public NbtStackState(int listLength, NbtTagType listType)
        => (data, extraData) = (listLength, (byte)(((byte)listType << 4) | ListFlagMask));
}
