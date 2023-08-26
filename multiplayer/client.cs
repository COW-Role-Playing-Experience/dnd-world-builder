using System;
using LiteNetLib;
using LiteNetLib.Utils;

public class Client
{
    public static void RunClient(int port)
    {
        Console.WriteLine("Running client");
        EventBasedNetListener listener = new();
        NetManager client = new(listener);
        client.Start();
        client.Connect("localhost", port, "SomeConnectionKey" /* text key or NetDataWriter */);
        listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
        {
            Console.WriteLine("We got: {0}", dataReader.GetString(100 /* max length of string */));
            dataReader.Recycle();
        };

        for (int i = 0; i < 600; i++)
        {
            client.PollEvents();
            Thread.Sleep(15);
        }
        Console.WriteLine("Stopping client");
        client.Stop();
    }
}