using System.Data;
using map_generator;

public class RoomTile : ICloneable
{
    private int x;
    private int y;
    private Direction direction;
    private bool empty;



    public RoomTile(int x, int y, bool isEmpty, Direction direction = Direction.NONE)
    {
        this.x = x;
        this.y = y;
        this.direction = direction;
        this.empty = isEmpty;
    }

    public char getChar()
    {
        if (this.empty)
        {
            return '.';
        }
        return 'X';
    }

    public bool isEmpty()
    {
        return this.empty;
    }

    public void setEmpty(bool empty)
    {
        this.empty = empty;
    }

    public int getX()
    {
        return this.x;
    }

    public int getY()
    {
        return this.y;
    }

    public object Clone()
    {
        return new RoomTile(this.x, this.y, this.empty, this.direction);
    }
}
