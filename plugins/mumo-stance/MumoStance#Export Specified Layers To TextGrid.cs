/* 
 * Plugin Name: Export - Export selected layers as TextGrids (Praat software format)
 * Plugin Description: Exports selected layers (specified by 'layerNames') as TextGrid (Praat) files.
 * Programming API: https://annotationpro.org/api/ [Access date: 2026-01-20]
 *
 * Plugin Author: Wojciech Klessa & Katarzyna Klessa
 * Plugin Created: 2015-02-03. Updated 2026-01-20 by Katarzyna Klessa (layerNames definition)
 * 
 * Plugin Version: 1.0
 * 
 */
 
 

using System;
using System.Windows.Forms;
using AnnotationPro.Plugin;
using AnnotationPro.Presentation;
using AnnotationPro.Logic;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;


namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        List<string> layerNames = new List<string> {
                "Input",
                "Word",
                "Syllable",
                "Phone",
        };

        private Annotation annotation;
        private Audio audio;

        public void Run(AnnotationEditor editor)
        {
            annotation = editor.Synchronizer.DataServer.Annotation;
            audio = editor.Synchronizer.DataServer.Audio;

            List<LayerObject> layers = annotation.Layers.Where(l=>layerNames.Contains(l.Name)).ToList();

            editor.Cursor = Cursors.WaitCursor;

            ExportLayersAsTextGrid(layers);

            editor.Cursor = Cursors.Default;

            editor.RefreshLayers(false);
        }


        /// <summary>
        /// exports the layer defined in layerName to TextGrid
        /// </summary>
        /// <param name="srcLayer">èsource layer to export</param>
        public void ExportLayersAsTextGrid(List<LayerObject> layers)
        {
            // folder ANT
            string folder = Path.GetDirectoryName(annotation.FilePath);

            // ANT name without extension
            string fileName = Path.GetFileNameWithoutExtension(annotation.FilePath);

            // creating TextGrid name, the same as ANT but with a different extension
            string textGridFile = Path.Combine(folder, fileName + ".TextGrid");

            // if TextGrid exist we do nothing
            if (File.Exists(textGridFile))
                return;

            // export layers to TextGrid
            ExportLayerToTextGrid(layers, textGridFile, audio.SamplesCount, 5);
        }

        public void ExportLayerToTextGrid(List<LayerObject> layers, string fileName, float lengthInSamples, int threshold)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(annotation.ExportTextGridHeader(lengthInSamples, layers.Count));

            foreach (LayerObject layer in layers)
            {
                int index = layers.IndexOf(layer);
                sb.Append(annotation.ExportLayerToTextGridString(layer, index, lengthInSamples, threshold));
            }

            File.WriteAllText(fileName, sb.ToString());
        }
    }
}