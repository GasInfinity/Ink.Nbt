namespace Ink.Nbt.Serialization;

public abstract class NbtConverter
{
    public abstract void WriteObject<TDatatypeWriter>(NbtWriter<TDatatypeWriter> writer, object value)
        where TDatatypeWriter : struct, INbtDatatypeWriter<TDatatypeWriter>;

    public abstract object? ReadObject<TDatatypeReader>(ref NbtReader<TDatatypeReader> reader)
        where TDatatypeReader : struct, INbtDatatypeReader<TDatatypeReader>;

    public abstract bool IsConverterFor(Type type);

    public bool CanConvert<T>()
        => IsConverterFor(typeof(T));
}
