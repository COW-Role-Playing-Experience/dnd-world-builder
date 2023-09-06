using System.Text.Json;
using map_generator;

namespace map_generator.MapMaker;

public class MapBuilder
{
    private RoomTile[,] tiles;
    private int xSize;
    private int ySize;
    private Random rng;
    private RoomTheme[] roomThemes;
    private Connector[] connectors;
    private double expectedPopulation;

    public MapBuilder(int xSize, int ySize, Random rng, double expectedPopulation)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.rng = rng;
        this.expectedPopulation = expectedPopulation % 1;
        this.tiles = new RoomTile[xSize, ySize];
        this.emptyTilesMap();
    }

    private void emptyTilesMap()
    {
        for (int x = 0; x < this.xSize; x++)
        {
            for (int y = 0; y < this.ySize; y++)
            {
                RoomTile tile = new RoomTile(x, y, true);
                tiles[x, y] = tile;
            }
        }
    }

    public MapBuilder initRoom()
    {
        // TEST GENERATION
        int initX = this.xSize / 2;
        int initY = this.ySize / 2;
        RoomBuilder room = new RoomBuilder(initX, initY, 5, 5, Direction.NONE, this.roomThemes[5], this);
        room.generateMap();
        return this;
    }

    public MapBuilder fillGaps()
    {
        Console.WriteLine("FILLING GAPS");
        RoomTile[,] tmpGrid = this.cloneTiles();
        for (int x = 1; x < this.xSize - 1; x++)
        {
            for (int y = 1; y < this.ySize - 1; y++)
            {
                if (this.tiles[x, y].isEmpty())
                {
                    int count = this.countEmptyNeighbors(x, y);
                    if (count < 2)
                    {
                        tmpGrid[x, y].setEmpty(false);
                    }
                }
            }
        }

        this.tiles = tmpGrid;
        return this;
    }


    private int countEmptyNeighbors(int x, int y)
    {
        int count = 0;
        if (this.tiles[x + 1, y].isEmpty())
            count++;
        if (this.tiles[x - 1, y].isEmpty())
            count++;
        if (this.tiles[x, y + 1].isEmpty())
            count++;
        if (this.tiles[x, y - 1].isEmpty())
            count++;
        return count;
    }

    private RoomTile[,] cloneTiles()
    {
        RoomTile[,] cloneTiles = new RoomTile[xSize, ySize];
        for (int x = 0; x < this.xSize; x++)
        {
            for (int y = 0; y < this.ySize; y++)
            {
                cloneTiles[x, y] = (RoomTile)this.tiles[x, y].Clone();
            }
        }

        return cloneTiles;
    }

    public bool populationGreaterThanExpected()
    {
        int count = 0;
        int required = Convert.ToInt32(xSize * ySize * expectedPopulation);
        for (int x = 0; x < this.xSize; x++)
        {
            for (int y = 0; y < this.ySize; y++)
            {
                if (!tiles[x, y].isEmpty()) count++;
                if (count > required) return true;
            }
        }

        return false;
    }

    public MapBuilder printMap()
    {
        for (int y = 0; y < this.ySize; y++)
        {
            for (int x = 0; x < this.xSize; x++)
            {
                RoomTile tile = this.tiles[x, y];
                System.Console.Write(tile.getChar());
            }
            System.Console.WriteLine("");
        }
        return this;
    }

    public MapBuilder setTheme(string filePath)
    {
        string rawRooms = File.ReadAllText(filePath + "rooms.json");

        this.roomThemes = JsonSerializer.Deserialize<RoomTheme[]>(rawRooms);

        string rawConnectors = File.ReadAllText(filePath + "connectors.json");

        this.connectors = JsonSerializer.Deserialize<Connector[]>(rawConnectors);

        Console.WriteLine(roomThemes[0]);
        Console.WriteLine(connectors[0]);

        return this;
    }

    public RoomTheme[] getRoomThemes()
    {
        return this.roomThemes;
    }

    public Connector[] getConnectors()
    {
        return this.connectors;
    }

    public Random getRNG()
    {
        return this.rng;
    }

    public RoomTile[,] getTiles()
    {
        return this.tiles;
    }
}
