using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UI.Classes;

public class Client
{
    private static readonly NetPacketProcessor _netPacketProcessor = new();
    private static NetManager _client;
    private static bool _canMove;

    static Client()
    {
        _netPacketProcessor.RegisterNestedType(() => new MapData());
        // _netPacketProcessor.SubscribeReusable<MapData, NetPeer>(OnMapDataReceived);
        _netPacketProcessor.RegisterNestedType(() => new Token());
        // _netPacketProcessor.SubscribeReusable<Token, NetPeer>(OnTokenReceived);
    }

    /// <summary>
    /// Receives map data from the server and then calls the map generate method to build the map. This is done once after the player has joined the game.
    /// </summary>
    private static void OnMapDataReceived(MapData md, NetPeer peer)
    {
        Console.WriteLine("Client " + peer.Id + " received map data with theme: " + md.Theme);
        // Call map generation
        // MapBuilder map = new(md.XSize, md.YSize, new Random(md.Seed), md.ExpectedPopulation);
        // map.setTheme(md.Theme).initRoom().fillGaps().printMap();
    }

    /// <summary>
    /// Receives token data from the server and calls the draw method for each token. Player can move tokens if allowed by the server (DM).
    /// </summary>
    private static void OnTokenReceived(Token t, NetPeer peer)
    {
        Console.WriteLine("Client " + peer.Id + " received token: " + t.Name);
        var rand = new Random();
        //Code to draw token
        if (!t.CheckMoved())
        {
            Console.WriteLine("Token " + t.Name + "'s position hasn't changed");
            return;
        }
        if (!t.PlayerMoveable)
        {
            Console.WriteLine("Token " + t.Name + " is not moveable by player");
            return;
        }
        // if (_canMove)
        // {
        //     t.MoveToken(rand.Next(0, 100), rand.Next(0, 100));
        //     NetDataWriter writer = new();
        //     _netPacketProcessor.Write(writer, t);
        //     peer.Send(writer, DeliveryMethod.ReliableOrdered);
        //     writer.Reset();
        // }
    }

    /// <summary>
    /// Executes the client which is done through the player view in the UI
    /// </summary>
    public static void RunClient(int port, string HostCode)
    {
        Console.WriteLine("Running client");
        EventBasedNetListener listener = new();
        _client = new(listener);
        _client.Start();
        _client.Connect("localhost", port, HostCode);

        listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
        {
            var OnWaitList = dataReader.GetBool();
            if (!OnWaitList)
            {
                _canMove = dataReader.GetBool();
                _netPacketProcessor.ReadAllPackets(dataReader, fromPeer);
            }
            dataReader.Recycle();
        };
    }

    public static void StopClient()
    {
        Console.WriteLine("Stopping client");
        _client.Stop();
    }
}