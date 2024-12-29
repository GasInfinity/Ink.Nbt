using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Ink.Nbt;

// TODO: Support ReadOnlySequence someday
public ref partial struct NbtReader<T> where T : struct, INbtDatatypeReader<T>
{
    private NbtReaderOptions options;
    private bool isFinal;

    private int consumed;
    private ReadOnlySpan<byte> buffer;

    private NbtTagType currentTag;
    private NbtReaderTokenType currentToken;
    private NbtTokenInfo tokenInfo;

    private NbtStackState[] stateStack;
    private int currentDepth = -1;

    public NbtReaderTokenType TokenType
        => this.currentToken;

    public bool isFinalBlock
        => this.isFinal;

    public int CurrentDepth
        => this.currentDepth;

    public int BytesConsumed
        => this.consumed;

    public NbtReader(ReadOnlySpan<byte> data, NbtReaderOptions options = default)
        : this(data, true, new(options))
    { }

    public NbtReader(ReadOnlySpan<byte> data, bool isFinalBlock, NbtReaderState state = default)
    {
        buffer = data;
        options = state.Options;
        stateStack = ArrayPool<NbtStackState>.Shared.Rent(options.MaxDepth == default ? NbtReaderOptions.DefaultMaxDepth : options.MaxDepth);
        isFinal = isFinalBlock;
    }

    public bool Read()
    {
        bool res = ReadSingleSegment();

        if(!res && isFinalBlock)
            ThrowNoMoreData();

        return res;
    }

    private bool ReadSingleSegment()
    {
        switch(currentToken)
        {
            case NbtReaderTokenType.None:
                    return options.NoRootName ? ConsumeUnnamedPropertySingle() : ConsumePropertySingle();
            case NbtReaderTokenType.EndCompound:
                {
                    if(--currentDepth == -1)
                        return false;

                    goto default;
                }
            default:
                    return ReadNextTokenSingle();
        }
    }

    private bool ReadNextTokenSingle()
    {
        if(currentDepth >= 0 && stateStack[currentDepth].InList)
        {
            Debug.Assert(currentToken != NbtReaderTokenType.PropertyName);
            Debug.Assert(stateStack[currentDepth].TagType == NbtTagType.Compound ? currentToken == NbtReaderTokenType.EndCompound || currentToken == NbtReaderTokenType.StartList : true);

            ref NbtStackState state = ref stateStack[currentDepth];
            state = new(state.ListLength - 1, state.TagType);
            if(state.ListLength == 0)
                --currentDepth;

            currentTag = state.TagType;
            currentToken = state.TagType.ToReaderTokenType();
            return ConsumeTagDataSingle();
        }

        if(currentToken == NbtReaderTokenType.PropertyName)
        {
            currentToken = currentTag.ToReaderTokenType();
            return ConsumeTagDataSingle();
        }

        return ConsumePropertySingle();
    }

    private bool ConsumeUnnamedPropertySingle()
    {
        if(consumed >= buffer.Length)
            return false;
        
        currentTag = (NbtTagType)buffer[consumed++];
        currentToken = currentTag.ToReaderTokenType();

        if(currentTag == NbtTagType.End)
            return true;

        if(!ConsumeTagDataSingle())
        {
            --consumed; // Rollback
            return false;
        }

        return true;
    }

    private bool ConsumePropertySingle()
    {
        if(consumed >= buffer.Length)
            return false;

        currentTag = (NbtTagType)buffer[consumed++];
        
        if(currentTag == NbtTagType.End)
        {
            currentToken = NbtReaderTokenType.EndCompound;
            return true;
        }

        if(!ConsumeStringSingle())
        {
            --consumed; // Rollback
            return false;
        }

        currentToken = NbtReaderTokenType.PropertyName;
        return true;
    }

    private bool ConsumeTagDataSingle()
    {
        switch(currentTag)
        {
            case NbtTagType.SByte:
                {
                    if(consumed >= buffer.Length)
                        return false;
                    
                    tokenInfo = new(buffer[consumed]);
                    consumed += sizeof(sbyte);
                    return true;
                }
            case NbtTagType.Short:
                {
                    if(consumed + sizeof(short) > buffer.Length)
                        return false;
                    
                    tokenInfo = new(T.ReadInt16(buffer.Slice(consumed, sizeof(short))));
                    consumed += sizeof(short);
                    return true;
                }
            case NbtTagType.Int:
                {
                    if(!T.TryReadInt32(buffer[consumed..], out int bytesRead, out int value))
                        return false;
                    
                    tokenInfo = new(value);
                    consumed += bytesRead;
                    return true;
                }
            case NbtTagType.Long:
                {
                    if(!T.TryReadInt64(buffer[consumed..], out int bytesRead, out long value))
                        return false;
                    
                    tokenInfo = new(value);
                    consumed += bytesRead;
                    return true;
                }
            case NbtTagType.Float:
                {
                    if(consumed + sizeof(float) > buffer.Length)
                        return false;
                    
                    tokenInfo = new(T.ReadSingle(buffer.Slice(consumed, sizeof(float))));
                    consumed += sizeof(float);
                    return true;
                }
            case NbtTagType.Double:
                {
                    if(consumed + sizeof(double) > buffer.Length)
                        return false;
                    
                    tokenInfo = new(T.ReadDouble(buffer.Slice(consumed, sizeof(double))));
                    consumed += sizeof(double);
                    return true;
                }
            case NbtTagType.SByteArray:
                return ConsumeArraySingle<sbyte>();
            case NbtTagType.String:
                return ConsumeStringSingle();
            case NbtTagType.List:
                return ConsumeListSingle();
            case NbtTagType.Compound:
                {
                    if(currentDepth >= stateStack.Length)
                        throw new NbtException("Depth limit reached");

                    stateStack[++currentDepth] = new ();
                    return true;
                }
            case NbtTagType.IntArray:
                return ConsumeArraySingle<int>();
            case NbtTagType.LongArray:
                return ConsumeArraySingle<long>();
            case NbtTagType.End:
                return true;
            default:
                throw new UnreachableException();
        }
    }

    private bool ConsumeArraySingle<A>()
        where A : unmanaged
    {
        if(!T.TryReadInt32(buffer[consumed..], out int bytesRead, out int length))
            return false;
        
        int possiblyConsumed = consumed + bytesRead + length * Unsafe.SizeOf<A>();
        if(possiblyConsumed > buffer.Length)
            return false;

        tokenInfo = new(length);
        consumed = possiblyConsumed;
        return true;
    }

    private bool ConsumeListSingle()
    {
        if(currentDepth >= stateStack.Length)
            throw new NbtException("Depth limit reached");

        if(consumed >= buffer.Length)
            return false;

        NbtTagType tag = (NbtTagType)buffer[consumed];

        if(!T.TryReadInt32(buffer[(consumed + sizeof(byte))..], out int bytesRead, out int length))
            return false;

        stateStack[++currentDepth] = new (length, tag);
        tokenInfo = new(new NbtListHeader(length, tag));
        consumed = consumed + sizeof(byte) + bytesRead;
        return true;
    }

    private bool ConsumeStringSingle()
    {
        if(!TryPeekStringSingle(out int prefixLength, out int length))
            return false;

        tokenInfo = new(length);
        consumed += prefixLength + length;
        return true;
    }

    private bool TryPeekStringSingle(out int prefixLength, out int valueLength)
    {
        if(!T.TryReadStringLength(buffer[consumed..], out prefixLength, out valueLength))
            return false;

        if(consumed + prefixLength + valueLength > buffer.Length)
            return false;

        return true;
    }

    [DoesNotReturn]
    private static void ThrowNoMoreData()
        => throw new NbtException("We were expecting more data but we're in the final block and no more data is available");
}
