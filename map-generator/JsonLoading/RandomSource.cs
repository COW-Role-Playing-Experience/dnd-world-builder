namespace map_generator.JsonLoading;

/**
 * Boxed variant of Random, allowing for hotswapping of random within DataLoader;
 */
public class RandomSource
{
    private Random? _random;

    public Random Random
    {
        get => _random ?? throw new NullReferenceException("Random has not been initialised!");
        set => _random = value;
    }
}