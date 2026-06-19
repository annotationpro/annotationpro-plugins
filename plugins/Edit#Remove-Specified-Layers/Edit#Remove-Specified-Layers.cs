/*
 * Plugin Name: Remove Selected Layers
 * Plugin Description: Removes specified layers from the current annotation.
 * Plugin Created: 2026-06-19
 * Plugin Id: 7a3f9c1e
 * Plugin Version: 1.0
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using AnnotationPro.Logic;
using AnnotationPro.Presentation;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        private readonly List<string> layersToDelete = new List<string>()
        {
            "Phrase",
            "Word",
            "Syllable",
            "Phone"
        };

        public void Run(AnnotationEditor editor)
        {
            Annotation annotation = editor.Synchronizer.DataServer.Annotation;

            try
            {
                editor.Cursor = Cursors.WaitCursor;

                int deletedCount = 0;

                foreach (string layerName in layersToDelete)
                {
                    LayerObject layer = annotation.Layers.FindByName(layerName);

                    if (layer == null)
                    {
                        continue;
                    }

                    annotation.Layers.Remove(layer);
                    deletedCount++;
                }

                if (deletedCount == 0)
                {
                    MessageBox.Show("No matching layers found to delete.", "Annotation Pro");
                    return;
                }

                annotation.IsModified = true;

                editor.RefreshLayers(false);

                if (editor.WorkspaceModeEnabled)
                {
                    editor.SaveAnnotation();
                }

                MessageBox.Show("Deleted layers: " + deletedCount.ToString(), "Annotation Pro");
            }
            catch (Exception ex)
            {
                LogError(annotation, ex);
                MessageBox.Show("Plugin error. Details saved to plugin-errors.txt.", "Annotation Pro");
            }
            finally
            {
                editor.Cursor = Cursors.Default;
            }
        }

        private void LogError(Annotation annotation, Exception ex)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (annotation.FilePath != string.Empty)
            {
                folder = Path.GetDirectoryName(annotation.FilePath);
            }

            string logPath = Path.Combine(folder, "plugin-errors.txt");

            File.AppendAllText(
                logPath,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine +
                ex.ToString() + Environment.NewLine + Environment.NewLine);
        }
    }
}
