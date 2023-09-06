namespace map_generator.MapMaker;

public record Connector
{
    private static readonly Lazy<Connector> empty = new Lazy<Connector>(
        () => new Connector
        {
            id = -1,
            padding = -1,
            themeIds = new int[] { },
            themeChances = new double[] { }
        }
        );

    public static Connector Empty
    {
        get { return empty.Value; }
    }

    public int id { get; init; }
    public double padding { get; init; }
    public int[] themeIds { get; init; }
    public double[] themeChances { get; init; }

    public RoomTheme getRandomRoomTheme(RoomTheme[] themes, Random rng)
    {
        if (this.Equals(Empty))
        {
            throw new InvalidOperationException("Cannot get a random theme from an empty Connector");
        }

        double randDouble = rng.NextDouble();
        int closestIndex = themeChances.Select((chance, index) => new { Index = index, Difference = Math.Abs(chance - randDouble) })
            .OrderBy(i => i.Difference)
            .First()
            .Index;
        return themes[this.themeIds[closestIndex]];
    }

}
