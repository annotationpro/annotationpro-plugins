using AnnotationPro.Logic;
using AnnotationPro.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        private readonly List<string> refLayerNames = new List<string>()
        {
            "105n_HeadPositionXYZ", "105n_NeckPositionXYZ", "105n_ChestPositionXYZ", "105n_MiddleSpinePositionXYZ", "105n_LowerSpinePositionXYZ",
            "105n_LShoulderPositionXYZ", "105n_HipPositionXYZ", "105n_RShoulderPositionXYZ"
        };

        //private readonly string srcLayerName = "105n_LFinger12PositionXYZ";
        private readonly string srcLayerName = "105n_RFinger12PositionXYZ";

        private Annotation annotation;
        private Audio audio;

        public LayerCollection SelectLayers()
        {
            LayerCollection layers = new LayerCollection();

            foreach (var layer in annotation.Layers)
            {
                if (!refLayerNames.Contains(layer.Name)) continue;
                layers.Add(layer);
            }

            return layers;
        }

        public void Run(AnnotationEditor editor)
        {
            annotation = editor.Synchronizer.DataServer.Annotation;
            audio = editor.Synchronizer.DataServer.Audio;

            LayerCollection refLayers = SelectLayers();
            LayerObject srcLayer = annotation.Layers.FindByName(srcLayerName);

            DetectLabels(refLayers, srcLayer);

            if (editor.WorkspaceModeEnabled)
                editor.SaveAnnotation();

            editor.RefreshLayers(false);
        }

        private void DetectLabels(LayerCollection refLayers, LayerObject srcLayer)
        {
            foreach (SegmentObject segment in srcLayer.Segments)
            {
                var result = DetermineBodyRegionLabel(segment, refLayers);
                segment.Parameter1 = result.XLabel;
                segment.Parameter2 = result.YLabel;
                segment.Parameter3 = result.ZLabel;
            }
        }

        public class AxisLabels
        {
            public string XLabel;
            public string YLabel;
            public string ZLabel;
        }

        public AxisLabels DetermineBodyRegionLabel(SegmentObject srcSegment, List<LayerObject> refLayers)
        {
            float centerTime = srcSegment.Start + srcSegment.Duration / 2f;

            string[] parts = srcSegment.Label.Split(';');
            if (parts.Length < 3) return new AxisLabels { XLabel = "Unknown", YLabel = "Unknown", ZLabel = "Unknown" };

            float srcX, srcY, srcZ;
            if (!float.TryParse(parts[0], out srcX) ||
                !float.TryParse(parts[1], out srcY) ||
                !float.TryParse(parts[2], out srcZ))
            {
                return new AxisLabels { XLabel = "Unknown", YLabel = "Unknown", ZLabel = "Unknown" };
            }

            var referenceMap = refLayers.ToDictionary(l => l.Name, l => l);

            var refNamesY = new List<string> { "105n_HeadPositionXYZ", "105n_NeckPositionXYZ", "105n_ChestPositionXYZ", "105n_MiddleSpinePositionXYZ", "105n_LowerSpinePositionXYZ", "105n_HipPositionXYZ" };
            var refNamesX = new List<string> { "105n_LShoulderPositionXYZ", "105n_HipPositionXYZ", "105n_RShoulderPositionXYZ" };
            var refNamesZ = new List<string> { "105n_ChestPositionXYZ", "105n_HipPositionXYZ" };

            return new AxisLabels
            {
                YLabel = DetermineRegion(refNamesY, referenceMap, centerTime, 1, srcY, "Above", "Below"),
                XLabel = DetermineRegion(refNamesX, referenceMap, centerTime, 0, srcX, "Left of", "Right of", null),
                ZLabel = DetermineRegion(refNamesZ, referenceMap, centerTime, 2, srcZ, "In front of", "Behind", null)
            };
        }

        private string DetermineRegion(List<string> refNames, Dictionary<string, LayerObject> refMap, float time, int axisIndex, float srcValue,
                                       string aboveOrLeft, string belowOrRight, string aligned = null)
        {
            var values = new Dictionary<string, float?>();

            foreach (var name in refNames)
            {
                if (refMap.ContainsKey(name))
                {
                    var seg = refMap[name].Segments.FirstOrDefault(s => time >= s.Start && time <= s.End);
                    if (seg != null)
                    {
                        string[] labelParts = seg.Label.Split(';');
                        if (labelParts.Length > axisIndex && float.TryParse(labelParts[axisIndex], out float val))
                        {
                            values[name] = val;
                            continue;
                        }
                    }
                }
                values[name] = null;
            }

            for (int i = 0; i < refNames.Count - 1; i++)
            {
                string upper = refNames[i];
                string lower = refNames[i + 1];

                if (values[upper].HasValue && values[lower].HasValue)
                {
                    float top = Math.Max(values[upper].Value, values[lower].Value);
                    float bottom = Math.Min(values[upper].Value, values[lower].Value);

                    if (srcValue <= top && srcValue >= bottom)
                        return upper + "-" + lower;
                }
            }

            if (values.ContainsKey(refNames[0]) && values[refNames[0]].HasValue && srcValue > values[refNames[0]].Value)
                return aboveOrLeft + " " + refNames[0];

            int lastIndex = refNames.Count - 1;
            if (values.ContainsKey(refNames[lastIndex]) && values[refNames[lastIndex]].HasValue && srcValue < values[refNames[lastIndex]].Value)
                return belowOrRight + " " + refNames[lastIndex];

            return "Unknown";
        }
    }
}