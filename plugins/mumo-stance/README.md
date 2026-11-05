This repository contains six Annotation Pro plugins (C# scripts) developed or adapted for use with the MumoStance corpus. The plugins are pre-configured for the corpus annotation layers and support annotation data extraction, format conversion, and acoustic feature analysis.

The MumoStance corpus, comprising 318 annotation files (ANTx, EAF, and TextGrid) and accompanying metadata, is available separately through the CLARIN-PL repository under the CC BY-NC 4.0 licence. The plugins provided here are distributed under the MIT Licence.

## F0 Estimation

Estimates the fundamental frequency (F0) contour from the audio linked to the current Annotation Pro annotation. 

Annotation Pro runs the estimate through its FundamentalFrequencyEstimator, based on the pYIN method of Mauch and Dixon (2014). The contour is added as a new annotation layer and the annotation is saved.

## Export Selected Layers to CSV (Affective Engagement)

Exports the MumoStance affective engagement layers to CSV: Interactive Unit, Foregrounding Cluster, Dynamic Musical Terms, Expressive Movement Unit, and Peak. Works for a single annotation file or for a collection in Workspace Mode.

The CSV is written next to the annotation file. The filename includes an hourly timestamp. If a file for that hour already exists, new rows are appended. Only layers present in the annotation are exported.

## Export Selected Layers to CSV (Speech)

Exports the MumoStance speech segmentation layers (Input, Word, Syllable, and Phone) to CSV, for one file or in Workspace Mode.

The CSV is written next to the annotation file (as with the Affective Engagement Export plugin). The filename includes an hourly timestamp. Only layers present in the annotation are exported.

## Export Specified Layers to EAF

Writes selected MumoStance affective engagement layers to an ELAN EAF file: Interactive Unit, Foregrounding Cluster, Peak, Dynamic Musical Terms, and Expressive Movement Unit. 

The listed layers are copied into a new annotation object and saved with Annotation Pro's ElanWriter as an `.eaf` file next to the current annotation.

## Import EAF

Imports ELAN EAF tiers into the open Annotation Pro (ANT/ANTx) file, for example when MumoStance annotations edited in ELAN need to return to Annotation Pro.

Looks for an `.eaf` file with the same base name as the open ANTx file, reads the tiers with ElanReader, and adds them as Annotation Pro layers.

## Export Specified Layers to TextGrid

Exports the MumoStance speech layers (Input, Word, Syllable, and Phone) to a Praat TextGrid format. 

The TextGrid is written beside the annotation under the same base name. If that TextGrid already exists, the plugin leaves it unchanged.



===

#### About the MuMoStance Project

The project: *MumoStance:* *Mltimodal Stancetaking: Expressive Movement and Affective Stance. Political Debates in German Bundestag and Polish Sejm*, was a German–Polish research project funded under the Beethoven programme by the Polish National Science Centre (NCN; UMO-2018/31/G/HS2/03633) and the German Research Foundation (DFG; MU 1587/3-1).

The Polish research team was led by Maciej Karpiński, and the German team by Cornelia Müller. Katarzyna Klessa was responsible for coordinating corpus development and data management.
