using AnnotationPro.Logic;
using AnnotationPro.Presentation;
using System.Collections.Generic;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        private Annotation annotation;
        private Audio audio;


        public void Run(AnnotationEditor editor)
        {
            annotation = editor.Synchronizer.DataServer.Annotation;
            audio = editor.Synchronizer.DataServer.Audio;

            List<LayerObject> layers = new List<LayerObject>();

            foreach (var layer in annotation.Layers)
            {
                if(layer.Segments.Count == 0)
                {
                    layers.Add(layer);
                }
                
            }

            annotation.Layers.RemoveAll(x => layers.Contains(x));

            if (editor.WorkspaceModeEnabled)
            {
                editor.SaveAnnotation();
            }

            editor.RefreshLayers(false);
        }
    }
}
