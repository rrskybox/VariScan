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
using System.IO;
using TheSky64Lib;


namespace VariScan
{
    public class FreshImage
    {
        //statements for linking logging method LogUpdate to the main form logging routine.
        public delegate void LogEventHandler(string LogText);
        public event LogEventHandler LogUpdate;

        private string freshImagePath = "";
        private string freshImageName = "";
        private double freshImageRA = 0;
        private double freshImageDec = 0;
        private double freshImageExposure = 0;
        private int freshImageFilterNumber = 0;
        //private double freshImageScale = 0;

        public FreshImage(TargetXList.TargetXDescriptor targetX, int filterNumber)
        {
            //populate freshimage info
            Configuration agncf = new Configuration();
            freshImageName = targetX.Name;
            freshImageRA = targetX.RA;
            freshImageDec = targetX.Dec;
            freshImageFilterNumber = filterNumber;
            freshImageExposure = targetX.Exposure;
            string imageDir = agncf.ImageBankFolder + "\\" + freshImageName;
            if (!Directory.Exists(imageDir)) Directory.CreateDirectory(imageDir);
            return;
        }

        private void SetNextImagePath(string baseDir)
        {
            //Finds the next image path that has a unique file name based on name format
            //  baseDir \\ image name \\ date \\ "F" \\ <filter> \\ "N" \\ <count> ".fits"
            Configuration cfg = new Configuration();
            int fCount = 0;
            do
            {
                DateTime sessionDate = Utility.GetImageSession(DateTime.Now);
                freshImagePath = baseDir + "\\" + freshImageName + " " +
                    sessionDate.ToString("dd-MMM-yyyy") +
                    " F_" + freshImageFilterNumber + " " +
                    " N" + fCount.ToString("0") + ".fit" +
                    " S" + cfg.CurrentSessionSet;
                fCount++;
            }
            while (File.Exists(freshImagePath));
        }

        //Creates a new instance of a FreshImage and sets the filepath for storing it
        //autoExpose is true if exposure is to be optimized
        //reposition is true if frame is to be recentered for each exposure
        //centerStar is true if maximum pixel for whole frame is to be used rather than center light source
        public bool Acquire(ref TargetXList.TargetXDescriptor currentTarget, bool autoExpose, bool reposition, bool frameCenter)
        {
            Configuration cfg = new Configuration();
            double currentExposure = currentTarget.Exposure;
            if (reposition)
                if (!SeekTarget()) return false;
            if (autoExpose)
                currentTarget.Exposure = OptimizeExposure(Convert.ToDouble(cfg.Exposure), frameCenter);
            if (!ShootTarget(currentTarget)) return false;
            return true;
        }

