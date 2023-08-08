using map_generator;

public class MapBuilder
{
    private RoomTile[,] tiles;
    private int xSize;
    private int ySize;
    private Random rng;

    public MapBuilder(int xSize, int ySize, Random rng)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.rng = rng;
        this.tiles = new RoomTile[xSize, ySize];
        this.emptyTilesMap();
        System.Console.WriteLine("testing");
        this.initRoom();
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

    private void initRoom()
    {
        // TEST GENERATION
        int initX = this.xSize / 2;
        int initY = this.ySize / 2;
        RoomBuilder room = new RoomBuilder(initX, initY, 5, 5, this.tiles, this.rng);
        room.bakeRoomTiles().generateConn(Direction.NORTH);
        Connector conn = room.getConnector(Direction.NORTH);
        RoomBuilder room2 = new RoomBuilder(conn.getX(), conn.getY() - 1, 2, 2, this.tiles, this.rng);
        room2.bakeRoomTiles();
    }

    public void printMap()
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                RoomTile tile = this.tiles[x, y];
                System.Console.Write(tile.getChar());
            }
            System.Console.WriteLine("");
        }
    }
}
