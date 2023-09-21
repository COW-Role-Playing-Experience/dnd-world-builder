using System;
using LiteNetLib.Utils;

[Serializable]
public class MapData : INetSerializable
{
    public int Seed { get; set; }

    public int XSize { get; set; }

    public int YSize { get; set; }

    public double ExpectedPopulation { get; set; }

    public string Theme { get; set; }

    public MapData()
    {

    }

    public MapData(int Seed, int XSize, int YSize, double ExpectedPopulation, string Theme)
    {
        this.Seed = Seed;
        this.XSize = XSize;
        this.YSize = YSize;
        this.ExpectedPopulation = ExpectedPopulation;
        this.Theme = Theme;
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Seed);
        writer.Put(XSize);
        writer.Put(YSize);
        writer.Put(ExpectedPopulation);
        writer.Put(Theme);
    }

    public void Deserialize(NetDataReader reader)
    {
        Seed = reader.GetInt();
        XSize = reader.GetInt();
        YSize = reader.GetInt();
        ExpectedPopulation = reader.GetDouble();
        Theme = reader.GetString();
    }
}