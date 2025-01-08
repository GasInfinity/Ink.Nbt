using Rena.Native.Extensions;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;

namespace Ink.Nbt;

public struct JavaNbtDatatypeWriter : INbtDatatypeWriter<JavaNbtDatatypeWriter>
{
    public static int IntReserveSize => sizeof(int);
    public static int LongReserveSize => sizeof(long);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetStringReservedSize(ReadOnlySpan<char> str)
        => sizeof(ushort) + Encoding.UTF8.GetMaxByteCount(str.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(Span<byte> output, short value, out int writtenBytes)
    {
        BinaryPrimitives.WriteInt16BigEndian(output.OptimizeUnsafe(sizeof(short)), value);
        writtenBytes = sizeof(short);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(Span<byte> output, int value, out int writtenBytes)
    {
        BinaryPrimitives.WriteInt32BigEndian(output.OptimizeUnsafe(sizeof(int)), value);
        writtenBytes = IntReserveSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(Span<byte> output, long value, out int writtenBytes)
    {
        BinaryPrimitives.WriteInt64BigEndian(output.OptimizeUnsafe(sizeof(long)), value);
        writtenBytes = LongReserveSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(Span<byte> output, float value, out int bytesWritten)
    {
        BinaryPrimitives.WriteSingleBigEndian(output.OptimizeUnsafe(sizeof(float)), value);
        bytesWritten = sizeof(float);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(Span<byte> output, double value, out int bytesWritten)
    {
        BinaryPrimitives.WriteDoubleBigEndian(output.OptimizeUnsafe(sizeof(double)), value);
        bytesWritten = sizeof(double);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(Span<byte> output, ReadOnlySpan<char> value, out int bytesWritten)
    {
        Debug.Assert(value.Length < ushort.MaxValue);

        if(Utf8.FromUtf16(value, output.SliceUnsafe(sizeof(ushort)), out _, out int utfBytesWritten) != OperationStatus.Done)
            Debug.Fail(string.Empty);


        BinaryPrimitives.WriteUInt16BigEndian(output.OptimizeUnsafe(sizeof(ushort)), (ushort)utfBytesWritten);
        bytesWritten = sizeof(ushort) + utfBytesWritten;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(Span<byte> output, ReadOnlySpan<sbyte> values, out int bytesWritten)
    {
        Write(output, values.Length, out bytesWritten);
        values.Cast().CopyTo(output.SliceUnsafe(bytesWritten));
        bytesWritten += values.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(Span<byte> output, ReadOnlySpan<int> values, out int bytesWritten)
    {
        Write(output, values.Length, out bytesWritten);

        if (BitConverter.IsLittleEndian) // Slow path, almost every platform is little endian
        {
            foreach (var value in values)
                Write(output = output.SliceUnsafe(IntReserveSize), value, out int _);
        }
        else
        {
            MemoryMarshal.Cast<int, byte>(values).CopyTo(output.SliceUnsafe(IntReserveSize));
        }

        bytesWritten += values.Length * IntReserveSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(Span<byte> output, ReadOnlySpan<long> values, out int bytesWritten)
    {
        Write(output, values.Length, out bytesWritten);
        output = output.SliceUnsafe(IntReserveSize);

        if (BitConverter.IsLittleEndian) // Slow path, almost every platform is little endian
        {
            foreach (var value in values)
            {
                Write(output, value, out int _);
                output = output.SliceUnsafe(LongReserveSize);
            }
        }
        else
        {
            MemoryMarshal.Cast<long, byte>(values).CopyTo(output);
        }

        bytesWritten += values.Length * LongReserveSize;
    }
}
