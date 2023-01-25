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
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using TheSky64Lib;

namespace VariScan
{
    public partial class FormTargetImageAnalysis : Form
    {
        //common constants
        public const double maxMagVal = 21;
        public const double minMagVal = 4;
        public const double MinSeparationArcSec = 5;
        public const double NyquistMinimumSamples = 2;
        //common procedure flags
        public bool IsInitializing { get; set; }
        public bool IsAnalyzing { get; set; }
        public bool FitsIsOpen { get; set; }
        //common TSX server objects
        public ccdsoftImage TSX_Image { get; set; }
        public ImageLink TSXimglnk { get; set; }

        //List of paths to target image bank folders for a processing session
        List<FormSampleCatalog.TargetShoot> SessionList = new List<FormSampleCatalog.TargetShoot>();
        //Descriptors for data on the target currently being processed
        public TargetData CurrentTargetData { get; set; }
        public int? currentTargetRegistration { get; set; }
        //Lists of descriptors for light sources and associated catalog information from images
        StarField.FieldLightSource[] masterLightSources;
        List<StarField.FieldLightSource[]> primaryLightSources = new List<StarField.FieldLightSource[]>();
        List<StarField.FieldLightSource[]> differentialLightSources = new List<StarField.FieldLightSource[]>();

        //Color Indexing and Transformation data
        ColorIndexing TargetColorIndex;
        List<double> targetStandardizedMag = new List<double>();
        List<double> colorTransformList = new List<double>();
        List<double> magnitudeTransformList = new List<double>();

        //FITS file processing fields
        FitsFileTSX FITImage { get; set; }
        //Derived device parameters
        public string ImageFilter { get; set; }
        public double SaturationADU { get; set; }
        public double NonLinearADU { get; set; }
        public double FocalRatio { get; set; }
        public double PixelScale_arcsec { get; set; }
        public double FWHMSeeing_arcsec { get; set; }

        public FormTargetImageAnalysis()
        {
            Configuration cfg = new Configuration();
            IsInitializing = true;
            InitializeComponent();
            //Color the group box titles as Visual Studio doesn't seem to get this done on its own
            SeeingGroupBox.ForeColor = Color.White;
            FitsImageDataBox.ForeColor = Color.White;
            InstrumentationGroupBox.ForeColor = Color.White;
            FitsImageDataBox.ForeColor = Color.White;
            CatalogGroupBox.ForeColor = Color.White;
            TargetedVariableGroupBox.ForeColor = Color.White;
            SessionGroupBox.ForeColor = Color.White;
            SourceGroupBox.ForeColor = Color.White;
            //CatalogGroupBox.ForeColor = Color.White;
            TransformResultsBox.ForeColor = Color.White;
            ReportGroupBox.ForeColor = Color.White;

            Utility.ButtonGreen(DoneButton);
            Utility.ButtonGreen(SelectSessionsButton);

            // Acquire the version information and put it in the form header
            try { this.Text = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(); }
            catch { this.Text = " in Debug"; } //probably in debug, no version info available
            this.Text = "VariScan V" + this.Text;
            //Create a single, reusable object for connecting to multiple TSX images, otherwise garbage collection is a problem
            TSX_Image = new ccdsoftImage();

            InitializeCollectionData();

            //Set window position based on ontopcheckbox
            OnTopCheckBox.Checked = Convert.ToBoolean(cfg.AnalysisFormOnTop);
            this.TopMost = OnTopCheckBox.Checked;
            IsInitializing = false;
            IsAnalyzing = false;
            return;
        }

        public void InitializeCollectionData()
        {
            //Populate collectionselectionbox and choose current collection
            Configuration cfg = new Configuration();
            string basepath = cfg.VariScanFolderPath;
            CollectionSelectionBox.Items.Clear();
            foreach (string fd in Directory.EnumerateDirectories(basepath))
            {
                Path.GetFileName(fd);
                CollectionSelectionBox.Items.Add(Path.GetFileName(fd));
            }
            CollectionSelectionBox.SelectedItem = Path.GetFileName(cfg.CollectionFolderPath);
            CollectionManagement.OpenCollection(Path.GetFileName(cfg.CollectionFolderPath));

            //Populate session controls and default target
            CurrentTargetData = new TargetData();
            //Populate filter and color lists
            ColorIndexing colorIndex = new ColorIndexing();
            string[] colors = ColorIndexing.StandardColorsList();
            PrimaryColorBox.Items.AddRange(colors);
            DifferentialColorBox.Items.AddRange(colors);
            string[] filters = colorIndex.GetSessionFilters().ToArray();
            PrimaryFilterBox.Items.AddRange(filters);
            DifferentialFilterBox.Items.AddRange(filters);

            PrimaryColorBox.SelectedIndex = 1;  //Vj
            DifferentialColorBox.SelectedIndex = 0;  //Bj
            if (PrimaryFilterBox.Items.Count >= 2)
                PrimaryFilterBox.SelectedIndex = 0; //First filter on list
            else
                PrimaryFilterBox.SelectedIndex = 0;
            if (DifferentialFilterBox.Items.Count >= 2)
                DifferentialFilterBox.SelectedIndex = 1; //Second filter on list
            else
                PrimaryFilterBox.SelectedIndex = 0;

            CurrentTargetData.PrimaryStandardColor = PrimaryColorBox.Text;
            CurrentTargetData.DifferentialStandardColor = DifferentialColorBox.Text;
            CurrentTargetData.PrimaryImageFilter = PrimaryFilterBox.Text;
            CurrentTargetData.DifferentialImageFilter = DifferentialFilterBox.Text;

            //Set ManualTransformValues to last set calculated
            //ColorIndexing ci = new ColorIndexing();
            //(double, double) trans = ci.GetAverageTransforms();
            CurrentTargetData.CatalogName = TargetCatalogBox.Text;
            if (TargetCatalogBox.Text == "APASS")
            {
                PresetColorTransformBox.Value = Convert.ToDecimal(cfg.ApassColorTransform);
                PresetMagnitudeTransformBox.Value = Convert.ToDecimal(cfg.ApassMagnitudeTransform);
            }
            else
            {
                PresetColorTransformBox.Value = Convert.ToDecimal(cfg.GaiaColorTransform);
                PresetMagnitudeTransformBox.Value = Convert.ToDecimal(cfg.GaiaMagnitudeTransform);
            }
            //Populate Fits Name list
            FitsNameBox.Items.Clear();
            foreach (string f in VariScanFileManager.GetCollectionFilenameList())
                FitsNameBox.Items.Add(f);
            if (FitsNameBox.Items.Count > 0)
                FitsNameBox.SelectedIndex = 0;
            //Initialize plot target fields
            TargetPlotBox.Items.Clear();
            foreach (string f in VariScanFileManager.GetTargetNameList())
                TargetPlotBox.Items.Add(f);
            if (TargetPlotBox.Items.Count > 0)
                TargetPlotBox.SelectedIndex = 0;

            return;
        }

        #region analysis

