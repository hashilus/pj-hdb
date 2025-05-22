using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WeightedRandomEntry<T>
{
    public T value;
    public int weight;

    public WeightedRandomEntry(int weight, T value)
    {
        this.weight = weight;
        this.value = value;
    }
}

public class WeightedRandom<T> : IEnumerable<WeightedRandomEntry<T>>
{
    readonly List<WeightedRandomEntry<T>> entries = new List<WeightedRandomEntry<T>>();

    public WeightedRandom() { }
    public WeightedRandom(IEnumerable<WeightedRandomEntry<T>> weightedEntries)
    {
        Add(weightedEntries);
    }

    public void Add(WeightedRandomEntry<T> entry)
    {
        entries.Add(entry);
    }

    public void Add(IEnumerable<WeightedRandomEntry<T>> weightedEntries)
    {
        entries.AddRange(weightedEntries);
    }

    public WeightedRandomEntry<T> Add(T value, int weight)
    {
        var entry = new WeightedRandomEntry<T>(weight, value);
        entries.Add(entry);
        return entry;
    }

    public void Remove(WeightedRandomEntry<T> entry)
    {
        entries.Remove(entry);
    }

    public void Remove(T value)
    {
        var index = entries.FindIndex(e => e.value.Equals(value));
        if (index < 0) return;

        entries.RemoveAt(index);
    }

    public T Sample()
    {
        return Mathh.WeightedRandom(entries.Select(e => e.value), entries.Select(e => e.weight));
    }

    public IEnumerator<WeightedRandomEntry<T>> GetEnumerator()
    {
        return entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return entries.GetEnumerator();
    }
}
