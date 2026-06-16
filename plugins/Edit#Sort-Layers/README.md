# MUMO#Edit_Sort Layers

## Overview

**MUMO#Edit_Sort Layers** is an Annotation PRO plugin that automatically reorders annotation layers according to a predefined list of layer names.

The plugin is useful when working with large annotation projects that require a consistent layer order across multiple annotation files.

After execution, the plugin:

- Sorts layers according to a predefined order.
- Moves unknown layers to the end of the layer list.
- Marks the annotation as modified.
- Refreshes the Annotation PRO interface.
- Saves the annotation automatically.

---

## Plugin Information

| Property | Value |
|-----------|---------|
| Plugin Name | Edit: Sort Layers |
| Author | Polskie Media |
| Updated | 2024-01-04 |
| Plugin ID | 0dfa2h0a |
| Version | 1.0 |

---

## Default Layer Order

```csharp
List<string> layerNames = new List<string>
{
    "Input",
    "Word",
    "Syllable",
    "Phone",
    "Interactive Unit",
    "Foregrounding Cluster",
    "Peak",
    "Dynamic Musical Terms",
    "Expressive Movement",
};
```

## Installation

1. Copy the plugin `.cs` file into:

```text
Documents\Annotation Pro\Plugins
```

2. Restart Annotation PRO.
3. Open an annotation file.
4. Run the plugin from the Plugins menu.

## Notes

This plugin only changes the order of layers.

It does not modify:
- Segments
- Labels
- Parameters
- Annotation content

All annotation data remains unchanged; only the layer order is updated.
