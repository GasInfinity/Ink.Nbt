namespace Ink.Nbt;

public readonly struct NbtReaderOptions
{
    internal const int DefaultMaxDepth = 64;

    public readonly bool NoRootName;
    public readonly int MaxDepth;
}
