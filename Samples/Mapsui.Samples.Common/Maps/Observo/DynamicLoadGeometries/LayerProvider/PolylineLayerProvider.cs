using Mapsui.Layers;
using Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.DataProvider;
using Mapsui.Styles;
using System.Collections.Generic;

namespace Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.LayerProvider;
public static class PolylineLayerProvider
{
    public static Layer GetLayer(List<CustomGeometryObject> geometries, bool withLabel = false)
    {
        var labelStyle = StyleGeometryHelper.GetLabelStyle();
        labelStyle.Enabled = withLabel;

        var dataSource = new GeometryProvider(geometries.ToFeatures()) { CRS = "EPSG:3857" };

        return new Layer()
        {
            Name = "Polylines",
            IsMapInfoLayer = true, // Allow the layer's features to be selected (comes on the Map.Info event)
            DataSource = dataSource,
            Style = new StyleCollection
            {
                Styles =
                {
                    StyleGeometryHelper.GetPolylineStyle(),
                    // Add a margin around the geometry to make it easier to select
                    StyleGeometryHelper.GetPolylineSelectionMarginStyle(30),
                    labelStyle
                }
            }
        };
    }
}
