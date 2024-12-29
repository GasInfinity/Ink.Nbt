using Ink.Nbt.Serialization.Metadata;

namespace Ink.Nbt.Serialization;

public static partial class NbtSerializer
{
    public static T Deserialize<D, T>(ReadOnlySpan<byte> nbt, NbtTypeInfo<T> typeInfo, NbtReaderOptions options = default)
        where D : struct, INbtDatatypeReader<D>
    {
        NbtReader<D> reader = new(nbt, options);

        return Deserialize(ref reader, typeInfo);
    }

    public static T Deserialize<D, T>(ref NbtReader<D> reader, NbtTypeInfo<T> typeInfo)
        where D : struct, INbtDatatypeReader<D>
    {
        reader.Read();

        if(reader.TokenType == NbtReaderTokenType.PropertyName)
            reader.Read();

        return typeInfo.Converter.Read(ref reader);
    }
}
