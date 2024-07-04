﻿using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mapsui.Samples.Common.Maps.Observo.DynamicLoadGeometries.DataProvider;
public sealed class GeometryProvider : IndexedMemoryProvider, IDynamic, IDisposable
{
    public event EventHandler? DataChanged;

    private readonly List<GeometryFeature> _datasource = new List<GeometryFeature>();

    public GeometryProvider(List<GeometryFeature> features)
    {
        _datasource = features;
    }

    public override Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        return Task.FromResult((IEnumerable<IFeature>)_datasource);
    }

    public void AddRange(List<GeometryFeature> features)
    {
        _datasource.AddRange(features);
        OnDataChanged();
    }

    public void ClearData()
    {
        _datasource.Clear();
        OnDataChanged();
    }

    /*public void UpdateGeometry(GeometryFeature feature)
    {
        _dat
    }*/

    void IDynamic.DataHasChanged()
    {
        OnDataChanged();
    }

    private void OnDataChanged()
    {
        DataChanged?.Invoke(this, new EventArgs());
    }

    public void Dispose()
    {
        _datasource.Clear();
    }
}