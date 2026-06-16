using AnnotationPro.Logic;
using AnnotationPro.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        // refLayerNames order from top to bottom
        private readonly List<string> refLayerNames = new List<string>() {
            "New_Import_HeadPositionY",
            "New_Import_NeckPositionY",
            "New_Import_ChestPositionY",
            "New_Import_MiddleSpinePositionY",
            "New_Import_LowerSpinePositionY",
        };

        // source layer name
        private readonly string srcLayerName = "New_Import_RHandPositionY";
        //private readonly string srcLayerName = "LFinger2PositionY";

        private Annotation annotation;
        private Audio audio;
        //metoda, ma nawiasy za nazwa metody
        public LayerCollection SelectLayers()
        {
            LayerCollection layers = new LayerCollection();

            foreach (var layer in annotation.Layers)
            {
                // ignore not selected layers
                if (!refLayerNames.Contains(layer.Name)) continue;

                layers.Add(layer);
            }

            return layers;
        }
        //metoda, metoda Run jest zawsze w pluginach AnnPro, jest w kontrakcie
        //metody siê uruchamia a zmienne zapisuje, w klasie nie ma "zmiennych", zmienne w klasie nazywaj¹ siê fields i properties
        public void Run(AnnotationEditor editor)
        {
            /// zmienne lokalne (te w metodzie) w klasie nazywa siê jednak zmiennymi
            annotation = editor.Synchronizer.DataServer.Annotation;
            audio = editor.Synchronizer.DataServer.Audio;

            //wywo³anie funkcji SelectLayers, zmienna lokalna
            LayerCollection refLayers = SelectLayers();
            LayerObject srcLayer = annotation.Layers.FindByName(srcLayerName);

            // detect src layer location between ref layers
            DetectLabels(refLayers, srcLayer);

            if (editor.WorkspaceModeEnabled)
            {
                editor.SaveAnnotation();
            }

            editor.RefreshLayers(false);
        }

        private void DetectLabels(LayerCollection refLayers, LayerObject srcLayer)
        {
            foreach (SegmentObject segment in srcLayer.Segments)
            {
                segment.Parameter1 = DetermineBodyRegionLabel(segment, refLayers);
            }
        }

        public string DetermineBodyRegionLabel(SegmentObject srcSegment, List<LayerObject> refLayers)
        {
            // Tworzymy mapê warstw referencyjnych
            Dictionary<string, LayerObject> referenceMap = refLayers.ToDictionary(l => l.Name, l => l);

            // Obliczamy œrodek segmentu Ÿród³owego (czasowo)
            float centerTime = srcSegment.Start + srcSegment.Duration / 2f;

            // Pozycja Y segmentu Ÿród³owego
            float srcY;
            if (!float.TryParse(srcSegment.Label, out srcY))
                return "Unknown";

            // Pobieramy wartoœci Y z warstw referencyjnych w tym samym momencie czasowym
            Dictionary<string, float?> refY = new Dictionary<string, float?>();
            foreach (string name in refLayerNames) // refLayerNames order from top to bottom
            {
                if (referenceMap.ContainsKey(name))
                {
                    LayerObject layer = referenceMap[name];
                    SegmentObject refSegment = layer.Segments.FirstOrDefault(s =>
                        centerTime >= s.Start && centerTime <= s.End);
                    float y;
                    if (refSegment != null && float.TryParse(refSegment.Label, out y))
                        refY[name] = y;
                    else
                        refY[name] = null;
                }
                else
                {
                    refY[name] = null;
                }
            }

            // Sprawdzamy zakresy miêdzy warstwami
            for (int i = 0; i < refLayerNames.Count - 1; i++)
            {
                string upper = refLayerNames[i];
                string lower = refLayerNames[i + 1];

                if (refY[upper].HasValue && refY[lower].HasValue)
                {
                    float y1 = refY[upper].Value;
                    float y2 = refY[lower].Value;

                    // Obs³uga ró¿nych kierunków osi Y
                    float top = Math.Max(y1, y2);
                    float bottom = Math.Min(y1, y2);

                    if (srcY <= top && srcY >= bottom)
                        return upper + "-" + lower;
                }
            }

            // Poza zakresem
            if (refY[refLayerNames.First()].HasValue && srcY > refY[refLayerNames.First()].Value)
                return "Above " + refLayerNames.First();

            if (refY[refLayerNames.Last()].HasValue && srcY < refY[refLayerNames.Last()].Value)
                return "Below " + refLayerNames.Last();

            return "Unknown";
        }
    }
}
