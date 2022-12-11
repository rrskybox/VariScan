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

using System.IO;

namespace VariScan
{
    public class Report
    {

        public static void CreateSummaryReport(string textFilePath)
        {
            // Target Name, Target RA(j2K), Target Dec(j2K), Image UTC, Image Filter, Differential Catalog Mag, APASS Catalog Mag, Standard Catalog Variance, APASS Catalog Variance, 
            //
            // Description:
            // Load the Starchive File
            // Create an output text file
            // For each element in the starchive file,
            //   Build a string in the proposed format, with comma separation
            //   Save the string as a line in the text file
            const string header = "Target Name" + "," +
                                  "Target RA(j2K)" + "," +
                                  "Target Dec(j2K)" + "," +
                                  "Session Date" + "," +
                                  "Catalog" + "," +
                                  "Primary Image Filter" + "," +
                                  "DifferentialImage Filter" + "," +
                                  "Primary Color Standard" + "," +
                                  "Differential Color Standard" + "," +
                                  "Standard Magnitude" + "," +
                                  "Magnitude Std Dev" + "," +
                                  "Color Transform" + "," +
                                  "Magnitude Transform" + "," +
                                  "APASS Star Count" + "," +
                                  "Gaia Star Count";

            StreamWriter csvFile = File.CreateText(textFilePath);
            //write header
            csvFile.WriteLine(header);
            foreach (TargetData tData in Starchive.RetrieveAllPhotometry())
            {

                string bLine = tData.TargetName + "," +
                               Utility.SexidecimalRADec(tData.TargetRA, true) + "," +
                               Utility.SexidecimalRADec(tData.TargetDec, false) + ", " +
                               tData.SessionDate.ToString("MMM/dd/yyyy") + "," +
                               tData.CatalogName + "," +
                               tData.PrimaryImageFilter.ToString() + "," +
                               tData.DifferentialImageFilter.ToString() + "," +
                               tData.PrimaryStandardColor.ToString() + "," +
                               tData.DifferentialStandardColor.ToString() + "," +
                               tData.StandardColorMagnitude.ToString("0.000") + ", " +
                               tData.StandardMagnitudeError.ToString("0.000") + "," +
                               tData.ColorTransform.ToString("0.000") + "," +
                               tData.MagnitudeTransform.ToString("0.000") + "," +
                               tData.ApassStarCount.ToString() + "," +
                               tData.GaiaStarCount.ToString();

                if (tData.IsTransformed && tData.StandardColorMagnitude != 0)
                    csvFile.WriteLine(bLine);
            }
            csvFile.Close();
            return;
        }

        public static void CreateAAVSOReport(string textFilePath, string AAVSO_Observers_Code)
        {
            //https://www.aavso.org/aavso-extended-file-format

            const string EXTENDED = "Extended";
            const string VARSURVEYNAME = "VariScan 1.0 (TheSkyX)";
            const string DELIMITER = ",";
            const string EXCELDATETYPE = "EXCEL"; //EXCEL: the format created by Excel's NOW() function (Ex: 12/31/2007 12:59:59 a.m )
            const string OBSTYPECODE = "CCD";

            const string TYPE = "#TYPE";
            const string OBSCODE = "#OBSCODE";
            const string SOFTWARE = "#SOFTWARE";
            const string DELIM = "#DELIM";
            const string DATE = "#DATE";
            const string OBSTYPE = "#OBSTYPE";

            const string HEADERLINE = "#NAME,DATE,MAG,MERR,FILT,TRANS,MTYPE,CNAME,CMAG,KNAME,KMAG,AMASS,GROUP,CHART,NOTES";
            //Form Header
            string header = TYPE + "=" + EXTENDED + "\n" +
                            OBSCODE + "=" + AAVSO_Observers_Code + "\n" +
                            SOFTWARE + "=" + VARSURVEYNAME + "\n" +
                            DELIM + "=" + DELIMITER + "\n" +
                            DATE + "=" + EXCELDATETYPE + "\n" +
                            OBSTYPE + "=" + OBSTYPECODE + "\n" +
                            HEADERLINE;

            StreamWriter csvFile = File.CreateText(textFilePath);
            //write header
            csvFile.WriteLine(header);

            //for each entry in the Starchive, create a line
            foreach (TargetData tData in Starchive.RetrieveAllPhotometry())
            {
                if (tData.IsTransformed)
                {
                    string bline = tData.TargetName + DELIMITER; //STARID
                    string cline = tData.PrimaryStandardColor + "/" + tData.DifferentialStandardColor;
                    bline += tData.ImageDate.ToString() + DELIMITER; //DATE
                    bline += tData.StandardColorMagnitude + DELIMITER; //MAGNITUDE
                    bline += tData.StandardMagnitudeError + DELIMITER;//MAGERR
                    bline += tData.PrimaryStandardColor + DELIMITER;
                    bline += "NO" + DELIMITER;  //TRANS NOT LANDOLT STANDARDS
                    bline += "STD" + DELIMITER; //MTYPE 
                    bline += "na" + DELIMITER; //CNAME
                    bline += tData.SourceInstrumentMagnitude.ToString("0.000") + DELIMITER; //CMAG
                    bline += "na" + DELIMITER; //KNAME
                    bline += "ensemble" + DELIMITER; //KMAG
                    bline += tData.AirMass.ToString("0.000") + DELIMITER; //AIRMASS
                    bline += "na" + DELIMITER; //GROUP
                    bline += tData.CatalogName + "(" + cline + ")" + DELIMITER; //CHART
                    bline += "Full frame light source calibration via differential transformation using " + tData.ApassStarCount.ToString("0") + " " +
                             tData.CatalogName + " cataloged stars to transform " +
                             "Primary " + tData.PrimaryImageFilter + "/" +
                             "Differential " + tData.DifferentialImageFilter +
                             " filter data to " +
                             tData.PrimaryStandardColor + " standard color."; //NOTES

                    csvFile.WriteLine(bline);
                }
            }
            csvFile.Close();
            return;
        }
    }
}
