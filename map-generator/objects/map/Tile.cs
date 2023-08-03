namespace map_generator;

public class Tile
{
    private string floorAssetPath;
    private int floorLayer;
    private bool hasDecor;
    private char tileChar;

    public Tile()
    {
        tileChar = 'X';
    }

    public char getTileChar()
    {
        return this.tileChar;
    }

}
