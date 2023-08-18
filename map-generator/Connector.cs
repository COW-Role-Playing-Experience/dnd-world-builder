namespace map_generator;

public class Connector
{
    private static readonly Lazy<Connector> empty = new Lazy<Connector>(
        () => new Connector(-1, -1, 1, Direction.NONE, new RoomTheme[] { }, new Random())
        );

    public static Connector Empty
    {
        get { return empty.Value; }
    }

    private int x;
    private int y;
    private int id;
    private Random rng;
    private RoomTheme[] themes;
    private int themeCount;
    private Direction direction;

    public Connector(int x, int y, int id, Direction direction, RoomTheme[] themes, Random rng)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.rng = rng;
        this.themes = themes;
        this.themeCount = themes.Length;
        this.direction = direction;
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }

    public Direction getDirection()
    {
        return this.direction;
    }
    public RoomTheme getRandomRoomTheme()
    {
        return this.themes[rng.Next(this.themeCount)];
    }

}