        public void RegisterLightSources(FormSampleCatalog.TargetShoot tgtShot)
        {
            LogIt("Registering " + tgtShot.Target + " on " + tgtShot.Date);
            List<StarField.FieldLightSource[]> unregisteredPLS = new List<StarField.FieldLightSource[]>();
            List<StarField.FieldLightSource[]> unregisteredDLS = new List<StarField.FieldLightSource[]>();
            List<SampleManager.SessionSample> priFiles = new List<SampleManager.SessionSample>();
            List<SampleManager.SessionSample> difFiles = new List<SampleManager.SessionSample>();

            primaryLightSources.Clear();
            differentialLightSources.Clear();
            //masterRegisteredList.Clear();

            //Housekeeping
            //Clear any existing SRC files, as they might be locked and TSX will quietly crash
            Configuration cfg = new Configuration();
            TSX_Process.DeleteSRC(cfg.ImageBankFolder);

            //Open a sample manager for the selected target
            SampleManager sMgr = new SampleManager(tgtShot.Target, Convert.ToDateTime(tgtShot.Date));
            //Acquire primary and differential lists of fits files for this target and this session
            //  based on session date box and filter designations from calling parameters
            //Image Link each fits file and collect the light source inventory into arrays for each image
            priFiles = sMgr.SelectSessionSamples(CurrentTargetData.SessionDate, CurrentTargetData.PrimaryImageFilter);
            difFiles = sMgr.SelectSessionSamples(CurrentTargetData.SessionDate, CurrentTargetData.DifferentialImageFilter);
            //check for images for this target, date, filter
            // exit if none
            if (priFiles.Count == 0 || difFiles.Count == 0)
            {
                LogIt("No primary and/or differential images for this date/filter in image bank");
                return;
            }

            #region acquireImageLightSources
            //Image link then list light source date for each primary image
            int ith = 0;  //counter
            foreach (SampleManager.SessionSample sample in priFiles)
            {
                LogIt("Image Linking " + Path.GetFileName(sample.ImagePath));
                //Image link the fits file
                StarField sf = new StarField(TSX_Image, sample);
                //Pick up the last date time of the primary images
                CurrentTargetData.ImageDate = sf.FitsImageDateTime();
                CurrentTargetData.AirMass = sf.AirMass();
                //Center star chart and set fov for click find
                //TSX_Resources.CenterStarChart(TSX_Image, CurrentTargetData.SourceRA, CurrentTargetData.SourceDec);
                //Add the lightsource array
                TSX_Process.MinimizeTSX();
                List<StarField.FieldLightSource> unregList = sf.PositionLightSources(sf.AssembleLightSources());
                DisplayCatalogData();
                TSX_Process.NormalizeTSX();
                if (unregList.Count > 0)
                {
                    LogIt("Added FITS file: " + Path.GetFileName(sample.ImagePath) + " for Primary " + ith.ToString());
                    unregisteredPLS.Add(unregList.ToArray());
                    ith++;
                }
                else
                    LogIt("Primary image failed Image Link");
                CurrentTargetData.ImageWidthInArcSec = TSX_Resources.ImageWidth(TSX_Image);
                sf.Close();
            }
            //Check for results -- if no light sources then return
            if (unregisteredPLS.Count < 1)
            {
                LogIt("Insufficient primary light sources");
                return;
            }
            //
            //Image link then list light source data for each differential image
            ith = 0; //counter
            foreach (SampleManager.SessionSample sample in difFiles)
            {
                LogIt("Image Linking " + Path.GetFileName(sample.ImagePath));
                StarField sf = new StarField(TSX_Image, sample);
                //Add the lightsource array
                //Center star chart and set fov for click find
                //TSX_Resources.CenterStarChart(TSX_Image, CurrentTargetData.SourceRA, CurrentTargetData.SourceDec);
                TSX_Process.MinimizeTSX();
                List<StarField.FieldLightSource> unregList = sf.PositionLightSources(sf.AssembleLightSources());
                DisplayCatalogData();
                TSX_Process.NormalizeTSX();
                if (unregList.Count > 0)
                {
                    LogIt("Added FITS file: " + Path.GetFileName(sample.ImagePath) + " for Differential " + ith.ToString());
                    unregisteredDLS.Add(unregList.ToArray());
                    ith++;
                }
                else
                    LogIt("Differential image failed Image Link");
                sf.Close();
            }
            //Check for results -- if no light sources then return
            if (unregisteredDLS.Count < 1)
            {
                LogIt("Insufficient differential light sources");
                return;
            }
            #endregion

            //Pick out a master list from the primary lightsources  -- first one for now, Pick just the top 40 stars by instrument magnitude
            masterLightSources = unregisteredPLS.First().ToArray();
            LogIt("Acquiring catalog data for light sources");

            //Load up target data
            LogIt("Finding coordinates for " + tgtShot.Target);
            (double tRA, double tDec) = TSX_Resources.FindTarget(tgtShot.Target);
            CurrentTargetData.TargetRA = tRA;
            CurrentTargetData.TargetDec = tDec;
            //Populate the master sample image with APASS, GAIA data, as available
            SourceCountBox.Text = "0/0";
            ApassStarCountBox.Text = "0";
            GaiaStarCountBox.Text = "0";
            //Starchart must be centered and sized for ClickFind to work
            TSX_Resources.CenterStarChart(TSX_Image,
                                            CurrentTargetData.TargetRA,
                                            CurrentTargetData.TargetDec,
                                            CurrentTargetData.ImageWidthInArcSec);

            TSX_Process.MinimizeTSX();
            for (int i = 0; i < masterLightSources.Length; i++)
            {
                StarField.CatalogData catDat = StarField.ClickFindCatalogData(masterLightSources[i].LightSourceRA, masterLightSources[i].LightSourceDec);
                masterLightSources[i].CatalogInfo = catDat;
                //Set registration index for each master light source -- but may not need this
                masterLightSources[i].RegistrationIndex = i;
                //Update count boxes
                SourceCountBox.Text = i.ToString() + " / " + masterLightSources.Length.ToString();
                if (catDat.IsAPASSCataloged)
                    ApassStarCountBox.Text = ((Convert.ToInt32(ApassStarCountBox.Text)) + 1).ToString();
                if (catDat.IsGAIACataloged)
                    GaiaStarCountBox.Text = ((Convert.ToInt32(GaiaStarCountBox.Text)) + 1).ToString();
            }
            TSX_Process.NormalizeTSX();

            //Get target registration index
            //  use Find function rather than believing the AAVSO fields
            //  if a cataloged star is not located close to the location of a light source
            //  then quit
            LogIt("Assigning light source to cataloged star");
            currentTargetRegistration = Registration.ClosestLightSource(masterLightSources, tRA, tDec, MinSeparationArcSec);
            if (currentTargetRegistration == null)
            {
                LogIt("Cannot register light source to cataloged target star");
                return;
            }
            CurrentTargetData.MasterRegistrationIndex = (int)currentTargetRegistration;
            CurrentTargetData.SourceRA = masterLightSources[(int)currentTargetRegistration].LightSourceRA;
            CurrentTargetData.SourceDec = masterLightSources[(int)currentTargetRegistration].LightSourceDec;
            CurrentTargetData.MasterCatalogInfo = StarField.ReadCatalogData(masterLightSources[(int)currentTargetRegistration]);

            //To align all the field light source arrays based on registration number
            //Register other primary samples and differential samples against master sample, including the image data selected for the master sample
            //then convert to lists for more easy management
            for (int i = 0; i < unregisteredPLS.Count; i++)
                primaryLightSources.Add(Registration.RegisterLightSources(masterLightSources, unregisteredPLS[i], MinSeparationArcSec));
            for (int i = 0; i < unregisteredDLS.Count; i++)
                differentialLightSources.Add(Registration.RegisterLightSources(masterLightSources, unregisteredDLS[i], MinSeparationArcSec));
            LogIt("Registration Complete");

            //DO NOT remove target from master list after recording target information
            // masterRegisteredList.RemoveAll(x => x.RegistrationIndex == currentTargetRegistration);
            //masterRegisteredList.RemoveAll(x => ((StarField.CatalogData)x.StandardMagnitudes).IsGCVSCataloged);
            CurrentTargetData.ApassStarCount = (from a in masterLightSources
                                                where a.CatalogInfo != null && a.CatalogInfo.Value.APASSCatalogName != null
                                                select a).Count();
            CurrentTargetData.GaiaStarCount = (from a in masterLightSources
                                               where a.CatalogInfo != null && a.CatalogInfo.Value.GAIACatalogName != null
                                               select a).Count();
            (CurrentTargetData.SourceToAPASSCatalogPositionError, CurrentTargetData.SourceToGAIACatalogPositionError) =
                StarField.CalculateSeparations(CurrentTargetData.MasterCatalogInfo, CurrentTargetData.SourceRA, CurrentTargetData.SourceDec);
            DisplayCatalogData();
            DisplayLightSourceData();
            Show();
            return;
        }

        private bool HasCat(StarField.CatalogData catDat, string priCol, string difCol)
        {
            ColorIndexing.ColorDataSource priColStd = ColorIndexing.ConvertColorEnum(priCol);
            ColorIndexing.ColorDataSource difColStd = ColorIndexing.ConvertColorEnum(difCol);

            double priCatMag = Transformation.GetCatalogedMagnitude(priColStd, catDat, TargetCatalogBox.Text);
            double difCatMag = Transformation.GetCatalogedMagnitude(difColStd, catDat, TargetCatalogBox.Text);

            if (priCatMag != 0 && difCatMag != 0)
                return true;
            else
                return false;
        }

