using Mapsui.Styles;

namespace Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries;
public static class StyleGeometryHelper
{
    public static LabelStyle GetLabelStyle(string labelColumn = "label") => new LabelStyle
    {
        ForeColor = Color.Black,
        BackColor = new Brush(Color.Transparent),
        Halo = new Pen(Color.White, 2),
        LabelColumn = labelColumn
    };

    public static SymbolStyle GetPointSelectionMarginStyle(int marginPx) => new SymbolStyle
    {
        Fill = new Brush(new Color(0, 0, 0, 1)),
        Outline = null,
        Line = new Pen
        {
            Width = marginPx,
            PenStyle = PenStyle.Solid,
            PenStrokeCap = PenStrokeCap.Round
        }
    };

    public static VectorStyle GetPolylineSelectionMarginStyle(int marginPx) => new VectorStyle
    {
        Line = new Pen
        {
            Color = new Color(0, 0, 0, 1),
            Width = marginPx,
            PenStyle = PenStyle.Solid,
            PenStrokeCap = PenStrokeCap.Round,
        }
    };

    public static VectorStyle GetPolygonSelectionMarginStyle(int marginPx) => new VectorStyle
    {
        Outline = new Pen
        {
            Color = new Color(0, 0, 0, 1),
            Width = marginPx,
            PenStyle = PenStyle.Solid,
            PenStrokeCap = PenStrokeCap.Round,
        },
        Fill = null,
    };
}
