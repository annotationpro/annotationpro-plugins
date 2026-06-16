# Annotation Pro Layer CSV Export Plugin

## Overview

This Annotation Pro plugin exports selected annotation layers into a CSV
file.

The plugin is designed for exporting data from one annotation file or a
collection of annotation files in Workspace Mode. It searches for
specific layers by name and appends their content to a CSV export file.

## Plugin Information

* **Plugin ID:** 7836867a
* **Version:** 1.5
* **Author:** Wojciech Klessa \& Katarzyna Klessa
* **Created:** 2018-03-28

## Description

The plugin exports predefined annotation layers into a CSV file.

Currently exported layers:

* `Interactive Unit`
* `Foregrounding Cluster`
* `Dynamic Musical Terms`
* `Peaks`

Additional layers can be exported by adding their names to the
`layerNames` list in the source code.

## How It Works

1. Checks whether an annotation file is open and saved.
2. Gets the folder containing the `.ant` annotation file.
3. Creates a CSV export filename using the current date and hour:

```{=html}
<!-- -->
```

&#x20;   Export Layers CSV - YYYY-MM-DD-HH.csv


Example:

&#x20;   Export Layers CSV - 2026-06-16-14.csv


4. If an export file already exists, the plugin loads the previous
content.
5. If the CSV file is new, it adds the annotation CSV header.
6. Searches for the selected layers.
7. Exports the layer contents into CSV format.
8. Saves the updated CSV file.
9. Refreshes Annotation Pro layers.

## Usage

1. Open an annotation file in Annotation Pro.
2. Make sure the annotation contains layers with the required names.
3. Run the plugin.
4. Find the exported CSV file in the same folder as the annotation.

## CSV Output

The CSV contains:

* Annotation metadata header
* Exported layer data
* Time information
* Layer intervals/segments
* Layer content

The exact format depends on Annotation Pro's CSV export function.

## Configuration

To change exported layers edit:

``` csharp
List<string> layerNames = new List<string>();

layerNames.Add("Interactive Unit");
layerNames.Add("Foregrounding Cluster");
layerNames.Add("Dynamic Musical Terms");
layerNames.Add("Peaks");
```

Example:

``` csharp
layerNames.Add("My New Layer");
```

## Workspace Mode

The plugin supports batch-style workflows where multiple annotation
files can be processed. Each annotation export is written to the
corresponding annotation folder.

## Error Handling

If no annotation is saved, the plugin exits without exporting.

If a specified layer does not exist, it is skipped.

## 

