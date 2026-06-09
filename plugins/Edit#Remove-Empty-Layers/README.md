# Remove Empty Layers Plugin

## Description

This plugin automatically removes all annotation layers that do not contain any segments.

It is useful for cleaning Annotation PRO projects after importing data, running automatic processing, or working with large annotation sets where empty layers are no longer needed.

## Features

- Scans all layers in the current annotation.
- Detects layers with zero segments.
- Removes empty layers from the annotation.
- Refreshes the layer view automatically.
- Saves the annotation automatically when running in Workspace Mode.

## How It Works

The plugin:

1. Reads all layers from the current annotation.
2. Checks the number of segments in each layer.
3. Collects layers where:

layer.Segments.Count == 0

4. Removes these layers from the annotation.
5. Refreshes the Annotation PRO interface.
6. Saves the annotation if Workspace Mode is enabled.

## Example

Before execution:

| Layer Name | Segments |
|------------|----------|
| Input | 120 |
| Word | 80 |
| Phone | 0 |
| Notes | 0 |
| Syllable | 150 |

After execution:

| Layer Name | Segments |
|------------|----------|
| Input | 120 |
| Word | 80 |
| Syllable | 150 |

The layers **Phone** and **Notes** are removed because they contain no segments.


