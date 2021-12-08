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
            public string ImagePath;
            public DateTime ImageDate;
            public string ImageFilter;
            public bool IsRegistrationMaster;
        }

        public SampleManager(string targetName)
        {
            Configuration cfg = new Configuration();
            string targetDirectoryPath = cfg.ImageBankFolder + "\\" + targetName;
            //Upon instantiation, create list of all images available for analysis
            SampleImages = new List<SessionSample>();
            //Open each of the fits files in target directory
            foreach (string path in Directory.GetFiles(targetDirectoryPath))
            {
                FitsFileStandAlone fit = new FitsFileStandAlone(path);
                SessionSample ss = new SessionSample()
                {
                    ImagePath = path,
                    ImageDate = fit.FitsLocalDateTime,
                    ImageFilter = fit.Filter
                };
                //string ext = Path.GetExtension(path);
                if (Path.GetExtension(path) == ".fit")
                    SampleImages.Add(ss);
            }
        }

        public List<DateTime> GetTargetSessions()
        {
            List<DateTime> dtList = new List<DateTime>();
            foreach (SessionSample ss in SampleImages)
                dtList.Add(Utility.GetImageSession(ss.ImageDate));
            return dtList.Distinct().ToList();
        }

        public List<string> GetTargetSessionFilters(DateTime sessionDT)
        {
            List<string> dtList = new List<string>();
            List<SessionSample> siList = new List<SessionSample>();
            siList = SampleImages.FindAll(x => Utility.NightTest(x.ImageDate, sessionDT));
            foreach (SessionSample ss in SampleImages)
                dtList.Add(ss.ImageFilter);
            return dtList.Distinct().ToList();
        }

        public List<SessionSample> GetSessionSamples(DateTime SessionDT, string filter)
        {
            //Extract and organize the images in the target directory which match time and filter
            List<SessionSample> siList = new List<SessionSample>();
            siList = SampleImages.FindAll(x => x.ImageFilter == filter && Utility.NightTest(x.ImageDate, SessionDT));
            return siList;
        }

        public static List<string> GetSessionTargets(DateTime SessionDT)
        {
            List<string> sessionTargets = new List<string>();
            Configuration cfg = new Configuration();
            foreach (string tDirPath in Directory.EnumerateDirectories(cfg.ImageBankFolder).ToList())
            {
                //Upon instantiation, create list of all images available for analysis
                List<SessionSample> sampleImages = new List<SessionSample>();
                //Open each of the fits files in target directory
                foreach (string fPath in Directory.GetFiles(tDirPath))
                {
                    FitsFileStandAlone fit = new FitsFileStandAlone(fPath);
                    SessionSample ss = new SessionSample()
                    {
                        ImagePath = fPath,
                        ImageDate = fit.FitsLocalDateTime,
                        ImageFilter = fit.Filter
                    };
                    sampleImages.Add(ss);
                }
                List<SessionSample> siList = new List<SessionSample>();
                siList = sampleImages.FindAll(x => Utility.NightTest(x.ImageDate, SessionDT));
                List<string> tList = new List<string>();
                if (tList.Count > 0)
                    sessionTargets.Add(Path.GetFileNameWithoutExtension(tDirPath));
            }
            return sessionTargets;
        }

        public List<string> GetSessionFilters(DateTime SessionDT, string filter)
        {
            List<SessionSample> siList = new List<SessionSample>();
            siList = SampleImages.FindAll(x => Utility.NightTest(x.ImageDate, SessionDT));
            List<string> tList = new List<string>();
            foreach (SessionSample ss in siList)
                if (!tList.Contains(ss.ImageFilter))
                    tList.Add(Path.GetFileNameWithoutExtension(ss.ImageFilter));
            return tList;
        }

    }
}


