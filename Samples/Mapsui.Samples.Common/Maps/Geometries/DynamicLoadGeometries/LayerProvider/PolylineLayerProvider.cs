using Mapsui.Layers;
using Mapsui.Styles;
using System.Collections.Generic;

namespace Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.LayerProvider;
public static class PolylineLayerProvider
{
    public static Layer GetLayer(List<CustomGeometryObject> geometries, bool withLabel = false)
    {
        var labelStyle = StyleGeometryHelper.GetLabelStyle();
        labelStyle.Enabled = withLabel;

        var dataSource = new DataProvider(geometries.ToFeatures()) { CRS = "EPSG:3857" };

        return new Layer()
        {
            Name = "Polylines",
            IsMapInfoLayer = true, // Allow the layer's features to be selected (comes on the Map.Info event)
            DataSource = dataSource,
            Style = new StyleCollection
            {
                Styles =
                {
                    GetPolylineStyle(),
                    // Add a margin around the geometry to make it easier to select
                    StyleGeometryHelper.GetPolylineSelectionMarginStyle(30),
                    labelStyle
                }
            }
        };
    }

    private static VectorStyle GetPolylineStyle() => new VectorStyle
    {
        Line = new Pen
        {
            Color = Color.Orange,
            Width = 2,
            PenStyle = PenStyle.Solid,
            PenStrokeCap = PenStrokeCap.Round,
        }
    };
}
