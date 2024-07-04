using Mapsui.Layers;
using Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.DataProvider;
using Mapsui.Styles;
using System.Collections.Generic;

namespace Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.LayerProvider;
public static class PointLayerProvider
{
    public static Layer GetLayer(List<CustomGeometryObject> geometries, bool withLabel = false)
    {
        var labelStyle = StyleGeometryHelper.GetLabelStyle();
        labelStyle.Enabled = withLabel;

        var datasource = new GeometryProvider(geometries.ToFeatures()) { CRS = "EPSG:3857" };

        return new Layer()
        {
            Name = "Points",
            IsMapInfoLayer = true, // Allow the layer's features to be selected (comes on the Map.Info event)
            DataSource = datasource,
            Style = new StyleCollection
            {
                Styles =
                {
                    StyleGeometryHelper.GetPointStyle(),
                    // Add a margin around the geometry to make it easier to select
                    StyleGeometryHelper.GetPointSelectionMarginStyle(30),
                    labelStyle
                }
            }
        };
    }
}
