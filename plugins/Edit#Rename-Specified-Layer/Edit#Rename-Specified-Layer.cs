/* 
 * Plugin Name: Rename specified layers
 * Plugin Description: Renames layers (specified by ‘layerName’ and 'newlayerName') for one or more files (Workspace Mode)
 * Plugin Author: Katarzyna Klessa
 * 
 * Plugin Version: 1.0
 * 
 */

using System;
using System.Windows.Forms;
using AnnotationPro.Logic;
using AnnotationPro.Presentation;
using System.Collections.Generic;
using AnnotationPro.Statistics;
using System.Text;
//using System.Linq;

namespace AnnotationPro.Plugin
{
    public class AnnotationPlugin : IAnnotationPlugin
    {

        private Annotation annotation;
        private Audio audio;
        private AnnotationEditor editor;

        public void Run(AnnotationEditor editor)
        {
            string layerName = "NewLayer";
            string newLayerName = "Read";
			
			Annotation annotation = editor.Synchronizer.DataServer.Annotation;
            LayerObject srcLayer = null;
			
            foreach (LayerObject layerToRename in annotation.Layers)
            {
                if (layerToRename.Name == layerName)
                {
                    srcLayer = layerToRename;
                    break;
                }
            }

            if (srcLayer == null)
            {
                string fileName = System.IO.Path.GetFileName(annotation.FilePath);
                MessageBox.Show(string.Format("Layer {0} doesn't exists in file '{1}' ", layerName, fileName), Application.ProductName);
                return;
            }

            editor.Cursor = Cursors.WaitCursor;
            editor.Cursor = Cursors.Default;
			

            srcLayer.Name = newLayerName;

            annotation.IsModified = true;
            editor.SaveAnnotation();
            editor.Refresh();

        }
    }
}