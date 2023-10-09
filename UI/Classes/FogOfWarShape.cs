using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace UI.Classes;

public class FogOfWarShape : Rectangle
{
    //X and Y
    public double XLoc { get; set; }
    public double RelativeX { get; set; }
    public double YLoc { get; set; }
    public double RelativeY { get; set; }
    public int Size { get; set; }

    public FogOfWarShape(double xLoc, double yLoc, double relativeX, double relativeY, int size)
    {
        Size = size;
        XLoc = xLoc;
        YLoc = yLoc;
        RelativeX = relativeX;
        RelativeY = relativeY;
        Height = Size;
        Width = Size;
        Fill = new SolidColorBrush(Color.Parse("#21262b"));
        IsHitTestVisible = false;
    }

    public void updateSize(double zoom)
    {
        Height = Size * zoom;
        Width = Size * zoom;
    }
}