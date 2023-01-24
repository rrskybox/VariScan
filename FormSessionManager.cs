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
using System.Deployment.Application;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WeatherWatch;

namespace VariScan
{
    public partial class FormSessionManager : Form
    {
        public const int HomeAZ = 220;

        private TargetXList varList;
        private Logger ss_log = new Logger();

        private bool scanOn = false;

        public FormSessionManager()
        {
            //Initialize application form interfaces
            InitializeComponent();
            this.Location = Properties.Settings.Default.StartLocationXY;
            InitializeCollection();
            //Load variscan query and SDB's
            TSX_Resources.InstallDBQs();
        }

        public void InitializeCollection()
        {
            //Initialize VariScan configuration folders, files, default data, as needed
            Configuration cfg = new Configuration();

            //Populate the form configuration parameters with the configuration entries.
            ExposureTimeSetting.Value = Convert.ToDecimal(cfg.Exposure);
            MinAltitudeSetting.Value = Convert.ToDecimal(cfg.MinAltitude);
            AutoRunCheckBox.Checked = Convert.ToBoolean(cfg.AutoStart);
            switch (cfg.AutoFocus)
            {
                case "None":
                    {
                        AtFocusTypeBox.SelectedIndex = 0;
                        break;
                    }
                case "2":
                    {
                        AtFocusTypeBox.SelectedIndex = 1;
                        break;
                    }
                case "3":
                    {
                        AtFocusTypeBox.SelectedIndex = 2;
                        break;
                    }
                default:
                    {
                        AtFocusTypeBox.SelectedIndex = 0;
                        break;
                    }
            }
            CollectionGroupBox.Text = Path.GetFileName(cfg.CollectionFolderPath);
            WatchWeatherCheckBox.Checked = Convert.ToBoolean(cfg.WatchWeather);
            DomeCheckBox.Checked = Convert.ToBoolean(cfg.UsesDome);
            OnTopCheckBox.Checked = Convert.ToBoolean(cfg.SurveyFormOnTop);
            ReductionListBox.SelectedItem = cfg.ReductionType;
            CCDTemperatureSetting.Value = Convert.ToDecimal(cfg.CCDTemp);
            RetakeIntervalBox.Value = Convert.ToDecimal(cfg.MinRetakeInterval);
            AutoExposureCheckBox.Checked = Convert.ToBoolean(cfg.AutoADU);
            MaxADUBox.Value = (int)Convert.ToInt32(cfg.ADUMax);
            ImagesPerSampleBox.Value = (int)Convert.ToInt32(cfg.ImagesPerSample);
            FocusPresetBox.Checked = Convert.ToBoolean(cfg.UseFocusPreset);
            FocusFilterBox.SelectedIndex = Convert.ToInt32(cfg.FocusFilter);
            EnableCLSBox.Checked = Convert.ToBoolean(cfg.UseCLS);
            switch (cfg.CLSReduction)
            {
                case "None":
                    {
                        CLSReductionBox.SelectedIndex = 0;
                        break;
                    }
                case "2":
                    {
                        CLSReductionBox.SelectedIndex = 1;
                        break;
                    }
                case "3":
                    {
                        CLSReductionBox.SelectedIndex = 2;
                        break;
                    }
                default:
                    {
                        CLSReductionBox.SelectedIndex = 0;
                        break;
                    }
            }

            try
            { this.Text = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(); }
            catch
            {
                //probably in debug mode
                this.Text = " in Debug";
            }
            this.Text = "VariScan V" + this.Text;

            //Make sure all fields have been logged and saved before proceeding.
            Application.DoEvents();
            System.Threading.Thread.Sleep(1000);

            //Check for number of targets
            varList = new TargetXList();
            CurrentTargetCount.Text = varList.TargetXCount.ToString();

            LogEventHandler("");
            LogEventHandler("\r\n" + "********** Initiating VariScan Session **********");
            LogEventHandler("********** for " + CollectionGroupBox.Text + " Collection **********");
            LogEventHandler("Found " + CurrentTargetCount.Text + " available targets.\r\n");
            return;
        }

