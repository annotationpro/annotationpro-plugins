using System;
using System.Windows.Forms;
using AnnotationPro.Plugin;
using AnnotationPro.Logic;
using AnnotationPro.Presentation;

namespace AnnotationPro.Plugin
{
  public class AnnotationPlugin : IAnnotationPlugin
  {
    public void Run( AnnotationEditor editor )
    {
     
      LayerObject layer = new LayerObject();
      layer = editor.SelectedLayer;

      if ( layer == null ) {
        MessageBox.Show( "Select layer with segments to consolidate.", "Annotation Pro Plugin" );
        return;
      }

      editor.Cursor = Cursors.WaitCursor;

      layer.Segments.Sort();

      SegmentCollection segmentsToDelete = new SegmentCollection();

      SegmentObject lastSegment = null;
      foreach ( SegmentObject segment in layer.Segments ) {
        if ( lastSegment == null ) {
          lastSegment = segment;
          continue;
        }
        if ( lastSegment.Label == "#$p" && segment.Label == "#$p" ) {
          lastSegment.End = segment.End;
          segmentsToDelete.Add( segment );
        }else {
          lastSegment = segment;
        }
      }

      foreach ( SegmentObject segment in segmentsToDelete ) {
        layer.Segments.Remove(segment);
      }


      editor.Cursor = Cursors.Default;

      editor.Synchronizer.DataServer.Annotation.IsModified = true;

      // END USER CODE
    }
  }
}
