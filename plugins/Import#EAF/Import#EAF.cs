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
			
            // Pobranie aktualnego pliku ANTX
            string antxFilePath = annotation.FilePath;
            if (string.IsNullOrEmpty(antxFilePath))
            {
                MessageBox.Show("Brak otwartego pliku ANTX.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Znalezienie odpowiadającego pliku EAF
            string eafFilePath = Path.ChangeExtension(antxFilePath, ".eaf");
            if (!File.Exists(eafFilePath))
            {
                MessageBox.Show("Plik EAF nie został znaleziony: " + eafFilePath, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Tworzenie obiektu ElanReader z odpowiednimi parametrami
                ElanReader elanReader = new ElanReader(eafFilePath, annotation.Samplerate);
                
                // Importowanie warstw z EAF
                LayerCollection importedLayers = elanReader.Read();
                
                // Dodanie warstw do Annotation Pro
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
                MessageBox.Show("Błąd podczas importu pliku EAF:" + ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
