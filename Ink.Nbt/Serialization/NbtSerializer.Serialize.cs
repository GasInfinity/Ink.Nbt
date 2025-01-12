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

    public static void Serialize<D, T>(Stream stream, string? name, T value, NbtSerializerOptions options)
        where D : struct, INbtDatatypeWriter<D>
    {
        using NbtWriter<D> nbtWriter = new(stream, new(options.MaxDepth, options.ShouldValidate));
        Serialize(nbtWriter, name, value, options);
    }

    public static void Serialize<D, T>(IBufferWriter<byte> writer, string? name, T value, NbtSerializerOptions options)
        where D : struct, INbtDatatypeWriter<D>
    {
        using NbtWriter<D> nbtWriter = new(writer, new(options.MaxDepth, options.ShouldValidate));
        Serialize(nbtWriter, name, value, options);
    }

    public static void Serialize<D, T>(NbtWriter<D> writer, string? name, T value, NbtSerializerOptions options)
        where D : struct, INbtDatatypeWriter<D>
    {
        if(name == null)
            writer.WriteUnnamedProperty();
        else
            writer.WriteProperty(name);

        foreach(var converter in options.Converters)
        {
            if(converter.IsConverterFor(typeof(T)))
            {
                converter.WriteObject(writer, value!);
                return;
            }
        }

        throw new NotImplementedException($"Missing nbt converter for type '{typeof(T).FullName}'");
    }

    public static void SerializeObject<D>(Stream stream, string? name, object value, NbtSerializerOptions options)
        where D : struct, INbtDatatypeWriter<D>
    {
        using NbtWriter<D> nbtWriter = new(stream, new(options.MaxDepth, options.ShouldValidate));
        SerializeObject(nbtWriter, name, value, options);
    }

    public static void SerializeObject<D>(IBufferWriter<byte> writer, string? name, object value, NbtSerializerOptions options)
        where D : struct, INbtDatatypeWriter<D>
    {
        using NbtWriter<D> nbtWriter = new(writer, new(options.MaxDepth, options.ShouldValidate));
        SerializeObject(nbtWriter, name, value, options);
    }

    public static void SerializeObject<D>(NbtWriter<D> writer, string? name, object value, NbtSerializerOptions options)
        where D : struct, INbtDatatypeWriter<D>
    {
        if(name == null)
            writer.WriteUnnamedProperty();
        else
            writer.WriteProperty(name);

        Type type = value.GetType();
        foreach(var converter in options.Converters)
        {
            if(converter.IsConverterFor(type))
            {
                converter.WriteObject(writer, value!);
                return;
            }
        }

        throw new NotImplementedException($"Missing nbt converter for type '{type.FullName}'");
    }
}
