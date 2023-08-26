using System;
using LiteNetLib;
using LiteNetLib.Utils;

public class Server
{
    public void runServer()
    {
        Console.WriteLine("Running server");
        EventBasedNetListener listener = new EventBasedNetListener();
        NetManager server = new NetManager(listener);
        server.Start(20500 /* port */);

        listener.ConnectionRequestEvent += request =>
        {
            if (server.ConnectedPeersCount < 10 /* max connections */)
                request.AcceptIfKey("SomeConnectionKey");
            else
                request.Reject();
        };

        listener.PeerConnectedEvent += peer =>
        {
            Console.WriteLine("We got connection: {0}", peer.EndPoint); // Show peer ip
            NetDataWriter writer = new NetDataWriter();                 // Create writer class
            writer.Put("Hello client!");                                // Put some string
            peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
        };

        for (int i = 0; i < 1800; i++)
        {
            server.PollEvents();
            Thread.Sleep(15);
        }
        Console.WriteLine("Stopping server");
        server.Stop();
    }
}