using System;

class Program
{
    static void Main()
    {
        Random rng = new Random(16);
        Console.WriteLine("Hello, World!");
        MapBuilder map = new MapBuilder(15, 15, rng);
        map.printMap();
    }
}
