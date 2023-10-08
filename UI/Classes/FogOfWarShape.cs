using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace UI.Classes;

public class FogOfWarShape : Rectangle
{
    //X and Y
    public double XLoc { get; set; }
    public double YLoc { get; set; }
    private int Size { get; set; }

    public FogOfWarShape(double xLoc, double yLoc, int size)
    {
        Size = size;
        XLoc = xLoc;
        YLoc = yLoc;
        Height = Size;
        Width = Size;
        Fill = new SolidColorBrush(Color.Parse("#21262b"));
        IsHitTestVisible = false;
    }
}