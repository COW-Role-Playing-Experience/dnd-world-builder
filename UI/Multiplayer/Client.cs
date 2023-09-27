using System;
using System.IO;
using System.Threading.Tasks;
using LiteNetLib;
using LiteNetLib.Utils;
using map_generator.MapMaker;
using UI.Classes;

public class Client
{
    private static readonly NetPacketProcessor _netPacketProcessor = new();
    private static NetManager _client;
    private static bool _canMove;
    private static bool _running = true;

    static Client()
    {
        // _netPacketProcessor.RegisterNestedType(() => new MapData());
        // _netPacketProcessor.SubscribeReusable<MapData, NetPeer>(OnMapDataReceived);
        // _netPacketProcessor.RegisterNestedType(() => new Token());
        // _netPacketProcessor.SubscribeReusable<Token, NetPeer>(OnTokenReceived);
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

        if (_canMove)
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
    public static void RunClient(int port, string HostCode)
    {
        Console.WriteLine("Running client");
        EventBasedNetListener listener = new();
        _client = new(listener);
        _client.Start();
        _client.Connect("localhost", port, HostCode);
        _running = true;

        listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
        {
            if (deliveryMethod == DeliveryMethod.ReliableOrdered)
            {
                int commandID = dataReader.GetInt(); // Read id

                if (commandID == 1)
                {
                    int availableBytes = dataReader.AvailableBytes; // Get the available bytes

                    byte[] buffer = new byte[availableBytes]; // Use the correct buffer size

                    dataReader.GetBytes(buffer, 0, availableBytes); // Read the available bytes

                    Console.WriteLine("Received image data with length: " + availableBytes);

                    var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    const string appFolderName = ".worldcrucible";
                    const string sentFolderName = "Sent";

                    var tokensFolderPath = Path.Combine(documentsDirectory, appFolderName, sentFolderName);

                    // Ensure that the target folder and its parent directories exist
                    Directory.CreateDirectory(tokensFolderPath);

                    // Use Path.Combine to create the complete file path
                    var filePath = Path.Combine(tokensFolderPath, "received_image.png");

                    // Save the received data as an image file
                    File.WriteAllBytes(filePath, buffer);

                    Console.WriteLine("Saved received image to: " + filePath);
                }
            }

            dataReader.Recycle();
        };


        while (_running)
        {
            _client.PollEvents();
            System.Threading.Thread.Sleep(1000);
        }

        ;

        // Poll the event in parallel
        // Task.Factory.StartNew(PollEvents);
    }


    private static void PollEvents()
    {
        while (_running)
        {
            _client.PollEvents();
            System.Threading.Thread.Sleep(1000);
        }

        ;
    }

    public static void StopClient()
    {
        Console.WriteLine("Stopping client");
        _running = false;
        _client.Stop();
    }
}