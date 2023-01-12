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
using System.IO;
using System.Linq;
using System.Xml.Linq;


namespace VariScan

{
    /*Starchive Class
     * 
     * This class encapsulates methods for storing and retrieving photometry data
     * on individual stars
     *
     */

    class Starchive
    {

        #region XML fields
        const string XMLRoot = "PhotometrySummaries";
        const string PhotometryRecordX = "PhotometryRecord";
        const string TargetNameX = "TargetName";
        const string TargetRAX = "TargetRA";
        const string TargetDecX = "TargetDec";
        const string IsTransformedX = "IsTransformed";
        const string CatalogNameX = "StarCatalog";
        const string SourceRAX = "SourceRA";
        const string SourceDecX = "SourceDec";
        const string SourceInstrumentMagnitudeX = "SourceInstrumentMagnitude";
        const string PrimaryFilterX = "PrimaryFilter";
        const string DifferentialFilterX = "DifferentialFilter";
        const string PrimaryStandardColorX = "PrimaryStandardColor";
        const string DifferentialStandardColorX = "DifferentialStandardColor";
        const string StandardColorMagnitudeX = "StandardColorMagnitude";
        const string StandardMagnitudeErrorX = "StandardMagnitudeError";
        const string TargetToSourcePositionErrorX = "T2SPositionError";
        const string SourceEllipticityX = "SourceEllipticity";
        const string SourceFWHMX = "SourceFWHM";
        const string SourceADUX = "SourceADU";
        const string ImageDateX = "ImageDate";
        const string SessionDateX = "SessionDate";
        const string ColorTransformX = "ColorTransform";
        const string MagnitudeTransformX = "MagnitudeTransform";
        const string SourceToAPASSErrorX = "SourceToAPASSError";
        const string SourceToGaiaErrorX = "SourceToGaiaError";
        const string APASSFieldStarCountX = "APASSFieldStarCount";
        const string GaiaFieldStarCountX = "GaiaFieldStarCount";
        const string AirMassX = "AirMass";


        #endregion

        public static bool CheckStarchiveFile()
        {
            Configuration cfg = new Configuration();
            if (!File.Exists(cfg.StarchiveFilePath))
            {
                XElement newStarchiveX = new XElement(XMLRoot);
                newStarchiveX.Save(cfg.StarchiveFilePath);
                return false;
            }
            return true;
        }

        public static void StorePhotometry(TargetData pData)
        {
            //Create an XElement with the data pData, after removing any old record
            //
            Configuration cfg = new Configuration();
            CheckStarchiveFile();
            XElement newStarchiveX = new XElement(XMLRoot);
            XElement oldStarchiveX = XElement.Load(cfg.StarchiveFilePath);
            List<XElement> targetXList = oldStarchiveX.Elements(PhotometryRecordX).ToList();
            foreach (XElement targetX in targetXList)
            {
                if (!((targetX.Element(TargetNameX).Value == pData.TargetName) &&
                     (targetX.Element(SessionDateX).Value == pData.SessionDate.ToShortDateString()) &&
                     (targetX.Element(PrimaryFilterX).Value == pData.PrimaryStandardColor.ToString()) &&
                     (targetX.Element(CatalogNameX).Value == pData.CatalogName)))
                {
                    newStarchiveX.Add(targetX);
                }
            }
            newStarchiveX.Add(FormXrecord(pData));
            newStarchiveX.Save(cfg.StarchiveFilePath);
            return;
        }

        public static List<TargetData> RetrievePhotometry(TargetData tgt)
        {
            //Open file, convert xml entries to list, if any and 
            //  retrieve target data on elements matching name, color and filter
            Configuration cfg = new Configuration();
            List<TargetData> starList = new List<TargetData>();
            if (!CheckStarchiveFile()) return starList;
            XElement starAllXdata = XElement.Load(cfg.StarchiveFilePath);
            IEnumerable<XElement> starXlist = starAllXdata.Elements(PhotometryRecordX);
            foreach (XElement star in starXlist)
            {
                if ((star.Element(TargetNameX).Value == tgt.TargetName))
                //&& (star.Element(CatalogNameX).Value == tgt.CatalogName)
                //&& (star.Element(PrimaryStandardColorX).Value == tgt.PrimaryStandardColor)
                //&& (star.Element(DifferentialStandardColorX).Value == tgt.DifferentialStandardColor)
                //&& (star.Element(PrimaryFilterX).Value == tgt.PrimaryImageFilter)
                //&& (star.Element(DifferentialFilterX).Value == tgt.DifferentialImageFilter))
                {
                    TargetData pd = CreateTarget(star);
                    starList.Add(pd);
                }
            }
            return starList;
        }

