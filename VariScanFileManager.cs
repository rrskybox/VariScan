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
using System.Windows.Forms;
using TheSky64Lib;


//Static class for opening fits files inside of TSX

namespace VariScan
{
    public static class VariScanFileManager
    {

        public static bool DirectOpenFitsFilePath(ccdsoftImage tsximg, string filePath)
        {
            tsximg.Path = filePath;
            try { tsximg.Open(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public static bool DirectOpenFitsFile(ccdsoftImage tsximg, string filePath)
        {
            tsximg.Path = filePath;
            try { tsximg.Open(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public static bool DialogOpenFitsFile(ccdsoftImage tsximg)
        {
            using (OpenFileDialog fitsDialog = new OpenFileDialog())
            {
                fitsDialog.Filter = "*.fit|*.fit";
                if (fitsDialog.ShowDialog() == DialogResult.OK)
                {
                    tsximg.Path = fitsDialog.FileName;
                    try { tsximg.Open(); }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return false;
                    }
                    return true;
                }
                else return false;
            }
        }

        public static bool AttachFitsFile(ccdsoftImage tsximg)
        {
            try { tsximg.AttachToActive(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        public static void CloseFitsFile(ccdsoftImage tsximg)
        {
            try { tsximg.Close(); }
            catch { return; };
            return;
        }

        public static string DialogOpenImageFile(ccdsoftImage tsximg, string varName)
        {
            using (OpenFileDialog imageFileDialog = new OpenFileDialog())
            {
                imageFileDialog.Filter = "*.fit|*.fit";
                Configuration cfg = new Configuration();
                string imageFileDirectory = cfg.ImageBankFolder + "\\" + varName;
                if (Directory.Exists(imageFileDirectory))
                {
                    imageFileDialog.InitialDirectory = imageFileDirectory;
                    if (imageFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        tsximg.Path = imageFileDialog.FileName;
                        try { tsximg.Open(); }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }

                        return imageFileDialog.FileName;
                    }
                    else return null;
                }
                return null;
            }
        }

        public static bool DialogOrphanImageFile(ccdsoftImage tsximg, string varName)
        {
            using (OpenFileDialog imageFileDialog = new OpenFileDialog())
            {
                imageFileDialog.Filter = "*.fit|*.fit";
                Configuration cfg = new Configuration();
                //string imageFileDirectory = cfg.ImageBankFolder + "\\" + varName;

                if (imageFileDialog.ShowDialog() == DialogResult.OK)
                {
                    tsximg.Path = imageFileDialog.FileName;
                    try { tsximg.Open(); }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return false;
                    }
                    return true;
                }
                return false;
            }
        }

        public static void CloseImageFile(ccdsoftImage tsximg)
        {
            try
            {
                tsximg.DetachOnClose = 0;
                tsximg.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error on fits close: " + ex.Message);
                return;
            };
            return;
        }

        public static List<string> TargetImageList(string targetName)
        {
            Configuration cfg = new Configuration();
            List<string> imageFiles = null;
            string imageDir = cfg.ImageBankFolder + "\\" + targetName;
            if (Directory.Exists(imageDir))
                imageFiles = Directory.EnumerateFiles(imageDir, "*.fit").ToList();
            return imageFiles;
        }

        public static List<string> GetTargetPathList()
        {
            //Make a list of paths for all target folders with images
            Configuration cfg = new Configuration();
            //get the image bank folder name from configuration file
            //make a new one if it didn't exist
            string iBank = cfg.ImageBankFolder;
            if (!Directory.Exists(iBank))
                Directory.CreateDirectory(iBank);
            //get the list of target directories in the bank and cull it for empty target directories
            List<string> imageBankDirs = Directory.GetDirectories(iBank).ToList();
            List<string> targetDirs = new List<string>();
            foreach (string iDir in imageBankDirs)
                if (Directory.GetFiles(iDir).Length > 0)
                    targetDirs.Add(iDir);
            //sort alphabetically
            targetDirs.Sort();
            return targetDirs;
        }

        public static List<string> GetTargetNameList()
        {
            //Make a list of target names for all target folders with images
            Configuration cfg = new Configuration();
            //get the image bank folder name from configuration file
            //make a new one if it didn't exist
            string iBank = cfg.ImageBankFolder;
            if (!Directory.Exists(iBank))
                Directory.CreateDirectory(iBank);
            //get the list of target directories in the bank and cull it for empty target directories
            List<string> imageBankDirs = Directory.GetDirectories(iBank).ToList();
            List<string> targetDirNames = new List<string>();
            foreach (string iDir in imageBankDirs)
                if (Directory.GetFiles(iDir, "*.fit").Length > 0)
                    targetDirNames.Add(Path.GetFileNameWithoutExtension(iDir));
            //cull duplicates and sort alphabetically
            targetDirNames.Distinct().ToList().Sort();
            return targetDirNames;
        }

        public static List<string> GetTargetSessionSetList(string tgtName)
        {
            //Make a list of target names for all target folders with images
            List<string> targetSessionSetList = new List<string>();
            List<string> targetNameList = GetTargetNameList();
            return targetNameList;
        }
 
        public static List<string> GetCollectionFilenameList()
        {
            //Make a list of all image filenames in image bank
            Configuration cfg = new Configuration();
            //get the image bank folder name from configuration file
            //make a new one if it didn't exist
            string iBank = cfg.ImageBankFolder;
            if (!Directory.Exists(iBank))
                Directory.CreateDirectory(iBank);
            //get the list of target directories in the bank and cull it for empty target directories
            List<string> targetDirNames = new List<string>();
            foreach (string iDir in Directory.GetDirectories(iBank).ToList())
                foreach (string iFile in Directory.GetFiles(iDir, "*.fit").ToList())
                    targetDirNames.Add(Path.GetFileNameWithoutExtension(iFile));
            //cull duplicates and sort alphabetically
            targetDirNames.Distinct().ToList().Sort();
            return targetDirNames;
        }

        public static List<string> GetTargetPathList(DateTime imageDate)
        {
            //Return list of image directories that contain images on the imageDate (full path)
            List<string> nonEmptyTargetDirectories = new List<string>();
            //Get list of imagefile out of each target directory
            foreach (string nonEmptyDirectory in GetTargetPathList())
            {
                foreach (string path in Directory.GetFiles(nonEmptyDirectory, "*.fit").ToList())
                {
                    (string tName, string iD, string iF, string iC, string iS) =
                            ParseImageFileName(Path.GetFileNameWithoutExtension(path));
                    if (Convert.ToDateTime(iD) == imageDate)
                    {
                        nonEmptyTargetDirectories.Add(nonEmptyDirectory);
                        break;
                    }
                }
            }
            nonEmptyTargetDirectories.Distinct().ToList().Sort();
            return nonEmptyTargetDirectories;
        }

        public static List<string> GetAllImageDates()
        {
            //Returns sorted list of all dates that have images in image bank
            List<string> imageDateList = new List<string>();
            //Get list of imagefile out of each target directory
            foreach (string nonEmptyDirectory in GetTargetPathList())
            {
                foreach (string path in Directory.GetFiles(nonEmptyDirectory, "*.fit").ToList())
                {
                    (string n, string iD, string iF, string iC, string iS) =
                            ParseImageFileName(Path.GetFileNameWithoutExtension(path));
                    imageDateList.Add(iD);
                }
            }
            imageDateList = imageDateList.Distinct().ToList();
            imageDateList.Sort(new SpecialDateComparer());
            return imageDateList;
        }

        public static List<string> GetTargetSessionPaths(string targetName, DateTime sessionDate)
        {
            //REturn list of files for a session date for a target
            //A Session Date is a window from +/- 6 hours of 0:00 AM of file creation time

            Configuration cfg = new Configuration();
            List<string> datedFiles = new List<string>();
            //Look for image bank folder, create it if missing then return empty handed
            string iBank = cfg.ImageBankFolder;
            if (!Directory.Exists(iBank))
            {
                Directory.CreateDirectory(iBank);
                return (datedFiles);
            }
            //take a look in the iBankName target directory
            string iBankDir = iBank + "\\" + targetName;
            //parse and convert each dir name to a date, then add to list
            foreach (string f in Directory.GetFiles(iBankDir, "*.fit").ToList())
            {
                (string tName, string iDate, string iFilter, string iSeq, string iSet) = ParseImageFileName(Path.GetFileNameWithoutExtension(f));
                DateTime fileDate = Convert.ToDateTime(iDate);
                //DateTime fileDate = File.GetCreationTime(f);
                if (Utility.NightTest(fileDate, sessionDate))
                    datedFiles.Add(f);
            }
            return datedFiles.ToList();
        }

        public static List<string> GetTargetSessionPaths(string targetName, DateTime sessionDate, int sessionSet)
        {
            //REturn list of files for a session date for a target
            //A Session Date is a window from +/- 6 hours of 0:00 AM of file creation time

            Configuration cfg = new Configuration();
            List<string> datedFiles = new List<string>();
            //Look for image bank folder, create it if missing then return empty handed
            string iBank = cfg.ImageBankFolder;
            if (!Directory.Exists(iBank))
            {
                Directory.CreateDirectory(iBank);
                return (datedFiles);
            }
            //take a look in the iBankName target directory
            string iBankDir = iBank + "\\" + targetName;
            //parse and convert each dir name to a date, then add to list
            foreach (string f in Directory.GetFiles(iBankDir, "*.fit").ToList())
            {
                (string tName, string iDate, string iFilter, string iSeq, string iSet) = ParseImageFileName(Path.GetFileNameWithoutExtension(f));
                DateTime fileDate = Convert.ToDateTime(iDate);
                //DateTime fileDate = File.GetCreationTime(f);
                if (Utility.NightTest(fileDate, sessionDate) && Convert.ToInt32(iSet) == sessionSet)
                    datedFiles.Add(f);
            }
            return datedFiles.ToList();
        }

        public static List<string> GetTargetSessionPaths(string targetName, DateTime sessionDate, string filter)
        {
            //REturn list of files for a session date for a target and filter
            Configuration cfg = new Configuration();
            List<string> datedFilterFiles = new List<string>();
            //Look for image bank folder, create it if missing then return empty handed
            string iBank = cfg.ImageBankFolder;
            if (!Directory.Exists(iBank))
            {
                Directory.CreateDirectory(iBank);
                return (datedFilterFiles);
            }
            //take a look in the iBankName target directory
            string iBankDir = iBank + "\\" + targetName;
            //parse and convert each dir name to a date, then add to list
            foreach (string f in Directory.GetFiles(iBankDir, "*.fit").ToList())
            {
                (string tName, string iDate, string iFilter, string iSeq, string iSet) = ParseImageFileName(Path.GetFileNameWithoutExtension(f));
                //if (Utility.NightTest(Convert.ToDateTime(iDate), sessionDate) && iFilter == filter)
                if (Convert.ToDateTime(iDate) == sessionDate && iFilter == filter)
                    datedFilterFiles.Add(f);
            }
            return datedFilterFiles.ToList();
        }

        public static List<DateTime> GetSessionDates(string targetName)
        {
            //Return list of session dates for this target
            Configuration cfg = new Configuration();
            List<DateTime> dateList = new List<DateTime>();
            //Look for image bank folder, create it if missing then return empty handed
            string iBank = cfg.ImageBankFolder;
            if (!Directory.Exists(iBank))
            {
                Directory.CreateDirectory(iBank);
                return (dateList);
            }
            //take a look in the iBankName target directory
            string iBankDir = iBank + "\\" + targetName;
            List<string> datedFiles = Directory.GetFiles(iBankDir, "*.fit").ToList();
            //parse and convert each dir name to a date, then add to list
            foreach (string f in datedFiles)
            {
                FitsFileStandAlone ffso = new FitsFileStandAlone(f);
                dateList.Add(Utility.GetImageSessionDate(ffso.FitsLocalDateTime));
            }
            dateList.Sort();
            return dateList.Distinct().ToList();
        }

        public static List<string> GetAllVaultFilters(string iBankName)
        {
            Configuration cfg = new Configuration();
            List<string> targetFilters = new List<string>();
            //Look for image bank folder, create it if missing then return empty handed
            string iBank = cfg.ImageBankFolder;
            if (!Directory.Exists(iBank))
            {
                Directory.CreateDirectory(iBank);
                return (targetFilters);
            }
            //take a look in the iBankName target directory
            string iBankDir = iBank + "\\" + iBankName;
            List<string> filterFiles = Directory.GetFiles(iBankDir, "*.fit").ToList();
            //parse and convert each dir name to a date, then add to list
            if (filterFiles.Count > 0)
            {
                foreach (string f in filterFiles)
                {
                    string[] splitName = Path.GetFileName(f).Split(' ');
                    for (int i = 0; i < splitName.Length; i++)
                        if (splitName[i] == "F")
                        {
                            int filterIndex = Convert.ToInt32(splitName[i + 1]);
                            targetFilters.Add(Filters.LookUpFilterName(filterIndex));
                        }
                }
                return targetFilters.Distinct().ToList();
            }
            else
                return targetFilters.ToList();
        }

        public static IEnumerable<string> ImageDirectoryList()
        {
            Configuration cfg = new Configuration();
            IEnumerable<string> imageDirs = null;
            string imageDir = cfg.ImageBankFolder;
            if (Directory.GetDirectories(imageDir).Length > 0)
                imageDirs = Directory.EnumerateDirectories(imageDir);
            return imageDirs;
        }

        public static bool HasImages(string targetName)
        {
            Configuration cfg = new Configuration();
            string[] imageFiles = null;
            string imageDir = cfg.ImageBankFolder + "\\" + targetName;
            if (Directory.Exists(imageDir))
                imageFiles = Directory.GetFiles(imageDir, "*.fit");
            if (imageFiles == null)
                return false;
            else return true;
        }

        public static DateTime GetCreationDate(string targetImageFilePath)
        {
            //Looks up the creation date for the file 
            return File.GetCreationTime(targetImageFilePath);
        }

        public static DateTime GetImagingDate(string targetImageFilePath)
        {
            //Looks up the imaging date for the file
            DateTime imageDate;
            FitsFileStandAlone ff = new FitsFileStandAlone(targetImageFilePath);
            imageDate = ff.FitsLocalDateTime;
            return imageDate;
        }

        public static DateTime GetSessionDate(string targetImageFilePath)
        {
            //Looks up the imaging date for the file
            DateTime imageDate;
            FitsFileStandAlone ff = new FitsFileStandAlone(targetImageFilePath);
            imageDate = Utility.GetImageSessionDate(ff.FitsLocalDateTime);
            return imageDate;
        }

        public static string StripPath(string dirPath)
        {
            //Removes all path information except the directory name 
            string[] map = dirPath.Split('\\');
            return map[map.Length - 1];
        }

        public static (string, string, string, string, string) ParseImageFileName(string imageFilenameWithoutExtension)
        {
            //returns separated strings for date, filter and sequence number and time of creation
            //  from variscan image filenaming conventions
            // 
            char[] sp = { ' ' };

            string[] p = imageFilenameWithoutExtension.Split(sp, StringSplitOptions.RemoveEmptyEntries);
            string setStr = p[p.Length - 1].Substring(1);
            string seqStr = p[p.Length - 2].Substring(1);
            char[] fHdr = new char[] { 'F', '_' };
            string filterStr = p[p.Length - 3].TrimStart(fHdr);
            string dateStr = p[p.Length - 4];
            string name = null;
            for (int i = 0; i <= p.Length - 4; i++)
                name += p[i] + " ";
            return (name.TrimEnd(' '), dateStr, filterStr, seqStr, setStr);
        }

    }
}
