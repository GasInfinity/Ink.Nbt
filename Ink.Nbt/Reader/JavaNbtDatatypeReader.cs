using System.Buffers.Binary;

namespace Ink.Nbt;

public struct JavaNbtDatatypeReader : INbtDatatypeReader<JavaNbtDatatypeReader>
{
    public static bool IsLittle => false;

    public static short ReadInt16(ReadOnlySpan<byte> buffer)
        => BinaryPrimitives.ReadInt16BigEndian(buffer);

    public static bool TryReadInt32(ReadOnlySpan<byte> buffer, out int bytesRead, out int value)
    {
        if(buffer.Length < sizeof(long))
        {
            bytesRead = default;
            value = default;
            return false;
        }
        
        bytesRead = sizeof(int);
        value = BinaryPrimitives.ReadInt32BigEndian(buffer);
        return true;
    }

    public static bool TryReadInt64(ReadOnlySpan<byte> buffer, out int bytesRead, out long value)
    {
        if(buffer.Length < sizeof(long))
        {
            bytesRead = default;
            value = default;
            return false;
        }
        
        bytesRead = sizeof(long);
        value = BinaryPrimitives.ReadInt64BigEndian(buffer);
        return true;
    }

    public static float ReadSingle(ReadOnlySpan<byte> buffer)
        => BinaryPrimitives.ReadSingleBigEndian(buffer);
        
    public static double ReadDouble(ReadOnlySpan<byte> buffer)
        => BinaryPrimitives.ReadDoubleBigEndian(buffer);

    public static bool TryReadStringLength(ReadOnlySpan<byte> buffer, out int bytesRead, out int value)
    {
        if(buffer.Length < sizeof(ushort))
        {
            bytesRead = default;
            value = default;
            return false;
        }
        
        bytesRead = sizeof(ushort);
        value = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        return true;
    }
}
