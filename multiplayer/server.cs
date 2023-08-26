using System;
using LiteNetLib;
using LiteNetLib.Utils;

public class Server
{
    static readonly int MAX_CONNECTIONS = 10;
    public static void RunServer(int port)
    {
        Console.WriteLine("Running server");
        EventBasedNetListener listener = new();
        NetManager server = new(listener);
        server.Start(port);

        listener.ConnectionRequestEvent += request =>
        {
            if (server.ConnectedPeersCount < MAX_CONNECTIONS)
                request.AcceptIfKey("SomeConnectionKey");
            else
                request.Reject();
        };

        listener.PeerConnectedEvent += peer =>
        {
            Console.WriteLine("We got connection: {0}", peer.EndPoint); // Show peer ip
            NetDataWriter writer = new();                               // Create writer class
            writer.Put("Hello client!");                                // Put some string
            peer.Send(writer, DeliveryMethod.ReliableOrdered);          // Send with reliability
        };

        for (int i = 0; i < 700; i++)
        {
            server.PollEvents();
            Thread.Sleep(15);
        }
        Console.WriteLine("Stopping server");
        server.Stop();
    }
}