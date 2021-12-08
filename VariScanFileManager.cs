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
            //IEnumerable<string> imageDirs = Directory.EnumerateDirectories(cfg.ImageBankFolder);
            if (Directory.Exists(imageDir))
                imageFiles = Directory.EnumerateFiles(imageDir, "*.fit").ToList();
            return imageFiles;
        }

        public static List<string> GetVaultList()
        {
            Configuration cfg = new Configuration();
            //get the image bank folder name from configuration file
            //make a new one if it didn't exist
            string iBank = cfg.ImageBankFolder;
            if (!Directory.Exists(iBank))
                Directory.CreateDirectory(iBank);
            //get the list of target directories in the bank and cull it for empty target directories
            List<string> imageBankDirs = Directory.GetDirectories(iBank).ToList();
            List<string> targetDirs = new List<string>();
            foreach (string f in imageBankDirs)
                if (Directory.GetFiles(f).Length > 0) targetDirs.Add(f);
            //sort alphabetically
            targetDirs.Sort();
            return targetDirs;
        }

        public static List<string> GetTargetSessionPaths(string targetName, DateTime sessionDate)
        {
            //REturn list of files for a session date for a target
            Configuration cfg = new Configuration();
            List<string> targetPaths = new List<string>();
            //Look for image bank folder, create it if missing then return empty handed
            string iBank = cfg.ImageBankFolder;
            if (!Directory.Exists(iBank))
            {
                Directory.CreateDirectory(iBank);
                return (targetPaths);
            }
            //take a look in the iBankName target directory
            string iBankDir = iBank + "\\" + targetName;
            List<string> datedFiles = Directory.GetFiles(iBankDir).ToList();
            //parse and convert each dir name to a date, then add to list
            foreach (string f in datedFiles)
            {
                string[] splitName = Path.GetFileName(f).Split(' ');
                foreach (string s in splitName)
                    if (s.Contains('-'))
                    {
                        DateTime fileSession = Convert.ToDateTime(s);
                        if (fileSession == sessionDate)
                            datedFiles.Add(f);
                    }
            }
            return datedFiles.ToList();
        }

        public static List<DateTime> SessionDates(string targetName)
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
            List<string> datedFiles = Directory.GetFiles(iBankDir).ToList();
            //parse and convert each dir name to a date, then add to list
            foreach (string f in datedFiles)
            {
                FitsFileStandAlone ffso = new FitsFileStandAlone(f);
                dateList.Add(Utility.GetImageSession(ffso.FitsLocalDateTime));
                //string[] splitName = Path.GetFileName(f).Split(' ');
                //foreach (string s in splitName)
                //    if (s.Contains('-'))
                //    {
                //        try
                //        {
                //            DateTime fileSession = Convert.ToDateTime(s);
                //            dateList.Add(fileSession);
                //        }
                //        catch { }
                //    }
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
            List<string> filterFiles = Directory.GetFiles(iBankDir).ToList();
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
            imageDate = Utility.GetImageSession(ff.FitsLocalDateTime);
            return imageDate;
        }

        public static string StripPath(string dirPath)
        {
            //Removes all path information except the directory name 
            string[] map = dirPath.Split('\\');
            return map[map.Length - 1];
        }


    }
}
