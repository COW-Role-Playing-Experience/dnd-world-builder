using System.Data;

public class RoomTile
{
    private int x;
    private int y;
    private bool conn;
    private Dir direction;
    private bool empty;

    public enum Dir
    {
        NORTH,
        EAST,
        WEST,
        SOUTH,
        NONE
    }

    public RoomTile(int x, int y, bool isEmpty, bool conn = false, Dir direction = Dir.NONE)
    {
        this.x = x;
        this.y = y;
        this.conn = conn;
        this.direction = direction;
        this.empty = isEmpty;
    }

    public char getChar()
    {
        if (this.empty)
        {
            return 'O';
        }
        return 'X';
    }

    public bool isConn()
    {
        return this.conn;
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
}
