using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using map_generator.JsonLoading;
using map_generator.MapMaker;
using map_generator.RenderPipeline;

namespace UI.Classes;

public static class MapHandler
{
    public static int MapSeed { set; get; }
    public static AvaloniaRenderPipeline Pipeline { set; get; }
    public static WriteableBitmap Buffer { set; get; }

    public static MapBuilder map { set; get; }

    public static String Theme { set; get; }

    public static void RebindBitmap(WriteableBitmap buffer)
    {
        Buffer = buffer;
        Pipeline.RebindBitmap(Buffer);
    }

    public static void ClearBitmap()
    {
        RebindBitmap(new WriteableBitmap(Buffer.PixelSize, Buffer.Dpi, Buffer.Format));
    }

    public static void GenerateMap(Image mapImage)
    {
        ClearBitmap();
        Random rng = new Random(MapSeed);
        DataLoader.Random = rng;
        int xSize = 200;
        int ySize = 40;
        MapBuilder map = new MapBuilder(xSize, ySize, rng, 0.8);
        map.setTheme($"{DataLoader.RootPath}/data/" + Theme + "-theme/").initRoom();
        Console.WriteLine(Theme);
        Pipeline.RebindBuilder(map); //bind the finished map to the renderer
        Pipeline.Render(xSize / 2.0f, ySize / 2.0f, 1); //call once with the default to update bitmap
        mapImage.Source = Buffer;
        MapHandler.map = map;
    }

    public static void RebindSource(Image mapImage)
    {
        mapImage.Source = Buffer;
    }

    public static void Render(float x, float y, float zoom)
    {
        Pipeline.Render(x, y, zoom);
    }
}