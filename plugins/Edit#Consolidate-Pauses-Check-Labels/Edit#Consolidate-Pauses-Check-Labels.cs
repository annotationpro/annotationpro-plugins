using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

using AnnotationPro.Plugin;
using AnnotationPro.Logic;
using AnnotationPro.Presentation;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        // =========================
        // PARAMETRY
        // =========================

        // Nazwa warstwy wejściowej
        private string sourceLayerName = "Cisza w dialogu";

        // Nazwa warstwy wynikowej
        private string resultLayerName = "Pauzy w dialogu";

        // Maksymalna długość przerwy między segmentami
        // (wszystko powyżej traktowane jako realna pauza/dźwięk)
        private double maxGapMs = 250.0;

        // Maksymalna długość mikrosegmentu/szumu
        // Jeśli segment pomiędzy dwoma identycznymi tagami
        // jest dłuższy -> NIE scalaj
        private double maxNoiseSegmentMs = 120.0;

        // Minimalna długość "realnego" dźwięku
        // pomaga uniknąć błędnego scalania
        private double significantSoundMs = 150.0;

        public void Run(AnnotationEditor editor)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                Annotation annotation =
                    editor.Synchronizer.DataServer.Annotation;

                LayerObject sourceLayer =
                    annotation.Layers.FindByName(sourceLayerName);

                if (sourceLayer == null)
                {
                    MessageBox.Show(
                        "Nie znaleziono warstwy: " + sourceLayerName
                    );
                    return;
                }

                if (sourceLayer.Segments.Count == 0)
                {
                    MessageBox.Show(
                        "Warstwa nie zawiera segmentów."
                    );
                    return;
                }

                // =========================
                // NOWA WARSTWA
                // =========================

                LayerObject resultLayer =
                    editor.AnnotationLayers.AddNewLayer();

                resultLayer.Name = resultLayerName;

                List<SegmentObject> segments =
                    new List<SegmentObject>();

                int i;

                for (i = 0; i < sourceLayer.Segments.Count; i++)
                {
                    segments.Add(sourceLayer.Segments[i]);
                }

                if (segments.Count == 0)
                {
                    MessageBox.Show("Brak segmentów.");
                    return;
                }

                // =========================
                // GŁÓWNA LOGIKA
                // =========================

                SegmentObject current = CopySegment(segments[0]);

                for (i = 1; i < segments.Count; i++)
                {
                    SegmentObject next = segments[i];

                    // ---------------------------------
                    // TYLKO IDENTYCZNE TAGI
                    // ---------------------------------

                    bool sameLabel =
                        Normalize(current.Label) ==
                        Normalize(next.Label);

                    // ---------------------------------
                    // DŁUGOŚĆ PRZERWY
                    // ---------------------------------

                    double gapMs =
                        SamplesToMilliseconds(
                            next.Start - current.End,
                            annotation.Samplerate
                        );

                    // ---------------------------------
                    // CZY PRZERWA WYGLĄDA JAK SZUM?
                    // ---------------------------------

                    bool shortGap =
                        gapMs <= maxGapMs;

                    // ---------------------------------
                    // OCHRONA PRZED ŁĄCZENIEM
                    // REALNYCH DŹWIĘKÓW
                    // ---------------------------------

                    bool probablyRealSound =
                        gapMs >= significantSoundMs;

                    // ---------------------------------
                    // FINALNA DECYZJA
                    // ---------------------------------

                    bool merge =
                        sameLabel &&
                        shortGap &&
                        !probablyRealSound;

                    if (merge)
                    {
                        // rozszerz segment
                        current.End = next.End;
                    }
                    else
                    {
                        resultLayer.Segments.Add(current);
                        current = CopySegment(next);
                    }
                }

                // dodaj ostatni segment
                resultLayer.Segments.Add(current);

                // =========================
                // ODŚWIEŻ
                // =========================

                editor.RefreshLayers(false);

                if (editor.WorkspaceModeEnabled)
                {
                    editor.SaveAnnotation();
                }

                MessageBox.Show(
                    "Utworzono warstwę: " +
                    resultLayerName +
                    Environment.NewLine +
                    Environment.NewLine +
                    "Scalanie zakończone."
                );
            }
            catch (Exception ex)
            {
                try
                {
                    string annotationPath = "";

                    try
                    {
                        annotationPath =
                            editor.Synchronizer.DataServer.Annotation.FilePath;
                    }
                    catch
                    {
                        annotationPath =
                            Environment.GetFolderPath(
                                Environment.SpecialFolder.MyDocuments
                            );
                    }

                    string directory =
                        Path.GetDirectoryName(annotationPath);

                    string logPath =
                        Path.Combine(
                            directory,
                            "plugin-errors.txt"
                        );

                    File.AppendAllText(
                        logPath,
                        DateTime.Now.ToString() +
                        Environment.NewLine +
                        ex.ToString() +
                        Environment.NewLine +
                        Environment.NewLine
                    );
                }
                catch
                {
                }

                MessageBox.Show(
                    ex.Message,
                    "Plugin Error"
                );
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        // =========================
        // COPY SEGMENT
        // =========================

        private SegmentObject CopySegment(SegmentObject s)
        {
            SegmentObject n = new SegmentObject();

            n.Start = s.Start;
            n.End = s.End;
            n.Label = s.Label;

            n.Parameter1 = s.Parameter1;
            n.Parameter2 = s.Parameter2;
            n.Parameter3 = s.Parameter3;

            return n;
        }

        // =========================
        // NORMALIZE LABEL
        // =========================

        private string Normalize(string s)
        {
            if (s == null)
            {
                return "";
            }

            return s.Trim().ToLower();
        }

        // =========================
        // SAMPLE -> MS
        // =========================

        private double SamplesToMilliseconds(
            float samples,
            int samplerate
        )
        {
            return
                ((double)samples / (double)samplerate)
                * 1000.0;
        }
    }
}
