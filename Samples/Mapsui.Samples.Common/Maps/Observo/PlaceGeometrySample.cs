using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Nts;
using Mapsui.Nts.Editing;
using Mapsui.Nts.Extensions;
using Mapsui.Providers;
using Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI;
using Mapsui.Widgets;
using Mapsui.Widgets.BoxWidgets;
using Mapsui.Widgets.ButtonWidgets;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GeometryFactory = Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.DataFactory.GeometryFactory;

namespace Mapsui.Samples.Common.Maps.Observo;
[RequiresUnreferencedCode("Calls Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.DataFactory.GeometryFactory.CreateGeometries()")]
[RequiresDynamicCode("Calls Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.DataFactory.GeometryFactory.CreateGeometries()")]
public sealed class PlaceGeometrySample : IMapControlSample, IDisposable
{
    public string Name => "Place geometries";
    public string Category => "Observo";


    private MemoryProvider _selectedGeometryProvider = new();
    private EditManager _editManager = new();
    private WritableLayer? _targetLayer;

    private Map _map = new();
    private IMapControl? _mapControl;

    public void Setup(IMapControl mapControl)
    {
        _mapControl = mapControl;
        _map = _mapControl.Map;
        _map.Navigator.PanLock = false;
        _map.Info += (sender, args) =>
        {
            if (args.MapInfo == null) return;
            if (args.MapInfo.Feature == null) return;

            var feature = args.MapInfo.Feature;

            if (feature is GeometryFeature geometryFeature)
                geometryFeature["isSelected"] = !((bool?)geometryFeature["isSelected"] ?? false);
        };

        var geometries = GeometryFactory.CreateLightGeometries();
        var pins = geometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePoint).Take(100).ToList();
        var polylines = geometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNameMultiLineString).Take(100).ToList();
        var polygons = geometries.Where(g => g.Geometry.GeometryType == Geometry.TypeNamePolygon).Take(100).ToList();

        _map.Layers.Add(OpenStreetMap.CreateTileLayer());
        _map.Layers.Add(CreateEditionLayer());
        _map.Layers.Add(CreatePolygonLayerWithStyleOnLayer(polygons));
        _map.Layers.Add(CreatePolylineLayerWithStyleOnLayer(polylines));
        _map.Layers.Add(CreatePinLayerWithStyleOnLayer(pins));


        _map.Widgets.Add(CreateSightWidget());
        _map.Widgets.Add(CreateBoxForEditWidgets());
        _map.Widgets.Add(CreateSelectPointModeButton(_map));
        _map.Widgets.Add(CreateSelectPolylineModeButton(_map));
        _map.Widgets.Add(CreateSelectPolygonModeButton(_map));
        _map.Widgets.Add(CreateAddPointButton(_map));

        var loggingwidget = _map.Widgets.Where(w => w.GetType().Name == "LoggingWidget").FirstOrDefault();
        if (loggingwidget != null)
            loggingwidget.Enabled = false;

        _editManager = InitEditMode(_map);
    }

    public static EditManager InitEditMode(Map map)
    {
        var editManager = new EditManager
        {
            Layer = (WritableLayer)map.Layers.First(l => l.Name == "EditLayer")
        };

        editManager.EditMode = EditMode.None;

        return editManager;
    }

    #region Widgets
    private static IWidget CreateSightWidget() => new BoxWidget
    {
        BackColor = new Color(255, 0, 0),
        CornerRadius = 8,
        HorizontalAlignment = HorizontalAlignment.Center,
        VerticalAlignment = VerticalAlignment.Center,
        Height = 5,
        Width = 5
    };

    private static BoxWidget CreateBoxForEditWidgets() => new BoxWidget
    {
        Width = 130,
        Height = 370,
        Position = new MPoint(2, 0),
        HorizontalAlignment = HorizontalAlignment.Absolute,
        VerticalAlignment = VerticalAlignment.Absolute,
    };

    private ButtonWidget CreateSelectPointModeButton(Map map) => new()
    {
        Position = new MPoint(5, 10),
        Height = 28,
        Width = 120,
        CornerRadius = 2,
        HorizontalAlignment = HorizontalAlignment.Absolute,
        VerticalAlignment = VerticalAlignment.Absolute,
        Text = "Point mode",
        BackColor = Color.LightGray,
        Tapped = (_, e) =>
        {
            _editManager.EditMode = EditMode.AddPoint;
            _targetLayer = map.Layers.First(f => f.Name == "Pin layer") as WritableLayer;

            var (minX, minY) = _map.Navigator.Viewport.ScreenToWorldXY(0, 0);
            var (maxX, maxY) = _map.Navigator.Viewport.ScreenToWorldXY(_map.Navigator.Viewport.Width, _map.Navigator.Viewport.Height);
            var extent = new MRect(minX, minY, maxX, maxY);

            var selectedFeatures = _targetLayer?.GetFeatures(extent, _map.Navigator.Viewport.Resolution).Where(f => (bool?)f["isSelected"] == true) ?? [];
            if (selectedFeatures.Count() > 0)
            {
                // _targetLayer.TryRemove(selectedFeatures.First());
                _editManager.Layer?.Add(selectedFeatures.First());
            }
            return true;
        }
    };

    private ButtonWidget CreateSelectPolylineModeButton(Map map) => new()
    {
        Position = new MPoint(5, 40),
        Height = 28,
        Width = 120,
        CornerRadius = 2,
        HorizontalAlignment = HorizontalAlignment.Absolute,
        VerticalAlignment = VerticalAlignment.Absolute,
        Text = "Polyline mode",
        BackColor = Color.LightGray,
        Tapped = (_, e) =>
        {
            _editManager.EditMode = EditMode.AddLine;
            _targetLayer = map.Layers.First(f => f.Name == "Polyline layer") as WritableLayer;
            return true;
        }
    };

    private ButtonWidget CreateSelectPolygonModeButton(Map map) => new()
    {
        Position = new MPoint(5, 70),
        Height = 28,
        Width = 120,
        CornerRadius = 2,
        HorizontalAlignment = HorizontalAlignment.Absolute,
        VerticalAlignment = VerticalAlignment.Absolute,
        Text = "Polygon mode",
        BackColor = Color.LightGray,
        Tapped = (_, e) =>
        {
            _editManager.EditMode = EditMode.AddPolygon;
            _targetLayer = map.Layers.First(f => f.Name == "Polygon layer") as WritableLayer;
            return true;
        }
    };

    private ButtonWidget CreateAddPointButton(Map map) => new()
    {
        Position = new MPoint(5, 100),
        Height = 28,
        Width = 120,
        CornerRadius = 2,
        HorizontalAlignment = HorizontalAlignment.Absolute,
        VerticalAlignment = VerticalAlignment.Absolute,
        Text = "Add point",
        BackColor = Color.LightGray,
        Tapped = (_, e) =>
        {
            if (_editManager.EditMode == EditMode.None) return false;

            // EditManipulation.OnTapped(new ScreenPosition(_map.Navigator.Viewport.Width / 2, _map.Navigator.Viewport.Height / 2), _editManager, _mapControl!, TapType.Single, false);
            _editManager.AddVertex(_map.Navigator.Viewport.ScreenToWorld(new ScreenPosition(_map.Navigator.Viewport.Width / 2, _map.Navigator.Viewport.Height / 2)).ToCoordinate());
            return true;
        }
    };

    private ButtonWidget CreatValidateButton(Map map) => new()
    {
        Position = new MPoint(5, 130),
        Height = 28,
        Width = 120,
        CornerRadius = 2,
        HorizontalAlignment = HorizontalAlignment.Absolute,
        VerticalAlignment = VerticalAlignment.Absolute,
        Text = "Validate",
        BackColor = Color.LightGray,
        Tapped = (_, e) =>
        {
            if (_editManager.EditMode == EditMode.None) return false;
            var features = _editManager.Layer?.GetFeatures();
            if (features == null) return false;

            _targetLayer?.AddRange(features.Copy());

            // _targetLayer = map.Layers.FirstOrDefault(f => f.Name == "Layer 1") as WritableLayer;
            return true;
        }
    };
    #endregion

    #region Layers

    private static WritableLayer CreateEditionLayer()
    {
        return new WritableLayer()
        {
            Name = "EditLayer",
            IsMapInfoLayer = true,
            Style = StyleGeometryHelper.GetEditBasicStyle()
        };
    }

    private static ILayer CreatePinLayerWithStyleOnLayer(List<CustomGeometryObject> pins)
    {
        var layer = new WritableLayer()
        {
            Name = "Pin layer",
            IsMapInfoLayer = true,
            Style = new StyleCollection()
            {
                Styles =
                {
                    StyleGeometryHelper.GetPointStyle(),
                    StyleGeometryHelper.GetPointSelectionMarginStyle(30)
                }
            }
        };

        layer.AddRange(pins.ToFeatures());
        return layer;
    }
    private static ILayer CreatePolylineLayerWithStyleOnLayer(List<CustomGeometryObject> polylines)
    {
        var layer = new WritableLayer()
        {
            Name = "Polyline layer",
            IsMapInfoLayer = true,
            Style = new StyleCollection()
            {
                Styles =
                {
                    StyleGeometryHelper.GetPolylineStyle(),
                    StyleGeometryHelper.GetPolylineSelectionMarginStyle(30)
                }
            }
        };

        layer.AddRange(polylines.ToFeatures());
        return layer;
    }
    private static ILayer CreatePolygonLayerWithStyleOnLayer(List<CustomGeometryObject> polygons)
    {
        var layer = new WritableLayer()
        {
            Name = "Polygon layer",
            IsMapInfoLayer = true,
            Style = new StyleCollection()
            {
                Styles =
                {
                    StyleGeometryHelper.GetPolygonStyle(),
                    StyleGeometryHelper.GetPolygonSelectionMarginStyle(30)
                }
            }
        };

        layer.AddRange(polygons.ToFeatures());
        return layer;
    }
    #endregion

    public void Dispose()
    {
        _targetLayer?.Dispose();
    }
}
