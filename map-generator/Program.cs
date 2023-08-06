using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello, World!");
        MapBuilder map = new MapBuilder(5, 5);
        map.printMap();
    }
}
