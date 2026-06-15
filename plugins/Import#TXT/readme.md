# Import TXT

## Overview

Import TXT is an Annotation Pro plugin that imports the contents of a plain text file into a new annotation layer.

The plugin searches for a `.txt` file with the same filename as the currently opened annotation and creates a single segment containing the entire text.

## Example

If the following files exist:

```text
Lecture01.ant
Lecture01.txt
Lecture01.wav
```

and `Lecture01.ant` is currently open, running the plugin will create a new layer containing the text from `Lecture01.txt`.

## Features

* Automatic TXT file detection.
* Reads the entire text file.
* Creates a new annotation layer.
* Creates a single segment containing the imported text.
* Uses most of the audio duration for the generated segment.

## How It Works

1. Retrieves the current annotation and audio.
2. Searches for a TXT file with the same base filename.
3. Reads the entire text file.
4. Creates a new layer named:

```text
Phrase
```

5. Creates a single segment:

   * Start: 10% of audio length
   * End: 90% of audio length
   * Label: entire contents of the TXT file

## Generated Annotation

Example text file:

```text
This is a transcript of the recording.
```

Generated segment:

| Property | Value                                  |
| -------- | -------------------------------------- |
| Layer    | Phrase                                 |
| Start    | 10% of audio duration                  |
| End      | 90% of audio duration                  |
| Label    | This is a transcript of the recording. |

## Limitations

The plugin does not:

* Split text into sentences.
* Split text into words.
* Import timestamps.
* Align text with audio.
* Check for existing "Phrase" layers.
* Create multiple segments.

## Requirements

* Annotation Pro
* Existing annotation file (`.ant` / `.antx`)
* Matching text file (`.txt`)

## Typical Workflow

1. Open an annotation in Annotation Pro.
2. Place a transcript text file with the same filename in the same folder.
3. Run the plugin.
4. Review the generated Phrase layer.
5. Perform further segmentation or annotation as needed.

## Intended Use

This plugin is useful for quickly attaching a transcript or textual description to an audio recording before manual segmentation and annotation.
