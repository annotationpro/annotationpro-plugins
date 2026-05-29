# Annotation Pro Plugins

A repository of official and community-developed plugins for Annotation Pro, including examples, documentation and educational materials. 
Plugins extend the functionality of Annotation Pro by enabling users to automate annotation, annotation mining and data processing tasks.

This repository contains:

- official Annotation Pro plugins,
- community-developed plugins,
- plugin documentation,
- examples and tutorials.

## Repository structure

plugins/
    PluginName/
        PluginName.cs
        README.md

docs/

examples/

## Plugin naming convention

Annotation Pro uses the `#` character in plugin file names to create submenu entries in the Plugins menu.

For example:

```text
Analysis#f0Estimation.cs
```

appears in Annotation Pro as:

```text
Plugins
└── Analysis
    └── f0Estimation
```

Similarly:

```text
Audio#Normalize.cs
```

appears as:

```text
Plugins
└── Audio
    └── Normalize
```

When creating new plugins, use the `Category#PluginName.cs` naming convention if you want the plugin to appear in a submenu.

## Contributing

Contributions from researchers, developers, and students are welcome.

## Annotation Pro

Website:
https://annotationpro.org

API:
https://annotationpro.org/api

## About Annotation Pro

Annotation Pro is developed through collaboration between researchers, software developers and contributors from the speech technology community.

## Contact

For questions regarding Annotation Pro, plugins and collaboration opportunities, please contact:


**Katarzyna Klessa**  
katarzyna@klessa.pl
