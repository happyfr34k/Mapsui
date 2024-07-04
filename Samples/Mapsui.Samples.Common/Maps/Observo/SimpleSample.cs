using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries;
using Mapsui.Tiling;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using GeometryFactory = Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.DataFactory.GeometryFactory;

namespace Mapsui.Samples.Common.Maps.Observo;
[RequiresUnreferencedCode("Calls Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.DataFactory.GeometryFactory.CreateGeometries()")]
[RequiresDynamicCode("Calls Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.DataFactory.GeometryFactory.CreateGeometries()")]
public class SimpleSample : ISample
{
    public string Name => "Simple";
    public string Category => "Observo";

    public Task<Map> CreateMapAsync()
    {
        var map = new Map();

        map.Layers.Add(OpenStreetMap.CreateTileLayer());
        map.Layers.Add(CreateLayerWithStyleOnLayer());

        return Task.FromResult(map);
    }

    private static ILayer CreateLayerWithStyleOnLayer()
    {
        var geometries = GeometryFactory.CreateGeometries();

        return new Layer("Style on Layer")
        {
            DataSource = new MemoryProvider(geometries.ToFeatures())
        };
    }
}
