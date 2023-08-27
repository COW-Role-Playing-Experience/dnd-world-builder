namespace map_generator;

public record Connector
{
    private static readonly Lazy<Connector> empty = new Lazy<Connector>(
        () => new Connector
        {
            x = -1,
            y = -1,
            id = 1,
            themes = new RoomTheme[] { },
            themeChances = new double[] { }
        }
        );

    public static Connector Empty
    {
        get { return empty.Value; }
    }

    public int x { get; init; }
    public int y { get; init; }
    public int id { get; init; }
    public RoomTheme[] themes { get; init; }
    public double[] themeChances { get; init; }

    public RoomTheme getRandomRoomTheme(Random rng)
    {
        double randDouble = rng.NextDouble();
        int closestIndex = themeChances.Select((chance, index) => new { Index = index, Difference = Math.Abs(chance - randDouble) })
            .OrderBy(i => i.Difference)
            .First()
            .Index;
        return this.themes[closestIndex];
    }

}
