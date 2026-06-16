/* 
 * IMPORT: txt 
 *
 * FOR WORKSPACE MODE
 * 
 * Plugin Author: Katarzyna Klessa
 * Plugin Created: 2015-06-19
 * 
 */

using System;
using System.Windows.Forms;
using AnnotationPro.Plugin;
using AnnotationPro.Presentation;
using AnnotationPro.Logic;
using System.IO;
using System.Collections.Generic;
using AnnotationPro.Statistics;
using System.Text;


namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        const string separator = "\t";
        const string txtExtension = ".txt";


        public void Run(AnnotationEditor editor)
        {

            Annotation annotation = editor.Synchronizer.DataServer.Annotation;
            Audio audio = editor.Synchronizer.DataServer.Audio;


            // An annotation must be open
            if (annotation.FilePath == string.Empty)
            {
                MessageBox.Show("Open annotation file to add TXT layers", Application.ProductName);
                return;
            }

            editor.Cursor = Cursors.WaitCursor;

            // ANT file folder
            string antFolder = Path.GetDirectoryName(annotation.FilePath);

			string antTitle = Path.GetFileNameWithoutExtension(annotation.FilePath);

			// Use a TXT file with the same base name as the ANT file
			string txtFileName = antTitle + txtExtension;
			// Full path to the TXT file corresponding to the ANT file
			string txtFilePath = Path.Combine(antFolder, txtFileName);

			// Import the TXT file if it exists
			if (File.Exists(txtFilePath))
			{
				string content = File.ReadAllText(txtFilePath);

				SegmentObject segment = new SegmentObject();
				segment.Start = audio.SamplesCount * 0.1f;
				segment.End = audio.SamplesCount * 0.9f;
				segment.Label = content;
				
				LayerObject txtLayer = editor.AnnotationLayers.AddNewLayer();
				txtLayer.Name = "Phrase";					
				txtLayer.Segments.Add(segment);
			}

			//annotation.IsModified = true;

            editor.RefreshLayers(false);

            if (editor.WorkspaceModeEnabled)
            {
                editor.SaveAnnotation();
            }        
		}
    }
}
