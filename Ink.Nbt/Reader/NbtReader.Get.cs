namespace Ink.Nbt;

// TODO: Support ReadOnlySequence someday
public ref partial struct NbtReader<T>
    where T : struct, INbtDatatypeReader<T>
{
    public sbyte GetExactInt8()
        => TryGetExactInt8(out sbyte value) ? value : throw new InvalidOperationException();

    public short GetExactInt16()
        => TryGetExactInt16(out short value) ? value : throw new InvalidOperationException();

    public int GetExactInt32()
        => TryGetExactInt32(out int value) ? value : throw new InvalidOperationException();

    public long GetExactInt64()
        => TryGetExactInt64(out long value) ? value : throw new InvalidOperationException();

    public float GetExactSingle()
        => TryGetExactSingle(out float value) ? value : throw new InvalidOperationException();

    public double GetExactDouble()
        => TryGetExactDouble(out double value) ? value : throw new InvalidOperationException();

    public string GetString()
        => TryGetString(out string value) ? value : throw new InvalidOperationException();

    public NbtListHeader GetListHeader()
        => TryGetListHeader(out NbtListHeader value) ? value : throw new InvalidOperationException();

    public sbyte[] GetInt8Array()
        => TryGetInt8Array(out sbyte[] value) ? value : throw new InvalidOperationException();

    public int[] GetInt32Array()
        => TryGetInt32Array(out int[] value) ? value : throw new InvalidOperationException();

    public long[] GetInt64Array()
        => TryGetInt64Array(out long[] value) ? value : throw new InvalidOperationException();

}