        private void StartScan()
        {
            //REad through the AGN entries, least recent first,
            // until either every target is up to date, or time runs out

            Configuration cfg = new Configuration();
            //Check for valid collection, if none then flash a warning and return
            if (cfg.CollectionFolderPath == "")
            {
                MessageBox.Show("You must configure a target collection before imaging.");
                return;
            }
            DeviceControl ss_hwp = new DeviceControl();
            //Check to see if scan already underway, if so, then just ignore
            //save current color of start scan button
            if (StartScanButton.BackColor == Color.LightCoral) return;
            Color scanbuttoncolorsave = StartScanButton.BackColor;
            StartScanButton.BackColor = Color.LightCoral;

            //AutoStart section
            //If AutoStart is enabled, then wait for 15 seconds for the user to disable, if desired
            //  Otherwise, initiate the scan
            if (AutoRunCheckBox.Checked)
            {
                LogEventHandler("\r\n" + "********** AutoRun Initiated **********" + "\r\n" + "Unless unchecked, AutoRun will begin in 15 seconds!\r\n");
                for (int i = 0; i < 60; i++)
                {
                    Show();
                    System.Windows.Forms.Application.DoEvents();
                    System.Threading.Thread.Sleep(250);
                    if (!AutoRunCheckBox.Checked)
                    {
                        LogEventHandler("\r\n" + "********** AutoRun Suspended **********" + "\r\n");
                        break;
                    }
                }
                //If AutoStart is still enabled, initiate PreScan, then StartScan, then PostScan
                //  while watching the weather
                //   Otherwise, exit on out
                if (AutoRunCheckBox.Checked)
                {
                    if (WatchWeatherCheckBox.Checked) WeatherManagement(ss_hwp, false);
                    LogEventHandler("Awaiting System Staging Time at " + cfg.StageSystemTime.ToString() + "\r\n");
                    Launcher.WaitStage();
                    if (WatchWeatherCheckBox.Checked) WeatherManagement(ss_hwp, false);
                    LogEventHandler("Running System Staging Program **********" + "\r\n");
                    Launcher.RunStageSystem();
                    if (WatchWeatherCheckBox.Checked) WeatherManagement(ss_hwp, false);
                    LogEventHandler("Awaiting Start Up Time at " + cfg.StartUpTime.ToString() + "\r\n");
                    Launcher.WaitStart();
                    LogEventHandler("Running Start Up Program **********" + "\r\n");
                    if (WatchWeatherCheckBox.Checked) WeatherManagement(ss_hwp, false);
                    Launcher.RunStartUp();
                }
            }

            varList = new TargetXList();
            CurrentTargetCount.Text = varList.TargetXCount.ToString();

            LogEventHandler("\r\n" + "********** Beginning Survey Run **********");
            LogEventHandler("Found " + CurrentTargetCount.Text + " prospective targets -- not all will be qualified.");

            // Scan Running...
            // Connect telescope mount and camera, and dome, if any
            if (ss_hwp.TelescopeStartUp()) LogEventHandler("Initializing Mount");
            else LogEventHandler("Mount initialization failed");
            if (ss_hwp.CameraStartUp()) LogEventHandler("Initializing Camera");
            else LogEventHandler("Camera initialization failed");
            if (Convert.ToBoolean(cfg.UsesDome))
            {
                if (DomeControl.DomeStartUp()) LogEventHandler("Initializing Dome");
                else LogEventHandler("Dome initialization failed");
            }
            ;
            Show();

            //Start the sequence on the west side.
            //Theoretically, nearly all targets on west side will be scanned before setting below limit,
            //  and all targets that have transited during that time.  Then all targets on the list which are east
            //  will be scanned.  Lastly, the scan will return to the west to pick up any targets that transited
            //  during the scan on the east side.  Get it?
            ss_hwp.TelescopePrePosition("West");
            //Check the focus with initialization (if enabled)
            FocusManagement(false);
            //Bring the camera temperature down
            LogEventHandler("Bringing camera to temperature");
            ss_hwp.SetCameraTemperature(Convert.ToDouble(CCDTemperatureSetting.Value));

            int gTriedCount = 0;
            int gSuccessfulCount = 0;

            //
            //
            //Main Loop on the list of targets =================
            //
            //Load the first (least recent) target
            // Loop
            //  If the next target's date is earlier than "today" then
            //  1. Check the weather, if enabled
            //  2. Check the focus (1 degree diff), if enabled
            //  3. Take an image and detect, if enabled
            //  4. Repeat if required.
            //  5. Get the next target 
            //

            double lowestAlt = Convert.ToDouble(cfg.MinAltitude);
            double westAz = 270;
            TargetXList.TargetXDescriptor currentTargetXD = varList.NextClosestTargetX(westAz, lowestAlt);
            //Load the filter plan
            ColorIndexing cL = new ColorIndexing();
            List<Filters.ActiveFilter> afList = cL.GetIndexFilters();
            //Load iteration count for this target
            while (currentTargetXD != null)
            {
                bool gotAllFilters = true;
                LogEventHandler("-------------------");
                LogEventHandler("New target: " + currentTargetXD.Name);
                currentTargetXD.LastImagingDate = DateTime.Now;
                //Check weather conditions, if enabled
                //  if unsafe then spin until it is safe or endingtime occurs.
                if (WatchWeatherCheckBox.Checked)
                    WeatherManagement(ss_hwp, true);
                //Check the focus, but turn off initialization
                FocusManagement(false);
                //Load target form fields
                CurrentTargetName.Text = currentTargetXD.Name;
                currentTargetXD.Exposure = Convert.ToDouble(ExposureTimeSetting.Value);
                //Loop through the list of filters to be imaged
                foreach (Filters.ActiveFilter af in afList)
                {
                    CurrentTargetFilter.Text = af.FilterName;
                    LogEventHandler("Imaging " + currentTargetXD.Name + ": Filter " + af.FilterName);
                    //Take fresh image
                    FreshImage fso = new FreshImage(currentTargetXD, af.FilterIndex);
                    fso.LogUpdate += LogEventHandler;
                    //Seek location of next target
                    //Ignor return value
                    if (fso.Acquire(ref currentTargetXD, Convert.ToBoolean(cfg.AutoADU), true, true))
                    {
                        LogEventHandler(currentTargetXD.Name + " Image capture complete.");
                        LogEventHandler(currentTargetXD.Name + ":" + " Banking new image in " + cfg.ImageBankFolder);
                    }
                    else
                    {
                        LogEventHandler(currentTargetXD.Name + ": " + " Image capture failed -- check log for problem.");
                        LogEventHandler("");
                        gotAllFilters = false;
                    }
                }
                //Increment the target count for reporting purposes
                if (gotAllFilters)
                    gSuccessfulCount++;
                //Update tries counter
                gTriedCount++;
                //Clear target from list and decrement targets left to image
                CurrentTargetCount.Text = (varList.TargetXCount - gTriedCount).ToString();
                Show();
                //Check for time to shut down
                LogEventHandler("Checking for ending time");
                if (Convert.ToBoolean(Launcher.CheckEnd()))
                {
                    LogEventHandler("Scan is past end time.  Shutting down.");
                    break;
                }
                //Get next target
                varList.UpdateTargetXDate(currentTargetXD);
                currentTargetXD = varList.NextClosestTargetX(currentTargetXD);
            }

            LogEventHandler("Session Ended at " + DateTime.Now.ToShortTimeString());
            LogEventHandler("Successfully imaged " + gSuccessfulCount + " out of " + gTriedCount + " qualified targets.");
            //List the focusing results
            LogEventHandler(AutoFocus.ListFocusResults());
            //Park the telescope so it doesn't drift too low
            ss_hwp.TelescopeShutDown();
            LogEventHandler("AutoRun Running Shut Down Program **********" + "\r\n");
            Launcher.RunShutDown();
            StartScanButton.BackColor = scanbuttoncolorsave;
            return;
        }

