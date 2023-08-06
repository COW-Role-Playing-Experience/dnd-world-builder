public class MapBuilder
{
    private RoomTile[][] tiles;
    private int xSize;
    private int ySize;

    public MapBuilder(int xSize, int ySize)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.tiles = null;
        this.emptyTilesMap();
        System.Console.WriteLine("testing");
    }

    private void emptyTilesMap()
    {
        RoomTile[][] xTiles = new RoomTile[this.xSize][];
        for (int x = 0; x < this.xSize; x++)
        {
            RoomTile[] yTiles = new RoomTile[ySize];
            for (int y = 0; y < this.ySize; y++)
            {
                yTiles[y] = new RoomTile(x, y);
            }
            xTiles[x] = yTiles;
        }
        this.tiles = xTiles;
    }

    public void printMap()
    {
        for (int x = 0; x < xSize; x++)
        {
            RoomTile[] xTiles = this.tiles[x];
            for (int y = 0; y < ySize; y++)
            {
                RoomTile tile = xTiles[y];
                System.Console.Write(tile.getChar());
            }
            System.Console.WriteLine("");
        }
    }
}
