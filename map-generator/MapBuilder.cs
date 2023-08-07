public class MapBuilder
{
    private Dictionary<(int, int), RoomTile> tiles;
    private int xSize;
    private int ySize;

    public MapBuilder(int xSize, int ySize)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.tiles = new Dictionary<(int, int), RoomTile>();
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
                tiles.Add((x, y), tile);
            }
        }
    }

    private void initRoom()
    {
        int initX = this.xSize / 2;
        int initY = this.ySize / 2;
        RoomBuilder room = new RoomBuilder(initX, initY, 5, 5, this.tiles);
        this.applyTiles(room.getTiles());
    }

    private void applyTiles(Dictionary<(int, int), RoomTile> tiles)
    {
        foreach (RoomTile tile in tiles.Values)
        {
            int x = tile.getX();
            int y = tile.getY();
            this.tiles[(x, y)] = tile;
        }
    }

    public void printMap()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                RoomTile tile = this.tiles[(x, y)];
                System.Console.Write(tile.getChar());
            }
            System.Console.WriteLine("");
        }
    }
}
