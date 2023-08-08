using System.Collections;
using System.Diagnostics.CodeAnalysis;
using map_generator;

public class RoomBuilder
{
    private int xSize;
    private int ySize;
    private int x;
    private int y;
    private readonly RoomTile[,] gridTiles;
    private Random rng;
    private bool[] connSides;
    private Connector[] connectors;

    public RoomBuilder(int x, int y, int xSize, int ySize, RoomTile[,] gridTiles, Random rng)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.x = x;
        this.y = y;
        this.gridTiles = gridTiles;
        this.rng = rng;
        this.connectors = new Connector[4];
        Array.Fill(this.connectors, Connector.Empty);
        this.connSides = new bool[4];
    }



    public bool hasConn(Direction direction)
    {
        if (direction != Direction.NONE)
        {
            return this.connSides[(int)direction];
        }
        throw new ArgumentException("Direction value does not correspond " +
                                    "with a valid side of this room", "direction");
    }

    public Connector getConnector(Direction direction)
    {
        if (direction != Direction.NONE)
        {
            return this.connectors[(int)direction];
        }
        throw new ArgumentException("Direction value does not correspond " +
                                    "with a valid side of this room", "direction");
    }

    // Builder Methods
    public RoomBuilder bakeRoomTiles()
    {
        for (int i = x; i < x + this.xSize; i++)
        {
            for (int j = y; j < y + this.ySize; j++)
            {
                RoomTile tile = this.gridTiles[i, j];
                tile.setEmpty(false);
            }
        }

        return this;
    }

    public RoomBuilder generateConn(Direction direction)
    {
        switch (direction)
        {
            case Direction.NORTH:
                if (!connSides[0])
                {
                    int xCoord = this.x + rng.Next(this.xSize);
                    this.connectors[0] = new Connector(xCoord, y - 1);
                }
                break;
            case Direction.EAST:
                if (!connSides[1])
                {
                    int yCoord = this.y + rng.Next(this.ySize);
                    this.connectors[1] = new Connector(x + this.xSize + 1, yCoord);
                }
                break;
            case Direction.SOUTH:
                if (!connSides[2])
                {
                    int xCoord = this.x + rng.Next(this.xSize);
                    this.connectors[2] = new Connector(xCoord, y + this.ySize + 1);
                }
                break;
            case Direction.WEST:
                if (!connSides[3])
                {
                    int yCoord = this.y + rng.Next(this.ySize);
                    this.connectors[3] = new Connector(x - 1, yCoord);
                }
                break;
            default:
                throw new ArgumentException("Direction value does not correspond " +
                                            "with a valid side of this room", "direction");
        }
        return this;
    }

}
