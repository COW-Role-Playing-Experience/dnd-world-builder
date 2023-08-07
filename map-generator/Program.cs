using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello, World!");
        MapBuilder map = new MapBuilder(25, 25);
        map.printMap();
    }
}
