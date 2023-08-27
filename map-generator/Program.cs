﻿using System;

class Program
{
    static void Main()
    {
        Random rng = new Random();
        Console.WriteLine("Hello, World!");
        MapBuilder map = new MapBuilder(200, 40, rng, 0.8);
        map.setTheme("data/dungeon-theme/").initRoom().fillGaps().printMap();
    }
}
