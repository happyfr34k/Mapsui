using System;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Tiling;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using Mapsui.Tiling.Layers;
using GeometryFactory = Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.DataFactory.GeometryFactory;
using System.Diagnostics.CodeAnalysis;
using Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries;
using Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.DataFactory;
using Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.DataProvider;
using Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.LayerProvider;

namespace Mapsui.Samples.Common.Maps.Observo;
[RequiresUnreferencedCode("")]
[RequiresDynamicCode("")]
public sealed class DynamicLoadGeometriesSample : ISample, IDisposable
{
    private bool _disposed;

    public string Name => "DynamicLoadOfGeometries";
    public string Category => "Observo";

    private List<CustomGeometryObject> _currentGeometries = new List<CustomGeometryObject>();

    private readonly Map _map = new Map();

    private Layer? _pointLayer;
    private RasterizingTileLayer? _pointRasterzingLayer;
    private Layer? _polylineLayer;
    private RasterizingTileLayer? _polylineRasterzingLayer;
    private Layer? _polygonLayer;
    private RasterizingTileLayer? _polygonRasterzingLayer;


    public Task<Map> CreateMapAsync()
    {
        _currentGeometries = GeometryFactory.CreateGeometries();

        // Evenement a chaque déplacement
        // _map.Navigator.ViewportChanged += Navigator_ViewportChanged;
        // _map.Navigator.RefreshDataRequest += Navigator_RefreshDataRequest;

        _map.Layers.Add(OpenStreetMap.CreateTileLayer());

        var pointGeometries = _currentGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePoint).ToList();
        _pointLayer?.Dispose();
        _pointLayer = PointLayerProvider.GetLayer(pointGeometries, true);
        _pointRasterzingLayer?.Dispose();
        _pointRasterzingLayer = new RasterizingTileLayer(_pointLayer);

        var polylineGeometries = _currentGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNameMultiLineString).ToList();
        _polylineLayer?.Dispose();
        _polylineLayer = PolylineLayerProvider.GetLayer(polylineGeometries, true);
        _polylineRasterzingLayer?.Dispose();
        _polylineRasterzingLayer = new RasterizingTileLayer(_polylineLayer);

        var polygonGeometries = _currentGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePolygon).ToList();
        _polygonLayer?.Dispose();
        _polygonLayer = PolygonLayerProvider.GetLayer(polygonGeometries, true);
        _polygonRasterzingLayer?.Dispose();
        _polygonRasterzingLayer = new RasterizingTileLayer(_polygonLayer);

        _map.Layers.Add(_pointRasterzingLayer);
        _map.Layers.Add(_polylineRasterzingLayer);
        _map.Layers.Add(_polygonRasterzingLayer);

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
        ((GeometryProvider)((Layer)_pointRasterzingLayer!.SourceLayer).DataSource!).ClearData();
        ((GeometryProvider)((Layer)_polylineRasterzingLayer!.SourceLayer).DataSource!).ClearData();
        ((GeometryProvider)((Layer)_polygonRasterzingLayer!.SourceLayer).DataSource!).ClearData();

        // Add new geometries
        var pointGeometries = newGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePoint).ToList();
        ((GeometryProvider)((Layer)_pointRasterzingLayer!.SourceLayer).DataSource!).AddRange(pointGeometries.ToFeatures());

        var polylineGeometries = newGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNameLineString).ToList();
        ((GeometryProvider)((Layer)_polylineRasterzingLayer!.SourceLayer).DataSource!).AddRange(polylineGeometries.ToFeatures());

        var polygonGeometries = newGeometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePolygon).ToList();
        ((GeometryProvider)((Layer)_polygonRasterzingLayer!.SourceLayer).DataSource!).AddRange(polygonGeometries.ToFeatures());
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
        _pointRasterzingLayer?.Dispose();
        _polylineRasterzingLayer?.Dispose();
        _polygonRasterzingLayer?.Dispose();
        _map.Dispose();
    }
}
