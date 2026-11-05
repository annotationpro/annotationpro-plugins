/* 
 * Plugin Name: Import - Import layers from EAF.
 * Plugin Description: Import Elan (EAF) Annotation to Annotation Pro (ANT / ANTx) format.
 * Programming API: https://annotationpro.org/api/ [Access date: 2026-01-20]
 *
 * Plugin Author: Wojciech Klessa & Katarzyna Klessa
 * Plugin Created: 2025-01-20. 
 * 
 * Plugin Version: 1.0
 * 
 */


using System;
using System.IO;
using System.Windows.Forms;
using AnnotationPro.Plugin;
using AnnotationPro.Presentation;
using AnnotationPro.Logic;
using AnnotationPro.Logic.ImportExport.ELAN;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        public void Run(AnnotationEditor editor)
        {
			var annotation = editor.Synchronizer.DataServer.Annotation;
			
            // Selecting the ANTx file
            string antxFilePath = annotation.FilePath;
            if (string.IsNullOrEmpty(antxFilePath))
            {
                MessageBox.Show("No open ANTX file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Find corresponding EAF
            string eafFilePath = Path.ChangeExtension(antxFilePath, ".eaf");
            if (!File.Exists(eafFilePath))
            {
                MessageBox.Show("EAF file not found: " + eafFilePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Creating ElanReader object with parameters
                ElanReader elanReader = new ElanReader(eafFilePath, annotation.Samplerate);
                
                // Importing EAF file
                LayerCollection importedLayers = elanReader.Read();
                
                // Adding layers to Annotation Pro
                foreach (var layer in importedLayers)
                {
                    editor.AnnotationLayers.AddNewLayer(layer);
                }

                if (editor.WorkspaceModeEnabled)
                {
					editor.SaveAnnotation();
                }
                
                editor.RefreshLayers(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error importing EAF file:" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
