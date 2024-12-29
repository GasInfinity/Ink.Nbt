namespace Ink.Nbt;

public readonly struct NbtListHeader(int Length, NbtTagType Type)
{
    public readonly int Length = Length;
    public readonly NbtTagType Type = Type;
}
