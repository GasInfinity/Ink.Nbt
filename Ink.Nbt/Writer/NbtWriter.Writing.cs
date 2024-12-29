namespace Ink.Nbt;

public sealed partial class NbtWriter<T>
{
    public void WriteSByte(sbyte value)
    {
        if (writerOptions.ShouldValidate)
            ValidateValue(NbtTagType.SByte);

        FixPropertyTagCore(NbtTagType.SByte);
        GrowIfNeeded(sizeof(sbyte));
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteShort(short value)
    {
        if (writerOptions.ShouldValidate)
            ValidateValue(NbtTagType.Short);

        FixPropertyTagCore(NbtTagType.Short);
        GrowIfNeeded(sizeof(short));
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteInt(int value)
    {
        if (writerOptions.ShouldValidate)
            ValidateValue(NbtTagType.Int);

        FixPropertyTagCore(NbtTagType.Int);
        GrowIfNeeded(T.IntReserveSize);
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteLong(long value)
    {
        if (writerOptions.ShouldValidate)
            ValidateValue(NbtTagType.Long);

        FixPropertyTagCore(NbtTagType.Long);
        GrowIfNeeded(T.LongReserveSize);
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteFloat(float value)
    {
        if (writerOptions.ShouldValidate)
            ValidateValue(NbtTagType.Float);

        FixPropertyTagCore(NbtTagType.Float);
        GrowIfNeeded(sizeof(float));
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteDouble(double value)
    {
        if (writerOptions.ShouldValidate)
            ValidateValue(NbtTagType.Double);

        FixPropertyTagCore(NbtTagType.Double);
        GrowIfNeeded(sizeof(double));
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteString(ReadOnlySpan<char> value)
    {
        if (writerOptions.ShouldValidate)
            ValidateValue(NbtTagType.String);

        FixPropertyTagCore(NbtTagType.String);
        GrowIfNeeded(T.GetStringReservedSize(value));
        WriteCore(value);

        EndWritingValue();
    }

    public void WriteSByteArray(ReadOnlySpan<sbyte> values)
    {
        if (writerOptions.ShouldValidate)
            ValidateValue(NbtTagType.SByteArray);

        FixPropertyTagCore(NbtTagType.SByteArray);
        GrowIfNeeded(T.IntReserveSize + values.Length);
        WriteCore(values);

        EndWritingValue();
    }

    public void WriteIntArray(ReadOnlySpan<int> values)
    {
        if (writerOptions.ShouldValidate)
            ValidateValue(NbtTagType.IntArray);

        FixPropertyTagCore(NbtTagType.IntArray);
        GrowIfNeeded(T.IntReserveSize + (values.Length * T.IntReserveSize));
        WriteCore(values);

        EndWritingValue();
    }

    public void WriteLongArray(ReadOnlySpan<long> values)
    {
        if (writerOptions.ShouldValidate)
            ValidateValue(NbtTagType.LongArray);

        FixPropertyTagCore(NbtTagType.LongArray);
        GrowIfNeeded(T.IntReserveSize + (values.Length * T.LongReserveSize));
        WriteCore(values);

        EndWritingValue();
    }

    public void WriteListStart(NbtTagType type, int length)
    {
        if (writerOptions.ShouldValidate)
        {
            ValidateValue(NbtTagType.List);
            PushState();
        }

        FixPropertyTagCore(NbtTagType.List);
        GrowIfNeeded(sizeof(NbtTagType) + T.IntReserveSize);
        WriteListStartCore(type, length);

        EndWritingValue();
    }

    public void WriteListEnd()
    {
        if (writerOptions.ShouldValidate)
        {
            ValidateListEnd();
            PopState();
        }

        WriteListEndCore();
    }

    public void WriteCompoundStart()
    {
        if (writerOptions.ShouldValidate)
        {
            ValidateValue(NbtTagType.Compound);
            PushState();
        }

        FixPropertyTagCore(NbtTagType.Compound);
        WriteCompoundStartCore();
    }

    public void WriteCompoundEnd()
    {
        if (writerOptions.ShouldValidate)
        {
            ValidateCompoundEnd();
            PopState();
        }

        GrowIfNeeded(sizeof(NbtTagType));
        WriteCompoundEndCore();

        EndWritingValue();
    }
}