        //Find the coordinates of the object targetName and perform a slew, then CLS to it.
        private bool SeekTarget()
        {
            Configuration cfg = new Configuration();
            sky6StarChart tsx_sc = new sky6StarChart();
            ClosedLoopSlew tsx_cl = new ClosedLoopSlew();
            sky6RASCOMTele tsx_mt = new sky6RASCOMTele();
            sky6Raven tsx_rv = new sky6Raven();
            sky6ObjectInformation tsx_obj = new sky6ObjectInformation();

            //Clear any camera set up stuff that might be hanging around
            //  and there has been some on occasion
            //Removed subframe on request for cameras with long download times
            ccdsoftCamera tsx_cc = new ccdsoftCamera()
            {
                Subframe = 0,
                Delay = 0
            };

            LogEntry("Finding coordinates for " + freshImageName);
            tsx_sc.Find(this.freshImageRA.ToString() + "," + this.freshImageDec.ToString());

            //Make sure that the mount commands are synchronous
            tsx_mt.Asynchronous = 0;
            //LogEntry("Initial slew to target");
            ////Slew the mount and dome should follow before completion...
            // try { tsx_mt.SlewToRaDec(tRA, tDec, freshImageName); }
            //catch (Exception ex) { LogEntry("Slew error: " + ex.Message); }

            //Test to see if a dome tracking operation is underway.
            // If so, doing a IsGotoComplete will throw an Error 212.
            //  Ignore it a wait a few seconds for stuff to clear

            //If using dome, toggle dome coupling:  this appears to clear most Error 123 problems
            bool hasDome = Convert.ToBoolean(cfg.UsesDome);
            if (hasDome)
            {
                sky6Dome tsx_dm = new sky6Dome();
                tsx_dm.IsCoupled = 0;
                System.Threading.Thread.Sleep(1000);
                tsx_dm.IsCoupled = 1;
            }

            //Wait for any Error 123//s to clear

            DeviceControl dctl = new DeviceControl();
            int slewStatus = 0;
            if (Convert.ToBoolean(cfg.UseCLS))
            {
                LogEntry("Precision slew (CLS) to target");
                slewStatus = dctl.ReliableClosedLoopSlew(freshImageRA, freshImageDec, freshImageName, hasDome, cfg.CLSReduction);
                LogEntry("Precision Slew Complete");
            }
            else
            {
                LogEntry("Simple slew to target");
                dctl.ReliableRADecSlew(freshImageRA, freshImageDec, freshImageName, hasDome);
                slewStatus = dctl.ReliableClosedLoopSlew(freshImageRA, freshImageDec, freshImageName, hasDome, cfg.CLSReduction);
                LogEntry("Simple slew Complete");
            }
            if (slewStatus == 0)
            {
                LogEntry("    CLS or Slew successful");
                return true;
            }
            else
            {
                LogEntry("    CLS unsucessful: Error: " + slewStatus.ToString());
                return false;
            }
        }

        private double OptimizeExposure(double maxExposure, bool centerStar)
        {
            //Subframe size
            const int subFrameSize = 192;
            //Subframe the center in order to get rid of any brighter outliers 
            const int testExposure = 10;
            const double minExposure = 0.1;

            ccdsoftImage tsxi = new ccdsoftImage();
            ccdsoftCamera tsxc = new ccdsoftCamera(); // = new ccdsoftCamera();
            ImageLinkResults tsxilr = new ImageLinkResults();

            Configuration cfg = new Configuration();
            int ADUMax = Convert.ToInt32(cfg.ADUMax);
            double correctExposure = maxExposure;

            if (centerStar)
            {
                //Take a quick 10 second image, do not subframe, no reduction
                //  This is so we can set up a subframe around the target star
                //  at a 1x1 binning (the CLS may have been binned at something else
                tsxc = new ccdsoftCamera
                {
                    BinX = 1,
                    BinY = 1,
                    AutoSaveOn = 0,          //Autosave Off
                    FilterIndexZeroBased = freshImageFilterNumber,
                    ExposureTime = testExposure,
                    Subframe = 0,
                    Frame = ccdsoftImageFrame.cdLight,
                    ImageReduction = ccdsoftImageReduction.cdNone,
                    Asynchronous = 0        //Asynchronous off for this short shot
                };
                tsxc.TakeImage();
                //Attach to the image so we can get the center
                tsxi.AttachToActive();
                double xcenter = (tsxi.WidthInPixels) / 2;
                double ycenter = (tsxi.HeightInPixels) / 2;
                //Set up subframe
                int subTop = (int)ycenter - subFrameSize / 2; //Y is top down
                int subBottom = (int)ycenter + subFrameSize / 2; //Y is top down
                int subRight = (int)xcenter + subFrameSize / 2;
                int subLeft = (int)xcenter - subFrameSize / 2;
                tsxc = new ccdsoftCamera
                {
                    AutoSaveOn = 0,          //Autosave Off
                    FilterIndexZeroBased = freshImageFilterNumber,
                    ExposureTime = testExposure,
                    Subframe = 1,
                    SubframeTop = subTop,
                    SubframeBottom = subBottom,
                    SubframeLeft = subLeft,
                    SubframeRight = subRight,
                    Frame = ccdsoftImageFrame.cdLight,
                    ImageReduction = ccdsoftImageReduction.cdAutoDark,
                    Asynchronous = 0        //Asynchronous off for this short shot
                };
            }
            else
            {
                tsxc = new ccdsoftCamera
                {
                    AutoSaveOn = 0,          //Autosave Off
                    FilterIndexZeroBased = freshImageFilterNumber,
                    ExposureTime = testExposure,
                    Frame = ccdsoftImageFrame.cdLight,
                    ImageReduction = ccdsoftImageReduction.cdAutoDark,
                    Asynchronous = 0        //Asynchronous off for this short shot
                };
            }
            do
            {
                tsxc.TakeImage();
                double maxPixel = tsxc.MaximumPixel;
                correctExposure = (ADUMax / maxPixel) * tsxc.ExposureTime;
                if (correctExposure > maxExposure) correctExposure = maxExposure;
                freshImageExposure = correctExposure;
                if (maxPixel > ADUMax)
                    tsxc.ExposureTime = freshImageExposure;
            } while (tsxc.MaximumPixel > ADUMax && freshImageExposure > minExposure);
            return correctExposure;
        }

