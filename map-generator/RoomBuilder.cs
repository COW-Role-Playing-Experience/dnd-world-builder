using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using map_generator;

public class RoomBuilder
{
    private int xSize;
    private int ySize;
    private int x;
    private int y;
    private readonly RoomTile[,] gridTiles;
    private readonly RoomTheme[] themes;
    private readonly Connector[] connectors;
    private Random rng;
    private int[] connectorIds;
    private int connectorCount;
    private bool[] connSides;
    private Connector[] roomConnectors;

    public RoomBuilder(int x, int y, int xSize, int ySize, RoomTheme roomTheme, RoomTile[,] gridTiles, Random rng, RoomTheme[] themes, Connector[] connectors)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.x = x;
        this.y = y;
        this.gridTiles = gridTiles;
        this.rng = rng;
        this.themes = themes;
        this.connectors = connectors;
        this.connectorIds = roomTheme.connectorIds;
        this.connectorCount = rng.Next(roomTheme.minConnectors, roomTheme.maxConnectors);
        this.roomConnectors = new Connector[4];
        Array.Fill(this.roomConnectors, Connector.Empty);
        this.connSides = new bool[4];
    }

    public void generateRoom()
    {
        this.bakeRoomTiles().generateConns();
        for (int i = 0; i < 4; i++)
        {
            if (!this.connSides[i]) continue;
            Connector connector = this.roomConnectors[i];
            RoomTheme roomTheme = connector.getRandomRoomTheme(this.themes, this.rng);
            int xPos = this.x + rng.Next(Convert.ToInt32(-(connector.padding * 2 - 1) * this.xSize)) + Convert.ToInt32(connector.padding * this.xSize);
            int yPos = this.y - 1;

            int width = this.rng.Next(roomTheme.minWidth, roomTheme.maxWidth);
            int height = this.rng.Next(roomTheme.minHeight, roomTheme.maxHeight);
            switch (i)
            {
                case 0:
                    yPos -= height - 1;
                    xPos -= width / 2;
                    break;
                case 1:
                    (width, height) = (height, width);
                    xPos -= 1;
                    yPos -= height / 2;
                    break;
                case 2:
                    yPos -= 1;
                    xPos -= width / 2;
                    break;
                case 3:
                    (width, height) = (height, width);
                    xPos -= width - 1;
                    yPos -= height / 2;
                    break;
            }

            bool safe = this.checkTilesEmptyOrAvailable(xPos, yPos, width, height);
            if (!safe) continue;

            RoomBuilder room = new RoomBuilder(xPos, yPos, width, height,
                roomTheme, this.gridTiles, this.rng, this.themes, this.connectors);
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

    private RoomBuilder generateConns()
    {
        List<Direction> directions = randomSides(this.connectorCount);
        int count = 0;
        foreach (Direction direction in directions)
        {
            switch (direction)
            {
                case Direction.NORTH:
                    this.roomConnectors[0] = connectors[this.connectorIds[count]];
                    this.connSides[0] = true;
                    break;
                case Direction.EAST:
                    this.roomConnectors[1] = connectors[this.connectorIds[count]];
                    this.connSides[1] = true;
                    break;
                case Direction.SOUTH:
                    this.roomConnectors[2] = connectors[this.connectorIds[count]];
                    this.connSides[2] = true;
                    break;
                case Direction.WEST:
                    this.roomConnectors[3] = connectors[this.connectorIds[count]];
                    this.connSides[3] = true;
                    break;
                default:
                    throw new ArgumentException("Direction value does not correspond " +
                                                "with a valid side of this room", "direction");
            }
            count++;
        }

        return this;
    }

    private List<Direction> randomSides(int count)
    {
        List<Direction> availableSides = new List<Direction> { Direction.NORTH, Direction.EAST, Direction.SOUTH, Direction.WEST };
        List<Direction> chosenSides = new List<Direction>();

        while (chosenSides.Count < count)
        {
            int index = rng.Next(availableSides.Count);
            chosenSides.Add(availableSides[index]);
            availableSides.RemoveAt(index);
        }

        return chosenSides;
    }

}
