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
        this.bakeRoomTiles().generateConn(Direction.SOUTH).generateConn(Direction.EAST);
        this.generateConn(Direction.WEST);
        this.generateConn(Direction.NORTH);
        for (int i = 0; i < 4; i++)
        {
            if (!this.connSides[i]) continue;
            Connector connector = this.connectors[i];
            RoomTheme roomTheme = connector.getRandomRoomTheme();
            int xPos = connector.getX();
            int yPos = connector.getY();
            //Height and width minMaxEqual;
            int width = this.rng.Next(roomTheme.minWidth, roomTheme.maxWidth);
            int height = this.rng.Next(roomTheme.minHeight, roomTheme.maxHeight);
            switch (connector.getDirection())
            {
                case Direction.NORTH:
                    yPos -= height - 1;
                    xPos -= width / 2;
                    break;
                case Direction.EAST:
                    (width, height) = (height, width);
                    xPos -= 1;
                    yPos -= height / 2;
                    break;
                case Direction.SOUTH:
                    yPos -= 1;
                    xPos -= width / 2;
                    break;
                case Direction.WEST:
                    (width, height) = (height, width);
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
            new RoomTheme
            {
                id = 1,
                minWidth = 2,
                maxWidth = 2,
                minHeight = 6,
                maxHeight = 8,
                connectorIds = new[] {1},
                maxConnectors = 4,
                minConnectors = 1
            },
            new RoomTheme
            {
                id = 1,
                minWidth = 6,
                maxWidth = 8,
                minHeight = 6,
                maxHeight = 8,
                connectorIds = new[] {1},
                maxConnectors = 2,
                minConnectors = 1
            }
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
