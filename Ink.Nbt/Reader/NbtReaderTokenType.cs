namespace Ink.Nbt;

public enum NbtReaderTokenType
{
    None,
    PropertyName,
    EndCompound,
    SByte,
    Short,
    Int,
    Long,
    Float,
    Double,
    SByteArray,
    String,
    StartList,
    StartCompound,
    IntArray,
    LongArray
}

public static class NbtReaderTokenTypeExtensions
{
    public static NbtReaderTokenType ToReaderTokenType(this NbtTagType tag)
        => (NbtReaderTokenType)((byte)tag + NbtReaderTokenType.EndCompound);
}
