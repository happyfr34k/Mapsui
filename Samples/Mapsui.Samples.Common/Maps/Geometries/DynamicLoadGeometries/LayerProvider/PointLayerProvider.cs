﻿using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using System.Collections.Generic;

namespace Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.LayerProvider;
public static class PointLayerProvider
{
    public static GenericCollectionLayer<List<GeometryFeature>> GetLayer(List<CustomGeometryObject> geometries, bool withLabel = false)
    {
        var labelStyle = StyleGeometryHelper.GetLabelStyle();
        labelStyle.Enabled = withLabel;

        return new GenericCollectionLayer<List<GeometryFeature>>()
        {
            Name = "Points",
            IsMapInfoLayer = true, // Allow the layer's features to be selected (comes on the Map.Info event)
            Features = geometries.ToFeatures(),
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
