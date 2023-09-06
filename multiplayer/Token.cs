using System;
using System.Drawing;
using LiteNetLib.Utils;

[Serializable]
public class Token : INetSerializable
{
    public string? Name { get; set; }

    public float X { get; set; }

    public float Y { get; set; }

    private (int, int, int) _Colour { get; set; }

    private string? _ImgID { get; set; }

    public Token()
    {

    }

    public Token(string name, float x, float y, (int, int, int) colour, string imgid)
    {
        this.Name = name;
        this.X = x;
        this.Y = y;
        this._Colour = colour;
        // TODO determine how we will get the id of the image (i.e path or something else)
        this._ImgID = imgid;
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
        writer.Put(_ImgID);
    }

    public void Deserialize(NetDataReader reader)
    {
        Name = reader.GetString();
        X = reader.GetFloat();
        Y = reader.GetFloat();
        _Colour = (reader.GetInt(), reader.GetInt(), reader.GetInt());
        _ImgID = reader.GetString();
    }
}
