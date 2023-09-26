using System;
using map_generator.JsonLoading;
using map_generator.MapMaker;

namespace map_generator;

class Program
{
    static void Main()
    {
        //ImageSharpTest.ImageTest.Demo();

        Random rng = new Random();
        DataLoader.Init();
        DataLoader.Random = new(0);
        Console.WriteLine("Hello, World!");
        MapBuilder map = new MapBuilder(200, 40, rng, 0.8);
        map.setTheme($"{DataLoader.RootPath}/data/dungeon-theme/").initRoom().fillGaps().printMap();
    }
}
