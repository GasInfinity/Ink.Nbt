namespace Ink.Nbt.Serialization.Metadata;

public sealed class NbtTypeInfo<T>(NbtConverter<T> Converter) : NbtTypeInfo
{
    public readonly NbtConverter<T> Converter = Converter;
}
