using System;

class Program
{
    static void Main()
    {
        Random rng = new Random(273465);
        Console.WriteLine("Hello, World!");
        MapBuilder map = new MapBuilder(50, 50, rng);
        map.printMap();
    }
}
