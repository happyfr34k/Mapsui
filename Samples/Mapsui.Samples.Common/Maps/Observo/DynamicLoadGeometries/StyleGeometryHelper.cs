using Mapsui.Styles;
using Mapsui.Styles.Thematics;

namespace Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries;
public static class StyleGeometryHelper
{
    public static LabelStyle GetLabelStyle(string labelColumn = "label") => new LabelStyle
    {
        ForeColor = Color.Black,
        BackColor = new Brush(Color.Transparent),
        Halo = new Pen(Color.White, 2),
        LabelColumn = labelColumn
    };

    public static VectorStyle GetEditBasicStyle() => new()
    {
        Fill = new Brush(Color.Red),
        Line = new Pen(Color.Red, 3),
        Outline = new Pen(Color.Red, 3)
    };

    #region Geometries Styles
    public static ThemeStyle GetPointStyle() => new ThemeStyle((f) =>
    {
        var imagePath = (string?)f["imagePath"] ?? "embedded://Mapsui.Samples.Common.Images.loc.png";
        var isSelected = (bool?)f["isSelected"] ?? false;
        return new SymbolStyle
        {
            ImageSource = isSelected ? "embedded://Mapsui.Samples.Common.Images.Pin.svg" : imagePath,
            SymbolScale = 0.2,
            Fill = new Brush(Color.White),
            SymbolOffset = new RelativeOffset(0.0, 0.5),
        };
    });

    public static ThemeStyle GetPolylineStyle() => new ThemeStyle(f =>
    {
        var isSelected = (bool?)f["isSelected"] ?? false;
        return new VectorStyle
        {
            Line = new Pen
            {
                Color = Color.Orange,
                Width = isSelected ? 4 : 2,
                PenStyle = PenStyle.Solid,
                PenStrokeCap = PenStrokeCap.Round,
            }
        };
    });

    public static ThemeStyle GetPolygonStyle() => new ThemeStyle((f) =>
    {
        var isSelected = (bool?)f["isSelected"] ?? false;
        return new VectorStyle()
        {
            Fill = new Brush(new Color(150, 150, 30, 128)),
            Outline = new Pen
            {
                Color = Color.Orange,
                Width = isSelected ? 4 : 2,
                PenStyle = PenStyle.DashDotDot,
                PenStrokeCap = PenStrokeCap.Round
            }
        };
    });
    #endregion

    #region Geometries Selection Margin Styles
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
    #endregion
}
