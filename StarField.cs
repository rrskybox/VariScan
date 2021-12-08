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
using System.Collections.Generic;
using TheSky64Lib;

namespace VariScan
{
    public partial class StarField
    {
        private ccdsoftImage TSX_Image;

        public bool IsImageLinked = false;

        public List<TargetData> LightSourceStars;
        public List<TargetData> validAPASSCatalogedStars;
        public List<TargetData> validGAIACatalogedStars;

        private object[] lsInvMagArr;
        private object[] lsInvFWHMArr;
        private object[] lsInvElipsArr;
        private object[] lsInvXPosArr;
        private object[] lsInvYPosArr;

        private static double fieldOfView;
        private static double rightAscension;
        private static double declination;

        public DateTime ImageDateTime;

        //public TargetData ImageTarget { get; set; }
        //private List<LightSourceInventory> fieldInventory;

        public StarField(ccdsoftImage tsx_Image, SampleManager.SessionSample sample)
        {
            TSX_Image = tsx_Image;
            //close the current tsx image if one is open
            if (TSX_Image != null)
                VariScanFileManager.CloseImageFile(TSX_Image);
            bool FitsIsOpen = VariScanFileManager.DirectOpenFitsFilePath(TSX_Image, sample.ImagePath);
        }

        public StarField(ccdsoftImage tsx_Image, string fitsFilePath)
        {
            TSX_Image = tsx_Image;
            //close the current tsx image if one is open
            if (TSX_Image != null)
                VariScanFileManager.CloseImageFile(TSX_Image);
            bool FitsIsOpen = VariScanFileManager.DirectOpenFitsFilePath(TSX_Image, fitsFilePath);
        }

        public double AirMass()
        {
            return Convert.ToDouble(TSX_Image.FITSKeyword("AIRMASS"));
        }

        public DateTime FitsImageDateTime()
        {
            //REturns the image date from the fits file (format '2021-10-30T09:52:13.336')
            string fitsUDT = TSX_Image.FITSKeyword("DATE-OBS").Split('.')[0];
            string[] dsts = fitsUDT.Split('T');
            string[] ds = dsts[0].Split('-');
            string[] dt = dsts[1].Split(':');
            int year = Convert.ToInt16(ds[0]);
            int month = Convert.ToInt16(ds[1]);
            int day = Convert.ToInt16(ds[2]);
            int hour = Convert.ToInt16(dt[0]);
            int minute = Convert.ToInt16(dt[1]) % 60;
            int second = Convert.ToInt16(dt[2]);
            //
            DateTime utcDT = new DateTime(year, month, day, hour, minute, second);
            return utcDT;
        }

        public List<LightSourceInventory> AssembleLightSources()
        {
            List<FieldLightSource> fieldStars = new List<FieldLightSource>();
            //ImageLink for light sources (Insert WCS)
            //Set Image Scale
            double scale = TSX_Resources.GetFOVImageScale();
            TSX_Image.ScaleInArcsecondsPerPixel = scale;
            int ferr;
            try
            {
                ferr = TSX_Image.InsertWCS(true);
                IsImageLinked = true;
            }
            catch (Exception ex)
            {
                IsImageLinked = false;
                return null;
            }
            //Collect astrometric light source data from the image linking into single index arrays: 
            //  magnitude, fmhm, ellipsicity, x and y position
            lsInvMagArr = TSX_Image.InventoryArray((int)TSX_Enums.ccdsoftInventoryIndex.cdInventoryMagnitude);   //Instrument magnitude
            lsInvFWHMArr = TSX_Image.InventoryArray((int)TSX_Enums.ccdsoftInventoryIndex.cdInventoryFWHM); //FMHW, we think
            lsInvElipsArr = TSX_Image.InventoryArray((int)TSX_Enums.ccdsoftInventoryIndex.cdInventoryEllipticity);//Ellipticity, we think
            lsInvXPosArr = TSX_Image.InventoryArray((int)TSX_Enums.ccdsoftInventoryIndex.cdInventoryX);//X position, we think
            lsInvYPosArr = TSX_Image.InventoryArray((int)TSX_Enums.ccdsoftInventoryIndex.cdInventoryY); //Y position, we think
            List<LightSourceInventory> fieldInventory = new List<LightSourceInventory>();
            for (int i = 0; i < lsInvMagArr.Length; i++)
                fieldInventory.Add(new LightSourceInventory
                {
                    Magnitude = (double)lsInvMagArr[i],
                    FWHM = (double)lsInvFWHMArr[i],
                    XPosition = (double)lsInvXPosArr[i],
                    YPosition = (double)lsInvYPosArr[i],
                    Ellipticity = (double)lsInvElipsArr[i]
                });
            return fieldInventory;
        }

