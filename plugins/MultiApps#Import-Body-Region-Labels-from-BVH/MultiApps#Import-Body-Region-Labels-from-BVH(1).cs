using System;
using System.IO;
using System.Windows.Forms;
using AnnotationPro.Logic;
using AnnotationPro.Presentation;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        // =====================================================
        // CONFIGURATION
        // =====================================================

        private readonly string[] referenceLayerNames =
        {
            "New_Import_HeadPositionY",
            "New_Import_NeckPositionY",
            "New_Import_ChestPositionY",
            "New_Import_MiddleSpinePositionY",
            "New_Import_LowerSpinePositionY"
        };

        private readonly string sourceLayerName = "New_Import_RHandPositionY";
        private readonly string resultLayerName = "BVH_RHandBodyRegion";

        // =====================================================
        // FIELDS
        // =====================================================

        private Annotation annotation;

        // =====================================================
        // MAIN
        // =====================================================

        public void Run(AnnotationEditor editor)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                annotation = editor.Synchronizer.DataServer.Annotation;

                LayerObject sourceLayer = annotation.Layers.FindByName(sourceLayerName);

                if (sourceLayer == null)
                {
                    MessageBox.Show(
                        "Source layer not found:\n" + sourceLayerName,
                        "BVH Import");
                    return;
                }

                if (sourceLayer.Segments.Count == 0)
                {
                    MessageBox.Show(
                        "Source layer contains no segments:\n" + sourceLayerName,
                        "BVH Import");
                    return;
                }

                LayerObject[] referenceLayers = new LayerObject[referenceLayerNames.Length];

                int i;

                for (i = 0; i < referenceLayerNames.Length; i++)
                {
                    referenceLayers[i] =
                        annotation.Layers.FindByName(referenceLayerNames[i]);

                    if (referenceLayers[i] == null)
                    {
                        MessageBox.Show(
                            "Reference layer not found:\n" +
                            referenceLayerNames[i],
                            "BVH Import");
                        return;
                    }

                    if (referenceLayers[i].Segments.Count == 0)
                    {
                        MessageBox.Show(
                            "Reference layer contains no segments:\n" +
                            referenceLayerNames[i],
                            "BVH Import");
                        return;
                    }
                }

                LayerObject resultLayer =
                    annotation.Layers.FindByName(resultLayerName);

                if (resultLayer == null)
                {
                    resultLayer = editor.AnnotationLayers.AddNewLayer();
                    resultLayer.Name = resultLayerName;
                }

                resultLayer.Segments.Clear();

                foreach (SegmentObject sourceSegment in sourceLayer.Segments)
                {
                    string region =
                        DetermineRegion(sourceSegment, referenceLayers);

                    SegmentObject newSegment =
                        resultLayer.Segments.AddNew();

                    newSegment.Start = sourceSegment.Start;
                    newSegment.End = sourceSegment.End;

                    newSegment.Label = region;
                    newSegment.Parameter1 = sourceSegment.Label;
                }

                if (editor.WorkspaceModeEnabled)
                {
                    editor.SaveAnnotation();
                }

                editor.RefreshLayers(false);
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        // =====================================================
        // BODY REGION DETECTION
        // =====================================================

        private string DetermineRegion(
            SegmentObject sourceSegment,
            LayerObject[] referenceLayers)
        {
            float sourceY;

            if (!float.TryParse(sourceSegment.Label, out sourceY))
            {
                return "Unknown";
            }

            float centerTime =
                sourceSegment.Start +
                (sourceSegment.Duration / 2.0f);

            float?[] referenceY =
                new float?[referenceLayerNames.Length];

            int i;

            for (i = 0; i < referenceLayers.Length; i++)
            {
                SegmentObject refSegment =
                    FindSegmentAtTime(
                        referenceLayers[i],
                        centerTime);

                if (refSegment == null)
                {
                    referenceY[i] = null;
                    continue;
                }

                float value;

                if (float.TryParse(refSegment.Label, out value))
                {
                    referenceY[i] = value;
                }
                else
                {
                    referenceY[i] = null;
                }
            }

            for (i = 0; i < referenceLayerNames.Length - 1; i++)
            {
                if (!referenceY[i].HasValue)
                {
                    continue;
                }

                if (!referenceY[i + 1].HasValue)
                {
                    continue;
                }

                float upperValue =
                    referenceY[i].Value;

                float lowerValue =
                    referenceY[i + 1].Value;

                float top =
                    Math.Max(upperValue, lowerValue);

                float bottom =
                    Math.Min(upperValue, lowerValue);

                if (sourceY <= top &&
                    sourceY >= bottom)
                {
                    return GetRegionName(i);
                }
            }

            if (referenceY[0].HasValue)
            {
                if (sourceY > referenceY[0].Value)
                {
                    return "AboveHead";
                }
            }

            if (referenceY[referenceY.Length - 1].HasValue)
            {
                if (sourceY <
                    referenceY[referenceY.Length - 1].Value)
                {
                    return "BelowTorso";
                }
            }

            return "Unknown";
        }

        private SegmentObject FindSegmentAtTime(
            LayerObject layer,
            float time)
        {
            foreach (SegmentObject segment in layer.Segments)
            {
                if (time >= segment.Start &&
                    time <= segment.End)
                {
                    return segment;
                }
            }

            return null;
        }

        private string GetRegionName(int index)
        {
            switch (index)
            {
                case 0:
                    return "Head-Neck";

                case 1:
                    return "Neck-Chest";

                case 2:
                    return "Chest-MiddleSpine";

                case 3:
                    return "MiddleSpine-LowerSpine";

                default:
                    return "Unknown";
            }
        }

        // =====================================================
        // ERROR LOGGING
        // =====================================================

        private void WriteErrorLog(Exception ex)
        {
            try
            {
                string path =
                    Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.MyDocuments),
                        "plugin-errors.txt");

                File.AppendAllText(
                    path,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    " | " +
                    ex.ToString() +
                    Environment.NewLine +
                    Environment.NewLine);
            }
            catch
            {
            }
        }
    }
}
