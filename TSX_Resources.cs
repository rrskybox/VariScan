// --------------------------------------------------------------------------------
// VariScan module
//
// Description:	
//
// Environment:  Windows 10 executable, 64 bit
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
using System.IO;
using System.Reflection;
using TheSky64Lib;

namespace VariScan
{
    class TSX_Resources
    {

        private static string QueryDestinationSubPath = "Software Bisque\\TheSky Professional Edition 64\\Database Queries\\VariScanStandardFields.dbq";
        private static string SDBDestinationSubPath = "Software Bisque\\TheSky Professional Edition 64\\SDBs\\StandardFields.SDBX";

        public static void InstallDBQs()
        {
            //Make sure the dbq file paths are set up correctly
            string userDocumentsDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string queryDestinationPath = userDocumentsDirectory + "\\" + QueryDestinationSubPath;
            string sdbDestinationPath = userDocumentsDirectory + "\\" + SDBDestinationSubPath;

            //Install the dbq file
            //Collect the file contents to be written

            Assembly dgassembly = Assembly.GetExecutingAssembly();
            Stream dgstream = dgassembly.GetManifestResourceStream("VariScan.VariScanStandardFields.dbq");
            Byte[] dgbytes = new Byte[dgstream.Length];
            FileStream dbqgfile = File.Create(queryDestinationPath);
            int dgreadout = dgstream.Read(dgbytes, 0, (int)dgstream.Length);
            dbqgfile.Close();
            //write to destination file
            File.WriteAllBytes(queryDestinationPath, dgbytes);
            dgstream.Close();

            //Collect the file contents to be written
            Assembly dcassembly = Assembly.GetExecutingAssembly();
            Stream dcstream = dcassembly.GetManifestResourceStream("VariScan.StandardFields.SDBX");
            Byte[] dcbytes = new Byte[dcstream.Length];
            FileStream dbqcfile = File.Create(sdbDestinationPath);
            int dcreadout = dcstream.Read(dcbytes, 0, (int)dcstream.Length);
            dbqcfile.Close();
            //write to destination file
            File.WriteAllBytes(sdbDestinationPath, dcbytes);
            dcstream.Close();

            return;
        }

        public static void CenterStarChart(ccdsoftImage TSX_Image, TargetData targetData)
        {
            //Center the skychart on the ra/dec coordinates
            //Set the star chart size to 1.5 times the image width (fits the whole thing on, persumably
            //double scale = (double)TSX_Image.ScaleInArcsecondsPerPixel;
            double scale = TSX_Resources.GetFOVImageScale();
            double pixWidth = (double)TSX_Image.WidthInPixels;
            double imageWidthInArcSec = scale * pixWidth;

            sky6StarChart tsxc = new sky6StarChart
            {
                RightAscension = targetData.TargetRA,
                Declination = targetData.TargetDec,
                //FieldOfView = (imageWidthInArcSec * 1.5) / 3600  //in degrees
                FieldOfView = 1
            };
            return;
        }

        public static double GetFOVImageScale()
        {
            sky6MyFOVs tsxf = new sky6MyFOVs();
            int fovCount = tsxf.Count;
            for (int i = 0; i < fovCount; i++)
            {
                tsxf.Name(i);
                string fovName = tsxf.OutString;
                tsxf.Property(fovName, 0, sk6MyFOVProperty.sk6MyFOVProp_Visible);
                int vis = tsxf.OutVar;
                if (vis == 1)
                {
                    tsxf.Property(fovName, 0, sk6MyFOVProperty.sk6MyFOVProp_Scale);
                    double scale = tsxf.OutVar;
                    return scale;
                }
            }
            return 0;
        }

    }
}
