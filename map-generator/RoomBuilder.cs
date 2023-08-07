using System.Collections;

public class RoomBuilder
{
    private int xSize;
    private int ySize;
    private int x;
    private int y;
    private Dictionary<(int, int), RoomTile> roomTiles;
    private readonly Dictionary<(int, int), RoomTile> gridTiles;

    private bool[] connSides;

    public RoomBuilder(int x, int y, int xSize, int ySize, Dictionary<(int, int), RoomTile> gridTiles)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.x = x;
        this.y = y;
        this.roomTiles = new Dictionary<(int, int), RoomTile>();
        this.gridTiles = gridTiles;

        this.initRoomTiles();
    }

    private void initRoomTiles()
    {
        for (int i = x; i < this.xSize; i++)
        {
            for (int j = y; j < this.ySize; j++)
            {
                RoomTile tile = this.gridTiles[(i, j)];
                tile.setEmpty(false);
                this.roomTiles.Add((i, j), tile);
            }
        }
    }

    public Dictionary<(int, int), RoomTile> getTiles()
    {
        return this.roomTiles;
    }

}
