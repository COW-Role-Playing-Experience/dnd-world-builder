using System;
using LiteNetLib;
using LiteNetLib.Utils;

public class Client
{
    private static readonly NetPacketProcessor _netPacketProcessor = new();

    private static readonly string HOST_CODE = "1234";

    static Client()
    {
        _netPacketProcessor.RegisterNestedType(() => new Token());
        _netPacketProcessor.SubscribeReusable<Token, NetPeer>(OnTokenReceived);
    }

    private static void OnTokenReceived(Token t, NetPeer peer)
    {
        Console.WriteLine("Client received token: " + t.Name);
        var rand = new Random();
        t.MoveToken(rand.Next(), rand.Next());
        NetDataWriter writer = new();
        _netPacketProcessor.Write(writer, t);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
        writer.Reset();
    }

    public static void RunClient(int port)
    {
        Console.WriteLine("Running client");
        EventBasedNetListener listener = new();
        NetManager client = new(listener);
        client.Start();
        client.Connect("localhost", port, HOST_CODE);

        listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
        {
            _netPacketProcessor.ReadAllPackets(dataReader, fromPeer);
            dataReader.Recycle();
        };

        for (int i = 0; i < 5; i++)
        {
            client.PollEvents();
            Thread.Sleep(1000);
        }

        Console.WriteLine("Stopping client");
        client.Stop();
    }
}