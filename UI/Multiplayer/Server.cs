using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using UI.Classes;
using UI.ViewModels;

public class Server
{
    public static DmViewModel ViewModel { get; set; }
    private static EventBasedNetListener? _netListener;
    private static NetManager _server;
    private const int MaxConnections = 1; //10 <- temporary to test the connections and waitlist;
    static bool _clientsCanMove = true;
    static bool _toggleFOW = false;
    private static bool _running = true;
    private static readonly NetPacketProcessor _netPacketProcessor = new();
    private static readonly Queue<int> WaitList = new();

    static Server()
    {
        _netPacketProcessor.RegisterNestedType(() => new MapData());
        // _netPacketProcessor.RegisterNestedType(() => new Token());
        // _netPacketProcessor.SubscribeReusable<Token, NetPeer>(OnTokenReceived);
    }

    /// <summary>
    /// Receives token data from the clients and updates player and/or token data via the DM UI
    /// </summary>
    private static void OnTokenReceived(Token t, NetPeer peer)
    {
        Console.WriteLine("Server received token: " + t.Name);
        // Console.WriteLine("New X pos: " + t.XLoc);
        // Console.WriteLine("New Y pos: " + t.YLoc);
        NetDataWriter writer = new();
        if (_clientsCanMove)
        {
            writer.Put(true);
        }
        else
        {
            writer.Put(false);
        }

        var rand = new Random();
        // t.MoveToken(t.X + rand.Next(0, 100), t.Y + rand.Next(0, 100));
        // _netPacketProcessor.Write(writer, t);
        // peer.Send(writer, DeliveryMethod.ReliableOrdered);
        writer.Reset();
    }

    // private static List<TokenData> ConvertTokens()
    // {
    //     List<TokenData> tokenDataList = new();
    //     foreach (Token token in ViewModel.TokensOnCanvas)
    //     {
    //         TokenData tokenData = new(token.Name, token.OnCavas, token.XLoc, token.YLoc, token.PlayerMoveable, token.PlayerVisible);
    //         tokenDataList.Add(tokenData);
    //     }
    //     return tokenDataList;
    // }

    /// <summary>
    /// Allows the client (player) to join the game and send out map and token data if the player is not on the waitlist.
    /// </summary>
    private static void JoinGame(NetPeer peer, bool OnWaitList)
    {
        NetDataWriter writer = new();
        writer.Put(OnWaitList);
        if (!OnWaitList)
        {
            writer.Put(_clientsCanMove);
            writer = SendMapData(peer, writer);
            writer = SendImageFile(peer, "Images/Decor/Barrel/Barrel_Large_Wood_Ashen_A_Side_3x3.png", writer);
        }
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
        writer.Reset();
    }

    /// <summary>
    /// Executes the server which is done through the DM view in the UI
    /// </summary>
    public static void RunServer(int PORT, string HOST_CODE)
    {
        _netListener = new EventBasedNetListener();
        _server = new NetManager(_netListener);
        Console.WriteLine("Running server");
        EventBasedNetListener listener = new();
        _server = new(listener);
        _server.Start(PORT);
        _running = true;

        listener.ConnectionRequestEvent += request => { request.AcceptIfKey(HOST_CODE); };

        listener.PeerConnectedEvent += peer =>
        {
            if (_server.ConnectedPeersCount <= MaxConnections)
            {
                // Connect client to game and add to the list of connected clients
                Console.WriteLine("Connection: {0} with ID: {1} joined the game", peer.EndPoint, peer.Id);
                JoinGame(peer, false);
                ViewModel.PlayerCount = GetPlayerCount();
            }
            else
            {
                // Put the client on a waitlist
                Console.WriteLine("Connnection: {0} with ID: {1} added to waitlist", peer.EndPoint, peer.Id);
                JoinGame(peer, true);
                WaitList.Enqueue(peer.Id);
                ViewModel.WaitlistCount = WaitList.Count;
            }

            Console.WriteLine("Connected: " + _server.ConnectedPeersCount);
            Console.WriteLine(string.Format("Waitlist: ({0}).", string.Join(", ", WaitList)));
        };

        listener.PeerDisconnectedEvent += (peer, dcInfo) =>
        {
            Console.WriteLine("Connnection: {0} with ID: {1} disconnected", peer.EndPoint, peer.Id);
            if (WaitList.Count != 0)
            {
                int ClientID = WaitList.Dequeue();
                NetPeer PeerFromWaitlist = _server.GetPeerById(ClientID);
                Console.WriteLine("Connection: {0} with ID: {1} removed from waitlist and joined the game",
                    PeerFromWaitlist.EndPoint, PeerFromWaitlist.Id);
                JoinGame(PeerFromWaitlist, false);
            }

            Console.WriteLine(string.Format("Waitlist: ({0}).", string.Join(", ", WaitList)));
        };

        listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
        {
            _netPacketProcessor.ReadAllPackets(dataReader, fromPeer);
            dataReader.Recycle();
        };

        // Poll the event in parallel
        Task.Factory.StartNew(PollEvents);
    }

    private static NetDataWriter SendMapData(NetPeer peer, NetDataWriter writer)
    {
        Console.WriteLine("Sending map data");
        // Send command id for map data
        // List: 0 - map, 1 images, 2 - tokens, 3 - FOW
        // Send map data to client
        MapData md = new(0, 200, 40, 0.8, "data/dungeon-theme/");
        _netPacketProcessor.Write(writer, md);
        return writer;
    }

    private static NetDataWriter SendImageFile(NetPeer peer, string imagePath, NetDataWriter writer)
    {
        try
        {
            if (File.Exists(imagePath))
            {
                // Read the image file into bytes
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                Console.WriteLine("Image bytes: " + imageBytes.Length);

                // Get the file name from the image path
                string fileName = Path.GetFileName(imagePath);

                // Serialize the file name to bytes
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);

                ImageData imgData = new(fileNameBytes.Length, fileNameBytes, imageBytes.Length, imageBytes);
                _netPacketProcessor.Write(writer, imgData);

                Console.WriteLine($"Sent image file '{imagePath}' to client {peer.EndPoint}");
                return writer;
            }
            else
            {
                Console.WriteLine($"Image file '{imagePath}' not found.");
                return writer;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending image file: {ex.Message}");
            return writer;
        }
    }

    private static void SendTokenDataWithID(NetPeer peer)
    {
        NetDataWriter writer = new();
        // Then send all tokens
        foreach (Token t in ViewModel.TokensOnCanvas)//ConvertTokens())
        {
            // _netPacketProcessor.Write(writer, t);
            // peer.Send(writer, DeliveryMethod.ReliableOrdered);
            // writer.Reset();
        }
    }

    private static void PollEvents()
    {
        while (_running)
        {
            _server.PollEvents();
            System.Threading.Thread.Sleep(1000);
        };
    }

    public static void StopServer()
    {
        Console.WriteLine("Stopping server");
        _running = false;
        _server.Stop();
    }

    public static int GetPlayerCount()
    {
        if (_server == null) return 0;
        return _server.ConnectedPeersCount;
    }
}