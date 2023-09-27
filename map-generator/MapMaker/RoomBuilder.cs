using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using map_generator;
using map_generator.DecorHandling;

namespace map_generator.MapMaker;

public class RoomBuilder
{
    private int xSize;
    private int ySize;
    private int x;
    private int y;
    private readonly RoomTile[,] gridTiles;
    private readonly RoomTheme[] themes;
    private readonly Connector[] connectors;
    private readonly MapBuilder mapBuilder;
    private readonly RoomTheme roomTheme;
    private Random rng;
    private Direction prevDirection;
    private int regenCount;
    private int[] connectorIds;
    private int connectorCount;
    private bool[] connSides;
    private Connector[] roomConnectors;

    public RoomBuilder(int x, int y, int xSize, int ySize, Direction prevDirection, RoomTheme roomTheme, MapBuilder mapBuilder)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.x = x;
        this.y = y;
        this.gridTiles = mapBuilder.getTiles();
        this.rng = mapBuilder.getRNG();
        this.themes = mapBuilder.getRoomThemes();
        this.connectors = mapBuilder.getConnectors();
        this.mapBuilder = mapBuilder;
        this.roomTheme = roomTheme;
        this.prevDirection = prevDirection;
        this.regenCount = 0;
        this.connectorIds = roomTheme.connectorIds;
        this.connectorCount = rng.Next(roomTheme.minConnectors, roomTheme.maxConnectors);
        this.roomConnectors = new Connector[4];
        Array.Fill(this.roomConnectors, Connector.Empty);
        this.connSides = new bool[4];
    }

    public void generateMap()
    {
        Queue<RoomBuilder> builderBuffer = new Queue<RoomBuilder>();
        generateRooms(builderBuffer);
        this.bakeRoomTiles();
        while (builderBuffer.Count != 0)
        {
            builderBuffer.Dequeue().generateRooms(builderBuffer);
        }
    }
    public void generateRooms(Queue<RoomBuilder> builderBuffer)
    {
        this.generateConns();
        List<RoomBuilder> rooms = new List<RoomBuilder>();
        List<int> randOrder = new List<int> { 0, 1, 2, 3 };

        // Fisher-Yates shuffle
        for (int i = randOrder.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (randOrder[i], randOrder[j]) = (randOrder[j], randOrder[i]);
        }

        foreach (int i in randOrder)
        {
            if (!this.connSides[i]) continue;
            Connector connector = this.roomConnectors[i];
            RoomTheme roomTheme = connector.getRandomRoomTheme(this.themes, this.rng);
            int xPos;
            int yPos;
            if (i % 2 == 0)
            {
                xPos = this.x + rng.Next(Convert.ToInt32(connector.padding * this.xSize)) +
                           Convert.ToInt32(-(connector.padding - 1) / 2 * this.xSize);
                yPos = this.y;
            }
            else
            {
                xPos = this.x;
                yPos = this.y + rng.Next(Convert.ToInt32(connector.padding * this.ySize)) +
                       Convert.ToInt32(-(connector.padding - 1) / 2 * this.ySize);
            }

            int width = this.rng.Next(roomTheme.minWidth, roomTheme.maxWidth);
            int height = this.rng.Next(roomTheme.minHeight, roomTheme.maxHeight);
            Direction prevDir = (Direction)i;
            switch (i)
            {
                case 0:
                    yPos -= height;
                    xPos -= width / 2;
                    break;
                case 1:
                    (width, height) = (height, width);
                    xPos += this.xSize;
                    yPos -= height / 2;
                    break;
                case 2:
                    yPos += this.ySize;
                    xPos -= width / 2;
                    break;
                case 3:
                    (width, height) = (height, width);
                    xPos -= width;
                    yPos -= height / 2;
                    break;
            }

            bool safe = this.checkTilesEmptyOrAvailable(xPos, yPos, width, height);
            if (!safe)
            {
                if (regenCount == 8) continue;
                regenCount++;
                builderBuffer.Enqueue(this);
                generateRooms(builderBuffer);
                return;
            }

            try
            {
                Console.Clear();
            }
            catch (IOException e)
            {
                //TODO Unhandled exception occurred when caling console.clear from the UI
            }
            //mapBuilder.printMap();


            RoomBuilder room = new RoomBuilder(xPos, yPos, width, height, prevDir,
                roomTheme, this.mapBuilder);
            rooms.Add(room);
        }

        foreach (RoomBuilder room in rooms)
        {
            bool safe = checkTilesEmptyOrAvailable(room.x, room.y, room.xSize, room.ySize);
            if (safe)
            {
                room.bakeRoomTiles();
                builderBuffer.Enqueue(room);
            }
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
                RoomTile tile = this.gridTiles[i, j];
                if (!tile.isEmpty())
                {
                    return false;
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
        if (!checkTilesEmptyOrAvailable(x, y, xSize, ySize))
        {
            throw new UnauthorizedAccessException("Cannot bake over existing room tiles here.");
        }
        RoomDecorator roomDecorator = new RoomDecorator(x, y, xSize, ySize, rng);
        mapBuilder.addMetaTiles(roomDecorator.GenerateDecor(this.roomTheme));
        for (int i = x; i < x + this.xSize; i++)
        {
            for (int j = y; j < y + this.ySize; j++)
            {
                RoomTile tile = this.gridTiles[i, j];
                tile.setTexture(this.roomTheme.floorTexture);
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
        if (this.prevDirection != Direction.NONE)
        {
            availableSides.Remove(this.prevDirection + 2 % 4);
            count = count > 3 ? 3 : count;
        }
        // Fisher-Yates shuffle
        for (int i = availableSides.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (availableSides[i], availableSides[j]) = (availableSides[j], availableSides[i]);
        }
        while (chosenSides.Count < count)
        {
            int index = rng.Next(0, availableSides.Count);
            chosenSides.Add(availableSides[index]);
            availableSides.RemoveAt(index);
        }

        return chosenSides;
    }



}
