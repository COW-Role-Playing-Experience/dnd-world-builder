using System;
using LiteNetLib;
using LiteNetLib.Utils;

public class Client
{
    public void runClient()
    {
        Console.WriteLine("Running client");
        EventBasedNetListener listener = new EventBasedNetListener();
        NetManager client = new NetManager(listener);
        client.Start();
        client.Connect("localhost" /* host ip or name */, 20500 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
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