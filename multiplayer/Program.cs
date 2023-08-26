using System.Threading.Tasks;
using System.Threading;

class Multiplayer
{
    static readonly int PORT = 20500;

    static readonly List<Task> tasks = new();

    static void StartServer()
    {
        Console.WriteLine("Starting mock server");
        Task serverTask = Task.Factory.StartNew(() => Server.RunServer(PORT));
        tasks.Add(serverTask);
    }

    static void StartClients()
    {
        Console.WriteLine("Starting up 3 new client instances");
        for (int i = 0; i < 3; i++)
        {
            Task clientTask = Task.Factory.StartNew(() => Client.RunClient(PORT));
            tasks.Add(clientTask);
        }
    }

    static void Main()
    {
        StartServer();
        StartClients();
        Task.WaitAll(tasks.ToArray());

    }
}
