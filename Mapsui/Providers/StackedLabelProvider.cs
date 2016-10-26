﻿using System.Collections.Generic;
using System.Linq;
using Mapsui.Geometries;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;

namespace Mapsui.Providers
{
    internal class StackedLabelProvider : IProvider
    {
        private const int SymbolSize = 32; // todo: determine margin by symbol size
        private const int BoxMargin = SymbolSize/2;
        private readonly IProvider _provider;

        public StackedLabelProvider(IProvider provider)
        {
            _provider = provider;
        }

        public LabelStyle LabelStyle { get; set; }

        public string CRS { get; set; }

        public IEnumerable<IFeature> GetFeaturesInView(BoundingBox box, double resolution)
        {
            var features = _provider.GetFeaturesInView(box, resolution);
            return GetFeaturesInView(resolution, LabelStyle, features);
        }

        public BoundingBox GetExtents()
        {
            return _provider.GetExtents();
        }

        private static List<Feature> GetFeaturesInView(double resolution, LabelStyle labelStyle,
            IEnumerable<IFeature> features)
        {
            var margin = resolution*50;
            var clusters = new List<Cluster>();
            // todo: repeat until there are no more merges
            ClusterFeatures(clusters, features, margin, labelStyle, resolution);

            const int textHeight = 18;

            var results = new List<Feature>();

            foreach (var cluster in clusters)
            {
                if (cluster.Features.Count > 1) results.Add(CreateBoxFeature(resolution, cluster));

                var offsetY = double.NaN;

                var orderedFeatures = cluster.Features.OrderBy(f => f.Geometry.GetBoundingBox().GetCentroid().Y);

                foreach (var pointFeature in orderedFeatures)
                {
                    var position = CalculatePosition(cluster);

                    offsetY = CalculateOffsetY(offsetY, textHeight);

                    var labelText = labelStyle.GetLabelText(pointFeature);
                    var labelFeature = CreateLabelFeature(position, labelStyle, offsetY, labelText);

                    results.Add(labelFeature);
                }
            }
            return results;
        }

        private static double CalculateOffsetY(double offsetY, int textHeight)
        {
            if (double.IsNaN(offsetY)) // first time
                offsetY = textHeight*0.5 + BoxMargin;
            else
                offsetY += textHeight; // todo: get size from text (or just pass stack nr)
            return offsetY;
        }

        private static Point CalculatePosition(Cluster cluster)
        {
            // Since the box can be rotated, find the minimal Y value of all 4 corners
            var rotatedBox = cluster.Box.Rotate(0); //todo: Add rotation '-viewport.Rotation'
            var minY = rotatedBox.Vertices.Select(v => v.Y).Min();
            var position = new Point(cluster.Box.GetCentroid().X, minY);
            return position;
        }

        private static Feature CreateLabelFeature(Point position, LabelStyle labelStyle, double offsetY,
            string text)
        {
            return new Feature
            {
                Geometry = position,
                Styles = new[]
                {
                    new LabelStyle(labelStyle)
                    {
                        Offset = {Y = offsetY},
                        LabelMethod = f => text
                    }
                }
            };
        }

        private static Feature CreateBoxFeature(double resolution, Cluster cluster)
        {
            return new Feature
            {
                Geometry = ToPolygon(GrowBox(cluster.Box, resolution)),
                Styles = new[]
                {
                    new VectorStyle
                    {
                        Outline = new Pen {Width = 2, Color = Color.Orange},
                        Fill = new Brush {Color = null}
                    }
                }
            };
        }

        private static Polygon ToPolygon(BoundingBox box)
        {
            return new Polygon
            {
                ExteriorRing = new LinearRing(new[]
                {
                    box.BottomLeft, box.TopLeft, box.TopRight, box.BottomRight, box.BottomLeft
                })
            };
        }

        private static BoundingBox GrowBox(BoundingBox box, double resolution)
        {
            const int symbolSize = 32; // todo: determine margin by symbol size
            const int boxMargin = symbolSize/2;
            return box.Grow(boxMargin*resolution);
        }

        private static void ClusterFeatures(
            ICollection<Cluster> clusters,
            IEnumerable<IFeature> features,
            double minDistance,
            IStyle layerStyle,
            double resolution)
        {
            var style = layerStyle;

            // todo: This method should repeated several times until there are no more merges
            foreach (var feature in features.OrderBy(f => f.Geometry.GetBoundingBox().GetCentroid().Y))
            {
                if (layerStyle is IThemeStyle) style = (layerStyle as IThemeStyle).GetStyle(feature);

                if ((style == null) ||
                    (style.Enabled == false) ||
                    (style.MinVisible > resolution) ||
                    (style.MaxVisible < resolution)) continue;

                var found = false;
                foreach (var cluster in clusters)
                    if (cluster.Box.Grow(minDistance).Contains(feature.Geometry.GetBoundingBox().GetCentroid()))
                    {
                        cluster.Features.Add(feature);
                        cluster.Box = cluster.Box.Join(feature.Geometry.GetBoundingBox());
                        found = true;
                        break;
                    }

                if (found) continue;

                clusters.Add(new Cluster
                {
                    Box = feature.Geometry.GetBoundingBox().Clone(),
                    Features = new List<IFeature> {feature}
                });
            }
        }

        private class Cluster
        {
            public BoundingBox Box { get; set; }
            public IList<IFeature> Features { get; set; }
        }
    }
}