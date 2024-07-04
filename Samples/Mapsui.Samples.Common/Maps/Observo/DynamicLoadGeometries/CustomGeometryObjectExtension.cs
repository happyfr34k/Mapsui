using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries;
using System.Collections.Generic;

namespace Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries;
public static class CustomGeometryObjectExtension
{
    public static GeometryFeature ToFeature(this CustomGeometryObject customGeometry)
    {
        var feature = customGeometry.Geometry.ToFeature();
        feature["id"] = customGeometry.Id;
        feature["label"] = customGeometry.Name;
        feature["imagePath"] = customGeometry.ImagePath;
        return feature;
    }

    public static List<GeometryFeature> ToFeatures(this List<CustomGeometryObject> customGeometries)
    {
        var features = new List<GeometryFeature>();
        foreach (var customGeometry in customGeometries)
        {
            features.Add(customGeometry.ToFeature());
        }
        return features;
    }
}
