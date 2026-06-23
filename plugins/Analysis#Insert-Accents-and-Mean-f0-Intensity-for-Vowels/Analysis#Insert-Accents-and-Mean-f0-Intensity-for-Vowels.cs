/*
 * Plugin Name: Edit - Insert Accents F0 and Intensity
 * Plugin Description: Creates stress layer from word/phone layers and writes mean F0 from F0_Praat into Parameter2 and mean Intensity from Intensity_Praat into Parameter3 for vowels.
 * Plugin Created: 2026-06-23
 * Plugin Id: 2f8c6a91
 * Plugin Version: 1.0
 */

using AnnotationPro.Logic;
using AnnotationPro.Presentation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        private readonly string wordLayerName = "word";
        private readonly string phonesLayerName = "phone";
        private readonly string f0LayerName = "F0_Praat";
        private readonly string intensityLayerName = "Intensity_Praat";
        private readonly string outputLayerName = "stress";

        private readonly List<string> vowels = new List<string>() { "a", "e", "o", "i", "y", "I", "u", "o~", "e~" };

        private Annotation annotation;

        public void Run(AnnotationEditor editor)
        {
            try
            {
                editor.Cursor = Cursors.WaitCursor;

                annotation = editor.Synchronizer.DataServer.Annotation;

                LayerObject wordsLayer = FindLayer(wordLayerName);
                LayerObject phonesLayer = FindLayer(phonesLayerName);
                LayerObject f0Layer = FindLayer(f0LayerName);
                LayerObject intensityLayer = FindLayer(intensityLayerName);

                if (wordsLayer == null)
                {
                    MessageBox.Show("Layer '" + wordLayerName + "' does not exist.", Application.ProductName);
                    return;
                }

                if (phonesLayer == null)
                {
                    MessageBox.Show("Layer '" + phonesLayerName + "' does not exist.", Application.ProductName);
                    return;
                }

                if (f0Layer == null)
                {
                    MessageBox.Show("Layer '" + f0LayerName + "' does not exist. Import the Praat F0 listing first.", Application.ProductName);
                    return;
                }

                if (intensityLayer == null)
                {
                    MessageBox.Show("Layer '" + intensityLayerName + "' does not exist. Import the Praat Intensity listing first.", Application.ProductName);
                    return;
                }

                if (wordsLayer.Segments.Count == 0 || phonesLayer.Segments.Count == 0 || f0Layer.Segments.Count == 0 || intensityLayer.Segments.Count == 0)
                {
                    MessageBox.Show("Required layers must contain segments.", Application.ProductName);
                    return;
                }

                List<WordModel> words = DivideIntoWords(wordsLayer, phonesLayer);
                LayerObject output = CreateOutputLayer(editor, outputLayerName);

                output.Parameter1Name = "Stress degree";
                output.Parameter2Name = "Mean F0";
                output.Parameter3Name = "Mean Intensity";

                CalculateStress(words, output, f0Layer, intensityLayer);

                annotation.IsModified = true;
                editor.RefreshLayers(false);

                if (editor.WorkspaceModeEnabled)
                {
                    editor.SaveAnnotation();
                }

                MessageBox.Show("Done. Stress layer created with stress degree in Parameter1, mean F0 in Parameter2 and mean Intensity in Parameter3.", Application.ProductName);
            }
            catch (Exception ex)
            {
                LogError(ex);
                MessageBox.Show("Plugin error. Details were written to plugin-errors.txt.", Application.ProductName);
            }
            finally
            {
                editor.Cursor = Cursors.Default;
            }
        }

        private void CalculateStress(List<WordModel> words, LayerObject output, LayerObject f0Layer, LayerObject intensityLayer)
        {
            foreach (WordModel word in words)
            {
                CalculateWordStress(word, output, f0Layer, intensityLayer);
            }
        }

        private void CalculateWordStress(WordModel word, LayerObject output, LayerObject f0Layer, LayerObject intensityLayer)
        {
            int vowelIndex = 0;

            foreach (SegmentObject phone in word.Phones)
            {
                if (!IsVowel(phone.Label))
                {
                    CopyPhone(phone, "", "", "", output);
                    continue;
                }

                vowelIndex++;

                string meanF0 = GetMeanValueText(phone, f0Layer);
                string meanIntensity = GetMeanValueText(phone, intensityLayer);

                if (word.VowelsCount == 1)
                {
                    // In Polish, monosyllabic word stress tagging would require an additional distinction between content words and clitics; this plugin does not assign a class because that distinction is not implemented.
                    CopyPhone(phone, "0", meanF0, meanIntensity, output);
                }
                else if (word.VowelsCount == 2)
                {
                    if (vowelIndex == 1)
                    {
                        CopyPhone(phone, "3", meanF0, meanIntensity, output);
                    }
                    else
                    {
                        CopyPhone(phone, "1", meanF0, meanIntensity, output);
                    }
                }
                else
                {
                    if (vowelIndex % 2 == 1 && vowelIndex < word.VowelsCount - 1)
                    {
                        CopyPhone(phone, "2", meanF0, meanIntensity, output);
                    }
                    else if (vowelIndex % 2 == 0 && vowelIndex < word.VowelsCount - 1)
                    {
                        CopyPhone(phone, "1", meanF0, meanIntensity, output);
                    }
                    else if (vowelIndex == word.VowelsCount - 1)
                    {
                        CopyPhone(phone, "3", meanF0, meanIntensity, output);
                    }
                    else
                    {
                        CopyPhone(phone, "1", meanF0, meanIntensity, output);
                    }
                }
            }
        }

        private string GetMeanValueText(SegmentObject targetSegment, LayerObject sourceLayer)
        {
            double sum = 0.0;
            int count = 0;

            foreach (SegmentObject sourceSegment in sourceLayer.Segments)
            {
                if (Overlaps(targetSegment, sourceSegment))
                {
                    double value = 0.0;

                    if (TryParseDouble(sourceSegment.Label, out value))
                    {
                        if (value > 0.0)
                        {
                            sum += value;
                            count++;
                        }
                    }
                }
            }

            if (count == 0)
            {
                return "";
            }

            return (sum / count).ToString("0.00", CultureInfo.InvariantCulture);
        }

        private bool Overlaps(SegmentObject a, SegmentObject b)
        {
            return b.Start < a.End && b.End > a.Start;
        }

        private bool TryParseDouble(string text, out double value)
        {
            value = 0.0;

            if (text == null)
            {
                return false;
            }

            text = text.Trim().Replace(",", ".");

            return Double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private void CopyPhone(SegmentObject phone, string stressDegree, string meanF0, string meanIntensity, LayerObject output)
        {
            SegmentObject segment = new SegmentObject(phone.Start, phone.Duration, stressDegree);
            segment.Parameter1 = stressDegree;
            segment.Parameter2 = meanF0;
            segment.Parameter3 = meanIntensity;
            output.Segments.Add(segment);
        }

        private LayerObject CreateOutputLayer(AnnotationEditor editor, string name)
        {
            LayerObject layer = editor.AnnotationLayers.AddNewLayer();
            layer.Name = name;
            return layer;
        }

        private List<WordModel> DivideIntoWords(LayerObject wordsLayer, LayerObject phonesLayer)
        {
            List<WordModel> words = new List<WordModel>();

            foreach (SegmentObject wordSegment in wordsLayer.Segments)
            {
                words.Add(ExtractWord(wordSegment, phonesLayer));
            }

            return words;
        }

        private WordModel ExtractWord(SegmentObject wordSegment, LayerObject phonesLayer)
        {
            WordModel word = new WordModel();

            foreach (SegmentObject phoneSegment in phonesLayer.Segments)
            {
                if (IsPhoneInWord(wordSegment, phoneSegment))
                {
                    word.Phones.Add(phoneSegment);

                    if (IsVowel(phoneSegment.Label))
                    {
                        word.VowelsCount++;
                    }
                }
            }

            return word;
        }

        private bool IsVowel(string label)
        {
            if (label == null)
            {
                return false;
            }

            return vowels.Contains(label.Trim());
        }

        private bool IsPhoneInWord(SegmentObject wordSegment, SegmentObject phoneSegment)
        {
            return phoneSegment.Start >= wordSegment.Start && phoneSegment.End <= wordSegment.End;
        }

        private LayerObject FindLayer(string layerName)
        {
            foreach (LayerObject layer in annotation.Layers)
            {
                if (layer.Name == layerName)
                {
                    return layer;
                }
            }

            return null;
        }

        private void LogError(Exception ex)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (annotation.FilePath != null && annotation.FilePath != string.Empty)
            {
                folder = Path.GetDirectoryName(annotation.FilePath);
            }

            File.AppendAllText(
                Path.Combine(folder, "plugin-errors.txt"),
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + ex.ToString() + Environment.NewLine);
        }
    }

    public class WordModel
    {
        public WordModel()
        {
            Phones = new List<SegmentObject>();
            VowelsCount = 0;
        }

        public List<SegmentObject> Phones { get; set; }
        public int VowelsCount { get; set; }
    }
}
