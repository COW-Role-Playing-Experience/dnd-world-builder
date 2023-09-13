namespace map_generator.JsonLoading;

/**
 * Uses a weighted pool to choose items of type T randomly.
 */
public class WeightedRandomSupplier<T>
{
    private float _sum = 0.0f;
    private readonly Random _random;
    private readonly List<(T, float)> _items = new();

    public WeightedRandomSupplier(Random random)
    {
        _random = random;
    }
    public WeightedRandomSupplier(Random random, ICollection<(T, float)> elements)
    {
        _random = random;
        AddAll(elements);
    }

    public void AddAll(ICollection<(T, float)> elements)
    {
        _items.AddRange(elements);
        foreach (var element in elements)
        {
            _sum += element.Item2;
        }
    }

    public void Add(T element, float weight)
    {
        _items.Add((element, weight));
        _sum += weight;
    }

    public T Get()
    {
        // Picks a random target in range of total pool size
        float target = _random.NextSingle() * _sum;
        float cumulated = 0.0f;

        // Iteratively finds the element which overflows the target
        foreach (var element in _items)
        {
            cumulated += element.Item2;
            if (cumulated >= target) return element.Item1;
        }

        // If due to floating-point error, none are found, default to last element
        return _items.Last().Item1;
    }
}