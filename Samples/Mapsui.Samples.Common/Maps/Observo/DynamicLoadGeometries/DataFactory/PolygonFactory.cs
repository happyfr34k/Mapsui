using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.DataFactory;

public class PolygonFactory
{
    private static Random random = new Random();

    public static List<Polygon> CreatePolygons(int numberOfPolygons, int minPoints, int maxPoints, MRect? extent = null)
    {
        var result = new List<Polygon>();

        for (var i = 0; i < numberOfPolygons; i++)
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

            var exteriorRing = GenerateRing(numPoints, 0.01, 0.01, offsetX, offsetY); // Using smaller width and height for WGS84
            var interiorRings = new List<LinearRing>();

            // For simplicity, let's generate one interior ring per polygon
            /*if (numPoints > 4) // Ensure there are enough points for a simple interior ring
            {
                var interiorRing = GenerateRing(numPoints, 0.008, 0.008, offsetX + 0.1, offsetY + 0.1);
                interiorRings.Add(interiorRing);
            }*/

            result.Add(new Polygon(exteriorRing, interiorRings.ToArray()));
        }
        return result;
    }

    private static LinearRing GenerateRing(int numPoints, double width, double height, double offsetX = 0, double offsetY = 0)
    {
        var coordinates = new Coordinate[numPoints + 1]; // +1 to close the ring

        var centerX = width / 2 + offsetX;
        var centerY = height / 2 + offsetY;
        var radiusX = width / 2;
        var radiusY = height / 2;

        for (var i = 0; i < numPoints; i++)
        {
            var angle = 2 * Math.PI * i / numPoints;
            var lon = centerX + radiusX * Math.Cos(angle) + random.NextDouble() * 0.05 - 0.025; // Adding slight randomness
            var lat = centerY + radiusY * Math.Sin(angle) + random.NextDouble() * 0.05 - 0.025; // Adding slight randomness
            coordinates[i] = new Coordinate(lon, lat);
        }
        coordinates[numPoints] = coordinates[0]; // Close the ring

        return new LinearRing(coordinates);
    }
}
