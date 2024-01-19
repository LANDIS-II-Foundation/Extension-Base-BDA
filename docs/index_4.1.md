# What is the Base BDA Extension?

The Base BDA module simulates tree mortality following major outbreaks of insects and/or disease, where major outbreaks are defined as those significant enough to influence forest succession, fire disturbance, or harvest disturbance at landscape scales. As with any of the Base disturbance extensions, this version is compatible with any succession extension. However, it only uses cohort age information. Partial cohort removal and growth reductions are not possible. This implementation therefore behaves similar to that described in the Sturtevant et al. (2004) paper, and in the LANDIS 4.0 User’s Guide, with some notable exceptions that include:variable temporal resolution; addition of climate modifiers affecting disturbance behavior, changes to the dispersal algorithm, and some additional options for communication with the Dynamic Fuels extension.

# Release Notes

- Latest official release: Version 4.1 — January 2024
- [View User Guide](https://github.com/LANDIS-II-Foundation/Extension-Base-BDA/blob/master/docs/LANDIS-II%20Biological%20Disturbance%20Agent%20v4.1%20User%20Guide.pdf).
- Full release details found in the User Guide and on GitHub.
- IMPORTANT: Version 4.1 is no longer backwards compatible due to changes in the dispersal algorithm that changed some variable names. 
  - If using BDA input files from prior versions that do NOT actually use the dispersal inputs (i.e., Dispersal set to “no”), replacing the old Dispersal Inputs with those from the example file, and resetting the Dispersal to “no” will allow you to reuse the old file.
  - If using BDA input files from prior versions that DO use the dispersal inputs (i.e., Dispersal set to “yes”), see the revised section in the User Guide for details on how to properly set up the new dispersal algorithms, using the example file as a template.


# Requirements

To use Base BDA, you need:

- The [LANDIS-II model v7.0](http://www.landis-ii.org/install) installed on your computer.
- Example files (see below)

# Download

Version 4.1 can be downloaded [here](https://github.com/LANDIS-II-Foundation/Extension-Base-BDA/blob/master/deploy/installer/LANDIS-II-V7%20Base%20BDA%204.1-setup.exe). To install it on your computer, launch the installer.

IMPORTANT: If you had a previous version of Base BDA installed:
-  The previous version of the Base BDA extension should be uninstalled before installing the current version.  This can be done using the Windows ‘Add or remove programs’ tool, or using a DOS command line by entering: landis-ii-extensions remove “Base BDA”. The uninstallation should remove the file C:\Program Files\LANDIS-II-v7\extensions\Landis.Extension.BaseBDA-v4.dll.  If that file is not removed during uninstallation, it can be deleted manually prior to installing the new version.
-  Previous versions of the extension installed example files in the following location: C:\Program Files\LANDIS-II-v7\examples\Base BDA.  The new version does not include example files packaged with the installer, and will not overwrite the examples provided by an earlier version.  To get an updated set of example files compatible with v4.1, follow the ‘Example Files’ download link below.


# Citation

Sturtevant, B.R., E.J. Gustafson, W. Li, and H.S. He. 2004. Modeling biological disturbances in LANDIS: A module description and demonstration using spruce budworm. Ecological Modelling 180: 153 – 174.

# Example Files

LANDIS-II requires a global parameter file for your scenario, and separate parameter files for each extension.


Example files are [here](https://github.com/LANDIS-II-Foundation/Extension-Base-BDA/blob/master/deploy/examples/Base-BDA-example.zip).

# Support

If you have a question, please contact Brian Sturtevant. 
You can also ask for help in the [LANDIS-II users group](http://www.landis-ii.org/users).

If you come across any issue or suspected bug, please post about it in the [issue section of the Github repository](https://github.com/LANDIS-II-Foundation/Extension-Base-BDA/issues).

# Author

[The LANDIS-II Foundation](http://www.landis-ii.org)

Mail : brian.r.sturtevant@usda.gov
