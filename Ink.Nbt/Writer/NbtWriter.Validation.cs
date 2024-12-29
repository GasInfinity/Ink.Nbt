using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Ink.Nbt;

public sealed partial class NbtWriter<T>
{
    const int InitialStackDepth = 64;

    private void InitializeValidation()
    {
        if (writerOptions.MaxDepth == default)
            writerOptions = new (NbtWriterOptions.DefaultMaxDepth, writerOptions.ShouldValidate);

        if (writerOptions.ShouldValidate)
            stateStack = ArrayPool<NbtStackState>.Shared.Rent(InitialStackDepth);
    }

    private void ValidateValue(NbtTagType type)
    {
        if (waitingValue)
        {
            if (inList)
            {
                if (remainingListLength-- <= 0)
                    throw new InvalidOperationException("Overflow of a list, got more items than expected");

                if (valueWaited != type && valueWaited != NbtTagType.End)
                    throw new InvalidOperationException($"Trying to write in a list a value of type '{type}' when we were expecting a value of type '{valueWaited}'");
            }
            else
            {
                if (valueWaited != NbtTagType.End)
                {
                    if (valueWaited != type)
                        throw new InvalidOperationException($"Trying to write a property of type '{type}' when we we were expecting a value of type '{valueWaited}' for it");
                }
                else
                {
                    Debug.Assert(propertyTagPosition != -1, "Position was not set for next property write?");
                }
            }

            waitingValue = inList;
            return;
        }

        throw new InvalidOperationException($"Trying to write a '{type}' value when we weren't expecting one");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EndWritingValue()
        => waitingValue = inList;

    private void ValidateProperty(NbtTagType type, ReadOnlySpan<char> propertyName)
    {
        if (!waitingValue)
        {
            valueWaited = type;
            waitingValue = true;
            return;
        }

        throw new InvalidOperationException($"Trying to write a '{type}' property (prefix?) with name '{propertyName}' when we were expecting a value of type '{valueWaited}'");
    }

    private void ValidateListEnd()
    {
        if (inList)
        {
            if (remainingListLength != 0)
                throw new InvalidOperationException($"Trying to end a list but we need '{remainingListLength}' values more to end it");
            return;
        }

        throw new InvalidOperationException("Trying to end a list but we're in a compound");
    }

    private void ValidateCompoundEnd()
    {
        if (!inList)
        {
            if (waitingValue)
                throw new InvalidOperationException($"Trying to end a compound but we were waiting a value of type '{valueWaited}'");

            return;
        }

        throw new InvalidOperationException("Trying to end a compound but we're in a list");
    }

    private void PushState()
    {
        if (currentDepth > writerOptions.MaxDepth)
            throw new InvalidOperationException("Depth overflow. Start(Compound)(List)/End(Compound)(List) Mismatch?");

        Debug.Assert(stateStack != null);

        if (currentDepth >= stateStack.Length)
            GrowStateStack(currentDepth + 1);

        stateStack[currentDepth++] = inList ? new(remainingListLength, valueWaited) : default;
    }

    private void PopState()
    {
        --currentDepth;

        Debug.Assert(stateStack != null);
        if(currentDepth < 0)
            throw new InvalidOperationException("Depth underflow, popping more than pushing. Start(Compound)(List)/End(Compound)(List) Mismatch?");

        ref NbtStackState state = ref stateStack[currentDepth];

        inList = state.InList;
        waitingValue = inList;

        valueWaited = state.TagType;
        remainingListLength = state.ListLength;
    }

    private void GrowStateStack(int newSize)
    {
        Debug.Assert(stateStack != null);
        ArrayPool<NbtStackState>.Shared.Return(stateStack);

        stateStack = ArrayPool<NbtStackState>.Shared.Rent(newSize);
    }
}