        private bool ShootTarget(TargetXList.TargetXDescriptor currentTarget)
        {
            Configuration cfg = new Configuration();
            int repetitions = Convert.ToInt32(cfg.ImagesPerSample);
            ccdsoftCamera tsx_cc = new ccdsoftCamera
            {
                AutoSaveOn = 0,          //Autosave Off
                FilterIndexZeroBased = freshImageFilterNumber,
                ExposureTime = freshImageExposure,
                Subframe = 0,
                Frame = ccdsoftImageFrame.cdLight,
                Asynchronous = 1        //Asynchronous on
            };
            //Set up for noise reduction, if any
            switch (cfg.ReductionType)
            {
                case "None":
                    {
                        tsx_cc.ImageReduction = ccdsoftImageReduction.cdNone;
                        LogEntry("No image calibration.");
                        break;
                    }
                case "Auto":
                    {
                        tsx_cc.ImageReduction = ccdsoftImageReduction.cdAutoDark;
                        LogEntry("Auto Dark image calibration set");
                        break;
                    }
                case "Full":
                    {
                        tsx_cc.ImageReduction = ccdsoftImageReduction.cdBiasDarkFlat;
                        Reduction calLib = new Reduction();
                        string binning = "1X1";
                        int camTemp = (int)tsx_cc.TemperatureSetPoint;
                        if (!calLib.SetReductionGroup(freshImageFilterNumber, freshImageExposure, camTemp, binning))
                        {
                            LogEntry("No calibration library found: " + "B_" + binning + "T_" + camTemp + "E_" + freshImageFilterNumber.ToString("0") + "F_" + freshImageFilterNumber.ToString("0"));
                            return false;
                        }
                        LogEntry("Full image calibration set: " + calLib.ReductionGroupName);
                        break;
                    }
            }
            //Loop on repetitions of image
            do
            {
                SetNextImagePath(cfg.ImageBankFolder + "\\" + freshImageName);
                ccdsoftImage tsx_im = new ccdsoftImage
                {
                    Path = freshImagePath
                };

                LogEntry("Imaging " + currentTarget.Name + " at RA: " + Utility.SexidecimalRADec(freshImageRA, true) +
                         " / Dec: " + Utility.SexidecimalRADec(freshImageDec, false));
                LogEntry("Filter set to " + freshImageFilterNumber.ToString("0"));
                LogEntry("Imaging target for " + freshImageExposure.ToString("0.0") + " secs");
                tsx_cc.TakeImage();
                //Wait for completion
                while (tsx_cc.State != ccdsoftCameraState.cdStateNone)
                {
                    System.Threading.Thread.Sleep(1000);
                    System.Windows.Forms.Application.DoEvents();
                }
                tsx_im.AttachToActiveImager();
                tsx_im.setFITSKeyword("OBJECT", freshImageName);
                tsx_im.Save();
                repetitions--;
            } while (repetitions > 0);
            LogEntry("Imaging target Complete");
            return true;
        }

        //Method to link to VariScan main form for logging progress.
        private void LogEntry(string upd)
        //Method for projecting log entry to the VariScan Main Form
        {
            LogUpdate(upd);
            return;
        }

    }
}
