using System;
using System.IO.Pipelines;
using System.Runtime.Intrinsics.X86;
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

    public static int XSize { set; get; } = 100;

    public static int YSize { set; get; } = 100;


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

        MapBuilder map = new MapBuilder(XSize, YSize, rng, 0.8);
        map.setTheme($"{DataLoader.RootPath}/data/" + Theme + "-theme/").initRoom();
        Pipeline.RebindBuilder(map); //bind the finished map to the renderer
        Pipeline.Render(90, 65, 0.7f); //call once with the default to update bitmap
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

    public static (double x, double y) ScreenToWorldspace(double x, double y, float zoom,
        (double x, double y) screenPosition)
    {
        return Pipeline.ScreenToWorldspace(x, y, zoom, screenPosition);
    }

    public static (double x, double y) WorldToScreenspace(double x, double y, float zoom,
        (double x, double y) worldPosition)
    {
        return Pipeline.WorldToScreenspace(x, y, zoom, worldPosition);
    }


    public static float TileSize(float zoom)
    {
        return Pipeline.TileSize(zoom);
    }
}