namespace Ink.Nbt;

public readonly struct NbtReaderOptions(bool NoRootName, int MaxDepth)
{
    internal const int DefaultMaxDepth = 64;

    public readonly bool NoRootName = NoRootName;
    public readonly int MaxDepth = MaxDepth;
}
