# Import EAF
## Overview

Import EAF is an Annotation Pro plugin that imports annotation layers from an ELAN .eaf file into the currently opened annotation.

The plugin automatically searches for an EAF file that has the same filename and is located in the same folder as the currently opened annotation file.

Example

If the following files exist:

Interview01.ant
Interview01.eaf
Interview01.wav

and Interview01.ant is currently open, running the plugin will import all tiers from Interview01.eaf.

## Features
Automatic EAF file detection.
Imports all ELAN tiers/layers.
Adds imported layers to the current annotation.
Refreshes the editor after import.
Automatically saves the annotation when running in Workspace Mode.
## How It Works
Retrieves the currently opened annotation.
Builds the path to a matching .eaf file.
Verifies that the EAF file exists.
Imports all layers from the EAF file.
Adds imported layers to the current annotation.
Refreshes the Annotation Pro interface.
## Notes
Existing layers are not removed.
Layers with identical names may result in duplicates.
The plugin imports all available ELAN tiers.
The EAF file must have the same base filename as the annotation.
## Requirements
Annotation Pro
Existing annotation file (.ant / .antx)
Matching ELAN file (.eaf)
Typical Workflow
Open an annotation in Annotation Pro.
Ensure a matching .eaf file exists in the same folder.
Run the plugin.
Review the imported layers.
Save the annotation.
