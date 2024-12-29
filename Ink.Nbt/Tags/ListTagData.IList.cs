using System.Collections;

namespace Ink.Nbt.Tags;

public partial class ListTagData : IList<NbtTag>
{
    public NbtTag this[int index] { get => values[index]; set => values[index] = value; }

    public int Count
        => values.Count;

    public bool IsReadOnly
        => false;

    public void Add(NbtTag item)
    {
        CheckAndThrow(item);
        values.Add(item);
    }

    public void Clear()
        => values.Clear();

    public bool Contains(NbtTag item)
        => ListType == item.Type && values.Contains(item);

    public void CopyTo(NbtTag[] array, int arrayIndex)
        => values.CopyTo(array, arrayIndex);

    public int IndexOf(NbtTag item)
        => ListType != item.Type ? -1 : values.IndexOf(item);

    public void Insert(int index, NbtTag item)
    {
        CheckAndThrow(item);
        values.Insert(index, item);
    }

    public bool Remove(NbtTag item)
        => ListType == item.Type && values.Remove(item);

    public void RemoveAt(int index)
        => values.RemoveAt(index);

    public List<NbtTag>.Enumerator GetEnumerator()
        => values.GetEnumerator();

    IEnumerator<NbtTag> IEnumerable<NbtTag>.GetEnumerator()
        => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private void CheckAndThrow(NbtTag value)
    {
        if (ListType == NbtTagType.End)
            ListType = value.Type;

        if (ListType != value.Type)
            throw new InvalidOperationException($"Trying to modify a ListTag<{ListType}> with a tag of type '{value.Type}'");
    }
}