        public List<FieldLightSource> LocateLightSources(List<LightSourceInventory> fieldInventory)
        {
            Configuration cfg = new Configuration();
            double nonLinearADU = Convert.ToDouble(cfg.ADUMax);
            //Center the skychart on the ra/dec coordinates
            //Set the star chart size to 1.5 times the image width (fits the whole thing on, persumably
            ImageLinkResults tsxilr = new ImageLinkResults();
            rightAscension = tsxilr.imageCenterRAJ2000;
            declination = tsxilr.imageCenterDecJ2000;
            fieldOfView = (((double)TSX_Image.ScaleInArcsecondsPerPixel * (double)TSX_Image.WidthInPixels) * 1.5) / 3600; //in degrees

            CenterSkyChart();

            List<FieldLightSource> lsStars = new List<FieldLightSource>();
            if (fieldInventory == null || fieldInventory.Count == 0)
                return lsStars;
            foreach (LightSourceInventory ls in fieldInventory)
            {
                //if the source is "active" (meaning used for imagelinking)
                //  get it's RA/Dec and pull up its Inventory data 
                //  Search the catalogs and create a target data object,
                //  and save to the culled list of targets (validStarPoints)

                //Convert X,Y to RA, Dec for the light source
                TSX_Image.XYToRADec(ls.XPosition, ls.YPosition);
                double raStar = (double)TSX_Image.XYToRADecResultRA();
                double decStar = (double)TSX_Image.XYToRADecResultDec();
                FieldLightSource starData = new FieldLightSource()
                {
                    SourceRA = raStar,
                    SourceDec = decStar,
                    InstrumentMagnitude = ls.Magnitude
                };

                //Get ADU at image X,Y position
                //  Note: the Zero Y line is the top line of the image
                int xPos = (int)ls.XPosition;
                int yPos = (int)ls.YPosition;
                object[] xDataLine = TSX_Image.scanLine(yPos);
                int xyADU = (int)xDataLine[xPos];
                starData.SourceADU = xyADU;
                //Remove saturated and antibloomed stars
                if (xyADU < nonLinearADU)
                    lsStars.Add(starData);
            };
            return lsStars;
        }

