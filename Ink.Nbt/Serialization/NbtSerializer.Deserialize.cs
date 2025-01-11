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

    public static T Deserialize<D, T>(ReadOnlySpan<byte> nbt, NbtSerializerOptions options)
        where D : struct, INbtDatatypeReader<D>
    {
        NbtReader<D> reader = new(nbt, new(options.NoRootName, options.MaxDepth));

        return Deserialize<D, T>(ref reader, options);
    }

    public static T Deserialize<D, T>(ref NbtReader<D> reader, NbtSerializerOptions options)
        where D : struct, INbtDatatypeReader<D>
    {
        reader.Read();

        if(reader.TokenType == NbtReaderTokenType.PropertyName)
            reader.Read();

        foreach(var converter in options.Converters)
        {
            if(converter.IsConverterFor(typeof(T)))
            {
                return (T)converter.ReadObject(ref reader)!;
            }
        }

        throw new NotImplementedException($"Missing nbt converter for type '{typeof(T).FullName}'");
    }
}
