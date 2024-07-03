using Mapsui.Layers;
using Mapsui.Styles;
using System.Collections.Generic;

namespace Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.LayerProvider;
public static class PolygonLayerProvider
{
    public static Layer GetLayer(List<CustomGeometryObject> geometries, bool withLabel = false)
    {
        var labelStyle = StyleGeometryHelper.GetLabelStyle();
        labelStyle.Enabled = withLabel;

        var dataSource = new DataProvider(geometries.ToFeatures()) { CRS = "EPSG:3857" };

        return new Layer()
        {
            Name = "Polygons",
            IsMapInfoLayer = true, // Allow the layer's features to be selected (comes on the Map.Info event)
            DataSource = dataSource,
            Style = new StyleCollection
            {
                Styles =
                    {
                        GetPolygonStyle(),
                        // Add a margin around the geometry to make it easier to select
                        StyleGeometryHelper.GetPolygonSelectionMarginStyle(30),
                        labelStyle
                    }
            }
        };
    }

    private static VectorStyle GetPolygonStyle() => new VectorStyle
    {
        Fill = new Brush(new Color(150, 150, 30, 128)),
        Outline = new Pen
        {
            Color = Color.Orange,
            Width = 2,
            PenStyle = PenStyle.DashDotDot,
            PenStrokeCap = PenStrokeCap.Round
        }
    };
}
