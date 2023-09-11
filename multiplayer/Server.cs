using LiteNetLib;
using LiteNetLib.Utils;

public class Server
{
    private static EventBasedNetListener? _netListener;
    private static NetManager? _netManager;
    static readonly int MAX_CONNECTIONS = 10;
    private static readonly NetPacketProcessor _netPacketProcessor = new();

    private static List<Token> tokens = new(){
        new("T1", 56, 200, (255, 50, 255), "Assets/Images/Assets/Images/Chest_Wood_Light_G_1x1.png"),
        new("T2", 234, 24, (50, 255, 255), "Assets/Images/Assets/Images/Chest_Wood_Light_G_1x1.png"),
        new("T3", 345, 12, (255, 255, 50), "Assets/Images/Assets/Images/Chest_Wood_Light_G_1x1.png"),
        new("T4", 87, 34, (255, 255, 255), "Assets/Images/Assets/Images/Chest_Wood_Light_G_1x1.png")
    };

    static Server()
    {
        _netPacketProcessor.RegisterNestedType(() => new MapData());
        _netPacketProcessor.RegisterNestedType(() => new Token());
        _netPacketProcessor.SubscribeReusable<Token, NetPeer>(OnTokenReceived);

    }

    private static void OnTokenReceived(Token t, NetPeer peer)
    {
        Console.WriteLine("Server received token: " + t.Name);
        Console.WriteLine("New X pos: " + t.X);
        Console.WriteLine("New Y pos: " + t.Y);
    }

    public static void RunServer(int PORT, string HOST_CODE)
    {
        _netListener = new EventBasedNetListener();
        _netManager = new NetManager(_netListener);
        Console.WriteLine("Running server");
        EventBasedNetListener listener = new();
        NetManager server = new(listener);
        server.Start(PORT);

        listener.ConnectionRequestEvent += request =>
        {
            if (server.ConnectedPeersCount < MAX_CONNECTIONS)
                request.AcceptIfKey(HOST_CODE);
            else
                request.Reject();
        };

        listener.PeerConnectedEvent += peer =>
        {
            Console.WriteLine("We got connection: {0}", peer.EndPoint);
            NetDataWriter writer = new();
            // Send map data to client
            MapData md = new(0, 200, 40, 0.8, "data/dungeon-theme/");
            _netPacketProcessor.Write(writer, md);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
            writer.Reset();
            // Then send all tokens
            foreach (Token t in tokens)
            {
                _netPacketProcessor.Write(writer, t);
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
                writer.Reset();
            }
        };

        listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
        {
            _netPacketProcessor.ReadAllPackets(dataReader, fromPeer);
            dataReader.Recycle();
        };

        for (int i = 0; i < 20; i++)
        {
            server.PollEvents();
            Thread.Sleep(1000);
        }

        Console.WriteLine("Stopping server");
        server.Stop();
    }
}