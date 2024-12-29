using System.Buffers;
using System.Diagnostics;

namespace Ink.Nbt;

public sealed partial class NbtWriter<T> : IDisposable
    where T : struct, INbtDatatypeWriter<T>
{
    private IBufferWriter<byte>? outputWriter;
    private Stream? outputStream;
    private ArrayBufferWriter<byte>? arrayWriter;

    private Memory<byte> realOutput;

    private NbtWriterOptions writerOptions;

    private bool waitingValue;
    private bool inList;
    private int propertyTagPosition;
    private NbtTagType valueWaited;
    private int remainingListLength;
    private int currentDepth;
    private NbtStackState[]? stateStack;

    private Span<byte> RealOutputSpan
        => realOutput.Span;

    public int BytesPending { get; private set; }

    public long BytesCommited { get; private set; }

    public NbtWriterOptions Options
        => writerOptions;

    public NbtWriter(IBufferWriter<byte> output, NbtWriterOptions options = default)
    {
        (outputWriter, writerOptions) = (output, options);
        InitializeValidation();
    }

    public NbtWriter(Stream stream, NbtWriterOptions options = default)
    {
        (outputStream, arrayWriter, writerOptions) = (stream, new(), options);
        InitializeValidation();
    }

    public void Flush()
    {
        realOutput = default;

        if (BytesPending > 0)
        {
            if (outputStream != null)
            {
                Debug.Assert(arrayWriter != null);
                arrayWriter.Advance(BytesPending);

                outputStream.Write(arrayWriter.WrittenSpan);
                arrayWriter.Clear();
                outputStream.Flush();

                BytesCommited += arrayWriter.WrittenCount;
                BytesPending = 0;
            }
            else
            {
                Debug.Assert(outputWriter != null);
                outputWriter.Advance(BytesPending);
                BytesCommited += BytesPending;
                BytesPending = 0;
            }
        }
    }

    public void Reset()
    {
        ThrowIfDisposed();

        arrayWriter?.Clear();
        ResetDefaults();
    }

    public void Reset(Stream newStream)
    {
        ThrowIfDisposed();

        Dispose();
        outputStream = newStream;
    }

    public void Dispose()
    {
        if (outputStream == null && outputWriter == null)
            return;

        Flush();
        ResetDefaults();

        outputStream = null;
        outputWriter = null;
        arrayWriter = null;

        if(stateStack != null)
            ArrayPool<NbtStackState>.Shared.Return(stateStack);
    }

    private void GrowIfNeeded(int neededSize)
    {
        ThrowIfDisposed();

        int sizeHint = BytesPending + neededSize;

        if (sizeHint < realOutput.Length)
            return;

        Flush();
        if (outputStream != null)
        {
            Debug.Assert(arrayWriter != null);
            realOutput = arrayWriter.GetMemory(neededSize);
        }
        else
        {
            Debug.Assert(outputWriter != null);
            realOutput = outputWriter.GetMemory(neededSize);

            if (realOutput.Length < neededSize)
                ThrowInvalidSize(realOutput.Length, neededSize);
        }

        static void ThrowInvalidSize(int length, int needed)
            => throw new InvalidProgramException($"Error while trying to grow buffer size to '{needed}', got '{length}'");
    }

    private void ResetDefaults()
    {
        BytesPending = default;
        BytesCommited = default;
        realOutput = default;

        waitingValue = default;
        inList = default;
        valueWaited = default;
        remainingListLength = default;
        currentDepth = default;
    }

    private void ThrowIfDisposed()
        => ObjectDisposedException.ThrowIf(outputStream == null && outputWriter == null, this);
}
