# Motion - Import BVH Body Regions

## Overview

Import BVH Body Regions is an Annotation PRO plugin that automatically determines the body region occupied by the right hand based on BVH motion capture data.

The plugin compares the Y-coordinate of the right hand against several anatomical reference points imported from BVH files:

- Head
- Neck
- Chest
- Middle Spine
- Lower Spine

The result is stored in a new annotation layer, allowing further behavioral, gesture, posture, and movement analysis.

---

## Features

- Reads BVH position layers imported into Annotation PRO.
- Detects the body region corresponding to the hand position.
- Preserves original BVH data.
- Creates a separate result layer.
- Supports Workspace mode.
- Compatible with .NET Framework 4.5 and C# 5.0.
- No external dependencies.

---

## Required Input Layers

The following layers must exist in the annotation:

```text
New_Import_HeadPositionY
New_Import_NeckPositionY
New_Import_ChestPositionY
New_Import_MiddleSpinePositionY
New_Import_LowerSpinePositionY
New_Import_RHandPositionY
```

Each segment label should contain a numeric Y-coordinate value.

---

## Generated Output Layer

The plugin creates:

```text
BVH_RHandBodyRegion
```

Each generated segment contains:

| Field | Description |
|---------|---------|
| Label | Detected body region |
| Parameter1 | Original hand Y-coordinate |

---

## Body Region Classification

The plugin classifies hand position into one of the following categories:

| Region | Description |
|----------|----------|
| Head-Neck | Between head and neck |
| Neck-Chest | Between neck and chest |
| Chest-MiddleSpine | Between chest and middle spine |
| MiddleSpine-LowerSpine | Between middle and lower spine |
| AboveHead | Above head position |
| BelowTorso | Below lower spine position |
| Unknown | Insufficient or invalid data |

---

## Algorithm

For each segment in:

```text
New_Import_RHandPositionY
```

the plugin:

1. Reads the hand Y-coordinate.
2. Finds the corresponding time point.
3. Retrieves reference Y-coordinates from body landmark layers.
4. Determines between which landmarks the hand is located.
5. Assigns a body-region label.
6. Creates a corresponding segment in the output layer.

### Processing Flow

```text
RHandPositionY
       |
       v
Read Y value
       |
       v
Find reference positions
       |
       v
Compare body landmarks
       |
       v
Assign region label
       |
       v
Create output segment
```

---

## Installation

1. Copy the plugin source file:

```text
Motion#Import BVH Body Regions.cs
```

to:

```text
Documents\Annotation Pro\Plugins
```

2. Restart Annotation PRO.

3. Open an annotation containing BVH-imported layers.

4. Run:

```text
Motion
 └── Import BVH Body Regions
```

---

## Example

### Input

| Time | RHand Y |
|---------|---------|
| 1.20 | 1.68 |
| 1.40 | 1.45 |
| 1.60 | 1.12 |

Reference positions:

| Landmark | Y |
|---------|---------|
| Head | 1.80 |
| Neck | 1.55 |
| Chest | 1.30 |
| MiddleSpine | 1.05 |
| LowerSpine | 0.80 |

### Output

| Time | Label |
|---------|---------|
| 1.20 | Head-Neck |
| 1.40 | Neck-Chest |
| 1.60 | Chest-MiddleSpine |

---

## Error Handling

The plugin validates:

- existence of source layer,
- existence of reference layers,
- presence of segments in required layers,
- numeric Y-coordinate values.

Unexpected errors are logged to:

```text
plugin-errors.txt
```

---

## Performance Notes

The current implementation prioritizes correctness and compatibility with Annotation PRO.

For very large BVH recordings (>100,000 segments), future versions may introduce optimized temporal indexing to improve processing speed.

---

## Compatibility

| Component | Version |
|------------|------------|
| Annotation PRO | Supported |
| .NET Framework | 4.5 |
| C# | 5.0 |
| BVH Import Layers | Supported |

---

## Future Development

Planned extensions:

- Left hand body-region detection.
- Foot region detection.
- Full-body region classification.
- Multi-joint analysis.
- Gesture abstraction layers.
- Automatic posture classification.

---

## Version History

### v1.0

- Initial release.
- Right-hand body-region detection.
- BVH landmark-based classification.
- Output layer generation.
- Error logging support.

---

## License

Internal research and annotation use.

Modify and distribute according to your Annotation PRO project requirements.
