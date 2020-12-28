# What is the Base BDA Extension?

The Base BDA module simulates tree mortality following major outbreaks of insects and/or disease, where major outbreaks are defined as those significant enough to influence forest succession, fire disturbance, or harvest disturbance at landscape scales. This version of the BDA will work with both age-only and biomass succession in LANDIS-II. However, it only uses cohort age information. Partial cohort removal and growth reductions are not possible. This implementation therefore behaves similar to that described in the Sturtevant et al. (2004) paper, and in the LANDIS 4.0 User’s Guide, with the exception of a variable temporal resolution where cohort ages are not limited to 10-year time steps.

# Release Notes

- Latest official release: Version 4.0.1 — May 2019
- [View User Guide](https://github.com/LANDIS-II-Foundation/Extension-Base-BDA/blob/master/docs/LANDIS-II%20Biological%20Disturbance%20Agent%20v4.0.1%20User%20Guide.pdf).
- Full release details found in the User Guide and on GitHub.

# Requirements

To use Base BDA, you need:

- The [LANDIS-II model v7.0](http://www.landis-ii.org/install) installed on your computer.
- Example files (see below)

# Download

Version 4.0.1 can be downloaded [here](https://github.com/LANDIS-II-Foundation/Extension-Base-BDA/blob/master/deploy/installer/LANDIS-II-V7%20Base%20BDA%204.0.1-setup.exe). To install it on your computer, launch the installer.

# Citation

Sturtevant, B.R., E.J. Gustafson, W. Li, and H.S. He. 2004. Modeling biological disturbances in LANDIS: A module description and demonstration using spruce budworm. Ecological Modelling 180: 153 – 174.

# Example Files

LANDIS-II requires a global parameter file for your scenario, and separate parameter files for each extension.

Example files are [here](https://github.com/LANDIS-II-Foundation/Extension-Base-BDA/blob/master/testings/version-tests/Core7-BaseBDA4.0/Base-bda-example.zip).

# Support

If you have a question, please contact Brian Sturtevant. 
You can also ask for help in the [LANDIS-II users group](http://www.landis-ii.org/users).

If you come across any issue or suspected bug, please post about it in the [issue section of the Github repository](https://github.com/LANDIS-II-Foundation/Extension-Base-BDA/issues).

# Author

[The LANDIS-II Foundation](http://www.landis-ii.org)

Mail : bsturtevant@fs.fed.us
