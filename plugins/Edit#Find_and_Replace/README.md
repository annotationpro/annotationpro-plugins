# Edit#Find-and-Replace

## Description
The `Edit#Find-and-Replace` plugin is a tool for Annotation Pro designed for bulk string replacement (find and replace) or complete removal (by replacing with an empty string) within annotation labels.

## Features
* **Layer Filtering:** The script automatically skips designated layers (e.g., `Word`, `Syllable`, `Phone`, `Expressive Movement Unit`), protecting primary data and raw transcriptions from accidental modification.
* **Label Filtering:** Offers the ability to exclude specific labels from the replacement process.
* **Replacement Rules (ReplacePatterns):** The user defines a list of `ReplacePattern(search_text, replacement)` patterns directly in the code. The plugin automatically iterates through all permitted segments and performs the text operations.

## Code Customization
To tailor the plugin to your specific analysis needs, modify the following lists within the `.cs` file:
- `ignoreLayers` - a collection of layer names that will be skipped during processing.
- `ignoreLabels` - a collection of labels that should remain untouched.
- `replacePatterns` - text replacement rules. To remove a character string, use `string.Empty` as the replacement argument.

## Authors
Wojciech Klessa & Katarzyna Klessa (2016)