        private void FocusManagement(bool isInitializing)
        {
            LogEventHandler("Checking Focus");
            Configuration cfg = new Configuration();
            if (isInitializing && Convert.ToBoolean(cfg.UseFocusPreset))
            {
                string preSet = AutoFocus.PresetFocus();
                LogEventHandler(preSet);
            }
            switch (cfg.AutoFocus)
            {
                case "None":
                    {
                        LogEventHandler("Autofocus not selected");
                        break;
                    };
                case "2":
                    {
                        string focStat = AutoFocus.Check(true, Convert.ToInt32(cfg.FocusFilter));
                        LogEventHandler(focStat);
                        LogEventHandler("Stored Postion at Zero = " + cfg.PositionAtZero);
                        LogEventHandler("Stored Steps Per Degree = " + cfg.StepsPerDegree);
                        break;
                    };
                case "3":
                    {
                        string focStat = AutoFocus.Check(false, Convert.ToInt32(cfg.FocusFilter));
                        LogEventHandler(focStat);
                        LogEventHandler("Stored Postion at Zero = " + cfg.PositionAtZero);
                        LogEventHandler("Stored Steps Per Degree = " + cfg.StepsPerDegree);
                        break;
                    };
            }

        }

        private void WeatherManagement(DeviceControl ss_hwp, bool inSession)
        {
            //Check weather conditions, if enabled
            //  if unsafe AND a session is underway, then spin until it is safe or endingtime occurs.
            Configuration cfg = new Configuration();
            if (WatchWeatherCheckBox.Checked)
            {
                LogEventHandler("Checking Weather");
                if (!IsWeatherSafe())
                {
                    if (inSession)
                    {
                        LogEventHandler("Waiting on unsafe weather conditions...");
                        LogEventHandler("Parking telescope");
                        if (ss_hwp.TelescopeShutDown()) LogEventHandler("Mount parked");
                        else LogEventHandler("Mount park failed");

                        LogEventHandler("Closing Dome");
                        if (Convert.ToBoolean(cfg.UsesDome))
                        {
                            int homeStatus = DomeControl.CloseDome(HomeAZ);
                            if (homeStatus != 0)
                                LogEventHandler("As expected, TSX reports dome homing failure: Error " + homeStatus.ToString());
                        }
                    }
                    else LogEventHandler("Waiting on unsafe weather conditions...");
                    do
                    {
                        System.Threading.Thread.Sleep(10000);  //ten second wait loop
                        if (Convert.ToBoolean(Launcher.CheckEnd()))
                        { break; };
                    } while (!IsWeatherSafe());
                    if (Convert.ToBoolean(Launcher.CheckEnd()))
                    { return; };
                    if (IsWeatherSafe())
                    {
                        if (inSession)
                        {
                            LogEventHandler("Weather conditions safe");
                            LogEventHandler("Opening Dome");
                            if (Convert.ToBoolean(cfg.UsesDome)) DomeControl.OpenDome(HomeAZ);
                            LogEventHandler("Unparking telescope");
                            if (ss_hwp.TelescopeStartUp()) LogEventHandler("Mount unparked");

                            //Wait for 30 seconds for everything to settle
                            LogEventHandler("Waiting for dome to settle");
                            System.Threading.Thread.Sleep(30000);
                            //Recouple dome
                            if (Convert.ToBoolean(cfg.UsesDome)) DomeControl.IsDomeCoupled = true;
                        }
                        else LogEventHandler("Weather conditions safe");
                    }
                }
                return;
            }

        }

