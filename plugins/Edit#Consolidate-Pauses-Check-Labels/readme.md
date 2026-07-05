Edit#Consolidate-Pauses-Check-Labels

## Description

The **Consolidate-Pauses-Check-Labels** plugin extends the standard pause consolidation functionality in Annotation Pro by merging only adjacent segments with identical labels. It was designed to support dialogue pause annotation by treating short interruptions as part of the same pause while preserving boundaries created by potentially meaningful sounds.

## Features

- Merges only neighbouring segments with identical labels.
- Creates a new output layer while preserving the original annotation.
- Uses configurable temporal thresholds to control consolidation.
- Prevents merging when the interval between segments is likely to represent a meaningful sound.
- Suitable for semi-automatic dialogue pause annotation.

## Code Customization

The plugin behaviour can be adjusted by modifying the following parameters in the source code:

- **sourceLayerName** – input annotation layer.
- **resultLayerName** – output layer for consolidated pauses.
- **maxGapMs** – maximum allowed gap between neighbouring segments.
- **significantSoundMs** – intervals longer than this threshold are treated as potentially meaningful sounds and are not merged.

## Limitations

The current implementation distinguishes between noise and meaningful sound using only temporal thresholds. It does not analyse acoustic properties such as amplitude, energy or frequency. Future versions may incorporate audio signal analysis for more accurate classification.

## Author

Barbara Szwiergiel