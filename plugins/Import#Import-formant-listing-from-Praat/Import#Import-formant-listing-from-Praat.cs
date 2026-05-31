/* 
 * IMPORT: Praat formant listing (TIMESTAMP_SEC, F1, F2, F3, F4), column headings: Time_s, F1_Hz, F2_Hz, F3_Hz, F4_Hz.
 *
 * FOR WORKSPACE MODE
 * 
 * Plugin Author: Katarzyna Klessa
 * Plugin Created: 2017-10-06
 * 
 */

using System;
using System.Windows.Forms;
using AnnotationPro.Plugin;
using AnnotationPro.Presentation;
using AnnotationPro.Logic;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using AnnotationPro.Statistics;
using System.Text;


namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
		const string separator = "\t";
        const string csvExtension = ".csv";

        public void Run(AnnotationEditor editor)
        {

            Annotation annotation = editor.Synchronizer.DataServer.Annotation;
            //Audio audio = editor.Synchronizer.DataServer.Audio;


            // musi byæ otwarta anotacja
            if (annotation.FilePath == string.Empty)
            {
                MessageBox.Show("Open formant listing file to add layers F1, F2, F3, F4", Application.ProductName);
                return;
            }

            editor.Cursor = Cursors.WaitCursor;

            // folder ANT
            string antFolder = Path.GetDirectoryName(annotation.FilePath);

            // title ANT
            string antTitle = Path.GetFileNameWithoutExtension(annotation.FilePath);
			
			// formant listing file
			string formantListingFileName = antTitle + csvExtension;
			
			// formant listing file from ANT
            string formantListingFilePath = Path.Combine(antFolder, formantListingFileName);

                // import if formant listing exists
                if (File.Exists(formantListingFilePath))
                {
                    LayerCollection formantListingLayers = annotation.ImportLayersFromPraatFormants(formantListingFilePath, annotation.Samplerate);
                    foreach (var formantListingLayer in formantListingLayers)
					{
						annotation.Layers.Add(formantListingLayer);
						formantListingLayer.BackColor = Color.LightGray;
					}
                    
                }

            editor.SaveAnnotation();

            editor.Cursor = Cursors.Default;

            editor.Refresh();
        }
    }
}