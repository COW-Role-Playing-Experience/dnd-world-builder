using System.Data;
using map_generator;
using map_generator.JsonLoading;

namespace map_generator.MapMaker;

public class RoomTile : ICloneable
{
    private int x;
    private int y;
    private Direction direction;
    private Image<Rgba32> texture;
    private bool noTexture;

    public RoomTile(int x, int y, string? texture, Direction direction = Direction.NONE)
    {
        this.x = x;
        this.y = y;
        this.direction = direction;
        this.texture = DataLoader.Textures.Get(texture ?? DataLoader.EMPTY);
        this.noTexture = texture == null;
    }

    public RoomTile(int x, int y, Image<Rgba32> texture, Direction direction = Direction.NONE)
    {
        this.x = x;
        this.y = y;
        this.direction = direction;
        this.texture = texture;
        this.noTexture = false;
    }

    public char getChar()
    {
        return this.noTexture ? '.' : 'X';
    }

    public bool isEmpty()
    {
        return noTexture;
    }

    public void setTexture(string? texture)
    {
        this.texture = DataLoader.Textures.Get(texture ?? DataLoader.EMPTY);
        this.noTexture = texture == null;
    }

    public int getX()
    {
        return this.x;
    }

    public int getY()
    {
        return this.y;
    }

    public Image<Rgba32> getTexture()
    {
        return texture;
    }

    public object Clone()
    {
        return new RoomTile(this.x, this.y, this.texture, this.direction);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashcode = 1430287;
            hashcode = hashcode * 7302013 ^ x;
            hashcode = hashcode * 7302013 ^ y;
            return hashcode;
        }
    }
}
