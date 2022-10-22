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

namespace VariScan
{
    public class SampleManager
    {
        //Class for extracting and managing sets of fits files which comprise a target "sample" set
        //Extract and organize the images in the target directory into Primary and Differential filtered sets
        //  Duration of sample should be 1 hour

        public List<string> PrimarySampleImageFilePaths { get; set; }
        public List<string> DifferentialSampleImageFilePaths { get; set; }
        private List<SessionSample> SampleImages { get; set; }

        public struct SessionSample
        {
            public string TargetName;
            public string ImagePath;
            public DateTime ImageDate;
            public string ImageFilter;
            public bool IsRegistrationMaster;
        }

        public SampleManager()
        {
            //open an empty list of SampleImages
            SampleImages = new List<SessionSample>();
            return;
        }

        public SampleManager(string targetName)
        {
            //Upon instantiation, create list of all images available for analysis
            // within a Collection (as defined by ImageBAnkFolder/Configuration.xml)
            // for a named targetName
            Configuration cfg = new Configuration();
            string targetDirectoryPath = cfg.ImageBankFolder + "\\" + targetName;
            SampleImages = new List<SessionSample>();
            //Open each of the fits files in target directory
            foreach (string path in Directory.GetFiles(targetDirectoryPath))
            {
                FitsFileStandAlone fit = new FitsFileStandAlone(path);
                SessionSample ss = new SessionSample()
                {
                    TargetName = targetName,
                    ImagePath = path,
                    ImageDate = fit.FitsLocalDateTime,
                    ImageFilter = fit.Filter
                };
                //string ext = Path.GetExtension(path);
                if (Path.GetExtension(path) == ".fit")
                    SampleImages.Add(ss);
            }
        }

        public SampleManager(DateTime targetDate)
        {
            //Upon instantiation, create list of all images available for analysis
            // within a Collection (as defined by ImageBankFolder/Configuration.xml)
            // for a given targetDate
            Configuration cfg = new Configuration();
            SampleImages = new List<SessionSample>();
            //Get list of image directories for this date
            List<string> targetDirectories = VariScanFileManager.GetTargetPathList(targetDate);
            //Open each of the fits files in target directory
            if (targetDirectories != null)
                foreach (string targetDir in targetDirectories)
                    foreach (string targetFilePath in Directory.EnumerateFiles(targetDir))
                    {
                        FitsFileStandAlone fit = new FitsFileStandAlone(targetDir);
                        SessionSample ss = new SessionSample()
                        {
                            ImagePath = targetFilePath,
                            ImageDate = fit.FitsLocalDateTime,
                            ImageFilter = fit.Filter
                        };
                        if (Path.GetExtension(targetFilePath) == ".fit")
                            SampleImages.Add(ss);
                    }
        }

        public SampleManager(string targetName, DateTime targetDate)
        {
            //Upon instantiation, create list of all images available for analysis
            // within a Collection (as defined by ImageBankFolder/Configuration.xml)
            // for a given target on a given targetDate
            Configuration cfg = new Configuration();
            SampleImages = new List<SessionSample>();
            //Get list of image directories for this target and date
            List<string> targetDirectories = VariScanFileManager.GetTargetSessionPaths(targetName, targetDate);
            //Open each of the fits files in target directory
            if (targetDirectories != null)
                foreach (string targetFilePath in targetDirectories)
                {
                    if (Path.GetExtension(targetFilePath) == ".fit")
                    {
                        FitsFileStandAlone fit = new FitsFileStandAlone(targetFilePath);
                        SampleImages.Add(new SessionSample()
                        {
                            ImagePath = targetFilePath,
                            ImageDate = fit.FitsLocalDateTime,
                            ImageFilter = fit.Filter
                        });
                    }
                }
        }

        public List<DateTime> GetSessionDates()
        {
            List<DateTime> dtList = new List<DateTime>();
            foreach (SessionSample ss in SampleImages)
                dtList.Add(Utility.GetImageSession(ss.ImageDate));
            return dtList.Distinct().ToList();
        }

        public List<string> GetSessionFilters(DateTime sessionDT)
        {
            List<string> dtList = new List<string>();
            List<SessionSample> siList = new List<SessionSample>();
            siList = SampleImages.FindAll(x => Utility.NightTest(x.ImageDate, sessionDT));
            foreach (SessionSample ss in SampleImages)
                dtList.Add(ss.ImageFilter);
            return dtList.Distinct().ToList();
        }

        public List<SessionSample> SelectSessionSamples(DateTime SessionDT, string filter)
        {
            //Extract and organize the images in the target directory which match time and filter
            List<SessionSample> siList = new List<SessionSample>();
            siList = SampleImages.FindAll(x => x.ImageFilter == filter && Utility.NightTest(x.ImageDate, SessionDT));
            return siList;
        }

    }
}



