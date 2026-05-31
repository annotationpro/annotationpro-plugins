/* 
 * EDIT: REPLACE PHONES AND SYLLABLES TO CV
 *
 * Annotation Pro Plugin
 *   
 * Plugin Author: Polskie Media
 * Plugin Updated: 2023-10-19
 * Plugin Id: 0dga6h0a
 * Plugin Version: 1.0
 * Plugin Description: It allows you to convert phones and sylables into CV patterns, where C stands for a consonant and V for a vowel. It needs layers containing Phones and Syllables to work.
 * 
 */
using AnnotationPro.Logic;
using AnnotationPro.Presentation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using static System.Net.Mime.MediaTypeNames;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        private readonly string PHONES_LAYER_NAME = "Phone_CV";
        private readonly string SYLLABLES_LAYER_NAME = "Syllable_CV";

        private Annotation annotation;
        private Audio audio;

        private enum CV
        {
            C,
            V
        }

        private readonly Dictionary<string, CV> phonesDict = new Dictionary<string, CV>()
        {
            {"a", CV.V},
            {"o", CV.V},
            {"e", CV.V},
            {"u", CV.V},
            {"i", CV.V},
            {"y", CV.V},
            {"o~", CV.V},
            {"e~", CV.V},
        };

        private LayerObject FindLayerByName(string name)
        {
            foreach (var layer in annotation.Layers)
            {
                if (layer.Name == name) return layer;
            }

            return null;
        }

        public void Run(AnnotationEditor editor)
        {
            annotation = editor.Synchronizer.DataServer.Annotation;
            audio = editor.Synchronizer.DataServer.Audio;

            var phones = FindLayerByName(PHONES_LAYER_NAME);
            var syllables = FindLayerByName(SYLLABLES_LAYER_NAME);

            if (phones == null || syllables == null)
            {
                Info(PHONES_LAYER_NAME + " and " + SYLLABLES_LAYER_NAME + " layers are required to run this plugin.");
                return;
            }

            LayerObject cvPhones = CreatePhonesCVLayer(phones);
            editor.AnnotationLayers.AddNewLayer(cvPhones);

            LayerObject cvSyllables = CreateSyllablesCVLayer(syllables, cvPhones);
            editor.AnnotationLayers.AddNewLayer(cvSyllables);

            editor.RefreshLayers(false);
        }

        private LayerObject CreatePhonesCVLayer(LayerObject phones)
        {
            var cvPhones = new LayerObject
            {
                Name = PHONES_LAYER_NAME + "_CV"
            };

            foreach (var segment in phones.Segments)
            {
                var phone = segment.Label;

                var cvSegment = new SegmentObject(segment.Start, segment.Duration, phone);
                cvPhones.Segments.Add(cvSegment);

                if (phonesDict.ContainsKey(phone))
                {
                    cvSegment.Label = phonesDict[phone].ToString();
                }
                else
                {
                    //cvSegment.BackColor = Color.Red;
                    //cvSegment.Parameter1 = "Label not found: " + phone;
                    cvSegment.Label = "C";
                }
            }

            return cvPhones;
        }

        private LayerObject CreateSyllablesCVLayer(LayerObject syllables, LayerObject cvPhones)
        {
            var cvSyllables = new LayerObject
            {
                Name = SYLLABLES_LAYER_NAME + "_CV"
            };

            foreach (var syllableSegment in syllables.Segments)
            {
                //var cvPhonesForSyllable = phonesCV.FindSegmentsInRange(syllableSegment.Start, syllableSegment.End, true);
                var cvPhonesForSyllable = FindCVPhonesForSyllable(syllableSegment, syllables, cvPhones);
                var cvSyllable = BuildSyllableLabelFromCVPhones(syllableSegment, cvPhonesForSyllable);
                cvSyllables.Segments.Add(cvSyllable);
            }

            return cvSyllables;
        }

        private SegmentCollection FindCVPhonesForSyllable(SegmentObject syllable, LayerObject syllables, LayerObject cvPhones)
        {
            var cvPhonesForSyllable = new SegmentCollection();

            bool NOT_FULL_IN_RANGE = false;
            var cvPhonesInRange = cvPhones.FindSegmentsInRange(syllable.Start, syllable.End, NOT_FULL_IN_RANGE);

            foreach (var svPhone in cvPhonesInRange)
            {
                var maxCoveredSyllable = FindMostCoveredSyllable(svPhone, syllables);
                if (maxCoveredSyllable != null)
                {
                    if (syllable == maxCoveredSyllable)
                    {
                        cvPhonesForSyllable.Add(svPhone);
                    }
                }
            }

            return cvPhonesForSyllable;
        }

        public float CalculateSegmentIntersection(SegmentObject segment, SegmentObject other)
        {
            float intersectionStart = Math.Max(segment.Start, other.Start);
            float intersectionEnd = Math.Min(segment.End, other.End);

            if (intersectionStart <= intersectionEnd)
            {
                return intersectionEnd - intersectionStart;
            }

            return 0;
        }

        private SegmentObject FindMostCoveredSyllable(SegmentObject cvPhone, LayerObject syllables)
        {
            SegmentObject maxCoveredSyllable = null;
            float maxCover = 0;

            foreach (var syllable in syllables.Segments)
            {
                float cover = CalculateSegmentIntersection(cvPhone, syllable);
                if (cover > maxCover)
                {
                    maxCover = cover;
                    maxCoveredSyllable = syllable;
                }
            }

            return maxCoveredSyllable;
        }

        private SegmentObject BuildSyllableLabelFromCVPhones(SegmentObject syllableSegment, SegmentCollection phonesCVSegments)
        {
            var segment = new SegmentObject(syllableSegment.Start, syllableSegment.Duration, "");

            StringBuilder sb = new StringBuilder();
            foreach (var phoneCVSegment in phonesCVSegments)
            {
                sb.Append(phoneCVSegment.Label);

                //if (phoneCVSegment.Label != CV.V.ToString() && phoneCVSegment.Label != CV.C.ToString())
                //{
                //    segment.BackColor = Color.Red;
                //    segment.Parameter1 = "Wrong Label: " + phoneCVSegment.Label;
                //}               
            }

            segment.Label = sb.ToString();

            return segment;
        }

        private void Info(string message)
        {
            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
