using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Ink.Nbt.Tags;

public sealed partial class CompoundTagData : IDictionary<string, NbtTag>
{
    private readonly Dictionary<string, NbtTag> map = new();

    public NbtTag this[string key] { get => map[key]; set => map[key] = value; }

    public Dictionary<string, NbtTag>.KeyCollection Keys
        => map.Keys;

    public Dictionary<string, NbtTag>.ValueCollection Values
        => map.Values;

    ICollection<string> IDictionary<string, NbtTag>.Keys
        => Keys;

    ICollection<NbtTag> IDictionary<string, NbtTag>.Values
        => Values;

    public int Count
        => map.Count;

    public bool IsReadOnly
        => false;

    public void Add(string key, NbtTag value)
        => map.Add(key, value);

    public void Add(KeyValuePair<string, NbtTag> item)
        => (map as ICollection<KeyValuePair<string, NbtTag>>)!.Add(item);

    public void Clear()
        => map.Clear();

    public bool Contains(KeyValuePair<string, NbtTag> item)
        => map.Contains(item);

    public bool ContainsKey(string key)
        => map.ContainsKey(key);

    public void CopyTo(KeyValuePair<string, NbtTag>[] array, int arrayIndex)
        => (map as ICollection<KeyValuePair<string, NbtTag>>)!.CopyTo(array, arrayIndex);

    public bool Remove(string key)
        => map.Remove(key);

    public bool Remove(KeyValuePair<string, NbtTag> item)
        => (map as ICollection<KeyValuePair<string, NbtTag>>)!.Remove(item);

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out NbtTag value)
        => map.TryGetValue(key, out value);

    public Dictionary<string, NbtTag>.Enumerator GetEnumerator()
        => map.GetEnumerator();

    IEnumerator<KeyValuePair<string, NbtTag>> IEnumerable<KeyValuePair<string, NbtTag>>.GetEnumerator()
        => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
