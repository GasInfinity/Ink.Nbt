namespace Ink.Nbt.Serialization;

public abstract class NbtConverter<T> : NbtConverter
{
    public abstract void Write<TDatatypeWriter>(NbtWriter<TDatatypeWriter> writer, T value)
        where TDatatypeWriter : struct, INbtDatatypeWriter<TDatatypeWriter>;

    public abstract T Read<TDatatypeReader>(ref NbtReader<TDatatypeReader> reader)
        where TDatatypeReader: struct, INbtDatatypeReader<TDatatypeReader>;

    public override object? ReadObject<TDatatypeReader>(ref NbtReader<TDatatypeReader> reader)
        => Read<TDatatypeReader>(ref reader);

    public override void WriteObject<TDatatypeWriter>(NbtWriter<TDatatypeWriter> writer, object value)
        => Write(writer, (T)value);

    public override sealed bool IsConverterFor(Type type)
        => typeof(T).IsAssignableFrom(type);
}