        public static CatalogData GetCatalogData(double ra, double dec)
        {
            //Adds cataloged data for stars very near the input catalog data

            //Center and frame star chart
            CenterSkyChart();

            sky6ObjectInformation tsxod = new sky6ObjectInformation();

            sky6StarChart tsxsc = new sky6StarChart();
            tsxsc.EquatorialToStarChartXY(ra, dec);
            int xC = (int)tsxsc.dOut0;
            int yC = (int)tsxsc.dOut1;

            //Turn on star display -- otherwise clickfind doesn't work
            tsxsc.SetDisplayProperty(Sk6DisplayPropertyObjectType.OT6_STAR,
                                     Sk6DisplayPropertySkyMode.sk6DisplayPropertySkyModeChartMode,
                                     Sk6DisplayProperty.sk6DisplayPropertyVisible,
                                     Sk6DisplayPropertyItem.sk6DisplayPropertyItemVisibleValue,
                                     1);
            //Retrieve list of cataloged stars near the target position
            tsxsc.ClickFind(xC, yC);
            int objcnt = tsxod.Count;
            CatalogData fc = new CatalogData()
            {
                IsAPASSCataloged = false,
                IsGAIACataloged = false,
                IsGCVSCataloged = false
            };
            for (int i = 0; i < objcnt; i++)
            {
                tsxod.Index = i;
                tsxod.Property(TheSky64Lib.Sk6ObjectInformationProperty.sk6ObjInfoProp_NAME1);
                string catalogedName = tsxod.ObjInfoPropOut;
                tsxod.Property(TheSky64Lib.Sk6ObjectInformationProperty.sk6ObjInfoProp_MAG);
                var catalogMag = tsxod.ObjInfoPropOut;
                tsxod.Property(TheSky64Lib.Sk6ObjectInformationProperty.sk6ObjInfoProp_RA_2000);
                var catalogRA = tsxod.ObjInfoPropOut;
                tsxod.Property(TheSky64Lib.Sk6ObjectInformationProperty.sk6ObjInfoProp_DEC_2000);
                var catalogDec = tsxod.ObjInfoPropOut;
                if (catalogedName.Contains("Gaia"))
                {
                    tsxod.Property(TheSky64Lib.Sk6ObjectInformationProperty.sk6ObjInfoProp_DB_FIELD_1); //FIlter G
                    var catalogMagG = Convert.ToDouble(tsxod.ObjInfoPropOut);
                    tsxod.Property(TheSky64Lib.Sk6ObjectInformationProperty.sk6ObjInfoProp_DB_FIELD_2); //Filter Gbp
                    var catalogMagGbp = Convert.ToDouble(tsxod.ObjInfoPropOut);
                    tsxod.Property(TheSky64Lib.Sk6ObjectInformationProperty.sk6ObjInfoProp_DB_FIELD_3);  //Filter Gbr
                    var catalogMagGrp = Convert.ToDouble(tsxod.ObjInfoPropOut);

                    fc.IsGAIACataloged = true;
                    fc.GAIACatalogName = catalogedName;
                    fc.GAIACatalogRA = catalogRA;
                    fc.GAIACatalogDec = catalogDec;
                    fc.GAIACatalogMagnitudeG = catalogMagG;
                    fc.GAIACatalogMagnitudeGbp = catalogMagGbp;
                    fc.GAIACatalogMagnitudeGrp = catalogMagGrp;
                }
                else if (catalogedName.Contains("APASS"))
                {
                    tsxod.Property(TheSky64Lib.Sk6ObjectInformationProperty.sk6ObjInfoProp_STAR_MAGB);
                    var catalogMagB = tsxod.ObjInfoPropOut;
                    tsxod.Property(TheSky64Lib.Sk6ObjectInformationProperty.sk6ObjInfoProp_STAR_MAGV);
                    var catalogMagV = tsxod.ObjInfoPropOut;
                    tsxod.Property(TheSky64Lib.Sk6ObjectInformationProperty.sk6ObjInfoProp_STAR_MAGR);
                    var catalogMagR = tsxod.ObjInfoPropOut;
                    tsxod.Property(TheSky64Lib.Sk6ObjectInformationProperty.sk6ObjInfoProp_DB_FIELD_4);
                    char[] s = new char[] { ' ' };
                    var catalogMagG = Convert.ToDouble(tsxod.ObjInfoPropOut.Split(s, StringSplitOptions.RemoveEmptyEntries)[0]);

                    fc.IsAPASSCataloged = true;
                    fc.APASSCatalogName = catalogedName;
                    fc.APASSCatalogRA = catalogRA;
                    fc.APASSCatalogDec = catalogDec;
                    fc.APASSCatalogMagnitudeB = catalogMagB;
                    fc.APASSCatalogMagnitudeV = catalogMagV;
                    fc.APASSCatalogMagnitudeR = catalogMagR;
                    //fc.APASSCatalogMagnitudeI = catalogMagI;
                    //fc.APASSCatalogMagnitudeU = catalogMagU;
                }
                else if (catalogedName.Contains("GCVS"))
                {
                    fc.IsGCVSCataloged = true;
                }
            }
            return fc;
        }

