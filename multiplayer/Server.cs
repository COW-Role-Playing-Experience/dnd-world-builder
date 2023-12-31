using LiteNetLib;
using LiteNetLib.Utils;

public class Server
{
    private static EventBasedNetListener? _netListener;
    private static NetManager? _netManager;
    private const int MaxConnections = 10;
    static bool _clientsCanMove = true;
    static bool _toggleFOW = false;
    private static readonly NetPacketProcessor _netPacketProcessor = new();
    private static readonly Queue<int> WaitList = new();
    // List of _tokens currently in the game
    private static List<Token> _tokens = new()
    {
        new("T1", 56, 200, (255, 50, 255), "Assets/Images/Assets/Images/Chest_Wood_Light_G_1x1.png", true, true),
        new("T2", 234, 24, (50, 255, 255), "Assets/Images/Assets/Images/Chest_Wood_Light_G_1x1.png", true, true),
        new("T3", 345, 12, (255, 255, 50), "Assets/Images/Assets/Images/Chest_Wood_Light_G_1x1.png", false, true),
        new("T4", 87, 34, (255, 255, 255), "Assets/Images/Assets/Images/Chest_Wood_Light_G_1x1.png", false, true)
    };

    static Server()
    {
        _netPacketProcessor.RegisterNestedType(() => new MapData());
        _netPacketProcessor.RegisterNestedType(() => new Token());
        _netPacketProcessor.SubscribeReusable<Token, NetPeer>(OnTokenReceived);

    }

    /// <summary>
    /// Receives token data from the clients and updates player and/or token data via the DM UI
    /// </summary>
    private static void OnTokenReceived(Token t, NetPeer peer)
    {
        Console.WriteLine("Server received token: " + t.Name);
        Console.WriteLine("New X pos: " + t.X);
        Console.WriteLine("New Y pos: " + t.Y);
        NetDataWriter writer = new();
        if (_clientsCanMove)
        {
            writer.Put(true);
            // t.PlayerMoveable = false;
        }
        else
        {
            writer.Put(false);
        }
        var rand = new Random();
        t.MoveToken(t.X + rand.Next(0, 100), t.Y + rand.Next(0, 100));
        _netPacketProcessor.Write(writer, t);
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
        writer.Reset();
    }

    /// <summary>
    /// Allows the client (player) to join the game and send out map and token data if the player is not on the waitlist.
    /// </summary>
    private static void JoinGame(NetPeer peer, bool OnWaitList)
    {
        NetDataWriter writer = new();
        writer.Put(OnWaitList);
        // peer.Send(writer, DeliveryMethod.ReliableOrdered);
        if (!OnWaitList)
        {
            if (_clientsCanMove)
            {
                writer.Put(true);
                // t.PlayerMoveable = false;
            }
            else
            {
                writer.Put(false);
            }
            // Send map data to client
            MapData md = new(0, 200, 40, 0.8, "data/dungeon-theme/");
            _netPacketProcessor.Write(writer, md);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
            // Then send all tokens
            foreach (Token t in _tokens)
            {
                _netPacketProcessor.Write(writer, t);
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            }
        }
        // Clear the NetDataWriter buffer after sending everything
        writer.Reset();
    }

    /// <summary>
    /// Executes the server which is done through the DM view in the UI
    /// </summary>
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
            request.AcceptIfKey(HOST_CODE);
        };

        listener.PeerConnectedEvent += peer =>
        {
            if (server.ConnectedPeersCount <= MaxConnections)
            {
                // Connect client to game and add to the list of connected clients
                Console.WriteLine("Connection: {0} with ID: {1} joined the game", peer.EndPoint, peer.Id);
                JoinGame(peer, false);
            }
            else
            {
                // Put the client on a waitlist
                Console.WriteLine("Connnection: {0} with ID: {1} added to waitlist", peer.EndPoint, peer.Id);
                JoinGame(peer, true);
                WaitList.Enqueue(peer.Id);
            }

            Console.WriteLine("Connected: " + server.ConnectedPeersCount);
            Console.WriteLine(string.Format("Waitlist: ({0}).", string.Join(", ", WaitList)));
        };

        listener.PeerDisconnectedEvent += (peer, dcInfo) =>
        {
            Console.WriteLine("Connnection: {0} with ID: {1} disconnected", peer.EndPoint, peer.Id);
            if (WaitList.Count != 0)
            {
                int ClientID = WaitList.Dequeue();
                NetPeer PeerFromWaitlist = server.GetPeerById(ClientID);
                Console.WriteLine("Connection: {0} with ID: {1} removed from waitlist and joined the game", PeerFromWaitlist.EndPoint, PeerFromWaitlist.Id);
                JoinGame(PeerFromWaitlist, false);
            }
            Console.WriteLine(string.Format("Waitlist: ({0}).", string.Join(", ", WaitList)));
        };

        listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
        {
            _netPacketProcessor.ReadAllPackets(dataReader, fromPeer);
            dataReader.Recycle();
        };

        // Run for 3 minutes
        for (int i = 0; i < (60 * 3); i++)
        {
            server.PollEvents();
            Thread.Sleep(1000);
        }

        Console.WriteLine("Stopping server");
        server.Stop();
    }
}