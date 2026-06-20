/* * EDIT: Find & Replace
 * Replace one string by another or remove a string from (a collection of) annotations, ie. replace it by nothing 
 * Plugin Author: Wojciech Klessa & Katarzyna Klessa
 * Plugin Created: 2016-05-20
 * * Plugin Id: 38d5ed60
 * Plugin Version: 1.0
 * */

using AnnotationPro.Logic;
using AnnotationPro.Presentation;
using System.Collections.Generic;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        // Usunięto duplikaty "Input" oraz "Peak" z listy warstw do zignorowania
        private readonly List<string> ignoreLayers = new List<string>() { "Input", "Word", "Syllable", "Phone", "Foregrounding Cluster", "Peak", "Expressive Movement Unit", "Dynamic Musical Terms" };
        private readonly List<string> ignoreLabels = new List<string>() { "Label_to_ignore", "Label_to_ignore2", "Label_to_ignore3" };

        // add new patterns to create new replacement rule, you can either replace one string by another or remove a string, ie. replace it by nothing (then use string.Empty for the string to be used as a replacement)
        private readonly List<ReplacePattern> replacePatterns = new List<ReplacePattern>()
        {
            new ReplacePattern("<p:>", string.Empty),
            new ReplacePattern("high Cluster", "high"),
            new ReplacePattern("low cluster", "low"),
            new ReplacePattern("(retraction) high Cluster", "high"),
            new ReplacePattern("low Cluster", "low"),
        };

        private Annotation annotation;
        private Audio audio;

        private class ReplacePattern
        {
            public ReplacePattern(string find, string replace)
            {
                Find = find;
                Replace = replace;
            }

            public string Find { get; set; }
            public string Replace { get; set; }
        }

        public void Run(AnnotationEditor editor)
        {
            annotation = editor.Synchronizer.DataServer.Annotation;
            audio = editor.Synchronizer.DataServer.Audio;

            foreach (LayerObject layer in annotation.Layers)
            {
                if (ignoreLayers.Contains(layer.Name)) continue;

                ClearLayer(layer);
            }

            editor.SaveAnnotation();
        }

        private void ClearLayer(LayerObject layer)
        {
            foreach (SegmentObject segment in layer.Segments)
            {
                ClearSegment(segment);
            }
        }

        private void ClearSegment(SegmentObject segment)
        {
            if (ignoreLabels.Contains(segment.Label)) return;

            foreach (var pattern in replacePatterns)
            {
                segment.Label = segment.Label.Replace(pattern.Find, pattern.Replace);
            }
        }
    }
}
