using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.DataFactory;

public static class PolylineFactory
{
    private static Random random = new Random();

    public static List<LineString> CreatePolylines(int numberOfPolylines, int minPoints, int maxPoints, MRect? extent = null)
    {
        var result = new List<LineString>();

        for (var i = 0; i < numberOfPolylines; i++)
        {
            var numPoints = random.Next(minPoints, maxPoints + 1);

            double offsetX;
            double offsetY;
            if (extent != null)
            {
                // Generate random coordinates within the specified rectangle
                offsetX = random.NextDouble() * (extent.Right - extent.Left) + extent.Left; // Longitude range [minX, maxX]
                offsetY = random.NextDouble() * (extent.Top - extent.Bottom) + extent.Bottom; // Latitude range [minY, maxY]
            }
            else
            {
                // Generate random coordinates within valid WebMercator ranges
                offsetX = random.NextDouble() * 40075016.6855784 - 20037508.3427892;
                offsetY = random.NextDouble() * 40075016.6855784 - 20037508.3427892;
            }

            var lineString = GenerateLineString(numPoints, offsetX, offsetY);
            result.Add(lineString);
        }

        return result;
    }

    private static LineString GenerateLineString(int numPoints, double offsetX = 0, double offsetY = 0)
    {
        var coordinates = new Coordinate[numPoints];

        for (var i = 0; i < numPoints; i++)
        {
            var lon = offsetX + random.NextDouble() * 0.1 - 0.05; // Adding slight randomness
            var lat = offsetY + random.NextDouble() * 0.1 - 0.05; // Adding slight randomness
            coordinates[i] = new Coordinate(lon, lat);
        }

        return new LineString(coordinates);
    }
}
