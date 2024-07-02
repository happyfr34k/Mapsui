using NetTopologySuite.Geometries;
using System;

namespace Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries;
public class CustomGeometryObject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public Geometry Geometry { get; set; } = new Point(0, 0);
}
