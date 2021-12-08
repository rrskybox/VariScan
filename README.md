# VariScan

Differential Photometry Application for TheSkyX

VariScan is Windows desktop software that automates the imaging and the photometric evaluation of variable astronomical targets through application of the TheSkyX™ Professional astroimaging platform.  

This project began with the objective to apply TSX to acquire and derive a standard color magnitude of a variable star using the filtered magnitudes of the surrounding field stars as correlated to their cataloged magnitudes.   Because stellar spectra vary, the calculated magnitude of a variable star in a standard color must be determined from a transform function that is based on differential magnitudes of filtered field stars and their differential standard color magnitudes.   For each target star, the process begins with the acquisition of one or more images, taken with at least two filters.  Light sources in these images are photometrically characterized and astrometrically registered to APASS catalog stars.  From the catalog and photometric data, differential color and magnitude transformations are computed and used to convert target star image intensities to a chosen standard color magnitude.

The software package consists of a session manager for sequencing image capture of target stars, and an analysis engine for extracting and translating image star fields into photometric data.  The session manager controls the acquisition of images for a set of AAVSO listed targets each night.  The analysis program processes each image to determine the magnitude of the target based on cataloged magnitudes of surrounding star field, then transforms instrument magnitudes into standard AAVSO color bands.  Results for successive sessions can be graphed and/or submitted to AAVSO.

A full description can be found in the VariScan_Description.docx document.
