using System.Threading.Tasks;
using System.Threading;

class Multiplayer
{
    static readonly int PORT = 20500;

    static readonly string HOST_CODE = "1234";

    static readonly List<Task> tasks = new();

    /*
        A mock server for testing
    */
    static void StartServer()
    {
        Console.WriteLine("Starting mock server");
        Task serverTask = Task.Factory.StartNew(() => Server.RunServer(PORT, HOST_CODE));
        tasks.Add(serverTask);
    }

    /*
        Starts 15 new client instances for testing
    */
    static void StartClients()
    {
        Console.WriteLine("Starting up 15 new client instances");
        for (int i = 0; i < 15; i++)
        {
            Task clientTask = Task.Factory.StartNew(() => Client.RunClient(PORT));
            tasks.Add(clientTask);
            // Start a new client every sec
            Thread.Sleep(1000);
        }
    }

    static void Main()
    {
        StartServer();
        StartClients();
        Task.WaitAll(tasks.ToArray());
    }
}