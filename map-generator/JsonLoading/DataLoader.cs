using System.Text.Json;
using System.Text.RegularExpressions;
using map_generator.DecorHandling;

namespace map_generator.JsonLoading;

public static class DataLoader
{
    public static readonly string RootPath = AppDomain.CurrentDomain.BaseDirectory;
    public static readonly Regex ImageRegex = new(".*\\.(png|jpg|jpeg|gif)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static readonly JsonSerializerOptions UsePairCollections = new()
    {
        Converters = { new PairCollectionJsonConverter() }
    };

    private static RandomSource _randomSource = new();

    public static Random Random
    {
        set => _randomSource.Random = new Random(0);
        //_randomSource.Random = value;
    }

    public static readonly JsonSerializerOptions CaseInsensitive = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static readonly string ConcreteTexturePath = $"{RootPath}/Images";
    public static readonly string ConcreteTextureRoot = "Images";
    public static readonly string RandomTexturePath = $"{RootPath}/data/texture_collections.json";

    public static readonly string ConcreteDecorGroupPath = $"{RootPath}/data/decorgroups.json";
    public static readonly string RandomDecorGroupPath = $"{RootPath}/data/decorgroup_collections.json";

    public static readonly string EMPTY = "EMPTY";

    private static Store<Image<Rgba32>>? _textures;
    private static Store<DecorGroup>? _decorGroups;

    public static Store<Image<Rgba32>> Textures => _textures
        ?? throw new NullReferenceException("DataLoader has not been initialised with .Init()!");
    public static Store<DecorGroup> DecorGroups => _decorGroups
        ?? throw new NullReferenceException("DataLoader has not been initialised with .Init()!");

    public static void Init()
    {
        _randomSource.Random = new Random(0);
        _textures = new TextureStoreBuilder()
            .AddDebug()
            .AddEmpty()
            .BindConcreteTextures(ConcreteTexturePath, ConcreteTextureRoot)
            .BindRandomisedTextures(RandomTexturePath, _randomSource)
            .Get();
        _decorGroups = new DecorGroupStoreBuilder()
            .AddDebug()
            .AddEmpty()
            .BindConcreteDecorGroups(ConcreteDecorGroupPath)
            .BindRandomisedDecorGroups(RandomDecorGroupPath, _randomSource)
            .Get();
    }

    /**
     * Recursively finds all files in a path which match the regex pattern.
     */
    public static IEnumerable<string> FindAllFiles(string path, Regex pattern)
    {
        return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(f => pattern.IsMatch(f));
    }
}