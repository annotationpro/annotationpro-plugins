/* 
 * Plugin Name: F0 Estimation
 * Plugin Description: Estimates fundamental frequency values using Annotation Pro Fundamental Frequency Estimator (based on Mauch, M., & Dixon, S. (2014). PYIN: A fundamental frequency estimator using probabilistic threshold distributions. 2014 IEEE International Conference on Acoustics, Speech and Signal Processing (ICASSP), 659-663). Annotation Pro implementer: Hanna Kuczka. 
 * Programming API: https://annotationpro.org/api/ [Access date: 2026-01-20]
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
