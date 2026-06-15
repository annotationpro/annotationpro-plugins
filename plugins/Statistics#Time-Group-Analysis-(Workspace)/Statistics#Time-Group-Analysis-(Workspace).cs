/* 
 * STATISTICS: TIME GROUP ANALYSIS
 *
 * FOR WORKSPACE MODE
 * 
 * Analysis method based on TGA by Dafydd Gibbon 
 * http://wwwhomes.uni-bielefeld.de/gibbon/tga-3.01.html
 * 
 * Plugin Author: Katarzyna Klessa & Wojciech Klessa
 * Plugin Created: 2015-01-28
 * 
 */

using System;
using System.Windows.Forms;
using AnnotationPro.Plugin;
using AnnotationPro.Presentation;
using AnnotationPro.Logic;
using System.IO;
using System.Collections.Generic;
using AnnotationPro.Statistics;
using System.Text;


namespace AnnotationPro.Plugin
{
  public class AnnotationPlugin : IAnnotationPlugin
  {

    public void Run(AnnotationEditor editor)
    {

      string layerName = "Syllable_All speech";

      Annotation annotation = editor.Synchronizer.DataServer.Annotation;
      //Audio audio = editor.Synchronizer.DataServer.Audio;

      LayerObject srcLayer = null;
      foreach (LayerObject layer in annotation.Layers) {
        if (layer.Name == layerName) {
          srcLayer = layer;
          break;
        }
      }

      if (srcLayer == null) {
        MessageBox.Show("Layer '" + layerName + "' doesn't exist.", Application.ProductName);
        return;
      }

      editor.Cursor = Cursors.WaitCursor;

      TGA(editor, srcLayer);

      editor.Cursor = Cursors.Default;

      editor.RefreshLayers(false);
    }


    /// <summary>
    /// Wykonuje TGA dla podanej warstwy
    /// </summary>
    /// <param name="layer">Warstwa dla której zostanie wykonane TGA</param>
    public void TGA(AnnotationEditor editor, LayerObject srcLayer)
    {
      // result layer for parameters
      LayerObject layer = srcLayer.Copy(true);
      editor.AnnotationLayers.AddNewLayer(layer);
      layer.Name = "TGA Parameters";
      layer.Parameter1Name = "Timestamp";
      layer.Parameter2Name = "Duration";
      layer.Parameter3Name = "? Duration";

      // result layer for time group summary (slope, intercept)
      LayerObject resultLayer = editor.AnnotationLayers.AddNewLayer();
      resultLayer.Name = "TGA Result";
      resultLayer.Parameter1Name = "Slope";
      resultLayer.Parameter2Name = "Intercept";
      resultLayer.Parameter3Name = "nPVI_syll";

      // split into phrases (by pauses)
      List<SegmentCollection> phrases = editor.Synchronizer.DataServer.Annotation.SplitByPauses(layer);

      // linear regression for each phrase
      foreach (SegmentCollection phrase in phrases) {
        SegmentObject resultSegment = TGALinearRegression(phrase, editor.Synchronizer.DataServer.Annotation.Samplerate);
        resultLayer.Segments.Add(resultSegment);
        resultSegment.Label = "TG " + phrases.IndexOf(phrase).ToString();
      }
    }

