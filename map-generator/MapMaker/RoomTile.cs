using System.Data;
using map_generator;
using map_generator.JsonLoading;

namespace map_generator.MapMaker;

public class RoomTile : ICloneable
{
    private int x;
    private int y;
    private Direction direction;
    private string? texture;



    public RoomTile(int x, int y, string? texture, Direction direction = Direction.NONE)
    {
        this.x = x;
        this.y = y;
        this.direction = direction;
        this.texture = texture;
    }

    public char getChar()
    {
        return this.texture == null ? '.' : 'X';
    }

    public bool isEmpty()
    {
        return this.texture == null;
    }

    public void setTexture(string? texture)
    {
        this.texture = texture;
    }

    public int getX()
    {
        return this.x;
    }

    public int getY()
    {
        return this.y;
    }

    public Image<Rgba32>? getTexture()
    {
        return DataLoader.Textures[texture ?? DataLoader.EMPTY];
    }

    public string? getTextureName()
    {
        return texture;
    }

    public object Clone()
    {
        return new RoomTile(this.x, this.y, this.texture, this.direction);
    }
}