        public void CalculateTransforms(string catalog)
        {
            //Set colors for primary and differential fields
            LogIt("Calculating transforms for " + CurrentTargetData.TargetName);

            ColorIndexing.ColorDataSource targetStandardColor = ColorIndexing.ConvertColorEnum(PrimaryColorBox.Text);
            ColorIndexing.ColorDataSource differentialColor = ColorIndexing.ConvertColorEnum(DifferentialColorBox.Text);
            ColorIndexing.ColorDataSource instrumentColor = ColorIndexing.ColorDataSource.Instrument;

            //targetStandardizedMag.Clear();
            colorTransformList.Clear();
            magnitudeTransformList.Clear();
            ColorTransformListBox.Items.Clear();
            MagnitudeTransformListBox.Items.Clear();

            //For each primary image, calculate the ColorTransform and MagnitudeTransform
            //  then calculate the standard color based on each comparison star
            //  then average the set of standard colors
            //List for results of target color
            int priIndex = 0;
            foreach (StarField.FieldLightSource[] pLS in primaryLightSources)
            {
                //Differential image loop
                int difIndex = 0;
                foreach (StarField.FieldLightSource[] dLS in differentialLightSources)
                {
                    //Color Transformation loop:
                    List<double> standardColorIndexList = new List<double>();
                    List<double> instrumentColorIndexList = new List<double>();
                    foreach (StarField.FieldLightSource compStar in masterLightSources)
                    {
                        if (compStar.CatalogInfo.Value.GAIACatalogPhotoVar != "Variable")
                        {
                            //Calculate Color Transform for each comparison star
                            //  First calculate B minus V :  we'll do this every time although it is not necessary
                            var magBV = Transformation.FormColorIndex((int)compStar.RegistrationIndex, differentialColor, pLS, targetStandardColor, pLS, catalog);
                            var magbv = Transformation.FormColorIndex((int)compStar.RegistrationIndex, instrumentColor, dLS, instrumentColor, pLS, catalog);
                            //if any value is 0 then stack it -- meaning that both light source arrays don't share a registered star
                            double instColorIndex = magbv.differentialMag - magbv.primaryMag;
                            double stdColorIndex = magBV.differentialMag - magBV.primaryMag;
                            bool nonZeroMags = !(magbv.primaryMag == 0 || magbv.differentialMag == 0 || magBV.primaryMag == 0 || magBV.differentialMag == 0);
                            bool nonZeroIndex = !(Math.Abs(instColorIndex) == 0 || Math.Abs(stdColorIndex) == 0);

                            if (nonZeroMags && nonZeroIndex)
                            {
                                instrumentColorIndexList.Add(instColorIndex);
                                standardColorIndexList.Add(stdColorIndex);
                            }
                        }
                    }

                    if (instrumentColorIndexList.Count <= 2 || standardColorIndexList.Count <= 2)
                    {
                        LogIt("Too few color differentials to create transforms");
                        Show();
                        System.Windows.Forms.Application.DoEvents();
                        return;
                    }
                    //Original two pass slope filter 
                    //(filteredInstrumentCI, filteredStandardCI) = Transformation.RoughOutlierFilter(instrumentColorIndexList, standardColorIndexList);
                    //(double colorIntercept, double colorTransform) = Transformation.ColorTransform(ref filteredInstrumentCI, ref filteredStandardCI);
                    (double colorIntercept, double colorTransform) = Transformation.ColorTransform(instrumentColorIndexList, standardColorIndexList);
                    ColorTransformBox.Text = colorTransform.ToString("0.00");
                    ColorTransformListBox.Items.Add(colorTransform.ToString("0.00") + " (P" + priIndex.ToString() + " D" + difIndex.ToString() + ")");
                    LogIt("Color Transform for " + "(P" + priIndex.ToString() + " D" + difIndex.ToString() + "): " + colorTransform.ToString("0.00"));

                    ColorTransformChart.Series[0].Points.Clear();
                    for (int i = 0; i < instrumentColorIndexList.Count(); i++)
                        ColorTransformChart.Series[0].Points.AddXY(instrumentColorIndexList[i], standardColorIndexList[i]);
                    //Trendline
                    ColorTransformChart.Series[1].Points.Clear();
                    double xMaxC = instrumentColorIndexList.Max();
                    double xMinC = instrumentColorIndexList.Min();
                    ColorTransformChart.Series[1].Points.AddXY(xMinC, (xMinC * colorTransform) + colorIntercept);
                    ColorTransformChart.Series[1].Points.AddXY(xMaxC, (xMaxC * colorTransform) + colorIntercept);

                    colorTransformList.Add(colorTransform);
                    difIndex++;
                    CheckTransformPause();
                }
                //Set the color transform axis labels to associated differentials
                ColorTransformChart.ChartAreas[0].AxisX.Title = DifferentialFilterBox.Text + "-" + PrimaryFilterBox.Text;
                ColorTransformChart.ChartAreas[0].AxisY.Title = DifferentialColorBox.Text + "-" + PrimaryColorBox.Text;

                //Magnitude Transformation loop:
                List<double> standardMagnitudeIndexList = new List<double>();
                List<double> instrumentMagnitudeIndexList = new List<double>();
                int compStarCount = 0;
                foreach (StarField.FieldLightSource compStar in masterLightSources)
                {
                    //Create list of color index for B-v and B-V for comparison stars in this primary image
                    var magBV = Transformation.FormColorIndex((int)compStar.RegistrationIndex, differentialColor, pLS, targetStandardColor, pLS, catalog);
                    var magBv = Transformation.FormColorIndex((int)compStar.RegistrationIndex, differentialColor, pLS, instrumentColor, pLS, catalog);
                    //if all values are not zero then stack it
                    if (magBV.primaryMag != 0 && magBV.differentialMag != 0 && magBv.primaryMag != 0 && magBv.differentialMag != 0)
                    {
                        instrumentMagnitudeIndexList.Add(magBV.differentialMag - magBv.primaryMag);
                        standardMagnitudeIndexList.Add(magBV.differentialMag - magBV.primaryMag);
                    }
                    compStarCount++;
                    SourceCountBox.Text = compStarCount.ToString() + " / " + masterLightSources.Length.ToString();
                    Show();
                }

                if (instrumentMagnitudeIndexList.Count <= 2 || standardMagnitudeIndexList.Count <= 2)
                {
                    LogIt("Too few magnitude differentials to create transforms");
                    Show();
                    System.Windows.Forms.Application.DoEvents();
                    return;
                }

                (double magIntercept, double magnitudeTransform) = Transformation.MagnitudeTransform(instrumentMagnitudeIndexList, standardMagnitudeIndexList);
                MagnitudeTransformListBox.Items.Add(magnitudeTransform.ToString("0.00") + " (P " + priIndex.ToString() + ")");
                LogIt("Magnitude Transform for " + "(P " + priIndex.ToString() + "): " + magnitudeTransform.ToString("0.00"));
                Show();
                System.Windows.Forms.Application.DoEvents();

                MagnitudeTransformChart.Series[0].Points.Clear();
                for (int i = 0; i < instrumentMagnitudeIndexList.Count(); i++)
                    MagnitudeTransformChart.Series[0].Points.AddXY(instrumentMagnitudeIndexList[i], standardMagnitudeIndexList[i]);
                //Trendline
                MagnitudeTransformChart.Series[1].Points.Clear();
                double xMaxM = instrumentMagnitudeIndexList.Max();
                double xMinM = instrumentMagnitudeIndexList.Min();
                MagnitudeTransformChart.Series[1].Points.AddXY(xMinM, (xMinM * magnitudeTransform) + magIntercept);
                MagnitudeTransformChart.Series[1].Points.AddXY(xMaxM, (xMaxM * magnitudeTransform) + magIntercept);
                Show();
                System.Windows.Forms.Application.DoEvents();

                magnitudeTransformList.Add(magnitudeTransform);
                priIndex++;
                CheckTransformPause(); //for "stepping" through individual transform graphs
            }

            //Set the magnitude transform axis labels to associated differentials
            MagnitudeTransformChart.ChartAreas[0].AxisX.Title = DifferentialColorBox.Text + "-" + PrimaryFilterBox.Text;
            MagnitudeTransformChart.ChartAreas[0].AxisY.Title = DifferentialColorBox.Text + "-" + PrimaryColorBox.Text;

            //Choose color transform from list
            if (colorTransformList.Count == 1)
                CurrentTargetData.ColorTransform = colorTransformList[0];
            else CurrentTargetData.ColorTransform = MathNet.Numerics.Statistics.ArrayStatistics.Mean(colorTransformList.ToArray());

            ColorTransformBox.Text = CurrentTargetData.ColorTransform.ToString("0.00") + " / " + colorTransformList.Count.ToString();
            ColorTransformListBox.SelectedIndex = 0;

            //add the color transform list to the color transform data and update the preset
            PresetColorTransformBox.Value = (decimal)TargetColorIndex.AddColorTransform(colorTransformList);

            //Choose Magnitude transform from list
            if (magnitudeTransformList.Count == 1)
                CurrentTargetData.MagnitudeTransform = magnitudeTransformList[0];
            else CurrentTargetData.MagnitudeTransform = MathNet.Numerics.Statistics.ArrayStatistics.Mean(magnitudeTransformList.ToArray());

            MagnitudeTransformBox.Text = CurrentTargetData.MagnitudeTransform.ToString("0.00") + " / " + magnitudeTransformList.Count.ToString();
            MagnitudeTransformListBox.SelectedIndex = 0;

            //add the color transform list to the color transform data and update the preset
            PresetMagnitudeTransformBox.Value = (decimal)TargetColorIndex.AddMagnitudeTransform(magnitudeTransformList);

            Show();
            System.Windows.Forms.Application.DoEvents();
        }