        public static bool HasMatchingPhotometryRecord(TargetData tgt)
        {
            List<TargetData> pRecords = RetrievePhotometry(tgt);
            foreach (TargetData p in pRecords)
            {
                if ((DateTime.Compare(p.SessionDate, tgt.SessionDate) == 0) &&
                    (p.PrimaryStandardColor == tgt.PrimaryStandardColor) &&
                    (p.CatalogName == tgt.CatalogName) &&
                    (p.DifferentialStandardColor == tgt.DifferentialStandardColor) &&
                    (p.CatalogName == tgt.CatalogName) &&
                    (p.PrimaryStandardColor == tgt.PrimaryStandardColor) &&
                    (p.DifferentialStandardColor == tgt.DifferentialStandardColor) &&
                    (p.PrimaryImageFilter == tgt.PrimaryImageFilter) &&
                    (p.DifferentialImageFilter == tgt.DifferentialImageFilter))

                    return true;
            }
            return false;
        }

        public static List<TargetData> RetrieveAllPhotometry()
        {
            Configuration cfg = new Configuration();
            List<TargetData> starList = new List<TargetData>();
            if (!CheckStarchiveFile()) return starList;
            XElement starAllXdata = XElement.Load(cfg.StarchiveFilePath);
            IEnumerable<XElement> starXlist = starAllXdata.Elements(PhotometryRecordX);
            foreach (XElement star in starXlist)
            {
                TargetData pd = CreateTarget(star);
                starList.Add(pd);
            }
            return starList;
        }

        public static List<TargetData> RetrieveAllPhotometrySummarized()
        {
            Configuration cfg = new Configuration();
            List<TargetData> starchiveList = new List<TargetData>();
            List<TargetData> starTgtList = new List<TargetData>();
            List<TargetData> starCondensedList = new List<TargetData>();

            //If no starchive, then return a null list
            if (!CheckStarchiveFile()) return starTgtList;
            //Load starchive
            XElement starchiveAllXdata = XElement.Load(cfg.StarchiveFilePath);
            IEnumerable<XElement> starchiveXlist = starchiveAllXdata.Elements(PhotometryRecordX);
            //Translate each entry to a target structure
            //Create list of all targets
            foreach (XElement srec in starchiveXlist)
            {
                TargetData pd = CreateTarget(srec);
                starTgtList.Add(pd);
            }
            var starGroups = starTgtList.OrderBy(s => s.TargetName).GroupBy(s => s.TargetName);

            return starCondensedList;
        }

        private static XElement FormXrecord(TargetData pData)
        {
            XElement newStarXdata = new XElement(PhotometryRecordX,
                                       new XElement(IsTransformedX, pData.IsTransformed),
                                       new XElement(TargetNameX, pData.TargetName),
                                       new XElement(SessionDateX, pData.SessionDate.ToString("d")),
                                       new XElement(CatalogNameX, pData.CatalogName),
                                       new XElement(ImageDateX, pData.ImageDate.ToString()),
                                       new XElement(TargetRAX, pData.TargetRA.ToString()),
                                       new XElement(TargetDecX, pData.TargetDec.ToString()),
                                       new XElement(PrimaryStandardColorX, pData.PrimaryStandardColor),
                                       new XElement(DifferentialStandardColorX, pData.DifferentialStandardColor),
                                       new XElement(PrimaryFilterX, pData.PrimaryImageFilter),
                                       new XElement(DifferentialFilterX, pData.DifferentialImageFilter),
                                       new XElement(ColorTransformX, pData.ColorTransform.ToString()),
                                       new XElement(MagnitudeTransformX, pData.MagnitudeTransform.ToString()),
                                       new XElement(StandardColorMagnitudeX, pData.StandardColorMagnitude.ToString()),
                                       new XElement(StandardMagnitudeErrorX, pData.StandardMagnitudeError.ToString()),
                                       new XElement(SourceToAPASSErrorX, pData.SourceToAPASSCatalogPositionError.ToString()),
                                       new XElement(SourceToGaiaErrorX, pData.SourceToGAIACatalogPositionError.ToString()),
                                       new XElement(APASSFieldStarCountX, pData.ApassStarCount.ToString()),
                                       new XElement(GaiaFieldStarCountX, pData.GaiaStarCount.ToString()),
                                       new XElement(AirMassX, pData.AirMass.ToString()));
            return newStarXdata;

        }

