using System.Diagnostics.CodeAnalysis;
using Ink.Nbt.Serialization;
using Ink.Nbt.Serialization.Metadata;
using Rena.Native.Extensions;

namespace Ink.Nbt.Tags;

public readonly record struct NbtTag
{
    public static readonly NbtTypeInfo<NbtTag> TypeInfo = new(NbtTagNbtConverter.Shared);

    private readonly ulong value;
    private readonly ITagData? extraData;

    public readonly NbtTagType Type
        => this.extraData?.Type ?? NbtTagType.End;

    public readonly NbtTag this[string property]
        => AsCompound()?[property] ?? ThrowInvalidTagType(Type, NbtTagType.Compound);

    public readonly NbtTag this[int index]
        => AsList()?[index] ?? ThrowInvalidTagType(Type, NbtTagType.List);

    private NbtTag(ulong value, ITagData tagData)
        => (this.value, this.extraData) = (value, tagData);

    public sbyte? AsInt8()
        => Type == NbtTagType.SByte ? (sbyte)this.value : null;

    public short? AsInt16()
        => Type == NbtTagType.Short ? (short)this.value : null;

    public int? AsInt32()
        => Type == NbtTagType.Int ? (int)this.value : null;

    public long? AsInt64()
        => Type == NbtTagType.Long ? (long)this.value : null;

    public float? AsSingle()
        => Type == NbtTagType.Float ? BitConverter.UInt32BitsToSingle((uint)this.value) : null;

    public double? AsDouble()
        => Type == NbtTagType.Double ? BitConverter.UInt64BitsToDouble(this.value) : null;

    public sbyte[] AsInt8Array()
        => Type == NbtTagType.SByteArray ? (this.extraData as SByteArrayTagData)!.Value : Array.Empty<sbyte>();

    public string? AsString()
        => Type == NbtTagType.String ? (this.extraData as StringTagData)!.Value : null;

    public ListTagData? AsList()
        => Type == NbtTagType.List ? (this.extraData as ListTagData)! : null;

    public CompoundTagData? AsCompound()
        => Type == NbtTagType.Compound ? (this.extraData as CompoundTagData)! : null;

    public int[] AsInt32Array()
        => Type == NbtTagType.IntArray ? (this.extraData as IntArrayTagData)!.Value : Array.Empty<int>();

    public long[] AsInt64Array()
        => Type == NbtTagType.LongArray ? (this.extraData as LongArrayTagData)!.Value : Array.Empty<long>();

    public void WriteTo<T>(NbtWriter<T> writer)
        where T : struct, INbtDatatypeWriter<T>
    {
        switch(Type)
        {
            case NbtTagType.SByte:
                {
                    writer.WriteSByte((sbyte)this.value);
                    break;
                }
            case NbtTagType.Short:
                {
                    writer.WriteShort((short)this.value);
                    break;
                }
            case NbtTagType.Int:
                {
                    writer.WriteInt((int)this.value);
                    break;
                }
            case NbtTagType.Long:
                {
                    writer.WriteLong((long)this.value);
                    break;
                }
            case NbtTagType.Float:
                {
                    writer.WriteFloat(BitConverter.UInt32BitsToSingle((uint)this.value));
                    break;
                }
            case NbtTagType.Double:
                {
                    writer.WriteDouble(BitConverter.UInt64BitsToDouble(this.value));
                    break;
                }
            case NbtTagType.SByteArray:
                {
                    writer.WriteSByteArray((extraData as SByteArrayTagData)!.Value);
                    break;
                }
            case NbtTagType.String:
                {
                    writer.WriteString((extraData as StringTagData)!.Value);
                    break;
                }
            case NbtTagType.List:
                {
                    ListTagData list = (extraData as ListTagData)!;

                    writer.WriteListStart(list.ListType, list.Count);
                    foreach (var tag in list)
                        tag.WriteTo(writer);
                    writer.WriteListEnd();
                    break;
                }
            case NbtTagType.Compound:
                {
                    CompoundTagData compound = (extraData as CompoundTagData)!;

                    writer.WriteCompoundStart();
                    foreach (var entry in compound)
                    {
                        writer.WriteProperty(entry.Value.Type, entry.Key);
                        entry.Value.WriteTo(writer);
                    }
                    writer.WriteCompoundEnd();
                    break;
                }
            case NbtTagType.IntArray:
                {
                    writer.WriteIntArray((extraData as IntArrayTagData)!.Value);
                    break;
                }
            case NbtTagType.LongArray:
                {
                    writer.WriteLongArray((extraData as LongArrayTagData)!.Value);
                    break;
                }
        }
    }

    public static NbtTag Bool(bool value)
        => new((ulong)value.AsByte(), SByteTagData.Shared);

    public static NbtTag SByte(sbyte value)
        => new((ulong)value, SByteTagData.Shared);

    public static NbtTag Short(short value)
        => new((ulong)value, ShortTagData.Shared);
    
    public static NbtTag Int(int value)
        => new((ulong)value, IntTagData.Shared);

    public static NbtTag Long(long value)
        => new((ulong)value, LongTagData.Shared);

    public static NbtTag Float(float value)
        => new(BitConverter.SingleToUInt32Bits(value), FloatTagData.Shared);

    public static NbtTag Double(double value)
        => new(BitConverter.DoubleToUInt64Bits(value), DoubleTagData.Shared);

    public static NbtTag SByteArray(sbyte[] value)
        => new(default, new SByteArrayTagData(value));

    public static NbtTag String(string value)
        => new(default, new StringTagData(value));

    public static NbtTag List(ListTagData value)
        => new(default, value);

    public static NbtTag Compound(CompoundTagData value)
        => new(default, value);

    public static NbtTag IntArray(int[] value)
        => new(default, new IntArrayTagData(value));

    public static NbtTag LongArray(long[] value)
        => new(default, new LongArrayTagData(value));
    
    [DoesNotReturn]
    private static NbtTag ThrowInvalidTagType(NbtTagType current, NbtTagType target)
        => throw new InvalidOperationException($"Trying to access a NbtTag as a '{target}' but the it is a '{current}'");
}
