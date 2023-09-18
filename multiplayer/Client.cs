using LiteNetLib;
using LiteNetLib.Utils;
using map_generator.MapMaker;

public class Client
{
    private static readonly NetPacketProcessor _netPacketProcessor = new();

    private static readonly string HOST_CODE = "1234";

    private static bool CanMove;

    static Client()
    {
        _netPacketProcessor.RegisterNestedType(() => new MapData());
        _netPacketProcessor.SubscribeReusable<MapData, NetPeer>(OnMapDataReceived);
        _netPacketProcessor.RegisterNestedType(() => new Token());
        _netPacketProcessor.SubscribeReusable<Token, NetPeer>(OnTokenReceived);
    }

    /// <summary>
    /// Receives map data from the server and then calls the map generate method to build the map. This is done once after the player has joined the game.
    /// </summary>
    private static void OnMapDataReceived(MapData md, NetPeer peer)
    {
        Console.WriteLine("Client " + peer.Id + " received map data with theme: " + md.Theme);
        // Call map generation
        MapBuilder map = new(md.XSize, md.YSize, new Random(md.Seed), md.ExpectedPopulation);
        map.setTheme(md.Theme).initRoom().fillGaps().printMap();
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
        if (CanMove)
        {
            t.MoveToken(rand.Next(0, 100), rand.Next(0, 100));
            NetDataWriter writer = new();
            _netPacketProcessor.Write(writer, t);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
            writer.Reset();
        }
    }

    /// <summary>
    /// Executes the client which is done through the player view in the UI
    /// </summary>
    public static void RunClient(int port)
    {
        Console.WriteLine("Running client");
        EventBasedNetListener listener = new();
        NetManager client = new(listener);
        client.Start();
        client.Connect("localhost", port, HOST_CODE);

        listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
        {
            bool OnWaitList = dataReader.GetBool();
            if (!OnWaitList)
            {
                CanMove = dataReader.GetBool();
                _netPacketProcessor.ReadAllPackets(dataReader, fromPeer);
            }
            dataReader.Recycle();
        };

        // Run for a minute
        for (int i = 0; i < 60; i++)
        {
            client.PollEvents();
            Thread.Sleep(1000);
        }

        Console.WriteLine("Stopping client");
        client.Stop();
    }
}