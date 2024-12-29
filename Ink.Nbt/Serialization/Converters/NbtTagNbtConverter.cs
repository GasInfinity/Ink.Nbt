using System.Diagnostics;
using Ink.Nbt.Tags;

namespace Ink.Nbt.Serialization;

public sealed class NbtTagNbtConverter : NbtConverter<NbtTag>
{
    public static NbtTagNbtConverter Shared = new();

    public override NbtTag Read<TDatatypeReader>(ref NbtReader<TDatatypeReader> reader)
    {
        switch(reader.TokenType)
        {
            case NbtReaderTokenType.SByte: return NbtTag.SByte(reader.GetExactInt8());
            case NbtReaderTokenType.Short: return NbtTag.Short(reader.GetExactInt16());
            case NbtReaderTokenType.Int: return NbtTag.Int(reader.GetExactInt32());
            case NbtReaderTokenType.Long: return NbtTag.Long(reader.GetExactInt64());
            case NbtReaderTokenType.Float: return NbtTag.Float(reader.GetExactSingle());
            case NbtReaderTokenType.Double: return NbtTag.Double(reader.GetExactDouble());
            case NbtReaderTokenType.SByteArray: return NbtTag.SByteArray(reader.GetInt8Array());
            case NbtReaderTokenType.String: return NbtTag.String(reader.GetString());
            case NbtReaderTokenType.StartList:
                {
                    NbtListHeader header = reader.GetListHeader();
                    ListTagData list = new ListTagData(header.Length, header.Type);

                    int length = header.Length;
                    while(--length >= 0)
                    {
                        reader.Read();
                        list.Add(Read(ref reader));
                    }
                    return NbtTag.List(list);
                }
            case NbtReaderTokenType.StartCompound:
                {
                    CompoundTagData compound = new();

                    while(reader.Read())
                    {
                        switch(reader.TokenType)
                        {
                            case NbtReaderTokenType.PropertyName:
                                {
                                    string propertyName = reader.GetString();
                                    bool res = reader.Read();
                                    Debug.Assert(res, "Property value must be present");

                                    compound.TryAdd(propertyName, Read(ref reader));
                                    break;
                                }
                            case NbtReaderTokenType.EndCompound:
                                goto finished;
                        }
                    }
                finished:
                    return NbtTag.Compound(compound);
                }
            case NbtReaderTokenType.IntArray: return NbtTag.IntArray(reader.GetInt32Array());
            case NbtReaderTokenType.LongArray: return NbtTag.LongArray(reader.GetInt64Array());
            default: throw new UnreachableException();
        }
    }

    public override void Write<TDatatypeWriter>(NbtWriter<TDatatypeWriter> writer, NbtTag value)
        => value.WriteTo(writer);
}
