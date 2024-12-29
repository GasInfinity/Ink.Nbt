using System.Runtime.InteropServices;

namespace Ink.Nbt;

[StructLayout(LayoutKind.Explicit)]
internal struct NbtTokenInfo
{
    [FieldOffset(0)] public readonly sbyte Int8;
    [FieldOffset(0)] public readonly short Int16;
    [FieldOffset(0)] public readonly int Int32;
    [FieldOffset(0)] public readonly long Int64;
    [FieldOffset(0)] public readonly float Single;
    [FieldOffset(0)] public readonly double Double;
    
    [FieldOffset(0)] public readonly NbtListHeader List;

    public NbtTokenInfo(sbyte value) => Int8 = value;
    public NbtTokenInfo(short value) => Int16 = value;
    public NbtTokenInfo(int value) => Int32 = value;
    public NbtTokenInfo(long value) => Int64 = value;
    public NbtTokenInfo(float value) => Single = value;
    public NbtTokenInfo(double value) => Double = value;
    public NbtTokenInfo(NbtListHeader value) => List = value;
}
