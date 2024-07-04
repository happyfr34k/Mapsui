using NetTopologySuite.IO;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Projections;
using NetTopologySuite.Geometries;

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

    public static List<CustomGeometryObject> GenerateRandomObjects(List<string> wkts)
    {
        var customGeometries = new List<CustomGeometryObject>();

        var wktReader = new WKTReader();

        foreach (var wkt in wkts)
        {
            if (wkt == null)
                continue;

            var wktConverted = ConvertWkt(wkt, Epsg.Wgs84, Epsg.Wgs84PseudoMercator);
            var nts = wktReader.Read(wktConverted);
            customGeometries.Add(new CustomGeometryObject() { Name = LabelFactory.CreateLabel(), Geometry = nts, ImagePath = "embedded://Mapsui.Samples.Common.Images.home.png" });
        }

        return customGeometries;
    }

    public static string ConvertWkt(string wkt, Epsg sourceEpsg, Epsg targetEpsg)
    {
        var rdr = new NetTopologySuite.IO.WKTReader();
        var wrt = new NetTopologySuite.IO.WKTWriter();
        var coords = new List<Coordinate>();
        var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory((int)targetEpsg);
        var geometry = rdr.Read(wkt);

        ProjectionInfo epsgWM = KnownCoordinateSystems.Geographic.World.WGS1984;
        ProjectionInfo epsg4326 = KnownCoordinateSystems.Projected.World.WebMercator;

        double[] xy;

        switch (geometry.GeometryType)
        {
            case "LineString":
                foreach (var coordinate in geometry.Coordinates)
                {
                    //convert
                    xy = new double[] { coordinate.X, coordinate.Y };
                    Reproject.ReprojectPoints(xy, new double[] { coordinate.Z }, epsgWM, epsg4326, 0, 1); ;
                    coords.Add(new Coordinate(xy[0], xy[1]));
                }
                return wrt.Write(gf.CreateLineString(coords.ToArray()));

            case "MultiLineString":
                var lineStrings = new List<LineString>();
                foreach (var linestring in ((MultiLineString)geometry).Geometries)
                {
                    var geometryCoords = new List<Coordinate>();
                    foreach (var coordinate in linestring.Coordinates)
                    {
                        //convert
                        xy = new double[] { coordinate.X, coordinate.Y };
                        Reproject.ReprojectPoints(xy, new double[] { coordinate.Z }, epsgWM, epsg4326, 0, 1); ;
                        geometryCoords.Add(new Coordinate(xy[0], xy[1]));
                    }

                    lineStrings.Add(new LineString(geometryCoords.ToArray()));
                }
                return wrt.Write(gf.CreateMultiLineString(lineStrings.ToArray()));

            case "Polygon":
                foreach (var coordinate in geometry.Coordinates)
                {
                    //convert
                    xy = new double[] { coordinate.X, coordinate.Y };
                    Reproject.ReprojectPoints(xy, new double[] { coordinate.Z }, epsgWM, epsg4326, 0, 1); ;
                    coords.Add(new Coordinate(xy[0], xy[1]));
                }
                return wrt.Write(gf.CreatePolygon(coords.ToArray()));

            case "MultiPolygon":
                var polygons = new List<Polygon>();
                foreach (var polygon in ((MultiPolygon)geometry).Geometries)
                {
                    var geometryCoords = new List<Coordinate>();
                    foreach (var coordinate in polygon.Coordinates)
                    {
                        //convert
                        xy = new double[] { coordinate.X, coordinate.Y };
                        Reproject.ReprojectPoints(xy, new double[] { coordinate.Z }, epsgWM, epsg4326, 0, 1); ;
                        geometryCoords.Add(new Coordinate(xy[0], xy[1]));
                    }

                    polygons.Add(new Polygon(new LinearRing(geometryCoords.ToArray())));
                }
                return wrt.Write(gf.CreateMultiPolygon(polygons.ToArray()));

            case "Point":
                //convert
                xy = new double[] { geometry.Coordinate.X, geometry.Coordinate.Y };
                Reproject.ReprojectPoints(xy, new double[] { geometry.Coordinate.X }, epsgWM, epsg4326, 0, 1); ;
                return wrt.Write(gf.CreatePoint(new Coordinate(xy[0], xy[1])));

            case "GeometryCollection":
                var wktResult = "GEOMETRYCOLLECTION(";
                foreach (var geometryGeometry in ((GeometryCollection)geometry).Geometries)
                {
                    //Ignore point in geometryCollection
                    if (geometryGeometry.GeometryType == "Point") continue;

                    wktResult += ConvertWkt(geometryGeometry.ToText(), sourceEpsg, targetEpsg);
                    wktResult += ", ";
                }

                wktResult = wktResult.Remove(wktResult.Length - 2, 2);
                wktResult += ")";
                return wktResult;
            default:
                break;
        }
        return "";
    }

    public enum Epsg
    {
        Unknown = 0,
        Wgs84 = 4326,   //EPSG 4326 uses a coordinate system the same as a GLOBE (curved surface). Google Earth
        //Wgs84SphericalMercator = 900913,  //not used
        Wgs84PseudoMercator = 3857, //EPSG 3857 uses a coordinate system the same as a MAP (flat surface). This is projected coordinate system used for rendering maps in Google Maps, OpenStreetMap, etc. Observo WMTS are only supported with this EPSG
        Lv03 = 21781,
        Lv95 = 2056,
        Lambert93 = 2154,
        //ETRS89_UTMzone32N = 25832, //Germany
        //ETRS89_AustriaLambert = 3416 // Austria
    };
}
