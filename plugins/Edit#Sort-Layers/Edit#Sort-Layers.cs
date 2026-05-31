/* 
 * EDIT: SORT LAYERS
 *
 * Annotation Pro Plugin
 *   
 * Plugin Author: Polskie Media
 * Plugin Updated: 2024-01-04
 * Plugin Id: 0dfa2h0a
 * Plugin Version: 1.0
 * Plugin Description: Sorts layers in annotation with given order. You need enter layer list using layer names.
 * 
 */
using AnnotationPro.Logic;
using AnnotationPro.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        List<string> layerNames = new List<string>
        {
            "Input",
            "Word",
            "Syllable",
            "Phone",
            "Interactive Unit",
            "Foregrounding Cluster",
            "Peak",
            "Dynamic Musical Terms",
            "Expressive Movement",
        };

        private Annotation annotation;
        private Audio audio;

        public void Run(AnnotationEditor editor)
        {
            annotation = editor.Synchronizer.DataServer.Annotation;
            audio = editor.Synchronizer.DataServer.Audio;

            List<LayerObject> sorted = SortLayers(annotation.Layers, layerNames);
            annotation.Layers.Clear();
            annotation.Layers.AddRange(sorted);
            annotation.IsModified = true;

            editor.RefreshLayers(false);
            {
                editor.SaveAnnotation();
            }
        }
        private List<LayerObject> SortLayers(List<LayerObject> layers, List<string> order)
        {
            // Tworzymy słownik dla szybkiego mapowania nazw warstw na ich indeksy
            var orderMap = order.Select((name, index) => new { name, index })
                                .ToDictionary(x => x.name, x => x.index);

            // Sortujemy listę layers, używając mapowania do ustalenia kolejności
            return layers.OrderBy(layer => orderMap.ContainsKey(layer.Name) ? orderMap[layer.Name] : int.MaxValue)
                         .ToList();
        }
        private void Info(string message)
        {
            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
