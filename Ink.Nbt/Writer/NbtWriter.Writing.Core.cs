using Rena.Native.Extensions;
using System.Runtime.CompilerServices;

namespace Ink.Nbt;

public sealed partial class NbtWriter<T>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(sbyte value)
        => RealOutputSpan.GetUnsafe(BytesPending++) = (byte)value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(short value)
    {
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), value, out int writtenBytes);
        BytesPending += writtenBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(int value)
    {
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), value, out int writtenBytes);
        BytesPending += writtenBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(long value)
    {
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), value, out int writtenBytes);
        BytesPending += writtenBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(float value)
    {
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), value, out int writtenBytes);
        BytesPending += writtenBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(double value)
    {
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), value, out int writtenBytes);
        BytesPending += writtenBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(ReadOnlySpan<char> value)
    {
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), value, out int writtenBytes);
        BytesPending += writtenBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(ReadOnlySpan<sbyte> values)
    {
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), values, out int writtenBytes);
        BytesPending += writtenBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(ReadOnlySpan<int> values)
    {
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), values, out int writtenBytes);
        BytesPending += writtenBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCore(ReadOnlySpan<long> values)
    {
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), values, out int writtenBytes);
        BytesPending += writtenBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteListStartCore(NbtTagType type, int length)
    {
        RealOutputSpan.GetUnsafe(BytesPending++) = (byte)type;
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), length, out int writtenBytes);
        BytesPending += writtenBytes;

        inList = true;
        waitingValue = true;
        valueWaited = type;
        remainingListLength = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WriteListEndCore()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCompoundStartCore()
    {
        inList = false;
        waitingValue = false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCompoundEndCore()
        => RealOutputSpan.GetUnsafe(BytesPending++) = (byte)NbtTagType.End;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteUnnamedPropertyCore(NbtTagType type)
    {
        valueWaited = type;
        propertyTagPosition = -1;

        RealOutputSpan.GetUnsafe(BytesPending++) = (byte)type;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteUnnamedPropertyCore()
    {
        valueWaited = NbtTagType.End;

        RealOutputSpan.GetUnsafe(propertyTagPosition = BytesPending++) = (byte)NbtTagType.End;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WritePropertyNameCore(NbtTagType type, ReadOnlySpan<char> propertyName)
    {
        valueWaited = type;
        propertyTagPosition = -1;

        RealOutputSpan.GetUnsafe(BytesPending++) = (byte)type;
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), propertyName, out int writtenBytes);
        BytesPending += writtenBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WritePropertyNameCore(ReadOnlySpan<char> propertyName)
    {
        valueWaited = NbtTagType.End;
        RealOutputSpan.GetUnsafe(propertyTagPosition = BytesPending++) = (byte)NbtTagType.End;
        T.Write(RealOutputSpan.SliceUnsafe(BytesPending), propertyName, out int writtenBytes);
        BytesPending += writtenBytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FixPropertyTagCore(NbtTagType type)
    {
        if(valueWaited == NbtTagType.End)
            RealOutputSpan[propertyTagPosition] = (byte)type;
    }

}
