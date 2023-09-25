using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using UI.Classes;
using UI.ViewModels;

public class Server
{
    public static DmViewModel ViewModel { get; set; }
    private static EventBasedNetListener? _netListener;
    private static NetManager? _netManager;
    private static NetManager _server;
    private const int MaxConnections = 1;//10 <- temporary to test the connections and waitlist;
    static bool _clientsCanMove = true;
    static bool _toggleFOW = false;
    private static bool _running = true;
    private static readonly NetPacketProcessor _netPacketProcessor = new();
    private static readonly Queue<int> WaitList = new();

    static Server()
    {
        _netPacketProcessor.RegisterNestedType(() => new MapData());
        _netPacketProcessor.RegisterNestedType(() => new Token());
        // _netPacketProcessor.SubscribeReusable<Token, NetPeer>(OnTokenReceived);

    }

    /// <summary>
    /// Receives token data from the clients and updates player and/or token data via the DM UI
    /// </summary>
    private static void OnTokenReceived(Token t, NetPeer peer)
    {
        Console.WriteLine("Server received token: " + t.Name);
        Console.WriteLine("New X pos: " + t.XLoc);
        Console.WriteLine("New Y pos: " + t.YLoc);
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
        t.MoveToken(t.XLoc + rand.Next(0, 100), t.YLoc + rand.Next(0, 100));
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
        peer.Send(writer, DeliveryMethod.ReliableOrdered);
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
            Console.WriteLine("Send map");
            // Send map data to client
            MapData md = new(0, 200, 40, 0.8, "data/dungeon-theme/");
            _netPacketProcessor.Write(writer, md);
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
            // Console.WriteLine("Send tokens");
            // // Then send all tokens
            // foreach (Token t in ViewModel.TokensOnCanvas)
            // {
            //     _netPacketProcessor.Write(writer, t);
            //     peer.Send(writer, DeliveryMethod.ReliableOrdered);
            // writer.Reset();
            // }
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
        _server = new(listener);
        _server.Start(PORT);
        _running = true;

        listener.ConnectionRequestEvent += request =>
        {
            request.AcceptIfKey(HOST_CODE);
        };

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

        while (_running)
        {
            _server.PollEvents();
            System.Threading.Thread.Sleep(1000);
        };

        // Poll the event in parallel
        // Task.Factory.StartNew(PollEvents);
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