/*
 * Plugin Name: Import - EAF
 * Plugin Description: Imports annotation layers from a matching ELAN (.eaf) file.
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
			
            // Get the currently opened ANTX file
            string antxFilePath = annotation.FilePath;
            if (string.IsNullOrEmpty(antxFilePath))
            {
                MessageBox.Show("There is no ANTX file opened.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Find the corresponding EAF file
            string eafFilePath = Path.ChangeExtension(antxFilePath, ".eaf");
            if (!File.Exists(eafFilePath))
            {
                MessageBox.Show("An EAF file was not found: " + eafFilePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Create an ElanReader instance with the appropriate parameters
                ElanReader elanReader = new ElanReader(eafFilePath, annotation.Samplerate);
                
                // Import layers from the EAF file
                LayerCollection importedLayers = elanReader.Read();
                
                // Add imported layers to Annotation Pro
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
                MessageBox.Show("Error while importing an EAF file:" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
