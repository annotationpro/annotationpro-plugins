/* 
 * Plugin Name: Export - Export selected layers as EAF files (ELAN software format)
 * Plugin Description: Exports selected layers (specified by 'selectLayers') as EAF (ELAN) files.
 * Programming API: https://annotationpro.org/api/ [Access date: 2026-01-20]
 *
 * Plugin Author: Wojciech Klessa & Katarzyna Klessa
 * Plugin Created: 2025-10-30. Updated 2026-01-20 by Katarzyna Klessa (selectLayers definition)
 * 
 * Plugin Version: 1.0
 * 
 */
 
 

using AnnotationPro.Logic;
using AnnotationPro.Logic.ImportExport.ELAN;
using AnnotationPro.Presentation;
using System;
using System.Collections.Generic;
using System.IO;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        private readonly List<string> selectLayers = new List<string>() {
            "Interactive Unit",
            "Foregrounding Cluster",
            "Peak",
            "Dynamic Musical Terms",
            "Expressive Movement Unit",

        };

        private Annotation annotation;
        private Audio audio;
        public LayerCollection SelectLayers()
        {
            LayerCollection layers = new LayerCollection();

            foreach (var layer in annotation.Layers)
            {
                // ignore not selected layers
                if (!selectLayers.Contains(layer.Name)) continue;

                layers.Add(layer);
            }

            return layers;
        }
        public void Run(AnnotationEditor editor)
        {
            annotation = editor.Synchronizer.DataServer.Annotation;
            audio = editor.Synchronizer.DataServer.Audio;
            LayerCollection layers = SelectLayers();

            Annotation elanAnnotation = CreateAnnotation(layers);

            string filePath = annotation.FilePath;

            ElanWriter writer = new ElanWriter(Path.ChangeExtension(filePath, "eaf"), elanAnnotation);
            writer.Write();

            if (editor.WorkspaceModeEnabled)
            {
                editor.SaveAnnotation();
            }

            editor.RefreshLayers(false);
        }

        private Annotation CreateAnnotation(LayerCollection layers)
        {
            Annotation result = new Annotation();
            result.Samplerate = audio.Samplerate;
            result.Layers = layers.Copy(false, true);
            return result;
        }
    }
}
