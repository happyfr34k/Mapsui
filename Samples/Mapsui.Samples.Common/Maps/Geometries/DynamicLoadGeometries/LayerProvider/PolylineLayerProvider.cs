using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using System.Collections.Generic;

namespace Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.LayerProvider;
public static class PolylineLayerProvider
{
    public static GenericCollectionLayer<List<GeometryFeature>> GetLayer(List<CustomGeometryObject> geometries, bool withLabel = false)
    {
        var labelStyle = StyleGeometryHelper.GetLabelStyle();
        labelStyle.Enabled = withLabel;

        return new GenericCollectionLayer<List<GeometryFeature>>()
        {
            Name = "Polylines",
            IsMapInfoLayer = true, // Allow the layer's features to be selected (comes on the Map.Info event)
            Features = geometries.ToFeatures(),
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
