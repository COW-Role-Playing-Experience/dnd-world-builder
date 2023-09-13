namespace map_generator.JsonLoading;

/**
 * Delegate candidate which recursively re-requests from the target Store.
 */
public class RandomProvider<T>
{
    public RandomProvider(Store<T> store, WeightedRandomSupplier<string> delegates)
    {
        _store = store;
        _randomSupplier = delegates;
    }

    private readonly Store<T> _store;
    private readonly WeightedRandomSupplier<string> _randomSupplier;

    public T Get() => _store.Get(_randomSupplier.Get());
}