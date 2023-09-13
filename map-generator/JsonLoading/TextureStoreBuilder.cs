using System.Text.Json;
using System.Text.RegularExpressions;

namespace map_generator.JsonLoading;

// Alias types for loaded data
using GroupData = ICollection<(string, ICollection<(string, float)>)>;

public class TextureStoreBuilder
{
    private bool _used = false;
    private readonly Store<Image<Rgba32>> _store = new();

    /**
     * Recursively adds all textures from the given path into the texture store.
     */
    public TextureStoreBuilder BindConcreteTextures(string path, string canonicalRoot)
    {
        Regex relativePathRegex = new($"{canonicalRoot}.*(?=\\.)", RegexOptions.IgnoreCase);
        IEnumerable<string> paths = DataLoader.FindAllFiles(path, DataLoader.ImageRegex);

        foreach (string filePath in paths)
        {
            string id = relativePathRegex.Match(filePath).Value;
            _store.Add(id, new LazyTextureProvider(filePath).Texture);
        }

        return this;
    }

    /**
     * Reads the provided json file, adding the probabilistic textures into the texture store.
     */
    public TextureStoreBuilder BindRandomisedTextures(string path, Random random)
    {
        GroupData textureGroups;
        using (FileStream fs = File.OpenRead(path))
        {
            textureGroups = JsonSerializer.Deserialize<GroupData>(fs, DataLoader.UsePairCollections)!;
        }

        foreach ((var name, var texture) in textureGroups)
        {
            WeightedRandomSupplier<string> selector = new(random, texture);

            // TODO: possibly replace RandomProvider<T> with lambda
            _store.Add(name, new RandomProvider<Image<Rgba32>>(_store, selector).Get);
        }

        return this;
    }

    public TextureStoreBuilder AddDebug()
    {
        Image<Rgba32> debug = new Image<Rgba32>(1, 1);
        debug[0, 0] = new Rgba32(128, 0, 0, 0);

        _store.Add("DEBUG", () => debug);
        return this;
    }

    public Store<Image<Rgba32>> Get()
    {
        if (_used) throw new InvalidOperationException("This builder has already been used.");
        _used = true;
        return _store;
    }
}