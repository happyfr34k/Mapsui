using System;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Tiling;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries;
using Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.LayerProvider;
using NetTopologySuite.Geometries;
using Mapsui.Nts;
using Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.DataFactory;

namespace Mapsui.Samples.Common.Maps.Geometries;
public sealed class DynamicLoadGeometriesSample : ISample, IDisposable
{
    private bool _disposed;

    public string Name => "DynamicLoadOfGeometries";
    public string Category => "Observo";

    private List<CustomGeometryObject> _currentGeometries = new List<CustomGeometryObject>();

    private readonly Map _map = new Map();

    private GenericCollectionLayer<List<GeometryFeature>>? _pointLayer;
    private GenericCollectionLayer<List<GeometryFeature>>? _polylineLayer;
    private GenericCollectionLayer<List<GeometryFeature>>? _polygonLayer;

    public Task<Map> CreateMapAsync()
    {
        _currentGeometries = CustomGeometryFactory.GenerateRandomObjects(250);

        // Evenement a chaque déplacement
        // _map.Navigator.ViewportChanged += Navigator_ViewportChanged;
        _map.Navigator.RefreshDataRequest += Navigator_RefreshDataRequest;

        _map.Layers.Add(OpenStreetMap.CreateTileLayer());

        var pointGeometries = _currentGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePoint).ToList();
        _pointLayer?.Dispose();
        _pointLayer = PointLayerProvider.GetLayer(pointGeometries, true);

        var polylineGeometries = _currentGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNameMultiLineString).ToList();
        _polylineLayer?.Dispose();
        _polylineLayer = PolylineLayerProvider.GetLayer(polylineGeometries, true);

        var polygonGeometries = _currentGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePolygon).ToList();
        _polygonLayer?.Dispose();
        _polygonLayer = PolygonLayerProvider.GetLayer(polygonGeometries, true);

        _map.Layers.Add(_pointLayer);
        _map.Layers.Add(_polylineLayer);
        _map.Layers.Add(_polygonLayer);

        return Task.FromResult(_map);
    }

    private void Navigator_RefreshDataRequest(object? sender, EventArgs e)
    {
        var (minX, minY) = _map.Navigator.Viewport.ScreenToWorldXY(0, 0);
        var (maxX, maxY) = _map.Navigator.Viewport.ScreenToWorldXY(_map.Navigator.Viewport.Width, _map.Navigator.Viewport.Height);
        var extent = new MRect(minX, minY, maxX, maxY);

        var geometries = CustomGeometryFactory.GenerateRandomObjects(250, extent);

        UpdateGeometries(newGeometries: geometries, oldGeometries: _currentGeometries);
        _currentGeometries = geometries;
    }

    public void UpdateGeometries(List<CustomGeometryObject> newGeometries, List<CustomGeometryObject> oldGeometries)
    {
        // Remove old geometries
        /*_pointLayer!.Features.RemoveAll(f => oldGeometries.Any(o => o.Id == (Guid)f["id"]!));
        _polylineLayer!.Features.RemoveAll(f => oldGeometries.Any(o => o.Id == (Guid)f["id"]!));
        _polygonLayer!.Features.RemoveAll(f => oldGeometries.Any(o => o.Id == (Guid)f["id"]!));*/
        _pointLayer!.Features.Clear();
        _polylineLayer!.Features.Clear();
        _polygonLayer!.Features.Clear();

        // Add new geometries
        var pointGeometries = newGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePoint).ToList();
        _pointLayer.Features.AddRange(pointGeometries.ToFeatures());

        var polylineGeometries = newGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNameLineString).ToList();
        _polylineLayer.Features.AddRange(polylineGeometries.ToFeatures());

        var polygonGeometries = newGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePolygon).ToList();
        _polygonLayer.Features.AddRange(polygonGeometries.ToFeatures());
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _pointLayer?.Dispose();
        _polylineLayer?.Dispose();
        _polygonLayer?.Dispose();
        _map.Dispose();
    }
}
