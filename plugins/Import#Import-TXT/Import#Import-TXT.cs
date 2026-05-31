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


            // musi byc otwarta anotacja
            if (annotation.FilePath == string.Empty)
            {
                MessageBox.Show("Open annotation file to add TXT layers", Application.ProductName);
                return;
            }

            editor.Cursor = Cursors.WaitCursor;

            // folder ANT
            string antFolder = Path.GetDirectoryName(annotation.FilePath);

			string antTitle = Path.GetFileNameWithoutExtension(annotation.FilePath);

			// chce zeby wzial tekst z pliku txt o nazwie takiej samej jak ant
			string txtFileName = antTitle + txtExtension;
			// txt file from ANT
			string txtFilePath = Path.Combine(antFolder, txtFileName);

			// import if txt exists
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