        public static (double, double) CalculateSeparations(TargetData tgt)
        {
            //Determine separation between source and APASS, and source and GAIA in arcsec?
            sky6Utils tsxu = new sky6Utils();
            tsxu.ComputeAngularSeparation(tgt.Catalog.APASSCatalogRA, tgt.Catalog.APASSCatalogDec, tgt.SourceRA, tgt.SourceDec);
            double currentAPASSseparation = tsxu.dOut0;
            tsxu.ComputeAngularSeparation(tgt.Catalog.GAIACatalogRA, tgt.Catalog.GAIACatalogDec, tgt.SourceRA, tgt.SourceDec);
            double currentGAIAseparation = tsxu.dOut0;
            return (currentAPASSseparation, currentGAIAseparation);
        }

        private static void CenterSkyChart()
        {
            //Center the skychart on the ra/dec coordinates
            //Set the star chart size to 1.5 times the image width (fits the whole thing on, persumably
            sky6StarChart tsxc = new sky6StarChart
            {
                RightAscension = rightAscension,
                Declination = declination,
                FieldOfView = fieldOfView
            };
        }

        public void Close()
        {
            if (TSX_Image != null) VariScanFileManager.CloseImageFile(TSX_Image);
        }


        public TargetData LoadTargetFromLightSourceInventory(TargetData lightSource)
        {
            //Gets location and magnitude data on light source closest to light source RA/Dec
            sky6Utils tsxu = new sky6Utils();
            double ra = lightSource.TargetRA;
            double dec = lightSource.TargetDec;
            double currentSeparation = double.MaxValue;
            double minimumSeparation = double.MaxValue;
            int sIndex = -1;
            for (int iSource = 0; iSource < lsInvXPosArr.Length; iSource++)
            {
                TSX_Image.XYToRADec((double)lsInvXPosArr[iSource], (double)lsInvYPosArr[iSource]);
                double raSrc = TSX_Image.XYToRADecResultRA();
                double decSrc = TSX_Image.XYToRADecResultDec();
                tsxu.ComputeAngularSeparation(ra, dec, raSrc, decSrc);
                currentSeparation = tsxu.dOut0;
                if (currentSeparation <= minimumSeparation)
                {
                    minimumSeparation = currentSeparation;
                    sIndex = iSource;
                    //populate lightSource targetData in anticipation...
                    lightSource.InventoryArrayIndex = sIndex;
                    lightSource.TargetToSourcePositionError = minimumSeparation * 3600.0;
                    lightSource.SourceRA = raSrc;
                    lightSource.SourceDec = decSrc;
                    lightSource.SourceX = (double)lsInvXPosArr[sIndex];
                    lightSource.SourceY = (double)lsInvYPosArr[sIndex];
                    lightSource.SourceInstrumentMagnitude = (double)lsInvMagArr[sIndex];
                }
            }
            return lightSource;
        }

        public double? FWHMAvg_Pixels
        {
            get
            {
                if (lsInvFWHMArr != null)
                    return MathNet.Numerics.Statistics.ArrayStatistics.Mean(Array.ConvertAll<object, double>(lsInvFWHMArr, x => (double)x));
                else
                    return null;
            }
        }

        public double? EllipticityAvg
        {
            get
            {
                if (lsInvElipsArr != null)
                    return MathNet.Numerics.Statistics.ArrayStatistics.Mean(Array.ConvertAll<object, double>(lsInvElipsArr, x => (double)x));
                else
                    return null;
            }
        }

