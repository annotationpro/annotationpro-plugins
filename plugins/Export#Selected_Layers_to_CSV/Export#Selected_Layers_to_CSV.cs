/* 
 * Plugin Name: 
 * Plugin Description: Plugin exports selected layers (named as specified in the plugin - layerNames) to a CSV file for one annotation file or file collection (Workspace Mode).
 * Plugin Author: Wojciech Klessa & Katarzyna Klessa
 * Plugin Created: 2018-03-28
 * 
 * Plugin Id: 7836867a

 * Plugin Version: 1.5
 * 
 */

using System;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AnnotationPro.Plugin;
using AnnotationPro.Logic;
using AnnotationPro.Presentation;
using System.Diagnostics;
using System.Collections.Generic;

namespace AnnotationPro.Plugin {
  public class AnnotationPlugin : IAnnotationPlugin {
	public void Run( AnnotationEditor editor ) {

	  // BEGIN USER CODE
	  Annotation annotation = editor.Synchronizer.DataServer.Annotation;
	  Audio audio = editor.Synchronizer.DataServer.Audio;

	  // export only for saved annotation
	  if ( annotation.FilePath == string.Empty )
		return;

	  // folder with ant file(s)
	  string folder = Path.GetDirectoryName( annotation.FilePath );

	  //yyyy-MM-dd-HH-mm-ss
	  string timestamp = DateTime.Now.ToString( "yyyy-MM-dd-HH" );

	  // output path
	  string exportPath = Path.Combine( folder, "Export Layers CSV - " + timestamp + ".csv" );

	  // old csv contents
	  string oldCsvContents = string.Empty;
	  if ( File.Exists( exportPath ) ) {
		oldCsvContents = File.ReadAllText( exportPath );
	  }

	  ////////////////////////////////////////////////////////////////
	  // EXPORT
	  
	  // define layer(s) to export, any number of layers can be defined
	  List<string> layerNames = new List<string>();
	 layerNames.Add( "ort" );
	 layerNames.Add( "word" );
	 layerNames.Add( "syllable" );
	 layerNames.Add( "phone" );	 
	  // csv
	  string csvContent = oldCsvContents;

	  // if the output file is empty add the header
	  if ( csvContent == string.Empty )
		csvContent = annotation.GetContentHeaderCSVString();

	  // search for the defined layers and export
	  foreach ( string layerName in layerNames ) {
		LayerObject layer = annotation.Layers.FindByName( layerName );
		if ( layer == null ) continue;
		csvContent += annotation.ExportLayerToCSVString( layer );
	  }
	  
	  File.WriteAllText( exportPath, csvContent );
	  
	  annotation.IsModified=true;
	  editor.SaveAnnotation();     
	  editor.RefreshLayers(false);

	  // END USER CODE
	}

	public void Info( string infoText ) {
	  MessageBox.Show( infoText, "Annotation Pro Plugin" );
	}

	public bool Question( string questionText ) {
	  if ( MessageBox.Show( questionText, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
		return true;

	  return false;
	}

	public string RemoveFileNameIllegalChars( string fileName ) {
	  string regex = String.Format( "[{0}]", Regex.Escape( new string( Path.GetInvalidFileNameChars() ) ) );
	  Regex removeInvalidChars = new Regex( regex, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.CultureInvariant );

	  return removeInvalidChars.Replace( fileName, "" );
	}
  }
}
