using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;

namespace Ink.Nbt;

// TODO: Support ReadOnlySequence someday
public ref partial struct NbtReader<T>
    where T : struct, INbtDatatypeReader<T>
{
    public bool TryGetExactInt8(out sbyte value)
    {
        if(TokenType != NbtReaderTokenType.SByte)
        {
            value = default;
            return false;
        }

        value = tokenInfo.Int8;
        return true;
    }

    public bool TryGetExactInt16(out short value)
    {
        if(TokenType != NbtReaderTokenType.Short)
        {
            value = default;
            return false;
        }

        value = tokenInfo.Int16;
        return true;
    }

    public bool TryGetExactInt32(out int value)
    {
        if(TokenType != NbtReaderTokenType.Int)
        {
            value = default;
            return false;
        }

        value = tokenInfo.Int32;
        return true;
    }

    public bool TryGetExactInt64(out long value)
    {
        if(TokenType != NbtReaderTokenType.Long)
        {
            value = default;
            return false;
        }

        value = tokenInfo.Int64;
        return true;
    }

    public bool TryGetExactSingle(out float value)
    {
        if(TokenType != NbtReaderTokenType.Float)
        {
            value = default;
            return false;
        }

        value = tokenInfo.Single;
        return true;
    }

    public bool TryGetExactDouble(out double value)
    {
        if(TokenType != NbtReaderTokenType.Double)
        {
            value = default;
            return false;
        }

        value = tokenInfo.Double;
        return true;
    }

    public int TryGetString(int byteOffset, Span<char> destination, out int charsWritten)
    {
        if(TokenType != NbtReaderTokenType.String
        && TokenType != NbtReaderTokenType.PropertyName)
        {
            charsWritten = 0;
            return -1;
        }

        int mutf8Length = tokenInfo.Int32 - byteOffset;
        ReadOnlySpan<byte> mutf8Str = buffer.Slice(consumed - mutf8Length, mutf8Length);
        Utf8.ToUtf16(mutf8Str.Slice(byteOffset), destination, out int bytesRead, out charsWritten, true, true);
        return mutf8Str.Length - bytesRead;
    }

    public bool TryGetString(out string value)
    {
        if(TokenType != NbtReaderTokenType.String
        && TokenType != NbtReaderTokenType.PropertyName)
        {
            value = string.Empty;
            return false;
        }

        value = Encoding.UTF8.GetString(buffer.Slice(consumed - tokenInfo.Int32, tokenInfo.Int32));
        return true;
    }

    public bool TryGetListHeader(out NbtListHeader value)
    {
        if(TokenType != NbtReaderTokenType.StartList)
        {
            value = default;
            return false;
        }

        value = tokenInfo.List;
        return true;
    }

    public int TryGetInt8Array(Span<sbyte> destination, int offset = 0)
    {
        if(TokenType != NbtReaderTokenType.SByteArray)
            return -1;

        int minLength = int.Min(tokenInfo.Int32 - offset, destination.Length);
        int remaining = tokenInfo.Int32 - minLength;
        MemoryMarshal.Cast<byte, sbyte>(buffer.Slice(consumed - tokenInfo.Int32, minLength)).TryCopyTo(destination);
        return remaining;
    }

    public bool TryGetInt8Array(out sbyte[] value)
    {
        if(TokenType != NbtReaderTokenType.SByteArray)
        {
            value = Array.Empty<sbyte>();
            return false;
        }

        value = MemoryMarshal.Cast<byte, sbyte>(buffer.Slice(consumed - tokenInfo.Int32, tokenInfo.Int32)).ToArray();
        return true;
    }

    public int TryGetInt32Array(Span<int> destination, int offset = 0)
    {
        if(TokenType != NbtReaderTokenType.IntArray)
            return -1;

        int minLength = int.Min(tokenInfo.Int32 - offset, destination.Length);        
        int remaining = tokenInfo.Int32 - minLength;
        ReadOnlySpan<int> start = MemoryMarshal.Cast<byte, int>(buffer.Slice(consumed - tokenInfo.Int32, minLength));

        if(BitConverter.IsLittleEndian == T.IsLittle)
        {
            start.TryCopyTo(destination);
            return remaining;
        }

        for(int i = 0; i < minLength; ++i)
            destination[i] = BinaryPrimitives.ReverseEndianness(start[i]);
        return remaining;
    }

    public bool TryGetInt32Array(out int[] value)
    {
        if(TokenType != NbtReaderTokenType.IntArray)
        {
            value = Array.Empty<int>();
            return false;
        }

        value = GC.AllocateUninitializedArray<int>(tokenInfo.Int32);
        _ = TryGetInt32Array(value);
        return true;
    }

    public int TryGetInt64Array(Span<long> destination, int offset = 0)
    {
        if(TokenType != NbtReaderTokenType.LongArray)
            return -1;

        int minLength = int.Min(tokenInfo.Int32 - offset, destination.Length);        
        int remaining = tokenInfo.Int32 - minLength;
        ReadOnlySpan<long> start = MemoryMarshal.Cast<byte, long>(buffer.Slice(consumed - tokenInfo.Int32, minLength));

        if(BitConverter.IsLittleEndian == T.IsLittle)
        {
            start.TryCopyTo(destination);
            return remaining;
        }

        for(int i = 0; i < minLength; ++i)
            destination[i] = BinaryPrimitives.ReverseEndianness(start[i]);
        return remaining;
    }

    public bool TryGetInt64Array(out long[] value)
    {
        if(TokenType != NbtReaderTokenType.LongArray)
        {
            value = Array.Empty<long>();
            return false;
        }

        value = GC.AllocateUninitializedArray<long>(tokenInfo.Int32);
        _ = TryGetInt64Array(value, 0);
        return true;
    }
}
