using System.Threading.Tasks;
using System.Threading;

class Multiplayer
{
    static readonly int PORT = 20500;

    static readonly string HOST_CODE = "1234";

    static readonly List<Task> tasks = new();

    static void StartServer()
    {
        Console.WriteLine("Starting mock server");
        Task serverTask = Task.Factory.StartNew(() => Server.RunServer(PORT, HOST_CODE));
        tasks.Add(serverTask);
    }

    static void StartClients()
    {
        Console.WriteLine("Starting up 3 new client instances");
        for (int i = 0; i < 3; i++) // 15 seconds to load 3 new instances
        {
            Task clientTask = Task.Factory.StartNew(() => Client.RunClient(PORT));
            tasks.Add(clientTask);
            Thread.Sleep(5000);
        }
    }

    static void Main()
    {
        StartServer();
        StartClients();
        Task.WaitAll(tasks.ToArray());
    }
}