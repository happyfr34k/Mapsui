using Mapsui.Layers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using System.Collections.Generic;

namespace Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.LayerProvider;
public static class PointLayerProvider
{
    public static Layer GetLayer(List<CustomGeometryObject> geometries, bool withLabel = false)
    {
        var labelStyle = StyleGeometryHelper.GetLabelStyle();
        labelStyle.Enabled = withLabel;

        var datasource = new DataProvider(geometries.ToFeatures()) { CRS = "EPSG:3857" };

        return new Layer()
        {
            Name = "Points",
            IsMapInfoLayer = true, // Allow the layer's features to be selected (comes on the Map.Info event)
            DataSource = datasource,
            Style = new StyleCollection
            {
                Styles =
                {
                    GetBitmapStyle(),
                    // Add a margin around the geometry to make it easier to select
                    StyleGeometryHelper.GetPointSelectionMarginStyle(30),
                    labelStyle
                }
            }
        };
    }

    private static ThemeStyle GetBitmapStyle()
    {
        return new ThemeStyle((f) =>
        {
            var imagePath = (string)f["imagePath"]!;
            return new SymbolStyle
            {
                ImageSource = imagePath,
                SymbolScale = 0.05,
                Fill = new Brush(Color.White),
                SymbolOffset = new RelativeOffset(0.0, 0.5),
            };
        });
    }
}
