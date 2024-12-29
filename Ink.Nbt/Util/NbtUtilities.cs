using System.Text;
using Ink.Nbt.Tags;

namespace Ink.Nbt.Util;

public static class NbtUtilities
{
    public static string PrettyString(this RootTag tag, int maxDepth = 8)
    {
        StringBuilder builder = new();

        if(tag.Name == null)
            builder.Append($"TAG_{tag.Tag.Type}(None): ");
        else
            builder.Append($"TAG_{tag.Tag.Type}('{tag.Name}'): ");

        AppendPrettyString(builder, tag.Tag, maxDepth);
        return builder.ToString();
    }

    public static string PrettyString(this NbtTag tag, int maxDepth = 8)
    {
        StringBuilder builder = new();
        builder.Append($"TAG_{tag.Type}(None): ");
        AppendPrettyString(builder, tag, maxDepth);
        return builder.ToString();
    }

    public static void AppendPrettyString(StringBuilder builder, NbtTag tag, int maxDepth = 8, int depth = 0)
    {
        

        switch(tag.Type)
        {
            case NbtTagType.SByte:
                {
                    builder.Append(tag.AsInt8()!.Value);
                    break;
                }
            case NbtTagType.Short:
                {
                    builder.Append(tag.AsInt16()!.Value);
                    break;
                }
            case NbtTagType.Int:
                {
                    builder.Append(tag.AsInt32()!.Value);
                    break;
                }
            case NbtTagType.Long:
                {
                    builder.Append(tag.AsInt64()!.Value);
                    break;
                }
            case NbtTagType.Float:
                {
                    builder.Append(tag.AsSingle()!.Value);
                    break;
                }
            case NbtTagType.Double:
                {
                    builder.Append(tag.AsDouble()!.Value);
                    break;
                }
            case NbtTagType.SByteArray:
                {
                    sbyte[] value = tag.AsInt8Array();
                    builder.Append(value.Length).Append(" bytes");
                    break;
                }
            case NbtTagType.String:
                {
                    builder.Append(tag.AsString()!);
                    break;
                }
            case NbtTagType.List:
                {
                    ListTagData list = tag.AsList()!;
                    builder.Append(list.Count).Append(" entries");

                    if(depth >= maxDepth)
                    {
                        builder.Append(" .. Depth Limit Reached ..");
                        return;
                    }

                    builder.AppendLine();
                    builder.Append(' ', depth).Append('{').AppendLine();
                    ++depth;

                    for(int i = 0; i < list.Count; ++i)
                    {
                        builder.Append(' ', depth).Append($"TAG_{list.ListType}(None): ");
                        AppendPrettyString(builder, list[i], maxDepth, depth);
                        builder.AppendLine();
                    }

                    --depth;
                    builder.Append(' ', depth).Append('}');
                    break;
                }
            case NbtTagType.Compound:
                {
                    CompoundTagData compound = tag.AsCompound()!;
                    builder.Append(compound.Count).Append(" entries");

                    if(depth >= maxDepth)
                    {
                        builder.Append(" .. Depth Limit Reached ..");
                        return;
                    }

                    builder.AppendLine();
                    builder.Append(' ', depth).Append('{').AppendLine();
                    ++depth;
                    
                    foreach(var entry in compound)
                    {
                        builder.Append(' ', depth).Append($"TAG_{entry.Value.Type}('{entry.Key}'): ");
                        AppendPrettyString(builder, entry.Value, maxDepth, depth);
                        builder.AppendLine();
                    }

                    --depth;
                    builder.Append(' ', depth).Append('}');
                    break;
                }
            case NbtTagType.IntArray:
                {
                    int[] value = tag.AsInt32Array()!;
                    builder.Append(value.Length).Append(" ints");
                    break;
                }
            case NbtTagType.LongArray:
                {
                    long[] value = tag.AsInt64Array()!;
                    builder.Append(value.Length).Append(" longs");
                    break;
                }
        }
    }
}
