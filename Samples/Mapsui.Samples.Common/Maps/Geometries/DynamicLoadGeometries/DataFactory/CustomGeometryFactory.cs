using System.Collections.Generic;
using System.Linq;

namespace Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.DataFactory;
public static class CustomGeometryFactory
{
    public static List<CustomGeometryObject> GenerateRandomObjects(int nbObjects, MRect? extent = null)
    {
        var portionByGeometryType = nbObjects / 3;
        var customGeometries = new List<CustomGeometryObject>();

        var points = PointFactory.CreatePoints(portionByGeometryType, extent);
        customGeometries.AddRange(points.Select(point => new CustomGeometryObject() { Name = LabelFactory.CreateLabel(), Geometry = point, ImagePath = "embedded://Mapsui.Samples.Common.Images.home.png" }));

        var polylines = PolylineFactory.CreatePolylines(portionByGeometryType, 5, 10, extent);
        customGeometries.AddRange(polylines.Select(polyline => new CustomGeometryObject() { Name = LabelFactory.CreateLabel(), Geometry = polyline, ImagePath = "embedded://Mapsui.Samples.Common.Images.home.png" }));

        var polygons = PolygonFactory.CreatePolygons(portionByGeometryType, 5, 10, extent);
        customGeometries.AddRange(polygons.Select(polygon => new CustomGeometryObject() { Name = LabelFactory.CreateLabel(), Geometry = polygon, ImagePath = "embedded://Mapsui.Samples.Common.Images.home.png" }));

        return customGeometries;
    }
}
