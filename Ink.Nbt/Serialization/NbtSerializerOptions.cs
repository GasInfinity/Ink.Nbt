namespace Ink.Nbt.Serialization;

public sealed class NbtSerializerOptions(int maxDepth = NbtWriterOptions.DefaultMaxDepth, bool shouldValidate = false, bool noRootName = false)
{
    private readonly List<NbtConverter> converters = new();
    private int maxDepth = maxDepth;
    private bool shouldValidate = shouldValidate;
    private bool noRootName = noRootName;

    public int MaxDepth { get => this.maxDepth; set => this.maxDepth = value; }
    public bool ShouldValidate { get => this.shouldValidate; set => this.shouldValidate = value; }
    public bool NoRootName { get => this.noRootName; set => this.noRootName = value; }

    public IReadOnlyCollection<NbtConverter> Converters
        => this.converters;

    public void AddConverter(NbtConverter converter)
        => this.converters.Add(converter);
}
