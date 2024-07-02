using System;
using System.Collections.Generic;
using Point = NetTopologySuite.Geometries.Point;

namespace Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.DataFactory;

public static class PointFactory
{
    private static Random random = new Random();

    public static List<Point> CreatePoints(int numberOfPoints, MRect? extent = null)
    {
        var result = new List<Point>();

        for (var i = 0; i < numberOfPoints; i++)
        {
            double x;
            double y;
            if (extent != null)
            {
                // Generate random coordinates within the specified rectangle
                x = random.NextDouble() * (extent.Right - extent.Left) + extent.Left; // Longitude range [minX, maxX]
                y = random.NextDouble() * (extent.Top - extent.Bottom) + extent.Bottom; // Latitude range [minY, maxY]
            }
            else
            {
                // Generate random coordinates within valid WebMercator ranges
                x = random.NextDouble() * 40075016.6855784 - 20037508.3427892;
                y = random.NextDouble() * 40075016.6855784 - 20037508.3427892;
            }

            result.Add(new Point(x, y));
        }

        return result;
    }

}
