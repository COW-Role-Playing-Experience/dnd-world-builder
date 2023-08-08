namespace map_generator;

public class Connector
{
    private static readonly Lazy<Connector> empty = new Lazy<Connector>(
        () => new Connector(-1, -1)
        );

    public static Connector Empty
    {
        get { return empty.Value; }
    }

    private int x;
    private int y;

    public Connector(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }

}
