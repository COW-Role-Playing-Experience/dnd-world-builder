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

    public void generateRoom()
    {
        this.bakeRoomTiles();
        this.generateConn(Direction.SOUTH);
        for (int i = 0; i < 4; i++)
        {
            if (!this.connSides[i]) continue;
            Connector connector = this.connectors[i];
            RoomTheme roomTheme = connector.getRandomRoomTheme();
            int xPos = connector.getX();
            int yPos = connector.getY();
            //Height and width minMaxEqual;
            int width = this.rng.Next(roomTheme.getMinSize(), roomTheme.getMaxSize());
            int height = this.rng.Next(roomTheme.getMinSize(), roomTheme.getMaxSize());
            switch (connector.getDirection())
            {
                case Direction.NORTH:
                    yPos -= height - 1;
                    xPos -= width / 2;
                    break;
                case Direction.EAST:
                    xPos -= 1;
                    yPos -= height / 2;
                    break;
                case Direction.SOUTH:
                    yPos -= 1;
                    xPos -= width / 2;
                    break;
                case Direction.WEST:
                    xPos -= width - 1;
                    yPos -= height / 2;
                    break;
            }

            bool safe = this.checkTilesEmptyOrAvailable(xPos, yPos, width, height);
            if (!safe) continue;

            RoomBuilder room = new RoomBuilder(xPos, yPos, width, height, gridTiles, rng);
            room.generateRoom();
        }
    }

    private bool checkTilesEmptyOrAvailable(int x, int y, int width, int height)
    {
        int gridWidth = this.gridTiles.GetLength(0);
        int gridHeight = this.gridTiles.GetLength(1);

        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                if (i < 0 || i >= gridWidth || j < 0 || j >= gridHeight)
                {
                    return false;
                }
                else
                {
                    RoomTile tile = this.gridTiles[i, j];
                    System.Console.WriteLine("" + tile.isEmpty());
                    if (!tile.isEmpty())
                    {
                        return false;
                    }
                }
            }
        }

        return true;
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
    private RoomBuilder bakeRoomTiles()
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

    private RoomBuilder generateConn(Direction direction)
    {
        RoomTheme[] themes = {
            new RoomTheme(1, 3, 5, 1, 4, new int[] {1}),
            new RoomTheme(1, 6, 8, 1, 2, new int[] {1})
        };
        switch (direction)
        {
            case Direction.NORTH:
                if (!connSides[0])
                {
                    int xCoord = this.x + rng.Next(this.xSize);
                    this.connectors[0] = new Connector(xCoord, y - 1, 1, direction, themes, this.rng);
                    this.connSides[0] = true;
                }
                break;
            case Direction.EAST:
                if (!connSides[1])
                {
                    int yCoord = this.y + rng.Next(this.ySize);
                    this.connectors[1] = new Connector(x + this.xSize + 1, yCoord, 1, direction, themes, this.rng);
                    this.connSides[1] = true;
                }
                break;
            case Direction.SOUTH:
                if (!connSides[2])
                {
                    int xCoord = this.x + rng.Next(this.xSize);
                    this.connectors[2] = new Connector(xCoord, y + this.ySize + 1, 1, direction, themes, this.rng);
                    this.connSides[2] = true;
                }
                break;
            case Direction.WEST:
                if (!connSides[3])
                {
                    int yCoord = this.y + rng.Next(this.ySize);
                    this.connectors[3] = new Connector(x - 1, yCoord, 1, direction, themes, this.rng);
                    this.connSides[3] = true;
                }
                break;
            default:
                throw new ArgumentException("Direction value does not correspond " +
                                            "with a valid side of this room", "direction");
        }
        return this;
    }

}