        private void LogEventHandler(string logline)
        {
            ss_log.LogEntry(logline);
            LogBox.AppendText(logline + "\r\n");
            this.Show();
            System.Windows.Forms.Application.DoEvents();
            return;
        }

        #region command buttons and fields

        private void StartScanButton_Click(object sender, EventArgs e)
        {
            if (scanOn) return;
            scanOn = true;
            LogEventHandler("\r\n" + "********** Full Scan Selected **********" + "\r\n");
            StartScan();
            scanOn = false;
            return;
        }

        private void AbortButton_Click(object sender, EventArgs e)
        {
            LogEventHandler("\r\n" + "********** Aborted by User **********" + "\r\n");
            Close();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            LogEventHandler("\r\n" + "********** Closed by User **********" + "\r\n");
            Properties.Settings.Default.StartLocationXY = this.Location;
            Properties.Settings.Default.Save();
            Close();
        }

        private void AutoStartCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //Set or reset AutoStart configuration to the value in the autostart checkbox
            Configuration cfg = new Configuration();
            if (AutoRunCheckBox.Checked)
            {
                FormAutoRun ss_asf = new FormAutoRun();
                ss_asf.ShowDialog();
                cfg.AutoStart = "True";
                LogEventHandler("\r\n");
                {
                    if (Convert.ToBoolean(cfg.StageSystemOn)) { LogEventHandler("Staging set for " + cfg.StageSystemTime); }
                    if (Convert.ToBoolean(cfg.StartUpOn)) { LogEventHandler("Start up set for " + cfg.StartUpTime); }
                    if (Convert.ToBoolean(cfg.ShutDownOn)) { LogEventHandler("Shut down set for " + cfg.ShutDownTime); }
                }
            }
            else
            { cfg.AutoStart = "False"; }
            return;
        }

