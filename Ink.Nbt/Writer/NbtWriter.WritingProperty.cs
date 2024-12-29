using Rena.Native.Extensions;

namespace Ink.Nbt;

public sealed partial class NbtWriter<T>
{
    public void WriteUnnamedProperty(NbtTagType type)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(type, string.Empty);
        
        GrowIfNeeded(sizeof(NbtTagType));
        WriteUnnamedPropertyCore(type);
    }

    public void WriteUnnamedProperty()
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.End, string.Empty);
        
        GrowIfNeeded(sizeof(NbtTagType));
        WriteUnnamedPropertyCore();
    }

    public void WriteProperty(NbtTagType type, ReadOnlySpan<char> propertyName)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(type, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName));
        WritePropertyNameCore(type, propertyName);
    }

    public void WriteProperty(ReadOnlySpan<char> propertyName)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.End, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName));
        WritePropertyNameCore(propertyName);
    }

    public void WriteSByte(ReadOnlySpan<char> propertyName, bool value)
        => WriteShort(propertyName, value.AsByte());

    public void WriteSByte(ReadOnlySpan<char> propertyName, sbyte value)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.SByte, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName) + sizeof(sbyte));

        WritePropertyNameCore(NbtTagType.SByte, propertyName);
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteShort(ReadOnlySpan<char> propertyName, short value)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.Short, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName) + sizeof(short));

        WritePropertyNameCore(NbtTagType.Short, propertyName);
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteInt(ReadOnlySpan<char> propertyName, int value)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.Int, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName) + T.IntReserveSize);

        WritePropertyNameCore(NbtTagType.Int, propertyName);
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteLong(ReadOnlySpan<char> propertyName, long value)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.Long, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName) + T.LongReserveSize);

        WritePropertyNameCore(NbtTagType.Long, propertyName);
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteFloat(ReadOnlySpan<char> propertyName, float value)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.Float, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName) + sizeof(float));

        WritePropertyNameCore(NbtTagType.Float, propertyName);
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteDouble(ReadOnlySpan<char> propertyName, double value)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.Double, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName) + sizeof(double));

        WritePropertyNameCore(NbtTagType.Double, propertyName);
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteString(ReadOnlySpan<char> propertyName, ReadOnlySpan<char> value)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.String, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName) + T.GetStringReservedSize(value));

        WritePropertyNameCore(NbtTagType.String, propertyName);
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteSByteArray(ReadOnlySpan<char> propertyName, ReadOnlySpan<sbyte> values)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.SByteArray, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName) + T.IntReserveSize + values.Length);

        WritePropertyNameCore(NbtTagType.SByteArray, propertyName);
        WriteCore(values);

        EndWritingValue();
    }

    public void WriteIntArray(ReadOnlySpan<char> propertyName, ReadOnlySpan<int> values)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.IntArray, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName) + T.IntReserveSize + (values.Length * T.IntReserveSize));

        WritePropertyNameCore(NbtTagType.IntArray, propertyName);
        WriteCore(values);

        EndWritingValue();
    }

    public void WriteLongArray(ReadOnlySpan<char> propertyName, ReadOnlySpan<long> values)
    {
        if (writerOptions.ShouldValidate)
            ValidateProperty(NbtTagType.LongArray, propertyName);

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName) + T.IntReserveSize + (values.Length * T.LongReserveSize));

        WritePropertyNameCore(NbtTagType.LongArray, propertyName);
        WriteCore(values);

        EndWritingValue();
    }

    public void WriteListStart(ReadOnlySpan<char> propertyName, NbtTagType type, int length)
    {
        if (writerOptions.ShouldValidate)
        {
            ValidateProperty(NbtTagType.List, propertyName);
            PushState();
        }

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName) + sizeof(NbtTagType) + T.IntReserveSize);

        WritePropertyNameCore(NbtTagType.List, propertyName);
        WriteListStartCore(type, length);

        EndWritingValue();
    }

    public void WriteCompoundStart(ReadOnlySpan<char> propertyName)
    {
        if (writerOptions.ShouldValidate)
        {
            ValidateProperty(NbtTagType.Compound, propertyName);
            PushState();
        }

        GrowIfNeeded(GetPropertyNameReservedSize(propertyName));

        WritePropertyNameCore(NbtTagType.Compound, propertyName);
        WriteCompoundStartCore();

        EndWritingValue();
    }

    private static int GetPropertyNameReservedSize(ReadOnlySpan<char> propertyName)
        => sizeof(NbtTagType) + T.GetStringReservedSize(propertyName);
}
