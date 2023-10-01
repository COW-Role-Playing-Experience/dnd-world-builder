using System.Text.Json;
using map_generator.DecorHandling;

namespace map_generator.JsonLoading;

// Alias types for loaded data
using GroupData = ICollection<(string, ICollection<(string, float)>)>;

public class DecorGroupStoreBuilder
{
    private bool _used = false;
    private readonly Store<DecorGroup> _store = new();

    public DecorGroupStoreBuilder BindConcreteDecorGroups(string path)
    {
        List<DecorGroup> groups;
        using (FileStream fs = File.OpenRead(path))
        {
            groups = JsonSerializer.Deserialize<List<DecorGroup>>(fs, DataLoader.CaseInsensitive)!;
        }

        foreach (var decorGroup in groups)
        {
            Console.WriteLine(decorGroup.Name);
            _store.Add(decorGroup.Name, () => decorGroup);
        }

        return this;
    }

    public DecorGroupStoreBuilder BindRandomisedDecorGroups(string path, RandomSource random)
    {
        GroupData dgGroups;

        using (FileStream fs = File.OpenRead(path))
        {
            dgGroups = JsonSerializer.Deserialize<GroupData>(fs, DataLoader.UsePairCollections)!;
        }

        foreach ((var name, var decorGroup) in dgGroups)
        {
            WeightedRandomSupplier<string> selector = new(random, decorGroup);

            // TODO: possibly replace RandomProvider<T> with lambda
            _store.Add(name, new RandomProvider<DecorGroup>(_store, selector).Get);
        }

        return this;
    }

    public DecorGroupStoreBuilder AddDebug()
    {
        List<DecorPosition> positions = new();
        DecorGroup debug = new DecorGroup("DEBUG", 2, 2, positions);
        positions.Add(new DecorPosition(new Decor(Path.Combine("Images", "Decor", "Corpse", "Decayed_Body_A_03_1x2")), 1, 1, 45));

        _store.Add("DEBUG", () => debug);
        return this;
    }

    public Store<DecorGroup> Get()
    {
        if (_used) throw new InvalidOperationException("This builder has already been used.");
        _used = true;
        return _store;
    }
}