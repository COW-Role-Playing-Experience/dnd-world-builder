using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System;

class Multiplayer
{

    static readonly List<Task> tasks = new();

    /*
        A mock server for testing
    */
    static void StartServer(int Port, string HostCode)
    {
        Console.WriteLine("Running server");
        Task serverTask = Task.Factory.StartNew(() => Server.RunServer(Port, HostCode));
        tasks.Add(serverTask);
    }

    static void StartNewClient(int Port, string HostCode)
    {
        Console.WriteLine("Starting a new client instance");
        Task clientTask = Task.Factory.StartNew(() => Client.RunClient(Port, HostCode));
        tasks.Add(clientTask);
    }

    /*
        Starts 15 new client instances for testing
    */
    static void StartMockClients(int Port, string HostCode)
    {
        Console.WriteLine("Starting up 15 new client instances");
        for (int i = 0; i < 15; i++)
        {
            Task clientTask = Task.Factory.StartNew(() => Client.RunClient(Port, HostCode));
            tasks.Add(clientTask);
            // Start a new client every sec
            Thread.Sleep(1000);
        }
    }
}