        private void OnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            if (OnTopCheckBox.Checked)
            {
                cfg.SurveyFormOnTop = "True";
                this.TopMost = true;
                this.Show();
            }
            else
            {
                cfg.SurveyFormOnTop = "False";
                this.TopMost = false;
                this.Show();
            }
            return;
        }

        private void MinAltitudeSetting_ValueChanged(object sender, EventArgs e)
        {
            //if (gList != null) gList.MinAltitude() = (double)MinAltitudeSetting.Value;
            Configuration cfg = new Configuration();
            cfg.MinAltitude = MinAltitudeSetting.Value.ToString();
            return;
        }

        private void ExposureTimeSetting_ValueChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.Exposure = ExposureTimeSetting.Value.ToString();
            return;
        }

        private void CCDTemperatureSetting_ValueChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.CCDTemp = CCDTemperatureSetting.Value.ToString();
            return;
        }

        private bool IsWeatherSafe()
        {
            //Returns true if no weather alert, false if it is unsafe

            WeatherFileReader wmon = new WeatherFileReader();
            //if no weather file or other problem, return false
            if (wmon == null) return false;
            if (wmon.AlertFlag == WeatherFileReader.WeaAlert.Alert) return false;
            else return true;
        }

        private void WatchWeatherCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            if (WatchWeatherCheckBox.Checked) cfg.WatchWeather = "True";
            else cfg.WatchWeather = "False";
            return;
        }

        private void DomeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            if (DomeCheckBox.Checked) cfg.UsesDome = "True";
            else cfg.UsesDome = "False";
            return;
        }

        private void ReductionListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Index 0 = No Cal
            Configuration cfg = new Configuration();
            cfg.ReductionType = ReductionListBox.SelectedItem.ToString();
            return;
        }

        private void AnalyzeButton_Click(object sender, EventArgs e)
        {
            if (scanOn) return;
            else
            {
                this.TopMost = false;
                Form iaForm = new FormTargetImageAnalysis();
                iaForm.ShowDialog();
            }
            return;
        }

        private void AtFocusBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            switch (AtFocusTypeBox.SelectedIndex)
            {
                case 0:
                    {
                        cfg.AutoFocus = "None";
                        break;
                    };
                case 1:
                    {
                        cfg.AutoFocus = "2";
                        break;
                    };
                case 2:
                    {
                        cfg.AutoFocus = "3";
                        break;
                    };
            }
            return;
        }

        private void FocusPresetBox_CheckedChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.UseFocusPreset = FocusPresetBox.Checked.ToString();
        }

        private void EnableCLSBox_CheckedChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.UseCLS = EnableCLSBox.Checked.ToString();
        }


        private void SetCollectionButton_Click(object sender, EventArgs e)
        {
            //Sets the folder for the Collection
            if (scanOn) return;
            else
            {
                Form cmForm = new FormCollectionManager();
                cmForm.ShowDialog();
                //Upon completion
                //InitializeCollection();
                Configuration cfg = new Configuration();
                CollectionGroupBox.Text = Path.GetFileName(cfg.CollectionFolderPath);
            }
            InitializeCollection();
            return;

        }

        private void AutoExposureCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.AutoADU = AutoExposureCheckBox.Checked.ToString();
        }

        private void MaxADUBox_ValueChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.ADUMax = MaxADUBox.Value.ToString("0");
        }

        private void ImagesPerSampleBox_ValueChanged(object sender, EventArgs e)
        {
            //Save newly entered images per sample value
            Configuration cfg = new Configuration();
            cfg.ImagesPerSample = ImagesPerSampleBox.Value.ToString("0");
        }

        private void RetakeIntervalBox_ValueChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.MinRetakeInterval = RetakeIntervalBox.Value.ToString("0.0");
        }
        private void FocusFilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.FocusFilter = FocusFilterBox.SelectedIndex.ToString("0");
        }

        #endregion

        //private void CollectExtinctionFrames()
        //{
        //    Configuration cfg = new Configuration();
        //    LogEventHandler("Gathering Extinction Frames");
        //    Extinction newExt = new Extinction();
        //    if (!newExt.GetStandardFields())
        //    {
        //        LogEventHandler("Extinction image capture failed -- No Differential field found.");
        //        LogEventHandler("");
        //        return;
        //    }
        //    //Shoot images for extinction targets
        //    //Loop through the list of filters to be imaged
        //    ColorIndexing cL = new ColorIndexing();
        //    List<Filters.ActiveFilter> afList = cL.GetIndexFilters();
        //    //Create targets for zenith and lower
        //    TargetXList.TargetXDescriptor zTargetDef = new TargetXList.TargetXDescriptor
        //                    (newExt.ZenithTarget.TargetName, newExt.ZenithTarget.TargetRA, newExt.ZenithTarget.TargetDec);
        //    zTargetDef.Exposure = 10;  // Ten second initial exposure.  Why not?
        //    zTargetDef.LastImagingDate = DateTime.Now;
        //    TargetXList.TargetXDescriptor lTargetDef = new TargetXList.TargetXDescriptor
        //                        (newExt.LowerTarget.TargetName, newExt.LowerTarget.TargetRA, newExt.LowerTarget.TargetDec);
        //    lTargetDef.Exposure = 10;  // Ten second initial exposure.  Why not?
        //    lTargetDef.LastImagingDate = DateTime.Now;
        //    //Image Zenith
        //    bool autoExposeAndReposition = true;
        //    foreach (Filters.ActiveFilter af in afList)
        //    {
        //        FreshImage fso = new FreshImage(zTargetDef, af.FilterIndex);
        //        fso.LogUpdate += LogEventHandler;
        //        //If this is the Johnson V filter, then AutoExpose to get standard exposure for all extinctions
        //        //if (af.JcAssign == Filters.JohnsonCousinsFilters.V)
        //        //    autoExposeAndReposition = true;
        //        //else autoExposeAndReposition = false;
        //        if (fso.Acquire(ref zTargetDef, autoExposeAndReposition, autoExposeAndReposition, false))
        //        {
        //            LogEventHandler(newExt.ZenithTarget.TargetName + " Zenith image capture complete.");
        //            LogEventHandler(newExt.ZenithTarget.TargetName + ":" + " Banking new image in " + cfg.ImageBankFolder);
        //        }
        //        else
        //        {
        //            LogEventHandler(newExt.ZenithTarget.TargetName + ": " + " Zenith image capture failed -- check log for problem.");
        //            LogEventHandler("");
        //        }
        //        autoExposeAndReposition = false;
        //    }
        //    //Image Lower
        //    autoExposeAndReposition = true;
        //    foreach (Filters.ActiveFilter af in afList)
        //    {
        //        FreshImage fso = new FreshImage(lTargetDef, af.FilterIndex);
        //        fso.LogUpdate += LogEventHandler;
        //        //If this is the Johnson V filter, then AutoExpose to get standard exposure for all extinctions
        //        //if (af.JcAssign == Filters.JohnsonCousinsFilters.V)
        //        //    autoExposeAndReposition = true;
        //        //else autoExposeAndReposition = false;
        //        if (fso.Acquire(ref lTargetDef, autoExposeAndReposition, autoExposeAndReposition, false))
        //        {
        //            LogEventHandler(newExt.LowerTarget.TargetName + " Lower image capture complete.");
        //            LogEventHandler(newExt.LowerTarget.TargetName + ":" + " Banking image in " + cfg.ImageBankFolder);
        //        }
        //        else
        //        {
        //            LogEventHandler(newExt.LowerTarget.TargetName + ": " + " Lower image capture failed -- check log for problem.");
        //            LogEventHandler("");
        //        }
        //        autoExposeAndReposition = false;
        //    }
        //    TargetXList.AddDifferentialToTargetXList(zTargetDef);
        //    TargetXList.AddDifferentialToTargetXList(lTargetDef);
        //    return;
        //}

    }
}

