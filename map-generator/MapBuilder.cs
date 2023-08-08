public class MapBuilder
{
    private RoomTile[,] tiles;
    private int xSize;
    private int ySize;

    public MapBuilder(int xSize, int ySize)
    {
        this.xSize = xSize;
        this.ySize = ySize;
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
        int initX = this.xSize / 2;
        int initY = this.ySize / 2;
        RoomBuilder room = new RoomBuilder(initX, initY, 5, 5, this.tiles);
    }

    public void printMap()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                RoomTile tile = this.tiles[x, y];
                System.Console.Write(tile.getChar());
            }
            System.Console.WriteLine("");
        }
    }
}
