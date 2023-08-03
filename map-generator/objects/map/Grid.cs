namespace map_generator;

public class Grid
{
    private Tile[][] tiles;
    private int xSize;
    private int ySize;

    public Grid(int xSize, int ySize)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        initTiles();
    }

    private void initTiles()
    {
        Tile[][] tileArray = new Tile[xSize][];
        for (int x = 0; x < xSize; x++)
        {
            Tile[] xTiles = new Tile[ySize];
            for (int y = 0; y < ySize; y++)
            {
                xTiles[y] = new Tile();
            }

            tileArray[x] = xTiles;
        }

        this.tiles = tileArray;
    }

    public void printGrid()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Tile tile = tiles[x][y];
                System.Console.Write(tile.getTileChar());
            }
            System.Console.WriteLine("");

        }

    }
}