        public LightSourceInventory GetLightSourceInventory(int inventoryIndex)
        {
            LightSourceInventory lsi = new LightSourceInventory
            {
                Magnitude = (double)lsInvMagArr[inventoryIndex],
                XPosition = (double)lsInvXPosArr[inventoryIndex],
                YPosition = (double)lsInvYPosArr[inventoryIndex],
                FWHM = (double)lsInvFWHMArr[inventoryIndex],
                Ellipticity = (double)lsInvElipsArr[inventoryIndex]
            };
            return lsi;
        }

        public struct LightSourceInventory
        {
            public double Magnitude { get; set; }
            public double XPosition { get; set; }
            public double YPosition { get; set; }
            public double FWHM { get; set; }
            public double Ellipticity { get; set; }
        }

        public struct FieldLightSource
        {
            public double ColorStandard(ColorIndexing.ColorDataSource cds)
            {
                double mag = 0;
                switch (cds)
                {
                    case ColorIndexing.ColorDataSource.Instrument:
                        {
                            mag = this.InstrumentMagnitude;
                            break;
                        }
                    case ColorIndexing.ColorDataSource.Bj:
                        {
                            mag = this.StandardMagnitudes.Value.APASSCatalogMagnitudeB;
                            break;
                        }
                    case ColorIndexing.ColorDataSource.Vj:
                        {
                            mag = this.StandardMagnitudes.Value.APASSCatalogMagnitudeV;
                            break;
                        }
                    case ColorIndexing.ColorDataSource.Rc:
                        {
                            mag = this.StandardMagnitudes.Value.APASSCatalogMagnitudeR;
                            break;
                        }
                    case ColorIndexing.ColorDataSource.Ic:
                        {
                            mag = this.StandardMagnitudes.Value.APASSCatalogMagnitudeI;
                            break;
                        }
                    case ColorIndexing.ColorDataSource.Uc:
                        {
                            mag = this.StandardMagnitudes.Value.APASSCatalogMagnitudeU;
                            break;
                        }
                    case ColorIndexing.ColorDataSource.Gp:
                        {
                            mag = this.StandardMagnitudes.Value.GAIACatalogMagnitudeG;
                            break;
                        }
                    case ColorIndexing.ColorDataSource.GBp:
                        {
                            mag = this.StandardMagnitudes.Value.GAIACatalogMagnitudeGbp;
                            break;
                        }
                    case ColorIndexing.ColorDataSource.GRp:
                        {
                            mag = this.StandardMagnitudes.Value.GAIACatalogMagnitudeGrp;
                            break;
                        }
                }
                return (mag);

            }

            public int? RegistrationIndex { get; set; }
            public double SourceRA { get; set; }
            public double SourceDec { get; set; }
            public double InstrumentMagnitude { get; set; }
            public double SourceADU { get; set; }
            public CatalogData? StandardMagnitudes { get; set; }
        }

        public struct CatalogData
        {
            //Structure for encapsulating star catalog data after look up
            public bool IsAPASSCataloged { get; set; }
            public string APASSCatalogName { get; set; }
            public double APASSCatalogRA { get; set; }
            public double APASSCatalogDec { get; set; }
            public double APASSCatalogMagnitudeB { get; set; }
            public double APASSCatalogMagnitudeV { get; set; }
            public double APASSCatalogMagnitudeR { get; set; }
            public double APASSCatalogMagnitudeI { get; set; }
            public double APASSCatalogMagnitudeU { get; set; }
            public bool IsGAIACataloged { get; set; }
            public string GAIACatalogName { get; set; }
            public double GAIACatalogRA { get; set; }
            public double GAIACatalogDec { get; set; }
            public double GAIACatalogMagnitudeG { get; set; }
            public double GAIACatalogMagnitudeGbp { get; set; }
            public double GAIACatalogMagnitudeGrp { get; set; }
            public bool IsGCVSCataloged { get; set; }
        }
    }
}
