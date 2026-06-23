/*
 * Plugin Name: Import - Praat Intensity Listing
 * Plugin Description: Imports Praat intensity listing TXT file into Annotation Pro layer. Segment label is intensity value at a given time point.
 * Plugin Created: 2026-06-23
 * Plugin Id: 9f3a7c1d
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

        private readonly string outputLayerName = "Intensity_Praat";
        private readonly string parameter1Name = "Intensity dB";
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

                List<IntensityPoint> points = ReadPraatIntensityListing(fileName);

                if (points.Count == 0)
                {
                    MessageBox.Show("No intensity points found in file.", Application.ProductName);
                    return;
                }

                LayerObject layer = editor.AnnotationLayers.AddNewLayer();
                layer.Name = outputLayerName;
                layer.Parameter1Name = parameter1Name;
                layer.Parameter2Name = parameter2Name;
                layer.Parameter3Name = parameter3Name;
                layer.ShowAsChart = true;
                layer.ChartMinimum = 0;
                layer.ChartMaximum = 100;

                ImportPointsToLayer(points, layer);

                annotation.IsModified = true;
                editor.RefreshLayers(false);

                if (editor.WorkspaceModeEnabled)
                {
                    editor.SaveAnnotation();
                }

                MessageBox.Show("Imported " + points.Count.ToString() + " intensity points to layer '" + outputLayerName + "'.", Application.ProductName);
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
            dialog.Title = "Select Praat intensity listing";
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

        private List<IntensityPoint> ReadPraatIntensityListing(string fileName)
        {
            List<IntensityPoint> points = new List<IntensityPoint>();
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
                double intensity = 0.0;

                if (TryParseDouble(parts[0], out time) && TryParseDouble(parts[1], out intensity))
                {
                    IntensityPoint point = new IntensityPoint();
                    point.TimeSeconds = time;
                    point.IntensityDb = intensity;
                    points.Add(point);
                }
            }

            return points;
        }

        private void ImportPointsToLayer(List<IntensityPoint> points, LayerObject layer)
        {
            int samplerate = annotation.Samplerate;

            if (samplerate <= 0)
            {
                samplerate = audio.Samplerate;
            }

            int index = 0;

            while (index < points.Count)
            {
                IntensityPoint point = points[index];

                float start = (float)(point.TimeSeconds * samplerate);
                float duration = GetDurationInSamples(points, index, samplerate);

                string intensityText = point.IntensityDb.ToString("0.000000", CultureInfo.InvariantCulture);
                string timeText = point.TimeSeconds.ToString("0.000000", CultureInfo.InvariantCulture);

                SegmentObject segment = new SegmentObject(start, duration, intensityText);
                segment.Parameter1 = intensityText;
                segment.Parameter2 = timeText;
                segment.Parameter3 = string.Empty;

                layer.Segments.Add(segment);

                index++;
            }
        }

        private float GetDurationInSamples(List<IntensityPoint> points, int index, int samplerate)
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

        private class IntensityPoint
        {
            public double TimeSeconds { get; set; }
            public double IntensityDb { get; set; }
        }
    }
}
