using System;
using System.Drawing;
using LiteNetLib.Utils;

[Serializable]
public class Token : INetSerializable
{
    public string? Name { get; set; }

    public float X { get; set; }

    public float Y { get; set; }

    private float LastX { get; set; }

    private float LastY { get; set; }

    private (int, int, int) _Colour { get; set; }

    private string? _ImgID { get; set; }

    public bool PlayerMoveable { get; set; }
    public bool PlayerVisible { get; set; }

    public Token()
    {

    }

    public Token(string name, float x, float y, (int, int, int) colour, string imgid, bool playermoveable, bool playervisible)
    {
        Name = name;
        X = x;
        Y = y;
        _Colour = colour;
        // TODO determine how we will get the id of the image (i.e path or something else)
        _ImgID = imgid;
        PlayerMoveable = playermoveable;
        PlayerVisible = playervisible;
    }

    public bool CheckMoved()
    {
        if (X == LastX && Y == LastY)
        {
            return false;
        }
        return true;
    }

    /*
    Calculate new x and y coords based on direction, if direction doesn't change, then don't update position.
    */
    public void MoveToken(float newX, float newY)
    {
        if (newX == X && newY == Y)
        {
            return;
        }
        LastX = X;
        LastY = Y;
        X = newX;
        Y = newY;
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Name);
        writer.Put(X);
        writer.Put(Y);
        writer.Put(_Colour.Item1);
        writer.Put(_Colour.Item2);
        writer.Put(_Colour.Item3);
        writer.Put(PlayerMoveable);
        writer.Put(_ImgID);
    }

    public void Deserialize(NetDataReader reader)
    {
        Name = reader.GetString();
        X = reader.GetFloat();
        Y = reader.GetFloat();
        _Colour = (reader.GetInt(), reader.GetInt(), reader.GetInt());
        PlayerMoveable = reader.GetBool();
        _ImgID = reader.GetString();
    }
}