        private void CheckTransformPause()
        {
            //Checks to see if transform graphs are to be paused
            //  then does so if checked
            Configuration cfg = new Configuration();
            if (StepTransformsCheckbox.Checked)
            {
                //MessageBox.Show("Transformation Sequence Paused");
                //
                System.Threading.Thread.Sleep(2000);
            }
            return;
        }

        public bool ConvertToColorStandard(string catalog)
        {
            //********************************************************************************
            //
            // Converstion to Standard Color Loop
            //
            //
            //********************************************************************************

            LogIt("Converting to color standard " + CurrentTargetData.TargetName);
            targetStandardizedMag.Clear();

            //Cull Master List to the brightest 2000 field stars (arbitrary just to speed up processing time)
            // masterRegisteredList = Transformation.SortByMagnitude(masterRegisteredList, 2000);
            int crossRegisteredLightSources = 0;
            int standardMagnitudeCalculated = 0;

            int plsIdx = 0;
            int dlsIdx = 0;

            //Primary field star images  loop
            foreach (StarField.FieldLightSource[] pLS in primaryLightSources)
            {
                plsIdx++;
                int? priTgtIndex = Registration.FindRegisteredLightSource(pLS, CurrentTargetData.MasterRegistrationIndex);
                //Differential field star images loop
                if (priTgtIndex == null)
                {
                    LogIt("No target light source found in primary registered image (P" + (plsIdx - 1).ToString() + ").");
                }
                else
                {
                    foreach (StarField.FieldLightSource[] dLS in differentialLightSources)
                    {
                        dlsIdx++;
                        int? diffTgtIndex = Registration.FindRegisteredLightSource(dLS, CurrentTargetData.MasterRegistrationIndex);
                        if (diffTgtIndex == null)
                        {
                            LogIt("No target light source found in differential registered image (D" + (dlsIdx - 1).ToString() + ").");
                        }
                        else
                        {
                            //target star magnitude standardization loop
                            //  Calculate the color transform of this differential image comparison star against this primary image star
                            //  Calculate Δ(b-v) => (b - v)var - (b - v)comp
                            //  Calculate Δ(B-V) = Tbv * Δ(b - v)
                            //  Calculate Δv = vvar – vcomp
                            //  Vvar = Δv + Tv_bv * Δ(B - V) + Vcomp
                            //Compute mean Vvar for all comparison stars
                            foreach (StarField.FieldLightSource compLS in masterLightSources)
                            {
                                if (compLS.CatalogInfo.Value.GAIACatalogPhotoVar != "Variable")
                                {
                                    int? diffCompIndex = Registration.FindRegisteredLightSource(dLS, (int)compLS.RegistrationIndex);
                                    int? priCompIndex = Registration.FindRegisteredLightSource(pLS, (int)compLS.RegistrationIndex);
                                    if (diffCompIndex != null && priCompIndex != null && diffTgtIndex != null && priTgtIndex != null)
                                    {
                                        crossRegisteredLightSources++;
                                        double vTgt = pLS[(int)priTgtIndex].LightSourceInstMag;
                                        double bTgt = dLS[(int)diffTgtIndex].LightSourceInstMag;
                                        double bComp = dLS[(int)diffCompIndex].LightSourceInstMag;
                                        double vComp = pLS[(int)priCompIndex].LightSourceInstMag;
                                        //double VjComp = pLS[(int)priCompIndex].ColorStandard(ColorIndexing.ConvertColorEnum(CurrentTargetData.PrimaryStandardColor));
                                        //double BjComp = pLS[(int)priCompIndex].ColorStandard(ColorIndexing.ConvertColorEnum(CurrentTargetData.DifferentialStandardColor));
                                        double VjComp = compLS.ColorStandard(ColorIndexing.ConvertColorEnum(CurrentTargetData.PrimaryStandardColor));
                                        double BjComp = compLS.ColorStandard(ColorIndexing.ConvertColorEnum(CurrentTargetData.DifferentialStandardColor));
                                        if (VjComp != 0)
                                        {
                                            double deltaInstTgtColor = bTgt - vTgt;
                                            double deltaInstCompColor = bComp - vComp;
                                            double deltaInstColor = deltaInstTgtColor - deltaInstCompColor;
                                            double tcInst = CurrentTargetData.ColorTransform * deltaInstColor;
                                            double deltaInstMag = vTgt - vComp;
                                            double tgtStd = deltaInstMag + CurrentTargetData.MagnitudeTransform * tcInst + VjComp;
                                            targetStandardizedMag.Add(tgtStd);
                                            standardMagnitudeCalculated++;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            LogIt(crossRegisteredLightSources.ToString() + " full registered light sources");
            LogIt(standardMagnitudeCalculated.ToString() + " standard magnitude calculations.");
            if (targetStandardizedMag.Count == 0)
            {
                LogIt("No usable target and/or comparison light sources.");
                return false;
            }
            // Algorithm that picks the number of buckets based on Sturgis Rule
            int sturgisRuleBuckets = (int)Math.Ceiling(Math.Log(targetStandardizedMag.Count, 2));
            //Algorithm that picks the number of buckets based on Scott's rule 3.49*sigma/cube root of n
            (double sMean, double sStdDev) = MathNet.Numerics.Statistics.ArrayStatistics.MeanStandardDeviation(targetStandardizedMag.ToArray());
            double hWidth = 3.49 * sStdDev / (Math.Pow(targetStandardizedMag.Count, 0.3));
            double maxVal = targetStandardizedMag.Max();
            double minVal = targetStandardizedMag.Min();
            int scottsRuleBuckets = 1;
            if (hWidth > 0)
                scottsRuleBuckets = (int)Math.Ceiling((maxVal - minVal) / hWidth);
            // Algorithm that picks the number of buckets based on Rick's Rule, i.e. 100 buckets
            int ricksRuleBuckets = 100;
            // Pick a bucket Rule
            int bucketRule = ricksRuleBuckets;
            MathNet.Numerics.Statistics.Histogram histBuckets = new MathNet.Numerics.Statistics.Histogram(targetStandardizedMag, bucketRule);
            MathNet.Numerics.Statistics.Bucket bigBucket = Utility.FullestBucket(histBuckets);
            //Find the statistical median of the bigBucket
            double median = (bigBucket.UpperBound + bigBucket.LowerBound) / 2;
            //Find the statistical mean of the bigBucket
            //by finding its members then finding the mean
            List<double> bigBucketList = new List<double>();
            foreach (double tsm in targetStandardizedMag)
                if (bigBucket.Contains(tsm) == 0)
                    bigBucketList.Add(tsm);
            //Then, find the mean of the bucket
            (double bbmean, double bbmstddev) = MathNet.Numerics.Statistics.ArrayStatistics.MeanStandardDeviation(bigBucketList.ToArray());

            TransformedTargetChart.Series[0].Points.Clear();
            TransformedTargetChart.ChartAreas[0].AxisX.LabelStyle.Format = "0.00";
            for (int i = 0; i < histBuckets.BucketCount; i++)
            {
                double bucketMean = (histBuckets[i].UpperBound + histBuckets[i].LowerBound) / 2;
                TransformedTargetChart.Series[0].Points.AddXY(bucketMean, histBuckets[i].Count);
            }
            //Mode-based averages -- old algo
            (double fieldmean, double fieldstddev) = MathNet.Numerics.Statistics.ArrayStatistics.MeanStandardDeviation(targetStandardizedMag.ToArray());
            //CurrentTargetData.StandardColorMagnitude = fieldmean;
            //CurrentTargetData.StandardMagnitudeError = fieldstddev;
            CurrentTargetData.StandardColorMagnitude = bbmean;
            CurrentTargetData.StandardMagnitudeError = bbmstddev;
            TargetModeBox.Text = bbmean.ToString("0.000");
            TargetStdDevBox.Text = bbmstddev.ToString("0.000");
            return true;
        }

        private void DisplayFITS(StarField sf)
        {
            //Using FITS file information...
            FITImage = new FitsFileTSX(TSX_Image);

            //Compute pixel scale = 206.256 * pixel size (in microns) / focal length
            //Set initial values in case the FITS words aren't there
            FITImage.PixSize = FITImage.PixSize ?? 9;
            FITImage.FocalLength = FITImage.FocalLength ?? 2563; //mm
            FITImage.Aperture = FITImage.Aperture ?? 356.0; //mm
            FITImage.Exposure = FITImage.Exposure ?? 0;
            //  focal length must be set to correct value in FITS header -- comes from camera set up in TSX
            FocalRatio = (double)FITImage.FocalLength / (double)FITImage.Aperture;
            PixelScale_arcsec = ConvertToArcSec((double)FITImage.PixSize, (double)FITImage.FocalLength);

            //Set the pixel scale for an InsertWCS image linking
            TSX_Image.ScaleInArcsecondsPerPixel = PixelScale_arcsec;

            //set saturation threshold
            SaturationADU = Math.Pow(2, (double)FITImage.PixBits) * 0.95;

            //Fill in Seeing Analysis information in the windows form: 
            //Instrument info
            FocalLengthBox.Text = ((double)FITImage.FocalLength).ToString("0");
            ApertureBox.Text = ((double)FITImage.Aperture).ToString("0");
            FocalRatioBox.Text = FocalRatio.ToString("0.0");
            PixSizeMicronBox.Text = ((double)FITImage.PixSize).ToString("0.0");
            PixSizeArcSecBox.Text = (ConvertToArcSec((double)FITImage.PixSize, (double)FITImage.FocalLength)).ToString("0.00");
            MaxResolutionArcSecBox.Text = ((ConvertToArcSec((double)FITImage.PixSize, (double)FITImage.FocalLength)) * 3.3).ToString("0.00");
            AirMassBox.Text = ((double)(FITImage.FitsAirMass ?? 0)).ToString("0.000");
            MeanFWHMBox.Text = (sf.FWHMAvg_Pixels ?? 0.0 * TSX_Image.ScaleInArcsecondsPerPixel).ToString("0.00");
            FWHMSeeing_arcsec = (sf.FWHMAvg_Pixels ?? 0.0 * (double)FITImage.PixSize) * 206.3 / ((double)FITImage.FocalLength);
            SeeingClassBox.Text = GetSeeingClass(sf.FWHMAvg_Pixels ?? 0.0 * TSX_Image.ScaleInArcsecondsPerPixel, (double)FITImage.Aperture);
            SeeingMeanEllipticityBox.Text = (sf.EllipticityAvg ?? 0.0).ToString("0.00");
            //FitsNameBox.Text = FITImage.FitsTarget;
            FitsDateBox.Text = FITImage.FitsUTCDate;
            FitsTimeBox.Text = FITImage.FitsUTCTime;
            FitsFilterBox.Text = FITImage.Filter;
            SourceBackgroundADUBox.Text = TSX_Image.Background.ToString("0");
            FitsExposureBox.Text = ((double)FITImage.Exposure).ToString("0.0");

            Show();
            System.Windows.Forms.Application.DoEvents();
            return;
        }

        private void AnalyzeFitsImage(string fitsFilePath)
        {
            //Use fits field parameters for target info storage
            //Use local variables for additional target information
            double targetRA;
            double targetDec;

            //The current image in TSX is activated and FITS information acquired.
            //  The image is { sent through image link to compute WCS information for each star.
            //  The results are sorted by magnitude, averaged, seeing estimated and results displayed.
            //
            Configuration cfg = new Configuration();


            //Housekeeping
            //Clear any existing SRC files, as they might be locked and TSX will quietly crash
            TSX_Process.DeleteSRC(cfg.ImageBankFolder);

            //Read the fits file into a starfield image object
            StarField sf = new StarField(TSX_Image, fitsFilePath);
            //Using open FITS file information...
            FITImage = new FitsFileTSX(TSX_Image);
            DisplayFITS(sf);

            //Get the target ra and dec from the fits file and load catalog coordinates
            (targetRA, targetDec) = TSX_Resources.FindTarget(FITImage.FitsTarget);

            //Image Link the fits and create array of light sources, set flag if successful, return if not
            StarField.FieldLightSource[] sfLSArray = (sf.PositionLightSources(sf.AssembleLightSources())).ToArray();
            if (sfLSArray.Length == 0)
            {
                LogIt("Attempted Image Link of FITS image is unsuccessful");
                return;
            }

            //Compute pixel scale = 206.256 * pixel size (in microns) / focal length
            //Set initial values in case the FITS words aren't there
            FITImage.PixSize = FITImage.PixSize ?? 9;
            FITImage.FocalLength = FITImage.FocalLength ?? 2563; //mm
            FITImage.Aperture = FITImage.Aperture ?? 356.0; //mm
            FITImage.Exposure = FITImage.Exposure ?? 0;
            FitsDateBox.Text = FITImage.FitsUTCDate;
            FitsTimeBox.Text = FITImage.FitsUTCTime;

            // ImageFilter = FITImage.Filter[0].ToString();
            //CurrentTargetData.ImageDate = FITImage.FitsUTCDateTime;
            //CurrentTargetData.AirMass = (double)(FITImage.FitsAirMass ?? 0);

            //  focal length must be set to correct value in FITS header -- comes from camera set up in TSX
            FocalRatio = (double)FITImage.FocalLength / (double)FITImage.Aperture;
            PixelScale_arcsec = ConvertToArcSec((double)FITImage.PixSize, (double)FITImage.FocalLength);

            ////Set the pixel scale for an InsertWCS image linking
            //TSX_Image.ScaleInArcsecondsPerPixel = PixelScale_arcsec;

            //set saturation threshold
            SaturationADU = Math.Pow(2, (double)FITImage.PixBits) * 0.95;

            //Fill in Seeing Analysis information in the windows form: 
            //Instrument info
            FocalLengthBox.Text = ((double)FITImage.FocalLength).ToString("0");
            ApertureBox.Text = ((double)FITImage.Aperture).ToString("0");
            FocalRatioBox.Text = FocalRatio.ToString("0.0");
            PixSizeMicronBox.Text = ((double)FITImage.PixSize).ToString("0.0");
            PixSizeArcSecBox.Text = (ConvertToArcSec((double)FITImage.PixSize, (double)FITImage.FocalLength)).ToString("0.00");
            MaxResolutionArcSecBox.Text = ((ConvertToArcSec((double)FITImage.PixSize, (double)FITImage.FocalLength)) * 3.3).ToString("0.00");
            AirMassBox.Text = ((double)(FITImage.FitsAirMass ?? 0)).ToString("0.000");

            double fWHMAvg_pixels = sf.FWHMAvg_Pixels ?? 0.0;
            double ellipticityAvg = sf.EllipticityAvg ?? 0.0;
            double FWHMAvg_arcsec = fWHMAvg_pixels * TSX_Image.ScaleInArcsecondsPerPixel;
            double FWHMAvg_micron = fWHMAvg_pixels * (double)FITImage.PixSize;
            MeanFWHMBox.Text = FWHMAvg_arcsec.ToString("0.00");
            FWHMSeeing_arcsec = FWHMAvg_micron * 206.3 / ((double)FITImage.FocalLength);
            SeeingClassBox.Text = GetSeeingClass(FWHMAvg_arcsec, (double)FITImage.Aperture);
            SeeingMeanEllipticityBox.Text = ellipticityAvg.ToString("0.00");

            //Create new target data for this variable target

            DisplayCatalogData();
            Show();
            if (FitsIsOpen)
            {
                //Done
                //Display target, date and time for fits file
                //FitsNameBox.Text = FITImage.FitsTarget;
                double backgroundADU = TSX_Image.Background;
                SourceBackgroundADUBox.Text = backgroundADU.ToString("0");
                FitsExposureBox.Text = ((double)FITImage.Exposure).ToString("0.0");
                FitsFilterBox.Text = FITImage.Filter;
            }
            //Zero out the count and error boxes
            ApassStarCountBox.Text = "0";
            GaiaStarCountBox.Text = "0";
            ApassToSourcePositionErrorBox.Text = "";
            GaiaToSourcePositionErrorBox.Text = "";

            double imas = TSX_Resources.ImageWidth(TSX_Image);
            TSX_Resources.CenterStarChart(TSX_Image, targetRA, targetDec, imas);

            TSX_Process.MinimizeTSX();
            for (int i = 0; i < sfLSArray.Count(); i++)
            {
                sfLSArray[i].CatalogInfo = StarField.ClickFindCatalogData(sfLSArray[i].LightSourceRA, sfLSArray[i].LightSourceDec);
                SourceCountBox.Text = i.ToString() + "/" + sfLSArray.Count().ToString();
                if (sfLSArray[i].CatalogInfo.Value.APASSCatalogName != null)
                    ApassStarCountBox.Text = (Convert.ToInt32(ApassStarCountBox.Text) + 1).ToString();
                if (sfLSArray[i].CatalogInfo.Value.GAIACatalogName != null)
                    GaiaStarCountBox.Text = (Convert.ToInt32(GaiaStarCountBox.Text) + 1).ToString();
            }
            TSX_Process.NormalizeTSX();

            //Find the closest light source to the target coordinates
            int? currentTargetLSIndex = Registration.ClosestLightSource(sfLSArray, targetRA, targetDec, MinSeparationArcSec);
            //if no target is found then return
            if (currentTargetLSIndex == null)
            {
                LogIt("Could not locate a light source at the target coordinates in fits image");
                return;
            }
            CurrentTargetData.MasterRegistrationIndex = (int)currentTargetLSIndex;
            StarField.FieldLightSource tgtLightSource = sfLSArray[(int)currentTargetLSIndex];

            //Load the CurrentTargetData descriptor from the registered target star light source
            //Find and load the catalog data for the cataloged star closest to the target light source
            (double apassError, double gaiaError) =
                    StarField.CalculateSeparations((StarField.CatalogData)tgtLightSource.CatalogInfo, tgtLightSource.LightSourceRA, tgtLightSource.LightSourceDec);
            ApassToSourcePositionErrorBox.Text = (apassError * 3600).ToString("0.0");
            GaiaToSourcePositionErrorBox.Text = (gaiaError * 3600).ToString("0.0");
            SourceRATextBox.Text = Utility.SexidecimalRADec(tgtLightSource.LightSourceRA, true);
            SourceDecTextBox.Text = Utility.SexidecimalRADec(tgtLightSource.LightSourceDec, false);
            SourceMagBox.Text = tgtLightSource.LightSourceInstMag.ToString("0.00");
            SourcePeakADUBox.Text = tgtLightSource.LightSourceADU.ToString("0.00");
            SourceFWHMBox.Text = tgtLightSource.LightSourceFWHM.ToString("0.00");
            SourceEllipticityBox.Text = tgtLightSource.LightSourceEllipticity.ToString("0.00");

            //Graph the target star
            GraphSource(CurrentTargetData, sf);

            return;
        }

        private void DisplayLightSourceData()
        {
            //Fills out Catalog Data group from Current Target Data
            SourceMagBox.Text = CurrentTargetData.SourceInstrumentMagnitude.ToString("0.00");
            SourceFWHMBox.Text = CurrentTargetData.SourceFWHM.ToString("0.00");
            SourcePeakADUBox.Text = CurrentTargetData.SourceADU.ToString("0.00");
            SourceEllipticityBox.Text = CurrentTargetData.SourceEllipticity.ToString("0.00");
            Show();
            return;
        }

        private void ClearLightSourceData()
        {
            //Clears the catalog data fields so they don't confuse the looker
            SourceMagBox.Text = "";
            SourceFWHMBox.Text = "";
            SourcePeakADUBox.Text = "";
            SourceEllipticityBox.Text = "";
            Show();
            return;
        }

        private void DisplayCatalogData()
        {
            //Fills out Catalog Data group from Current Target Data
            if (CurrentTargetData.MasterCatalogInfo.IsAPASSCataloged)
                ApassToSourcePositionErrorBox.Text = (CurrentTargetData.SourceToAPASSCatalogPositionError * 3600).ToString("0.0");
            else
                ApassToSourcePositionErrorBox.Text = "No Cat";
            if (CurrentTargetData.MasterCatalogInfo.IsGAIACataloged)
                GaiaToSourcePositionErrorBox.Text = (CurrentTargetData.SourceToGAIACatalogPositionError * 3600).ToString("0.0");
            else
                GaiaToSourcePositionErrorBox.Text = "No Cat";
            ApassStarCountBox.Text = CurrentTargetData.ApassStarCount.ToString();
            GaiaStarCountBox.Text = CurrentTargetData.GaiaStarCount.ToString();
            Show();
            return;
        }

        private void ClearCatalogData()
        {
            //Clears the catalog data fields so they don't confuse the looker
            SourceCountBox.Text = "";
            ApassToSourcePositionErrorBox.Text = "";
            GaiaToSourcePositionErrorBox.Text = "";
            ApassStarCountBox.Text = "";
            GaiaStarCountBox.Text = "";
            Show();
            return;

        }

        private void GraphSource(TargetData sourceStar, StarField starField)
        {
            //This routine produces a 3D graph of a star//s astrometric information, from the center out to twice the FWHM radius.
            // using the Source Inventory as at the inventory Index

            //Check to see if the star was in the inventory array
            // if not, then just return
            int inventoryIndex = sourceStar.InventoryArrayIndex;
            if (inventoryIndex < 0)
                return;

            StarField.LightSourceInventory lsi = starField.GetLightSourceInventory(inventoryIndex);
            //Get the star center (x,y) and 2xFWHM (in pixels), as well as the maximum X and Y values
            double XMax = TSX_Image.WidthInPixels - 1;
            double YMax = TSX_Image.HeightInPixels - 1;
            double XCen = (double)lsi.XPosition;
            double YCen = (double)lsi.YPosition;
            int fwhmSpan = (int)((double)lsi.FWHM * 2);
            if (fwhmSpan < 0) fwhmSpan = 0;
            //Calculate the lowest and highest X positions for twice the FWHM
            int Pix0 = (int)(XCen - (fwhmSpan / 2));
            if (Pix0 < 0) Pix0 = 0;
            double PixN = XCen + (fwhmSpan / 2);
            if (PixN > XMax) PixN = XMax;

            Series[] Ypix = new Series[fwhmSpan];
            object[] Yline;

            StarADUChart.Series.Clear();

            SourceFWHMBox.Text = ((double)lsi.FWHM).ToString("0.00");
            SourceEllipticityBox.Text = ((double)lsi.Ellipticity).ToString("0.00");

            sourceStar.SourceEllipticity = (double)lsi.Ellipticity;
            sourceStar.SourceFWHM = (double)lsi.FWHM;

            //Convert X,Y coordinates to RA,Dec coordinates
            double cerr = TSX_Image.XYToRADec((double)lsi.XPosition, (double)lsi.YPosition);

            sourceStar.SourceRA = TSX_Image.XYToRADecResultRA();
            sourceStar.SourceDec = TSX_Image.XYToRADecResultDec();

            //Cataloged star should have already been found during Analysis
            StarADUChart.Titles[0].Text = sourceStar.MasterCatalogInfo.APASSCatalogName;
            //PhotometryCatMagBox.Text = sourceStar.CatMagnitude.ToString("0.00");
            //PhotometrySourceMagBox.Text = ((double)MagArr[inventoryIndex]).ToString("0.00");
            //PhotometryAllCatalogCorrectedMagBox.Text = ((double)MagArr[inventoryIndex] + MeanMagDelta).ToString("0.00");

            //Create the graph series for the 2XFWHM X range around X center, for the 2XFWHM Y range around the Y center
            //  making sure to cut off any excursion either below 0 or about the X or Y ranges
            //
            //So... for a 2XFWHM number of data series,
            int maxStarADU = 0;
            for (int i = 0; i < fwhmSpan; i++)
            {
                //Create a series object for the scanned line and attach it to the graph
                Ypix[i] = new Series(i.ToString());
                StarADUChart.Series.Add(Ypix[i]);
                //Scan in the } data series from the image, if it runs over the range, presumably it will truncate normally
                int lineIndex = (int)(i + YCen - (fwhmSpan / 2));
                if (lineIndex >= 0 && lineIndex < YMax)
                {
                    Yline = TSX_Image.scanLine(lineIndex);
                    //Write the pixel values to the series while checking for saturation (indicate in the label).
                    //  and for the maximum ADU value

                    for (int j = Pix0; j <= PixN; j++)
                    {
                        if (j < Yline.Length)
                        {
                            int adu = (int)Yline[j];
                            if (adu > maxStarADU) maxStarADU = adu;
                            Ypix[i].Points.Add(adu * PixelScale_arcsec);
                        }
                    }
                }

                //Set the chart configuration and display
                Ypix[i].XValueType = ChartValueType.Auto;
                Ypix[i].ChartType = SeriesChartType.SplineArea;
                Ypix[i].MarkerStyle = MarkerStyle.Circle;
            }
            //set the maximum ADU as the value at the center
            if (maxStarADU > SaturationADU)
            {
                MaxADULabel.ForeColor = Color.Black;
                MaxADULabel.BackColor = Color.Pink;
            }
            else
            {
                MaxADULabel.ForeColor = Color.White;
                MaxADULabel.BackColor = Color.Empty;
            }
            SourcePeakADUBox.Text = maxStarADU.ToString("0");
            sourceStar.SourceADU = maxStarADU;
            //All done
            return;
        }

        private void PlotPhotometryHistory(TargetData tgt)
        {
            //Plots out history of current star photometry
            //
            //Graph for both catalogs, using different colors
            //string catName;
            //if (UseGaiaBox.Checked)
            //    catName = "Gaia";
            //else catName = "APASS";


            string priColor = PrimaryColorBox.Text;
            string difColor = DifferentialColorBox.Text;
            string priFilter = PrimaryFilterBox.Text;
            string difFilter = DifferentialFilterBox.Text;

            List<TargetData> tDataList = new List<TargetData>();
            tDataList = Starchive.RetrievePhotometry(tgt);
            //Clear HistoryChart data
            HistoryChart.Series[0].Points.Clear();
            HistoryChart.Series[1].Points.Clear();
            HistoryChart.Series[2].Points.Clear();
            HistoryChart.Series[3].Points.Clear();

            HistoryChart.Series[0].ChartType = (SeriesChartType.FastPoint);
            HistoryChart.Series[0].MarkerStyle = MarkerStyle.None;
            HistoryChart.Series[0].MarkerColor = Color.Blue;
            HistoryChart.Series[1].ChartType = (SeriesChartType.ErrorBar);
            HistoryChart.Series[1].MarkerColor = Color.Blue;
            HistoryChart.Series[2].ChartType = (SeriesChartType.FastPoint);
            HistoryChart.Series[2].MarkerStyle = MarkerStyle.None;
            HistoryChart.Series[2].MarkerColor = Color.Green;
            HistoryChart.Series[3].ChartType = (SeriesChartType.ErrorBar);
            HistoryChart.Series[3].MarkerColor = Color.Green;

            HistoryChart.Legends[0].Title = CurrentTargetData.TargetName;

            foreach (TargetData tData in tDataList)
            {
                double tgtMag = tData.StandardColorMagnitude;
                if (tData.IsTransformed &&
                    tgtMag != 0 &&
                    //tData.CatalogName == catName &&
                    tData.PrimaryImageFilter == priFilter &&
                    tData.DifferentialImageFilter == difFilter &&
                    tData.PrimaryStandardColor == priColor &&
                    tData.DifferentialStandardColor == difColor)
                {
                    double errorBar = tData.StandardMagnitudeError;
                    if (tData.CatalogName == "APASS")
                    {
                        HistoryChart.Series[0].Points.AddXY(tData.SessionDate, tgtMag);
                        HistoryChart.Series[1].Points.AddXY(tData.SessionDate, tgtMag, tgtMag - errorBar, tgtMag + errorBar);
                    }
                    else
                    {
                        HistoryChart.Series[2].Points.AddXY(tData.SessionDate, tgtMag);
                        HistoryChart.Series[3].Points.AddXY(tData.SessionDate, tgtMag, tgtMag - errorBar, tgtMag + errorBar);
                    }

                }
            }
            return;
        }

        private void TransformTargetImageSet()
        {
            //Transform each target/date in SessionList
            foreach (FormSampleCatalog.TargetShoot session in SessionList)
            {
                //returns true if transformed, false otherwise
                //Build lightsource database
                bool isTransformed = false;
                CurrentTargetData.TargetName = session.Target;
                CurrentTargetData.SessionDate = Convert.ToDateTime(session.Date);
                CurrentTargetData.CatalogName = TargetCatalogBox.Text;

                RegisterLightSources(session);
                if (primaryLightSources.Count == 0 || differentialLightSources.Count == 0)
                {
                    LogIt("Insufficient light sources to process.");
                }
                else
                {
                    if (PresetTransformsBox.Checked)
                    {
                        CurrentTargetData.ColorTransform = (double)PresetColorTransformBox.Value;
                        ColorTransformBox.Text = CurrentTargetData.ColorTransform.ToString("0.00");
                        ColorTransformListBox.Items.Clear();
                        CurrentTargetData.MagnitudeTransform = (double)PresetMagnitudeTransformBox.Value;
                        MagnitudeTransformBox.Text = CurrentTargetData.MagnitudeTransform.ToString("0.00");
                        MagnitudeTransformListBox.Items.Clear();
                        CurrentTargetData.ColorTransform = CurrentTargetData.ColorTransform;
                        CurrentTargetData.MagnitudeTransform = CurrentTargetData.MagnitudeTransform;
                    }
                    else
                    {
                        CalculateTransforms(CurrentTargetData.CatalogName);
                    }
                    isTransformed = ConvertToColorStandard(CurrentTargetData.CatalogName);
                }

                if (isTransformed)
                {
                    ResultsTargetBox.Text = CurrentTargetData.TargetName;
                    ResultsDateBox.Text = CurrentTargetData.SessionDate.ToShortDateString();
                    ResultsPrimaryColorBox.Text = CurrentTargetData.PrimaryStandardColor;
                    ResultsPrimaryFilterBox.Text = CurrentTargetData.PrimaryImageFilter;
                    ResultsDifferentialColorBox.Text = CurrentTargetData.DifferentialStandardColor;
                    ResultsDifferentialFilterBox.Text = CurrentTargetData.DifferentialImageFilter;
                }

                CurrentTargetData.IsTransformed = isTransformed;
                Starchive.StorePhotometry(CurrentTargetData);
                PlotPhotometryHistory(CurrentTargetData);
            }
        }

        private string GetSeeingClass(double FWHM, double aperture)
        {
            //Seeing calculator based on aperature and FWHM, using Canadian Weather Service table
            //FWHM is adjusted proportionally to a 12" aperture, gauged against the Canadian seeing table values
            //FWHM is in units of arcsec
            //apertrue is in units of mm
            double apadj = 12 / (aperture / 25.4);
            if (FWHM < (0.4 * apadj)) return ("V (Excellent)");
            else if (FWHM < (1.0 * apadj)) return ("IV (Good)");
            else if (FWHM < (2.5 * apadj)) return ("III (Average)");
            else if (FWHM < (4.0 * apadj)) return ("II (Poor)");
            else return ("I (Bad)");
        }

        private bool AreFiltersSet()
        {
            //Check to see if filter boxes have been properly filled in
            if (PrimaryColorBox.Text == "" || DifferentialColorBox.Text == "" || PrimaryFilterBox.Text == "" || DifferentialFilterBox.Text == "")
                return false;
            else
                return true;
        }

        private void ResetTargetData(string tname)
        {
            //Get list of targets
            TargetXList vList = new TargetXList();
            TargetXList.TargetXDescriptor targetX = vList.GetTargetXList().First(x => x.Name == tname);
            CurrentTargetData = new TargetData()
            {
                TargetName = targetX.Name,
                TargetRA = targetX.RA,
                TargetDec = targetX.Dec,
                IsTransformed = false,
                PrimaryImageFilter = PrimaryFilterBox.Text,
                DifferentialImageFilter = DifferentialFilterBox.Text,
                PrimaryStandardColor = PrimaryColorBox.Text,
                DifferentialStandardColor = DifferentialColorBox.Text
            };
            CurrentTargetData.CatalogName = TargetCatalogBox.Text;
            return;
        }

        #endregion 

        #region Conversions
        private double ConvertToArcSec(double microns, double focalLength)
        {
            return 206.256 * microns / focalLength;

        }

        private double ConvertToMicrons(double arcSec, double focalLength)
        {
            return arcSec * focalLength / 206.256;

        }

        private bool Near(double x, double y, double closeness)
        {
            if (Math.Abs(x - y) < closeness)
                return true;
            else return false;
        }
        #endregion

        #region logging
        private void LogIt(string logLine)
        {
            //Copies logLine into LogBox and eventually into a log file
            LogBox.AppendText(logLine + Environment.NewLine);
            Show();
            return;
        }
        #endregion

        #region clickevents

        private void TransformButton_Click(object sender, EventArgs e)
        {
            Utility.ButtonRed(TransformButton);
            IsAnalyzing = true;
            if (SessionList == null)
            {
                LogIt("Cancelling transformation. No target selected");
                Utility.ButtonGreen(TransformButton);
                IsAnalyzing = false;
                return;
            }
            //Verify that filters and colors are set to something
            if (!AreFiltersSet())
            {
                LogIt("Cancelling transformation.  Improper standard colors or filters selected.");
                Utility.ButtonGreen(TransformButton);
                IsAnalyzing = false;
                return;
            }
            TargetColorIndex = new ColorIndexing();

            TransformTargetImageSet();

            Utility.ButtonGreen(TransformButton);
            IsAnalyzing = false;
            return;
        }

        private void OnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //Sets the window to stay on top... or not
            Configuration cfg = new Configuration();
            cfg.AnalysisFormOnTop = OnTopCheckBox.Checked.ToString();
            this.TopMost = OnTopCheckBox.Checked;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            //Close out the application
            VariScanFileManager.CloseFitsFile(TSX_Image);
            Close();
            return;
        }

        private void AAVSOButton_Click(object sender, EventArgs e)
        {
            Utility.ButtonRed(AAVSOButton);
            //Spoof the observers code for now
            const string ocode = "na";
            DialogResult odr = ReportSaveFileDialog.ShowDialog();
            if (odr == DialogResult.OK)
            {
                Report.CreateAAVSOReport(ReportSaveFileDialog.FileName, ocode);
            }
            Utility.ButtonGreen(AAVSOButton);
        }

        private void SummaryButton_Click(object sender, EventArgs e)
        {
            Utility.ButtonRed(SummaryButton);
            DialogResult odr = ReportSaveFileDialog.ShowDialog();
            if (odr == DialogResult.OK)
            {
                Report.CreateSummaryReport(ReportSaveFileDialog.FileName);
            }
            Utility.ButtonGreen(SummaryButton);
            return;
        }

        private void PlotHistoryButton_Click(object sender, EventArgs e)
        {
            Utility.ButtonRed(PlotHistoryButton);
            if (TargetPlotBox.SelectedItem != null)
            {
                CurrentTargetData.TargetName = TargetPlotBox.SelectedItem.ToString();
                PlotPhotometryHistory(CurrentTargetData);
            }
            Utility.ButtonGreen(PlotHistoryButton);
            return;
        }

        private void FitsReadButton_Click(object sender, EventArgs e)
        {
            Utility.ButtonRed(FitsReadButton);
            Configuration cfg = new Configuration();

            string fitsName = FitsNameBox.SelectedItem.ToString() + ".fit";
            List<string> fp = Directory.GetFiles(cfg.ImageBankFolder, fitsName, SearchOption.AllDirectories).ToList();
            if (fp.Count == 0)
                return;
            AnalyzeFitsImage(fp[0]);
            Utility.ButtonGreen(FitsReadButton);
            return;
        }

        private void CollectionSessionDateBox_ValueChanged(object sender, EventArgs e)
        {
            if (!IsInitializing)
            {
                //Pick another collection to analyze
                if (!IsInitializing)
                {
                    Configuration cfg = new Configuration();
                    cfg.TargetListPath = CollectionManagement.OpenCollection(CollectionSelectionBox.SelectedItem.ToString());

                    IsInitializing = true;
                    InitializeCollectionData();
                    IsInitializing = false;
                }
                return;
            }
            return;
        }

        private void PrimaryFilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsInitializing)
                CurrentTargetData.PrimaryImageFilter = PrimaryFilterBox.Text;
            return;
        }

        private void DifferentialFilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsInitializing)
                CurrentTargetData.DifferentialImageFilter = DifferentialFilterBox.Text;
            return;
        }

        private void PrimaryColorBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsInitializing)
                CurrentTargetData.PrimaryStandardColor = PrimaryColorBox.Text;
            return;
        }

        private void DifferentialColorBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsInitializing)
                CurrentTargetData.DifferentialStandardColor = DifferentialColorBox.Text;
            return;
        }

