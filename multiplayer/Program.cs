using System.Threading.Tasks;
using System.Threading;

class Multiplayer
{
    static void server()
    {
        Console.WriteLine("Starting mock server");
        Server s = new();
        s.runServer();
    }

    static void client()
    {
        Console.WriteLine("Starting up 3 new client instances");
        for (int i = 0; i < 3; i++)
        {
            Client c = new();
            c.runClient();
        }
    }

    static void Main(string[] args)
    {
        Task task1 = Task.Factory.StartNew(() => server());
        Task task2 = Task.Factory.StartNew(() => client());
        Task.WaitAll(task1, task2);

    }
}
