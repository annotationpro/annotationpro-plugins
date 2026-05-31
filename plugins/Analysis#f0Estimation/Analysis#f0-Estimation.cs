/* 
 * Analysis: f0 Estimation
 * Estimate the fundamental frequency (F0) contour from speech recordings.
 * Based on the Annotation Pro F0 estimation module (estimation method: pYIN by Mauch & Dixon; Annotation Pro implementer: H. Kuczka).
 * See README.md for implementation notes and references.
 *
 * Plugin Author: Katarzyna Klessa
 * Plugin Created: 2017-05-20
 *
 * Plugin Version: 1.0
 *
 */


using AnnotationPro.Logic;
using AnnotationPro.Presentation;  
using AnnotationPro.Statistics;
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
                        
            
            var estimator = new FundamentalFrequencyEstimator(audio);
            
            LayerObject layer = estimator.Estimate(1000, audio.SamplesCount-1000, 40, 20); 
            
            editor.AnnotationLayers.AddNewLayer(layer);

            editor.SaveAnnotation();

            
           editor.RefreshLayers(false);

        }

       
    }
}
