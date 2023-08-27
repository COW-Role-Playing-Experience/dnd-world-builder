using System;

class Program
{
    static void Main()
    {
        Random rng = new Random();
        Console.WriteLine("Hello, World!");
        MapBuilder map = new MapBuilder(50, 50, rng);
        map.setTheme("data/dungeon-theme/").initRoom();
    }
}
