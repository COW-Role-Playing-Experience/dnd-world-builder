using System;
using LiteNetLib.Utils;

[Serializable]
public class ImageData : INetSerializable
{
    public int FileNameBytesLen { get; set; }
    public byte[] FileNameBytes { get; set; }
    public int ImageBytesLen { get; set; }
    public byte[] ImageBytes { get; set; }

    public ImageData()
    {

    }

    public ImageData(int fileNameBytesLen, byte[] fileNameBytes, int imageBytesLen, byte[] imageBytes)
    {
        this.FileNameBytesLen = fileNameBytesLen;
        this.FileNameBytes = fileNameBytes;
        this.ImageBytesLen = imageBytesLen;
        this.ImageBytes = imageBytes;
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(FileNameBytesLen);
        writer.Put(FileNameBytes);
        writer.Put(ImageBytesLen);
        writer.Put(ImageBytes);
    }

    public void Deserialize(NetDataReader reader)
    {
        FileNameBytesLen = reader.GetInt();
        reader.GetBytes(FileNameBytes, 0, FileNameBytesLen);
        ImageBytesLen = reader.GetInt();
        reader.GetBytes(ImageBytes, 0, ImageBytesLen);
    }
}