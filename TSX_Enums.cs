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


namespace VariScan
{
    public class TSX_Enums
    {

        public enum ccdsoftInventoryIndex
        {
            cdInventoryX,
            cdInventoryY,
            cdInventoryMagnitude,
            cdInventoryClass,
            cdInventoryFWHM,
            cdInventoryMajorAxis,
            cdInventoryMinorAxis,
            cdInventoryTheta,
            cdInventoryEllipticity
        }

        public enum ccdsoftWCSIndex
        {
            cdWCSRA,
            cdWCSDec,
            cdWCSX,
            cdWCSY,
            cdWCSPositionError,
            cdWCSResidual,
            cdWCSCatalogID,
            cdActive
        }

        public enum ccdsoftAutoContrastMethod
        {
            cdAutoContrastUseAppSetting = -1,
            cdAutoContrastSBIG,
            cdAutoContrastBjorn,
            cdAutoContrastDSS100X,
        }

        public enum ccdsoftBjornBackground
        {
            cdBgNone,
            cdBgWeak,
            cdBgMedium,
            cdBgStrong,
            cdBgVeryStrong
        }

        public enum ccdsoftBjornHighlight
        {
            cdHLNone,
            cdHLWeak,
            cdHLMedium,
            cdHLStrong,
            cdHLVeryStrong,
            cdHLAdaptive,
            cdHLPlanetary
        }

        public enum ccdsoftCoordinates
        {
            cdRA,
            cdDec
        }

        public enum ccdsoftSaveAs
        {
            cdGIF,
            cdBMP,
            cdJPG,
            cd48BitTIF

        }
    }
}
