namespace Ink.Nbt;

public interface INbtDatatypeWriter<T>
    where T : struct, INbtDatatypeWriter<T>
{
    static abstract int IntReserveSize { get; }
    static abstract int LongReserveSize { get; }

    static abstract int GetStringReservedSize(ReadOnlySpan<char> str);

    static abstract void Write(Span<byte> output, short value, out int bytesWritten);
    static abstract void Write(Span<byte> output, int value, out int bytesWritten);
    static abstract void Write(Span<byte> output, long value, out int bytesWritten);
    static abstract void Write(Span<byte> output, float value, out int bytesWritten);
    static abstract void Write(Span<byte> output, double value, out int bytesWritten);

    static abstract void Write(Span<byte> output, ReadOnlySpan<char> value, out int bytesWritten);

    static abstract void Write(Span<byte> output, ReadOnlySpan<sbyte> values, out int bytesWritten);
    static abstract void Write(Span<byte> output, ReadOnlySpan<int> values, out int bytesWritten);
    static abstract void Write(Span<byte> output, ReadOnlySpan<long> values, out int bytesWritten);
}
