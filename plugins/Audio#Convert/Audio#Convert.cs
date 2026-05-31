//plugin sets audio parameters provided by private const = samplerate, channels, bits

using AnnotationPro.Logic;
using AnnotationPro.Presentation;
using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {
        private const int samplerate = 16000;
        private const int channels = 1;
        private const int bits = 16;

        private Annotation annotation;
        private Audio audio;

        public void Run(AnnotationEditor editor)
        {
            annotation = editor.Synchronizer.DataServer.Annotation;
            audio = editor.Synchronizer.DataServer.Audio;

            if (audio.Samplerate != samplerate || audio.BitsPerSample != bits || audio.Channels != channels)
            {
                annotation.Samplerate = samplerate;               
                audio.Convert(samplerate, bits, channels);
                editor.SaveAudio();
                editor.ZoomToFullAudio();
                editor.SaveAnnotation();
            }

        }
    }
}