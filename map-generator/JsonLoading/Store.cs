namespace map_generator.JsonLoading;

/**
 * Generic delegate accessor allowing for the use of behaviour hooks for values.
 *
 *
 * <summary>
 * <para>Values can be function hooks to remote objects, allowing for the use of
 * lazy loading, composite pattern, and other behavioural hooks.</para>
 * </summary>>
 */
public class Store<T>
{
    public Dictionary<string, Func<T>> _lookup = new();

    public void Add(string id, Func<T> supplier) => _lookup.Add(id, supplier);
    public T Get(string name) => _lookup[name]();
    public T this[string name] => _lookup[name]();
}