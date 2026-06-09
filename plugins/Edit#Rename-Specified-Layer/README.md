# Rename Specified Layer Plugin

## Description

This plugin automatically renames a layer with a predefined name to a new predefined name.

It is designed for batch processing and Workspace Mode scenarios where annotations need to be standardized across multiple files.

## Features

- Searches for a layer with a specified name.
- Renames the layer to a new name.
- Works with single annotation files and Workspace Mode collections.
- Automatically saves the annotation after the modification.
- Refreshes the Annotation PRO interface.

## Configuration

Edit the following variables in the source code:

```csharp
string layerName = "NewLayer";
string newLayerName = "Read";

layerName – name of the layer to find.
newLayerName – new name assigned to the layer.

## How It Works

The plugin:

1. Loads the current annotation.
2. Searches all annotation layers.
3. Finds the first layer matching the specified name.
4. Renames the layer.
5. Marks the annotation as modified.
6. Saves the annotation.
7. Refreshes the editor view.

## Validation

If the source layer does not exist, the plugin displays a message containing:

- the missing layer name,
- the current annotation file name.

No changes are made in this case.

## Important Notes

- Only the first matching layer is renamed.
- Layer contents and segments remain unchanged.
- Existing layers with the target name are not checked for conflicts.
- The annotation is automatically saved after renaming.

## Version

1.0

## Author

Katarzyna Klessa