        private void StepTransformsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            //Sets flag to pause execution after updating each transform graph
            Configuration cfg = new Configuration();
            if (!IsInitializing)
                cfg.StepTransforms = StepTransformsCheckbox.Checked.ToString();
            return;
        }

        private void CollectionSelectionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Pick another collection to analyze
            if (!IsInitializing)
            {
                Configuration cfg = new Configuration();
                cfg.TargetListPath = CollectionManagement.OpenCollection(CollectionSelectionBox.SelectedItem.ToString());
                IsInitializing = true;
                InitializeCollectionData();
                IsInitializing = false;
            }
            return;
        }

        private void SelectSessionsButton_Click(object sender, EventArgs e)
        {
            //Open session grid catalog form
            Utility.ButtonRed(SelectSessionsButton);
            Show();
            System.Windows.Forms.Application.DoEvents();
            using (Form scForm = new FormSampleCatalog())
            {
                scForm.ShowDialog();
                //Upon return, retrieve the session list
                SessionListTextBox.Clear();
                SessionList = FormSampleCatalog.SessionList;
                if (SessionList.Count != 0)
                    foreach (FormSampleCatalog.TargetShoot ts in SessionList)
                    {
                        SessionListTextBox.Text += (ts.Target + " on " + ts.Date + "\r\n");
                    }
                Utility.ButtonGreen(SelectSessionsButton);
                return;
            }
        }

        private void FitsNameBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void TargetPlotBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Unless we are initializing, then a new target has been selected for history plotting.
            //  Do the same thing as the plot history button, if it still exists
            if (!IsInitializing)
            {
                Utility.ButtonRed(PlotHistoryButton);
                if (TargetPlotBox.SelectedItem != null)
                {
                    CurrentTargetData.TargetName = TargetPlotBox.SelectedItem.ToString();
                    PlotPhotometryHistory(CurrentTargetData);
                }
                Utility.ButtonGreen(PlotHistoryButton);
                return;
            }
        }

        private void SetTransformsButton_Click(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            if (!IsInitializing)
                if (TargetCatalogBox.Text == "APASS")
                {
                    cfg.ApassColorTransform = PresetColorTransformBox.Value.ToString();
                    cfg.ApassMagnitudeTransform = PresetMagnitudeTransformBox.Value.ToString();
                }
                else
                {
                    cfg.GaiaColorTransform = PresetColorTransformBox.Value.ToString();
                    cfg.GaiaMagnitudeTransform = PresetMagnitudeTransformBox.Value.ToString();
                }
            return;
        }

        private void TargetCatalogBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //When the catalog type box changes then reload the preset transforms accordingly
            Configuration cfg = new Configuration();
            if (!IsInitializing)
            {
                if (TargetCatalogBox.Text == "APASS")
                {
                    PresetColorTransformBox.Value = Convert.ToDecimal(cfg.ApassColorTransform);
                    PresetMagnitudeTransformBox.Value = Convert.ToDecimal(cfg.ApassMagnitudeTransform);
                }
                else
                {
                    PresetColorTransformBox.Value = Convert.ToDecimal(cfg.GaiaColorTransform);
                    PresetMagnitudeTransformBox.Value = Convert.ToDecimal(cfg.GaiaMagnitudeTransform);
                }
            }
        }

        #endregion


    }
}



