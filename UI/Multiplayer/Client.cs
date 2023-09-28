using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
    private static Dictionary<int, string> FileNameIDs = new();

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
            // t.MoveToken(rand.Next(0, 100), rand.Next(0, 100));
            // NetDataWriter writer = new();
            // _netPacketProcessor.Write(writer, t);
            // peer.Send(writer, DeliveryMethod.ReliableOrdered);
            // writer.Reset();
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
                Console.WriteLine("Command ID: " + commandID);

                if (commandID == 1)
                {
                    int fileNameLength = dataReader.GetInt(); // Read the file name length
                    Console.WriteLine("File Name Length" + fileNameLength);

                    byte[] fileNameBytes = new byte[fileNameLength];

                    dataReader.GetBytes(fileNameBytes, 0, fileNameLength); // Read the file name bytes
                    string fileName = Encoding.UTF8.GetString(fileNameBytes); // Convert bytes to string
                    Console.WriteLine(fileName);

                    int availableBytes = dataReader.AvailableBytes;
                    Console.WriteLine("imageBytesLength: " + availableBytes);
                    byte[] buffer = new byte[availableBytes]; // Use the correct buffer size

                    dataReader.GetBytes(buffer, 0, availableBytes); // Read the available bytes

                    dataReader.GetRemainingBytes(); // Read the image data

                    Console.WriteLine($"Received image data with file name: {fileName} and length: {availableBytes}");

                    var documentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    const string appFolderName = ".worldcrucible";
                    const string receivedFolderName = "Received";

                    var receivedFolderPath = Path.Combine(documentsDirectory, appFolderName, receivedFolderName);

                    // Ensure that the target folder and its parent directories exist
                    Directory.CreateDirectory(receivedFolderPath);

                    // Use Path.Combine to create the complete file path
                    var filePath = Path.Combine(receivedFolderPath, fileName);

                    // Save the received data as an image file
                    File.WriteAllBytes(filePath, buffer);

                    Console.WriteLine($"Saved received image to: {filePath}");
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