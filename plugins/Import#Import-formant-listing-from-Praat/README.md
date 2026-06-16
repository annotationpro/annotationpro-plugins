# Annotation Pro Praat Formant Listing Import Plugin

## Overview

This plugin imports Praat formant listing data into an Annotation Pro
workspace.

It expects a Praat formant listing file in CSV format containing the
following columns:

* `Time\_s`
* `F1\_Hz`
* `F2\_Hz`
* `F3\_Hz`
* `F4\_Hz`

The plugin creates annotation layers for the imported formants.

## Author

* Plugin Author: Katarzyna Klessa
* Created: 2017-10-06

## Requirements

* Annotation Pro
* An open annotation (`.ant`) file
* A matching Praat formant listing CSV file

## File Naming Convention

The plugin looks for a CSV file in the same directory as the opened
annotation file.

Example:

&#x20;   project.ant
    project.csv


The CSV file must have the same filename as the annotation file (only
the extension changes).

## Usage

1. Open an annotation file in Annotation Pro.
2. Run the plugin.
3. The plugin searches for the corresponding CSV formant listing.
4. If found, it imports the formants as new layers:

   * F1
   * F2
   * F3
   * F4
5. Imported layers are assigned a light gray background.
6. The annotation is saved automatically.

## Behavior

If no annotation file is open, the plugin displays:

&#x20;   Open formant listing file to add layers F1, F2, F3, F4


If the matching CSV file does not exist, no layers are imported.

## Source Structure

Main class:

&#x20;   AnnotationPlugin : IAnnotationPlugin


Main method:

&#x20;   Run(AnnotationEditor editor)


Processing steps:

1. Get current annotation.
2. Check annotation path.
3. Locate the annotation folder.
4. Build the expected CSV filename.
5. Import Praat formant layers.
6. Add layers to the annotation.
7. Save and refresh the editor.

## 