        private static TargetData CreateTarget(XElement xData)
        {
            TargetData tData = new TargetData
            {
                TargetName = FetchX(xData, TargetNameX, "None"),
                IsTransformed = Convert.ToBoolean(FetchX(xData, IsTransformedX, "False")),
                SessionDate = Convert.ToDateTime(FetchX(xData, SessionDateX, DateTime.MinValue.ToString())),
                CatalogName = FetchX(xData, CatalogNameX, "APASS"),
                ImageDate = Convert.ToDateTime(FetchX(xData, ImageDateX, DateTime.MinValue.ToString())),
                TargetRA = Convert.ToDouble(FetchX(xData, TargetRAX, "0.0")),
                TargetDec = Convert.ToDouble(FetchX(xData, TargetDecX, "0.0")),
                PrimaryImageFilter = FetchX(xData, PrimaryFilterX, "None"),
                DifferentialImageFilter = FetchX(xData, DifferentialFilterX, "None"),
                PrimaryStandardColor = FetchX(xData, PrimaryStandardColorX, "None"),
                DifferentialStandardColor = FetchX(xData, DifferentialStandardColorX, "None"),
                ColorTransform = Convert.ToDouble(FetchX(xData, ColorTransformX, "0.00")),
                MagnitudeTransform = Convert.ToDouble(FetchX(xData, MagnitudeTransformX, "0.00")),
                StandardColorMagnitude = Convert.ToDouble(FetchX(xData, StandardColorMagnitudeX, "0.0")),
                StandardMagnitudeError = Convert.ToDouble(FetchX(xData, StandardMagnitudeErrorX, "0.0")),
                SourceToAPASSCatalogPositionError = Convert.ToDouble(FetchX(xData, SourceToAPASSErrorX, "0.0")),
                SourceToGAIACatalogPositionError = Convert.ToDouble(FetchX(xData, SourceToGaiaErrorX, "0.0")),
                ApassStarCount = Convert.ToInt32(FetchX(xData, APASSFieldStarCountX, "0")),
                GaiaStarCount = Convert.ToInt32(FetchX(xData, GaiaFieldStarCountX, "0")),
                AirMass = Convert.ToDouble(FetchX(xData, AirMassX, "0")),
                MasterCatalogInfo = new StarField.CatalogData(),
            };

            return tData;
        }

        private static string FetchX(XElement x, string name, string defaultValue)
        {
            if (x.Element(name) == null)
                return defaultValue;
            else return x.Element(name).Value;
        }

        private static bool IsSameNight(DateTime a, DateTime b)
        {
            //Checks if two datatimes are within same night, i.e. within 12 hours of each other.
            if ((a - b).TotalHours < 12) return true;
            else return false;
        }

        public static void ClearStarchiveSession(DateTime sessionDate, string catName)
        {
            //Removes all entries from Starchive for a given session
            Configuration cfg = new Configuration();
            XElement newStarchive = new XElement(XMLRoot);
            if (!CheckStarchiveFile())
                return;
            XElement starAllXdata = XElement.Load(cfg.StarchiveFilePath);
            List<XElement> starXlist = starAllXdata.Elements(PhotometryRecordX).ToList();
            var vb = starXlist.Where(x => (Convert.ToDateTime(x.Element(SessionDateX).Value) != sessionDate) || (x.Element(CatalogNameX).Value != catName));
            foreach (XElement x in vb)
                newStarchive.Add(vb);
            newStarchive.Save(cfg.StarchiveFilePath);
            return;
        }

        public static void ClearStarchiveTarget(string targetName, string catName)
        {
            //Removes all entries from Starchive for a given target and field catalog
            Configuration cfg = new Configuration();
            XElement newStarchive = new XElement(XMLRoot);
            if (!CheckStarchiveFile())
                return;
            XElement starAllXdata = XElement.Load(cfg.StarchiveFilePath);
            List<XElement> starXlist = starAllXdata.Elements(PhotometryRecordX).ToList();
            var vb = starXlist.Where(x => (x.Element(TargetNameX).Value != targetName) || (x.Element(CatalogNameX).Value != catName));
            foreach (XElement x in vb)
                newStarchive.Add(vb);
            newStarchive.Save(cfg.StarchiveFilePath);
            return;
        }

        public static void ClearStarchiveCatalog(string catName)
        {
            //Removes all entries from Starchive for a given target and field catalog
            Configuration cfg = new Configuration();
            XElement newStarchive = new XElement(XMLRoot);
            if (!CheckStarchiveFile())
                return;
            XElement starAllXdata = XElement.Load(cfg.StarchiveFilePath);
            List<XElement> starXlist = starAllXdata.Elements(PhotometryRecordX).ToList();
            var vb = starXlist.Where(x => x.Element(CatalogNameX).Value != catName);
            foreach (XElement x in vb)
                newStarchive.Add(vb);
            newStarchive.Save(cfg.StarchiveFilePath);
            return;
        }

        public static void ClearStarchive()
        {
            //Removes all entries from Starchive
            Configuration cfg = new Configuration();
            XElement newStarchive = new XElement(XMLRoot);
            if (!CheckStarchiveFile())
                return;
            newStarchive.Save(cfg.StarchiveFilePath);
            return;
        }
    }
}

