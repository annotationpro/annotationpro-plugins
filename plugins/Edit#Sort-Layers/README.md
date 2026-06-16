# MUMO#Edit\_Replace Phones and Syllables to CV

## Overview

This Annotation PRO plugin converts phone-level and syllable-level annotations into CV (Consonant-Vowel) patterns.

The plugin classifies each phone as either:

* **C** = Consonant
* **V** = Vowel

It then generates new layers containing the CV representation of phones and syllables.

\---

## Plugin Information

|Property|Value|
|-|-|
|Plugin Name|Edit: Replace Phones and Syllables to CV|
|Author|Polskie Media|
|Updated|2023-10-19|
|Plugin ID|0dga6h0a|
|Version|1.0|

\---

## Required Layers

The plugin requires the following layers:

```text
Phone\_CV
Syllable\_CV
```

If either layer is missing, the plugin will stop and display an error message.

\---

## How It Works

### Step 1 – Classify Phones

The following phones are treated as vowels:

```text
a
o
e
u
i
y
o\~
e\~
```

All other phones are treated as consonants.

### Example

|Phone|CV|
|-|-|
|k|C|
|a|V|
|t|C|

\---

### Step 2 – Create Phone CV Layer

A new layer named:

```text
Phone\_CV\_CV
```

is created.

Example:

|Original|CV|
|-|-|
|k|C|
|a|V|
|t|C|

Timing information remains unchanged.

\---

### Step 3 – Create Syllable CV Layer

A new layer named:

```text
Syllable\_CV\_CV
```

is created.

The plugin finds all phones belonging to each syllable and combines their CV labels.

Examples:

|Phones|Result|
|-|-|
|p a|CV|
|k a t|CVC|
|s t a|CCV|
|s t r a j k|CCCVCC|

\---

## Output

Input layers:

```text
Phone\_CV
Syllable\_CV
```

Output layers:

```text
Phone\_CV
Syllable\_CV
Phone\_CV\_CV
Syllable\_CV\_CV
```

The original layers remain unchanged.

\---

## Example Workflow

Phones:

```text
k
a
t
```

Generated Phone Layer:

```text
C
V
C
```

Generated Syllable Layer:

```text
CVC
```

\---

## Limitations

The built-in vowel dictionary contains only:

```text
a
o
e
u
i
y
o\~
e\~
```

Any phone not listed above is automatically classified as a consonant.

Additional vowels must be added manually in the source code.

\---

## Installation

1. Copy the plugin .cs file into:

```text
Documents\\Annotation Pro\\Plugins
```

2. Click refresh or restart Annotation PRO.
3. Open an annotation containing Phone\_CV and Syllable\_CV layers.
4. Run the plugin from the Plugins menu.

\---

## Notes

This plugin does not modify existing annotations.

Instead, it creates two new layers containing CV representations that can be used for:

* Phonotactic analysis
* Syllable structure analysis
* Speech rhythm analysis
* Prosodic studies
* Statistical processing

