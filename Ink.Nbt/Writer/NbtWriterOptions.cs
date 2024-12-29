namespace Ink.Nbt;

public readonly struct NbtWriterOptions(int MaxDepth = NbtWriterOptions.DefaultMaxDepth, bool ShouldValidate = true)
{
    internal const int DefaultMaxDepth = 64;

    public readonly int MaxDepth = MaxDepth;
    public readonly bool ShouldValidate = ShouldValidate;
}