    /// <summary>
    /// Linear regression for TGA
    /// </summary>
    /// <param name="segments">Input segments</param>
    /// <param name="samplerate">Annotation samplerate to calculate timestamps from samples</param>
    /// <returns>Output segment with R script inside</returns>
    public SegmentObject TGALinearRegression(SegmentCollection segments, int samplerate)
    {

      // prepare X and Y arrays

      double position = 0;
      SegmentObject firstSegment = null;
      SegmentObject lastSegment = null;

      double[] X = new double[segments.Count];
      double[] Y = new double[segments.Count];
      List<string> labels = new List<string>();

      foreach (SegmentObject segment in segments) {
        if (firstSegment == null) firstSegment = segment;

        int index = segments.IndexOf(segment);

        X[index] = (position /*+ segment.Duration*/ ) / samplerate * 1000;
        Y[index] = segment.Duration / samplerate * 1000;

        labels.Add(segment.Label);

        segment.Parameter1 = X[index].ToString("0.00");
        segment.Parameter2 = Y[index].ToString("0.00");

        if (lastSegment != null) {
          segment.Parameter3 = ((segment.Duration - lastSegment.Duration) / samplerate * 1000).ToString("0.00");
        } else {
          segment.Parameter3 = (segment.Duration / samplerate * 1000).ToString("0.00");
        }
        position += segment.Duration;
        lastSegment = segment;
      }

      double slope = 0;
      double intercept = 0;
      double rsquared = 0;

      // linear regression

      GeneralStatistics.LinearRegression(X, Y, out rsquared, out intercept, out slope);

      // create R Script for segment

      R r = new R();

      StringBuilder rscript = new StringBuilder();
      rscript.AppendLine("# TGA Linear Regression");
      rscript.AppendLine("x <- " + r.Vector(X));
      rscript.AppendLine("y <- " + r.Vector(Y));
      rscript.AppendLine("l <- " + r.Vector(labels));

      rscript.AppendLine("par(mfrow=c(2,1))");
      rscript.AppendLine("plot(x,y,ylim=range(0,y),type=\"s\", pch=9, lwd=1,col=\"black\", xlab=\"Timestamp (ms)\", ylab=\"Duration (ms)\")");
      rscript.AppendLine("title(main=\"Time Group Analysis, Slope: " + slope.ToString("0.000") + ", Intercept: " + intercept.ToString("0.000") + "\")");
      rscript.AppendLine("points(x,y,pch=16)");
      rscript.AppendLine("text(x,y,l,pos=3,cex=0.7)");
      rscript.AppendLine("abline(" + intercept.ToString() + "," + slope.ToString() + ",lwd=1, lty=\"dashed\", col=\"black\")");

      rscript.AppendLine("plot(x,y,ylim=rev(range(0,y)),type=\"h\",pch=9, lwd=1,col=\"dark green\", xlab=\"Timestamp (ms)\", ylab=\"Duration (ms)\")");
      rscript.AppendLine("points(x,y,pch=16,col=\"dark green\")");
      rscript.AppendLine("text(x,y,l,pos=1,cex=0.7)");
      rscript.AppendLine("abline(" + intercept.ToString() + "," + slope.ToString() + ",lwd=1, lty=\"dashed\", col=\"black\")");
      rscript.AppendLine("title(sub=\"Analysis method based on TGA by Dafydd Gibbon (http://wwwhomes.uni-bielefeld.de/gibbon/tga-3.01.html)\", cex.sub=0.5)");

      rscript.AppendLine("par(mfrow=c(1,1))");
      rscript.AppendLine("plot(x,y,ylim=range(0,y),type=\"s\", pch=9, lwd=1,col=\"black\", main=\"Time Group Analysis, Slope: " + slope.ToString("0.000") + ", Intercept: " + intercept.ToString("0.000") + "\", xlab=\"Timestamp (ms)\", ylab=\"Duration (ms)\")");
      rscript.AppendLine("points(x,y,pch=16)");
      rscript.AppendLine("text(x,y,l,pos=3,cex=0.7)");
      rscript.AppendLine("abline(" + intercept.ToString() + "," + slope.ToString().Replace(",", ".") + ",lwd=1, lty=\"dashed\", col=\"black\")");
      rscript.AppendLine("title(sub=\"Analysis method based on TGA by Dafydd Gibbon (http://wwwhomes.uni-bielefeld.de/gibbon/tga-3.01.html)\", cex.sub=0.5)");

      rscript.AppendLine("plot(x,y,ylim=rev(range(0,y)),type=\"h\",pch=9, lwd=1,col=\"dark green\", main=\"Time Group Analysis, Slope: " + slope.ToString("0.000") + ", Intercept: " + intercept.ToString("0.000") + "\", xlab=\"Timestamp (ms)\", ylab=\"Duration (ms)\")");
      rscript.AppendLine("points(x,y,pch=16,col=\"dark green\")");
      rscript.AppendLine("text(x,y,l,pos=1,cex=0.7)");
      rscript.AppendLine("abline(" + intercept.ToString() + "," + slope.ToString().Replace(",", ".") + ",lwd=1, lty=\"dashed\", col=\"black\")");
      rscript.AppendLine("title(sub=\"Analysis method based on TGA by Dafydd Gibbon (http://wwwhomes.uni-bielefeld.de/gibbon/tga-3.01.html)\", cex.sub=0.5)");

      string info = "Time Group Analysis (LR): slope: " + slope.ToString() + ", intercept: " + intercept.ToString();

      // output segment

      SegmentObject seg = new SegmentObject(firstSegment.Start, (float)position, info);
      seg.Parameter1 = slope.ToString();
      seg.Parameter2 = intercept.ToString();
      seg.Parameter3 = GeneralStatistics.nPVI(Y).ToString();
      seg.RScript = rscript.ToString();

      return seg;
    }

  }
}
