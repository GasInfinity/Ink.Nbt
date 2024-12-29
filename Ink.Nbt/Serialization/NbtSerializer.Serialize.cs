using System.Buffers;
using Ink.Nbt.Serialization.Metadata;

namespace Ink.Nbt.Serialization;

public static partial class NbtSerializer
{
    public static void Serialize<D, T>(Stream stream, string? name, T value, NbtTypeInfo<T> typeInfo, NbtWriterOptions options = default)
        where D : struct, INbtDatatypeWriter<D>
    {
        using NbtWriter<D> nbtWriter = new(stream, options);
        Serialize(nbtWriter, name, value, typeInfo);
    }

    public static void Serialize<D, T>(IBufferWriter<byte> writer, string? name, T value, NbtTypeInfo<T> typeInfo, NbtWriterOptions options = default)
        where D : struct, INbtDatatypeWriter<D>
    {
        using NbtWriter<D> nbtWriter = new(writer, options);
        Serialize(nbtWriter, name, value, typeInfo);
    }

    public static void Serialize<D, T>(NbtWriter<D> writer, string? name, T value, NbtTypeInfo<T> typeInfo)
        where D : struct, INbtDatatypeWriter<D>
    {
        if(name == null)
            writer.WriteUnnamedProperty();
        else
            writer.WriteProperty(name);

        typeInfo.Converter.Write(writer, value);
    }
}
