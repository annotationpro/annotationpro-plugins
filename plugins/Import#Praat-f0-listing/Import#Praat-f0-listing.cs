/*
 * Plugin Name: Import - Praat F0 Listing
 * Plugin Description: Imports Praat F0 listing TXT file into Annotation Pro layer. Segment label is F0 value at a given time point.
 * Plugin Created: 2026-06-23
 * Plugin Id: 4b8e2a91
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
        private Annotation annotation;
        private Audio audio;

        private readonly string outputLayerName = "F0_Praat";
        private readonly string parameter1Name = "F0 value";
        private readonly string parameter2Name = "Time s";
        private readonly string parameter3Name = "Parameter 3";

        public void Run(AnnotationEditor editor)
        {
            try
            {
                editor.Cursor = Cursors.WaitCursor;

                annotation = editor.Synchronizer.DataServer.Annotation;
                audio = editor.Synchronizer.DataServer.Audio;

                string fileName = SelectInputFile();

                if (fileName == string.Empty)
                {
                    return;
                }

                List<F0Point> points = ReadPraatF0Listing(fileName);

                if (points.Count == 0)
                {
                    MessageBox.Show("No F0 points found in file.", Application.ProductName);
                    return;
                }

                LayerObject layer = editor.AnnotationLayers.AddNewLayer();
                layer.Name = outputLayerName;
                layer.Parameter1Name = parameter1Name;
                layer.Parameter2Name = parameter2Name;
                layer.Parameter3Name = parameter3Name;
                layer.ShowAsChart = true;
                layer.ChartMinimum = 0;
                layer.ChartMaximum = 500;

                ImportPointsToLayer(points, layer);

                annotation.IsModified = true;
                editor.RefreshLayers(false);

                if (editor.WorkspaceModeEnabled)
                {
                    editor.SaveAnnotation();
                }

                MessageBox.Show("Imported " + points.Count.ToString() + " F0 points to layer '" + outputLayerName + "'.", Application.ProductName);
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

        private string SelectInputFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Praat F0 listing";
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (annotation.FilePath != null && annotation.FilePath != string.Empty)
            {
                dialog.InitialDirectory = Path.GetDirectoryName(annotation.FilePath);
            }

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return string.Empty;
            }

            return dialog.FileName;
        }

        private List<F0Point> ReadPraatF0Listing(string fileName)
        {
            List<F0Point> points = new List<F0Point>();
            string[] lines = File.ReadAllLines(fileName);

            foreach (string line in lines)
            {
                string trimmed = line.Trim();

                if (trimmed == string.Empty)
                {
                    continue;
                }

                if (trimmed.StartsWith("Time", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string[] parts = trimmed.Split(new char[] { ' ', '\t', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2)
                {
                    continue;
                }

                double time = 0.0;
                double f0 = 0.0;

                if (TryParseDouble(parts[0], out time) && TryParseDouble(parts[1], out f0))
                {
                    F0Point point = new F0Point();
                    point.TimeSeconds = time;
                    point.F0 = f0;
                    points.Add(point);
                }
            }

            return points;
        }

        private void ImportPointsToLayer(List<F0Point> points, LayerObject layer)
        {
            int samplerate = annotation.Samplerate;

            if (samplerate <= 0)
            {
                samplerate = audio.Samplerate;
            }

            int index = 0;

            while (index < points.Count)
            {
                F0Point point = points[index];

                float start = (float)(point.TimeSeconds * samplerate);
                float duration = GetDurationInSamples(points, index, samplerate);

                string f0Text = point.F0.ToString("0.000000", CultureInfo.InvariantCulture);
                string timeText = point.TimeSeconds.ToString("0.000000", CultureInfo.InvariantCulture);

                SegmentObject segment = new SegmentObject(start, duration, f0Text);
                segment.Parameter1 = f0Text;
                segment.Parameter2 = timeText;
                segment.Parameter3 = string.Empty;

                layer.Segments.Add(segment);

                index++;
            }
        }

        private float GetDurationInSamples(List<F0Point> points, int index, int samplerate)
        {
            double durationSeconds = 0.01;

            if (points.Count > 1)
            {
                if (index < points.Count - 1)
                {
                    durationSeconds = points[index + 1].TimeSeconds - points[index].TimeSeconds;
                }
                else
                {
                    durationSeconds = points[index].TimeSeconds - points[index - 1].TimeSeconds;
                }
            }

            if (durationSeconds <= 0.0)
            {
                durationSeconds = 0.01;
            }

            return (float)(durationSeconds * samplerate);
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

        private void LogError(Exception ex)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (annotation != null && annotation.FilePath != null && annotation.FilePath != string.Empty)
            {
                folder = Path.GetDirectoryName(annotation.FilePath);
            }

            File.AppendAllText(
                Path.Combine(folder, "plugin-errors.txt"),
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine + ex.ToString() + Environment.NewLine);
        }

        private class F0Point
        {
            public double TimeSeconds { get; set; }
            public double F0 { get; set; }
        }
    }
}
