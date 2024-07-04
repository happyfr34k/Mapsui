using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Providers;
using Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries;
using Mapsui.Styles;
using Mapsui.Tiling;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using GeometryFactory = Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.DataFactory.GeometryFactory;

namespace Mapsui.Samples.Common.Maps.Observo;
[RequiresUnreferencedCode("Calls Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.DataFactory.GeometryFactory.CreateGeometries()")]
[RequiresDynamicCode("Calls Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.DataFactory.GeometryFactory.CreateGeometries()")]
public class SelectionSample : ISample
{
    public string Name => "Selection";
    public string Category => "Observo";


    private MemoryProvider _selectedGeometryProvider = new();

    public Task<Map> CreateMapAsync()
    {
        var map = new Map();

        map.Info += (sender, args) =>
        {
            if (args.MapInfo == null) return;
            if (args.MapInfo.Feature == null) return;

            var feature = args.MapInfo.Feature;

            if (feature is GeometryFeature geometryFeature)
                geometryFeature["isSelected"] = !((bool?)geometryFeature["isSelected"] ?? false);
        };

        var geometries = GeometryFactory.CreateGeometries();
        var pins = geometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePoint).Take(200).ToList();
        var polylines = geometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNameMultiLineString).Take(200).ToList();
        var polygons = geometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePolygon).Take(200).ToList();

        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.Layers.Add(CreatePolygonLayerWithStyleOnLayer(polygons));
        map.Layers.Add(CreatePolylineLayerWithStyleOnLayer(polylines));
        map.Layers.Add(CreatePinLayerWithStyleOnLayer(pins));

        return Task.FromResult(map);
    }

    private static ILayer CreatePinLayerWithStyleOnLayer(List<CustomGeometryObject> pins)
    {
        return new Layer("Pin layer")
        {
            IsMapInfoLayer = true,
            DataSource = new MemoryProvider(pins.ToFeatures()),
            Style = new StyleCollection()
            {
                Styles =
                {
                    StyleGeometryHelper.GetPointStyle(),
                    StyleGeometryHelper.GetPointSelectionMarginStyle(30)
                }
            }
        };
    }
    private static ILayer CreatePolylineLayerWithStyleOnLayer(List<CustomGeometryObject> polylines)
    {
        return new Layer("Polyline layer")
        {
            IsMapInfoLayer = true,
            DataSource = new MemoryProvider(polylines.ToFeatures()),
            Style = new StyleCollection()
            {
                Styles =
                {
                    StyleGeometryHelper.GetPolylineStyle(),
                    StyleGeometryHelper.GetPolylineSelectionMarginStyle(30)
                }
            }
        };
    }
    private static ILayer CreatePolygonLayerWithStyleOnLayer(List<CustomGeometryObject> polygons)
    {
        return new Layer("Polygon layer")
        {
            IsMapInfoLayer = true,
            DataSource = new MemoryProvider(polygons.ToFeatures()),
            Style = new StyleCollection()
            {
                Styles =
                {
                    StyleGeometryHelper.GetPolygonStyle(),
                    StyleGeometryHelper.GetPolygonSelectionMarginStyle(30)
                }
            }
        };
    }
}
