using System.Text;
using map_generator.MapMaker;

namespace map_generator.DecorHandling;

public class RoomDecorator
{
    private int xSize;
    private int ySize;
    private int xOrigin;
    private int yOrigin;
    private Random _random;


    private bool[,] occupancyMap;
    private int freeTiles;

    public static float idealDecorPercent = 0.3f; //ideally we want 30% of the room, at a minimum, to be metaTiles

    public static readonly float maxPlacementAttempts = 30; //the number of times you will choose a decor group to attempt to place
    public RoomDecorator(int xOrigin, int yOrigin, int xSize, int ySize, Random random)
    {
        this._random = random;
        this.xOrigin = xOrigin;
        this.yOrigin = yOrigin;
        this.xSize = xSize;
        this.ySize = ySize;
        this.occupancyMap = new bool[xSize, ySize];
        this.freeTiles = occupancyMap.Length;
    }

    public RoomDecorator(int xOrigin, int yOrigin, bool[,] occupancyMap, Random random)
    {
        this._random = random;
        this.xOrigin = xOrigin;
        this.yOrigin = yOrigin;
        this.xSize = occupancyMap.GetLength(0);
        this.ySize = occupancyMap.GetLength(1);
        this.occupancyMap = occupancyMap;
        this.freeTiles = occupancyMap.Length; //TODO: replace with check for count of boolean false (empty tiles)
    }

    /**Add MetaTiles to a room until idealDecorPercent or more of it is filled with MetaTiles */
    public List<MetaTile> GenerateDecor(RoomTheme roomTheme)
    {
        int idealOccupiedTiles = (int)(idealDecorPercent * freeTiles);
        int occupiedTiles = 0;
        int placementAttemptNumber = 0;

        List<MetaTile> metaTiles = new List<MetaTile>();

        var traversalMap = new (int, int, int, int)[xSize, ySize];

        while (occupiedTiles < idealOccupiedTiles)
        {
            //TODO: replace with call to RoomTheme
            DecorGroup toPlace = roomTheme.GetDecorGroup();

            List<(int, int)> positions = FindValidPlacements(toPlace, traversalMap);

            // If the tile was unable to be placed
            if (positions.Count == 0)
            {
                placementAttemptNumber++;
                if (placementAttemptNumber > maxPlacementAttempts)
                {
                    break; //if you've tried many times to place decor and none of it has worked
                }
                else
                {
                    continue; //if you can't place this decor, try again with a new decor
                }
            }

            occupiedTiles += toPlace.Width * toPlace.Height;

            (int xPos, int yPos) = positions[_random.Next(0, positions.Count - 1)];

            // Update the occupancy map to reflect new MetaTile
            for (int x = 0; x < toPlace.Width; x++)
            {
                for (int y = 0; y < toPlace.Height; y++)
                {
                    occupancyMap[x + xPos, y + yPos] = true;
                }
            }

            metaTiles.Add(new MetaTile(xPos + xOrigin, yPos + yOrigin, toPlace));
        }

        return metaTiles;
    }

    /**
     * Returns a List of all valid positions which a Metatile could be placed.
     *
     * Uses a custom dynamic programming algorithm to find all valid positions in O(N)
     * with respect to the size of the room being populated
     */
    private List<(int, int)> FindValidPlacements(DecorGroup toPlace, (int, int, int, int)[,] traversalMap)
    {
        List<(int, int)> validPositions = new List<(int, int)>();

        // Positions are reverse-iterated as this places the found positions in the top-left corner
        for (int x = xSize - 1; x >= 0; x--)
        {
            for (int y = ySize - 1; y >= 0; y--)
            {
                var neighbourX = x < xSize - 1 ? traversalMap[x + 1, y] : (0, 0, 0, 0);
                var neighbourY = y < ySize - 1 ? traversalMap[x, y + 1] : (0, 0, 0, 0);

                if (occupancyMap[x, y])
                {
                    traversalMap[x, y] = (0, 0, 0, 0);
                    continue;
                }

                // If the tile is empty, generatively accumulate from neighbouring cells
                traversalMap[x, y] = (
                    neighbourX.Item1 + 1,
                    neighbourY.Item2 + 1,
                    neighbourX.Item1 + 1 >= toPlace.Width ? neighbourY.Item3 + 1 : 0,
                    neighbourY.Item2 + 1 >= toPlace.Height ? neighbourX.Item4 + 1 : 0
                );

                if (traversalMap[x, y].Item4 >= toPlace.Width && traversalMap[x, y].Item3 >= toPlace.Height)
                {
                    validPositions.Add((x, y));
                }
            }
        }

        return validPositions;
    }

    //TODO: remove before production!
    public string DEBUG_showOccupancyMap()
    {
        StringBuilder st = new StringBuilder();
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                st.Append(occupancyMap[x, y] ? '#' : '_');
            }
            st.Append(Environment.NewLine);
        }

        return st.ToString();
    }
}
