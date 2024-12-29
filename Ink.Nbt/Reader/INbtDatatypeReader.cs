namespace Ink.Nbt;

public interface INbtDatatypeReader<T>
    where T : struct, INbtDatatypeReader<T>
{
    static abstract bool IsLittle { get; }

    static abstract short ReadInt16(ReadOnlySpan<byte> buffer);
    static abstract bool TryReadInt32(ReadOnlySpan<byte> buffer, out int bytesRead, out int value);
    static abstract bool TryReadInt64(ReadOnlySpan<byte> buffer, out int bytesRead, out long value);

    static abstract float ReadSingle(ReadOnlySpan<byte> buffer);
    static abstract double ReadDouble(ReadOnlySpan<byte> buffer);

    static abstract bool TryReadStringLength(ReadOnlySpan<byte> buffer, out int bytesRead, out int value);
}
