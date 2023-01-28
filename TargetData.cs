// --------------------------------------------------------------------------------
// VariScan module
//
// Description:	
//
// Environment:  Windows 10 executable, 32 and 64 bit
//
// Usage:        TBD
//
// Author:		(REM) Rick McAlister, rrskybox@yahoo.com
//
// Edit Log:     Rev 1.0 Initial Version
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// 
// ---------------------------------------------------------------------------------
//

using System;

namespace VariScan
{
    public class TargetData
    {
        //Structure for handling star data after look up
        public string TargetName { get; set; }
        public bool IsImageLinked { get; set; }
        public bool IsTransformed { get; set; }
        public string CatalogName { get; set; }
        public int MasterRegistrationIndex { get; set; }
        public StarField.CatalogData MasterCatalogInfo { get; set; }
        public int InventoryArrayIndex { get; set; }
        public double TargetRA { get; set; }
        public double TargetDec { get; set; }
        public double SourceRA { get; set; }
        public double SourceDec { get; set; }
        public double SourceX { get; set; }
        public double SourceY { get; set; }
        public double SourceADU { get; set; }
        public double SourceEllipticity { get; set; }
        public double SourceFWHM { get; set; }
        public double TargetToSourcePositionError { get; set; }
        public double SourceInstrumentMagnitude { get; set; }
        public double StandardColorMagnitude { get; set; }
        public double StandardMagnitudeError { get; set; }
        public double SourceToAPASSCatalogPositionError { get; set; }
        public double SourceToGAIACatalogPositionError { get; set; }
        public int ApassStarCount { get; set; }
        public int GaiaStarCount { get; set; }
        public DateTime SessionDate { get; set; }
        public DateTime ImageDate { get; set; }
        public int SessionSet { get; set; }
        public double ImageWidthInArcSec { get; set; }
        public string PrimaryImageFilter { get; set; }
        public string DifferentialImageFilter { get; set; }
        public string PrimaryStandardColor { get; set; }
        public string DifferentialStandardColor { get; set; }
        public double ColorTransform { get; set; }
        public double MagnitudeTransform { get; set; }
        public string ComputedSeeing { get; set; }
        public double AirMass { get; set; }

        public TargetData() { }

    }

}
