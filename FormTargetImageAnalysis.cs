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
        public const double NyquistMinimumSamples = 2;
        public bool isInitializing;
        public bool isAnalyzing;
        private int currentPickIndex = 0;
        public SampleManager SampleSet;

        public const double maxMagVal = 21;
        public const double minMagVal = 4;

        public int currentTargetRegistration;

        public double Xcen;
        public double Ycen;

        public List<int> magAArrPlotIndex;
        public List<int> magGArrPlotIndex;

        //List of light source targets
        public TargetData CurrentTargetData { get; set; }
        public ccdsoftImage TSX_Image { get; set; }
        public FitsFileTSX FITImage { get; set; }
        public ImageLink TSXimglnk { get; set; }
        public double SaturationADU { get; set; }
        public double NonLinearADU { get; set; }
        public double FocalRatio { get; set; }
        public double PixelScale_arcsec { get; set; }
        public bool FitsIsOpen { get; set; }
        public double FWHMSeeing_arcsec { get; set; }
        public double MagnitudeTransform { get; set; }
        public double ColorTransform { get; set; }
        public ColorIndexing TargetColorIndex { get; set; }

        List<StarField.FieldLightSource[]> primaryLightSources = new List<StarField.FieldLightSource[]>();
        List<StarField.FieldLightSource[]> differentialLightSources = new List<StarField.FieldLightSource[]>();
        List<StarField.FieldLightSource> masterRegisteredList = new List<StarField.FieldLightSource>();

        List<double> colorTransformList = new List<double>();
        List<double> magnitudeTransformList = new List<double>();

        List<double> targetStandardizedMag = new List<double>();

        public FormTargetImageAnalysis()
        {
            Configuration cfg = new Configuration();
            isInitializing = true;
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
            CatalogGroupBox.ForeColor = Color.White;
            TransformResultsBox.ForeColor = Color.White;
            ReportGroupBox.ForeColor = Color.White;

            Utility.ButtonGreen(DoneButton);
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
            isInitializing = false;
            isAnalyzing = false;
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
            CollectionSessionDateBox.Value = DateTime.Now.Date;
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
            //Populate variable list box in the form
            List<string> tdirs = VariScanFileManager.GetVaultList();
            TargetPickListBox.Items.Clear();
            foreach (string t in tdirs)
                TargetPickListBox.Items.Add(VariScanFileManager.StripPath(t));
            if (TargetPickListBox.Items.Count > 0) TargetPickListBox.SelectedIndex = 0;
            CurrentTargetData.TargetName = TargetPickListBox.Text;

            SampleManager SampleSet = new SampleManager(TargetPickListBox.Text);
            TargetDateSelectBox.Items.Clear();
            foreach (DateTime dt in SampleSet.GetTargetSessions())
                TargetDateSelectBox.Items.Add(dt.ToShortDateString());
            if (TargetDateSelectBox.Items.Count > 0) TargetDateSelectBox.SelectedIndex = 0;

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
            ColorIndexing ci = new ColorIndexing();
            (double, double) trans = ci.GetAverageTransforms();
            ManualColorTransformValueBox.Value = (decimal)trans.Item1;
            ManualMagTransformValueBox.Value = (decimal)trans.Item2;

            if (UseGaiaBox.Checked)
                CurrentTargetData.CatalogName = "Gaia";
            else
                CurrentTargetData.CatalogName = "APASS";
            return;
        }


        #region analysis
        public void RegisterLightSources()
        {
            LogIt("Registering " + CurrentTargetData.TargetName);
            List<StarField.FieldLightSource[]> unregisteredPLS = new List<StarField.FieldLightSource[]>();
            List<StarField.FieldLightSource[]> unregisteredDLS = new List<StarField.FieldLightSource[]>();
            List<SampleManager.SessionSample> priFiles = new List<SampleManager.SessionSample>();
            List<SampleManager.SessionSample> difFiles = new List<SampleManager.SessionSample>();
            primaryLightSources.Clear();
            differentialLightSources.Clear();
            masterRegisteredList.Clear();

            //Housekeeping
            //Clear any existing SRC files, as they might be locked and TSX will quietly crash
            Configuration cfg = new Configuration();
            TSX_Process.DeleteSRC(cfg.ImageBankFolder);

            //Open a sample manager for the selected target
            SampleManager sMgr = new SampleManager(CurrentTargetData.TargetName);
            //Acquire primary and differential lists of fits files for this target and this session
            //  based on session date box and filter designations from calling parameters
            DateTime targetSession = Convert.ToDateTime(TargetDateSelectBox.SelectedItem.ToString());
            //Image Link each fits file and collect the light source inventory into arrays for each image
            priFiles = sMgr.GetSessionSamples(CurrentTargetData.SessionDate, CurrentTargetData.PrimaryImageFilter);
            difFiles = sMgr.GetSessionSamples(CurrentTargetData.SessionDate, CurrentTargetData.DifferentialImageFilter);
            //Center the skychart on the ra/dec coordinates
            //Set the star chart size to 1.5 times the image width (fits the whole thing on, persumably
            TSX_Resources.CenterStarChart(TSX_Image, CurrentTargetData);
            //check for images for this target, date, filter
            // exit if none
            if (priFiles.Count == 0 || difFiles.Count == 0)
            {
                LogIt("No primary and/or differential images for this date/filter in image bank");
                return;
            }
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
                //Add the lightsource array
                TSX_Process.MinimizeTSX();
                List<StarField.FieldLightSource> unregList = sf.LocateLightSources(sf.AssembleLightSources());
                DisplayCatalogData();
                TSX_Process.NormalizeTSX();
                if (unregList.Count > 0)
                {
                    LogIt("Added FITS file: " + Path.GetFileName(sample.ImagePath) + " for Primary " + ith.ToString());
                    unregisteredPLS.Add(unregList.ToArray());
                    ith++;
                }
                sf.Close();
            }
            //Check for results -- if no light sources then return
            if (unregisteredPLS.Count < 1)
            {
                LogIt("Insufficient primary light sources");
                return;
            }
            //
            //Image link then list light source date for each primary image
            ith = 0; //counter
            foreach (SampleManager.SessionSample sample in difFiles)
            {
                LogIt("Image Linking " + Path.GetFileName(sample.ImagePath));
                StarField sf = new StarField(TSX_Image, sample);
                //Add the lightsource array
                TSX_Process.MinimizeTSX();
                List<StarField.FieldLightSource> unregList = sf.LocateLightSources(sf.AssembleLightSources());
                DisplayCatalogData();
                TSX_Process.NormalizeTSX();
                if (unregList.Count > 0)
                {
                    LogIt("Added FITS file: " + Path.GetFileName(sample.ImagePath) + " for Differential " + ith.ToString());
                    unregisteredDLS.Add(unregList.ToArray());
                    ith++;
                }
                sf.Close();
            }
            //Check for results -- if no light sources then return
            if (unregisteredDLS.Count < 1)
            {
                LogIt("Insufficient primary light sources");
                return;
            }
            //Pick out a master list from the primary lightsources  -- first one for now, Pick just the top 40 stars by instrument magnitude
            StarField.FieldLightSource[] masterLightSources = unregisteredPLS.First().ToArray();
            LogIt("Acquiring catalog data for light sources");

            //Populate the master sample image with APASS, GAIA and GCVS data, as available
            TSX_Process.MinimizeTSX();
            for (int i = 0; i < masterLightSources.Length; i++)
            {
                SourceCountBox.Text = i.ToString() + " / " + masterLightSources.Length.ToString();
                StarField.CatalogData catDat = StarField.GetCatalogData(masterLightSources[i].SourceRA, masterLightSources[i].SourceDec);
                masterLightSources[i].StandardMagnitudes = catDat;
                //Set registration index for each master light source
                masterLightSources[i].RegistrationIndex = i;
            }
            //Get target registration index
            LogIt("Assigning light source to cataloged star");
            currentTargetRegistration = Registration.ClosestFieldLightSource(CurrentTargetData.TargetRA, CurrentTargetData.TargetDec, masterLightSources);

            //To align all the field light source arrays based on registration number
            //Register other primary samples and differential samples against master sample, including the image data selected for the master sample
            //then convert to lists for more easy management
            for (int i = 0; i < unregisteredPLS.Count; i++)
                primaryLightSources.Add(Registration.RegisterLightSources(masterLightSources, unregisteredPLS[i]));
            for (int i = 0; i < unregisteredDLS.Count; i++)
                differentialLightSources.Add(Registration.RegisterLightSources(masterLightSources, unregisteredDLS[i]));
            masterRegisteredList = masterLightSources.ToList<StarField.FieldLightSource>();
            LogIt("Registration Complete");
            //remove target from master list after recording target information
            CurrentTargetData.SourceRA = masterRegisteredList[currentTargetRegistration].SourceRA;
            CurrentTargetData.SourceDec = masterRegisteredList[currentTargetRegistration].SourceDec;
            masterRegisteredList.RemoveAll(x => x.RegistrationIndex == currentTargetRegistration);
            masterRegisteredList.RemoveAll(x => ((StarField.CatalogData)x.StandardMagnitudes).IsGCVSCataloged);
            CurrentTargetData.ApassStarCount = (from a in masterRegisteredList
                                                where a.StandardMagnitudes != null && a.StandardMagnitudes.Value.APASSCatalogName != null
                                                select a).Count();
            CurrentTargetData.GaiaStarCount = (from a in masterRegisteredList
                                               where a.StandardMagnitudes != null && a.StandardMagnitudes.Value.GAIACatalogName != null
                                               select a).Count();
            CurrentTargetData.Catalog = StarField.GetCatalogData(CurrentTargetData.SourceRA, CurrentTargetData.SourceDec);
            (CurrentTargetData.SourceToAPASSCatalogPositionError, CurrentTargetData.SourceToGAIACatalogPositionError) = StarField.CalculateSeparations(CurrentTargetData);
            DisplayCatalogData();
            DisplayLightSourceData();
            Show();
            return;
        }

        private bool HasCat(StarField.CatalogData catDat, string priCol, string difCol)
        {
            ColorIndexing.ColorDataSource priColStd = ColorIndexing.ConvertColorEnum(priCol);
            ColorIndexing.ColorDataSource difColStd = ColorIndexing.ConvertColorEnum(difCol);

            double priCatMag = Transformation.GetCatalogedMagnitude(priColStd, catDat, UseGaiaBox.Checked);
            double difCatMag = Transformation.GetCatalogedMagnitude(difColStd, catDat, UseGaiaBox.Checked);

            if (priCatMag != 0 && difCatMag != 0)
                return true;
            else
                return false;
        }

        public void CalculateTransforms(bool useGaia)
        {
            //Set colors for primary and differential fields
            LogIt("Calculating transforms for " + CurrentTargetData.TargetName);

            ColorIndexing.ColorDataSource targetStandardColor = ColorIndexing.ConvertColorEnum(PrimaryColorBox.Text);
            ColorIndexing.ColorDataSource differentialColor = ColorIndexing.ConvertColorEnum(DifferentialColorBox.Text);
            ColorIndexing.ColorDataSource instrumentColor = ColorIndexing.ColorDataSource.Instrument;

            //targetStandardizedMag.Clear();
            colorTransformList.Clear();
            magnitudeTransformList.Clear();

            //For each primary image, calculate the ColorTransform and MagnitudeTransform
            //  then calculate the standard color based on each comparison star
            //  then average the set of standard colors
            //List for results of target color
            ColorTransformListBox.Items.Clear();
            MagnitudeTransformListBox.Items.Clear();
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
                    foreach (StarField.FieldLightSource compStar in masterRegisteredList)
                    {
                        //Calculate Color Transform for each comparison star
                        //  First calculate B minus V :  we'll do this every time although it is not necessary
                        var magBV = Transformation.FormColorIndex((int)compStar.RegistrationIndex, differentialColor, pLS, targetStandardColor, pLS, useGaia);
                        var magbv = Transformation.FormColorIndex((int)compStar.RegistrationIndex, instrumentColor, dLS, instrumentColor, pLS, useGaia);
                        //if any value is 0 then stack it -- meaning that both light source arrays don't share a registered star
                        double instColorIndex = magbv.differentialMag - magbv.primaryMag;
                        double stdColorIndex = magBV.differentialMag - magBV.primaryMag;
                        bool nonZeroMags = !(magbv.primaryMag == 0 || magbv.differentialMag == 0 || magBV.primaryMag == 0 || magBV.differentialMag == 0);
                        bool nonZeroIndex = !(Math.Abs(instColorIndex) == 0 || Math.Abs(stdColorIndex) == 0);

                        if (nonZeroIndex)
                        { }
                        if (nonZeroMags && nonZeroIndex)
                        {
                            instrumentColorIndexList.Add(instColorIndex);
                            standardColorIndexList.Add(stdColorIndex);
                        }
                    }
                    List<double> filteredInstrumentCI = new List<double>();
                    List<double> filteredStandardCI = new List<double>();
                    if (instrumentColorIndexList.Count <= 2 || standardColorIndexList.Count <= 2)
                    {
                        LogIt("Too few color differentials to create transforms");
                        Show();
                        System.Windows.Forms.Application.DoEvents();
                        return;
                    }
                    (filteredInstrumentCI, filteredStandardCI) = Transformation.TwoPassSlope(instrumentColorIndexList, standardColorIndexList);
                    (double colorIntercept, double colorTransform) = Transformation.ColorTransform(ref filteredInstrumentCI, ref filteredStandardCI);
                    ColorTransformBox.Text = colorTransform.ToString("0.00");
                    ColorTransformListBox.Items.Add("(P" + priIndex.ToString() + " D" + difIndex.ToString() + "): " + colorTransform.ToString("0.00"));
                    LogIt("Color Transform for " + "(P" + priIndex.ToString() + " D" + difIndex.ToString() + "): " + colorTransform.ToString("0.00"));

                    ColorTransformChart.Series[0].Points.Clear();
                    for (int i = 0; i < filteredInstrumentCI.Count(); i++)
                        ColorTransformChart.Series[0].Points.AddXY(instrumentColorIndexList[i], standardColorIndexList[i]);
                    //Trendline
                    ColorTransformChart.Series[1].Points.Clear();
                    double xMaxC = filteredInstrumentCI.Max();
                    double xMinC = filteredInstrumentCI.Min();
                    ColorTransformChart.Series[1].Points.AddXY(xMinC, (xMinC / colorTransform) + colorIntercept);
                    ColorTransformChart.Series[1].Points.AddXY(xMaxC, (xMaxC / colorTransform) + colorIntercept);

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
                foreach (StarField.FieldLightSource compStar in masterRegisteredList)
                {
                    //Create list of color index for B-v and B-V for comparison stars in this primary image
                    var magBV = Transformation.FormColorIndex((int)compStar.RegistrationIndex, differentialColor, pLS, targetStandardColor, pLS, useGaia);
                    var magBv = Transformation.FormColorIndex((int)compStar.RegistrationIndex, differentialColor, pLS, instrumentColor, pLS, useGaia);
                    //if all values are not zero then stack it
                    if (magBV.primaryMag != 0 && magBV.differentialMag != 0 && magBv.primaryMag != 0 && magBv.differentialMag != 0)
                    {
                        instrumentMagnitudeIndexList.Add(magBV.differentialMag - magBv.primaryMag);
                        standardMagnitudeIndexList.Add(magBV.differentialMag - magBV.primaryMag);
                    }
                    compStarCount++;
                    SourceCountBox.Text = compStarCount.ToString() + " / " + masterRegisteredList.Count.ToString();
                    Show();
                    System.Windows.Forms.Application.DoEvents();
                }

                if (instrumentMagnitudeIndexList.Count <= 2 || standardMagnitudeIndexList.Count <= 2)
                {
                    LogIt("Too few magnitude differentials to create transforms");
                    Show();
                    System.Windows.Forms.Application.DoEvents();
                    return;
                }

                List<double> filteredInstrumentMag = new List<double>();
                List<double> filteredStandardMag = new List<double>();
                (filteredInstrumentMag, filteredStandardMag) = Transformation.TwoPassSlope(instrumentMagnitudeIndexList, standardMagnitudeIndexList);
                (double magIntercept, double magnitudeTransform) = Transformation.MagnitudeTransform(filteredInstrumentMag.ToArray(), filteredStandardMag.ToArray());
                MagnitudeTransformListBox.Items.Add("(P " + priIndex.ToString() + "): " + magnitudeTransform.ToString("0.00"));
                LogIt("Magnitude Transform for " + "(P " + priIndex.ToString() + "): " + magnitudeTransform.ToString("0.00"));
                Show();
                System.Windows.Forms.Application.DoEvents();

                MagnitudeTransformChart.Series[0].Points.Clear();
                for (int i = 0; i < filteredInstrumentMag.Count(); i++)
                    MagnitudeTransformChart.Series[0].Points.AddXY(instrumentMagnitudeIndexList[i], standardMagnitudeIndexList[i]);
                //Trendline
                MagnitudeTransformChart.Series[1].Points.Clear();
                double xMaxM = filteredInstrumentMag.Max();
                double xMinM = filteredInstrumentMag.Min();
                MagnitudeTransformChart.Series[1].Points.AddXY(xMinM, (xMinM * magnitudeTransform) + magIntercept);
                MagnitudeTransformChart.Series[1].Points.AddXY(xMaxM, (xMaxM * magnitudeTransform) + magIntercept);
                Show();
                System.Windows.Forms.Application.DoEvents();

                magnitudeTransformList.Add(magnitudeTransform);
                priIndex++;
                CheckTransformPause();
            }
            //Set the magnitude transform axis labels to associated differentials
            MagnitudeTransformChart.ChartAreas[0].AxisX.Title = DifferentialColorBox.Text + "-" + PrimaryFilterBox.Text;
            MagnitudeTransformChart.ChartAreas[0].AxisY.Title = DifferentialColorBox.Text + "-" + PrimaryColorBox.Text;

            if (colorTransformList.Count == 1)
                ColorTransform = colorTransformList[0];
            else if (colorTransformList.Count < 5)
                ColorTransform = MathNet.Numerics.Statistics.ArrayStatistics.Mean(colorTransformList.ToArray());
            else
                ColorTransform = MathNet.Numerics.Statistics.ArrayStatistics.Mean(Transformation.OutlierRemover(colorTransformList).ToArray());
            ColorTransformBox.Text = ColorTransform.ToString("0.00") + " / " + colorTransformList.Count.ToString();
            ColorTransformListBox.SelectedIndex = 0;
            //update the current target data
            CurrentTargetData.ColorTransform = ColorTransform;
            //add the color transform list to the color transform data and update the preset
            ManualColorTransformValueBox.Value = (decimal)TargetColorIndex.AddColorTransform(colorTransformList);

            if (magnitudeTransformList.Count == 1)
                MagnitudeTransform = magnitudeTransformList[0];
            else if (magnitudeTransformList.Count < 5)
                MagnitudeTransform = MathNet.Numerics.Statistics.ArrayStatistics.Mean(magnitudeTransformList.ToArray());
            else
                MagnitudeTransform = MathNet.Numerics.Statistics.ArrayStatistics.Mean(Transformation.OutlierRemover(magnitudeTransformList).ToArray());
            MagnitudeTransformBox.Text = MagnitudeTransform.ToString("0.00") + " / " + magnitudeTransformList.Count.ToString();
            MagnitudeTransformListBox.SelectedIndex = 0;
            //update the current target data
            CurrentTargetData.MagnitudeTransform = MagnitudeTransform;
            //add the color transform list to the color transform data and update the preset
            ManualMagTransformValueBox.Value = (decimal)TargetColorIndex.AddMagnitudeTransform(magnitudeTransformList);

            Show();
            System.Windows.Forms.Application.DoEvents();
        }

        private void CheckTransformPause()
        {
            //Checks to see if transform graphs are to be paused
            //  then does so if checked
            Configuration cfg = new Configuration();
            if (StepTransformsCheckbox.Checked) MessageBox.Show("Transformation Sequence Paused");
            return;
        }

        public bool ConvertToColorStandard(bool useGaia)
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
            masterRegisteredList = Transformation.SortByMagnitude(masterRegisteredList, 2000);
            int crossRegisteredLightSources = 0;
            int standardMagnitudeCalculated = 0;

            //Primary field star images  loop
            foreach (StarField.FieldLightSource[] pLS in primaryLightSources)
            {
                //Differential field star images loop
                int? priTgtIndex = Registration.FindRegisteredLightSource(pLS, currentTargetRegistration);
                if (priTgtIndex == null)
                {
                    LogIt("No target light source found in primary registered image.");
                }
                else
                {
                    double vVar = pLS[(int)priTgtIndex].InstrumentMagnitude;
                    foreach (StarField.FieldLightSource[] dLS in differentialLightSources)
                    {
                        int? diffTgtIndex = Registration.FindRegisteredLightSource(dLS, currentTargetRegistration);
                        if (diffTgtIndex == null)
                        {
                            LogIt("No target light source found in differential registered image.");
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
                            foreach (StarField.FieldLightSource compLS in masterRegisteredList)
                            {
                                int regNumber = (int)compLS.RegistrationIndex;
                                int? diffCompIndex = Registration.FindRegisteredLightSource(dLS, regNumber);
                                int? priCompIndex = Registration.FindRegisteredLightSource(pLS, regNumber);
                                if (diffCompIndex != null && priCompIndex != null && diffTgtIndex != null && priTgtIndex != null)
                                {
                                    crossRegisteredLightSources++;
                                    double bVar = dLS[(int)diffTgtIndex].InstrumentMagnitude;
                                    double bComp = dLS[(int)diffCompIndex].InstrumentMagnitude;
                                    double vComp = pLS[(int)priCompIndex].InstrumentMagnitude;
                                    double VComp = pLS[(int)priCompIndex].ColorStandard(ColorIndexing.ConvertColorEnum(CurrentTargetData.PrimaryStandardColor));
                                    if (VComp != 0)
                                    {
                                        double deltaInstVarColor = bVar - vVar;
                                        double deltaInstCompColor = bComp - vComp;
                                        double deltaInstColor = deltaInstVarColor - deltaInstCompColor;
                                        double tcInst = ColorTransform * deltaInstColor;
                                        double deltaInstMag = vVar - vComp;
                                        double varStd = deltaInstMag + MagnitudeTransform * tcInst + VComp;
                                        targetStandardizedMag.Add(varStd);
                                        standardMagnitudeCalculated++;
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
            int scottsRuleBuckets = (int)Math.Ceiling((maxVal - minVal) / hWidth);
            // Algorithm that picks the number of buckets based on Rick's Rule, i.e. 100 buckets
            int ricksRuleBuckets = 100;
            // Pick a bucket Rule
            int bucketRule = ricksRuleBuckets;
            MathNet.Numerics.Statistics.Histogram histBuckets = new MathNet.Numerics.Statistics.Histogram(targetStandardizedMag, scottsRuleBuckets);
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
            CurrentTargetData.StandardColorMagnitude = fieldmean;
            CurrentTargetData.StandardMagnitudeError = fieldstddev;
            TargetModeBox.Text = fieldmean.ToString("0.000");
            TargetStdDevBox.Text = fieldstddev.ToString("0.000");
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
            double fWHMAvg_pixels = sf.FWHMAvg_Pixels ?? 0.0;
            double ellipticityAvg = sf.EllipticityAvg ?? 0.0;
            double FWHMAvg_arcsec = fWHMAvg_pixels * TSX_Image.ScaleInArcsecondsPerPixel;
            double FWHMAvg_micron = fWHMAvg_pixels * (double)FITImage.PixSize;
            MeanFWHMBox.Text = FWHMAvg_arcsec.ToString("0.00");
            FWHMSeeing_arcsec = FWHMAvg_micron * 206.3 / ((double)FITImage.FocalLength);
            SeeingClassBox.Text = GetSeeingClass(FWHMAvg_arcsec, (double)FITImage.Aperture);
            SeeingMeanEllipticityBox.Text = ellipticityAvg.ToString("0.00");
            //FitsNameBox.Text = FITImage.FitsTarget;
            FitsDateBox.Text = FITImage.FitsUTCDate;
            FitsTimeBox.Text = FITImage.FitsUTCTime;
            double backgroundADU = TSX_Image.Background;
            SourceBackgroundADUBox.Text = backgroundADU.ToString("0");
            FitsExposureBox.Text = ((double)FITImage.Exposure).ToString("0.0");
            FitsFilterBox.Text = FITImage.Filter;

            Show();
            System.Windows.Forms.Application.DoEvents();
            return;
        }

        private void AnalyzeImage(string fitsFilePath)
        {
            //The current image in TSX is activated and FITS information acquired.
            //  The image is { sent through image link to compute WCS information for each star.
            //  The results are sorted by magnitude, averaged, seeing estimated and results displayed.
            //
            Configuration cfg = new Configuration();

            //Housekeeping
            //Clear any existing SRC files, as they might be locked and TSX will quietly crash
            TSX_Process.DeleteSRC(cfg.ImageBankFolder);
            //close the current tsx image if one is open

            //Center the skychart on the ra/dec coordinates
            //Set the star chart size to 1.5 times the image width (fits the whole thing on, persumably
            TSX_Resources.CenterStarChart(TSX_Image, CurrentTargetData);

            StarField sf = new StarField(TSX_Image, fitsFilePath);

            string path = TSX_Image.Path;

            StarField.FieldLightSource[] sfLSArray = (sf.LocateLightSources(sf.AssembleLightSources())).ToArray();
            CurrentTargetData.IsImageLinked = sf.IsImageLinked;
            if (CurrentTargetData.IsImageLinked)
                CurrentTargetData = sf.LoadTargetFromLightSourceInventory(CurrentTargetData);

            CurrentTargetData.Catalog = StarField.GetCatalogData(CurrentTargetData.SourceRA, CurrentTargetData.SourceDec);
            (CurrentTargetData.SourceToAPASSCatalogPositionError, CurrentTargetData.SourceToGAIACatalogPositionError) = StarField.CalculateSeparations(CurrentTargetData);

            //Using FITS file information...
            FITImage = new FitsFileTSX(TSX_Image);
            DisplayFITS(sf);

            //Compute pixel scale = 206.256 * pixel size (in microns) / focal length
            //Set initial values in case the FITS words aren't there
            FITImage.PixSize = FITImage.PixSize ?? 9;
            FITImage.FocalLength = FITImage.FocalLength ?? 2563; //mm
            FITImage.Aperture = FITImage.Aperture ?? 356.0; //mm
            FITImage.Exposure = FITImage.Exposure ?? 0;
            CurrentTargetData.PrimaryImageFilter = FITImage.Filter[0].ToString();
            CurrentTargetData.ImageDate = FITImage.FitsUTCDateTime;
            CurrentTargetData.AirMass = (double)(FITImage.FitsAirMass ?? 0);

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

            double fWHMAvg_pixels = sf.FWHMAvg_Pixels ?? 0.0;
            double ellipticityAvg = sf.EllipticityAvg ?? 0.0;
            double FWHMAvg_arcsec = fWHMAvg_pixels * TSX_Image.ScaleInArcsecondsPerPixel;
            double FWHMAvg_micron = fWHMAvg_pixels * (double)FITImage.PixSize;
            MeanFWHMBox.Text = FWHMAvg_arcsec.ToString("0.00");
            FWHMSeeing_arcsec = FWHMAvg_micron * 206.3 / ((double)FITImage.FocalLength);

            CurrentTargetData.ComputedSeeing = GetSeeingClass(FWHMAvg_arcsec, (double)FITImage.Aperture);
            SeeingClassBox.Text = CurrentTargetData.ComputedSeeing;
            SeeingMeanEllipticityBox.Text = ellipticityAvg.ToString("0.00");

            //Create new target data for this variable target
            if (CurrentTargetData.IsImageLinked)
            {
                CurrentTargetData = sf.LoadTargetFromLightSourceInventory(CurrentTargetData);
                SourceRATextBox.Text = Utility.SexidecimalRADec(CurrentTargetData.SourceRA, true);
                SourceDecTextBox.Text = Utility.SexidecimalRADec(CurrentTargetData.SourceDec, false);
                DisplayLightSourceData();
                DisplayCatalogData();
                Show();
                if (FitsIsOpen)
                {
                    //Done
                    //Display target, date and time for fits file
                    //FitsNameBox.Text = FITImage.FitsTarget;
                    FitsDateBox.Text = FITImage.FitsUTCDate;
                    FitsTimeBox.Text = FITImage.FitsUTCTime;
                    double backgroundADU = TSX_Image.Background;
                    SourceBackgroundADUBox.Text = backgroundADU.ToString("0");
                    FitsExposureBox.Text = ((double)FITImage.Exposure).ToString("0.0");
                    FitsFilterBox.Text = FITImage.Filter;
                }
                ApassStarCountBox.Text = "";
                GaiaStarCountBox.Text = "";

                TSX_Process.MinimizeTSX();
                for (int i = 0; i < sfLSArray.Count(); i++)
                {
                    sfLSArray[i].StandardMagnitudes = StarField.GetCatalogData(sfLSArray[i].SourceRA, sfLSArray[i].SourceDec);
                    SourceCountBox.Text = i.ToString();
                }
                TSX_Process.NormalizeTSX();

                //Light source data:
                SourceCountBox.Text = sfLSArray.Count().ToString();
                ApassStarCountBox.Text = (from a in sfLSArray
                                          where a.StandardMagnitudes != null && a.StandardMagnitudes.Value.APASSCatalogName != null
                                          select a).Count().ToString();
                GaiaStarCountBox.Text = (from a in sfLSArray
                                         where a.StandardMagnitudes != null && a.StandardMagnitudes.Value.GAIACatalogName != null
                                         select a).Count().ToString();
                //Graph the target star
                CurrentTargetData = GraphSource(CurrentTargetData, sf);
            }
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
            if (CurrentTargetData.Catalog.IsAPASSCataloged)
                ApassToSourcePositionErrorBox.Text = (CurrentTargetData.SourceToAPASSCatalogPositionError * 3600).ToString("0.0");
            else
                ApassToSourcePositionErrorBox.Text = "N/A";
            if (CurrentTargetData.Catalog.IsGAIACataloged)
                GaiaToSourcePositionErrorBox.Text = (CurrentTargetData.SourceToGAIACatalogPositionError * 3600).ToString("0.0");
            else
                GaiaToSourcePositionErrorBox.Text = "N/A";
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

        private TargetData GraphSource(TargetData sourceStar, StarField starField)
        {
            //This routine produces a 3D graph of a star//s astrometric information, from the center out to twice the FWHM radius.
            // using the Source Inventory as at the inventory Index

            //Check to see if the star was in the inventory array
            // if not, then just return
            int inventoryIndex = sourceStar.InventoryArrayIndex;
            if (inventoryIndex < 0) return sourceStar;

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
            StarADUChart.Titles[0].Text = sourceStar.Catalog.APASSCatalogName;
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
            return sourceStar;
        }

        private void PlotPhotometryHistory(TargetData tgt)
        {
            //Plots out history of current star photometry
            List<TargetData> tDataList = new List<TargetData>();
            tDataList = Starchive.RetrievePhotometry(tgt);
            this.HistoryChart.Series[0].Points.Clear();
            this.HistoryChart.Series[1].Points.Clear();
            HistoryChart.Series[0].ChartType = (SeriesChartType.FastPoint);
            HistoryChart.Series[0].MarkerStyle = MarkerStyle.None;
            HistoryChart.Series[1].ChartType = (SeriesChartType.ErrorBar);
            HistoryChart.Series[1].MarkerColor = Color.Blue;
            foreach (TargetData tData in tDataList)
            {
                double tgtMag = tData.StandardColorMagnitude;
                if (tData.IsTransformed && tgtMag != 0)
                {
                    double errorBar = tData.StandardMagnitudeError;
                    HistoryChart.Series[0].Points.AddXY(tData.SessionDate, tgtMag);
                    HistoryChart.Series[1].Points.AddXY(tData.SessionDate, tgtMag, tgtMag - errorBar, tgtMag + errorBar);
                }
            }
            return;
        }

        private void TransformTargetImageSet()
        {
            //returns true if transformed, false otherwise
            //Build lightsource database
            bool isTransformed = false;
            if (UseGaiaBox.Checked)
                CurrentTargetData.CatalogName = "Gaia";
            else
                CurrentTargetData.CatalogName = "APASS";
            RegisterLightSources();
            if (primaryLightSources.Count == 0 || differentialLightSources.Count == 0)
            {
                LogIt("Insufficient light sources to process.");
            }
            else
            {
                if (PresetTransformsBox.Checked)
                {
                    ColorTransform = (double)ManualColorTransformValueBox.Value;
                    ColorTransformBox.Text = ColorTransform.ToString("0.00");
                    ColorTransformListBox.Items.Clear();
                    MagnitudeTransform = (double)ManualMagTransformValueBox.Value;
                    MagnitudeTransformBox.Text = MagnitudeTransform.ToString("0.00");
                    MagnitudeTransformListBox.Items.Clear();
                    CurrentTargetData.ColorTransform = ColorTransform;
                    CurrentTargetData.MagnitudeTransform = MagnitudeTransform;
                }
                else
                    CalculateTransforms(UseGaiaBox.Checked);
                isTransformed = ConvertToColorStandard(UseGaiaBox.Checked);
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
                SessionDate = Convert.ToDateTime(TargetDateSelectBox.SelectedItem.ToString()),
                PrimaryImageFilter = PrimaryFilterBox.Text,
                DifferentialImageFilter = DifferentialFilterBox.Text,
                PrimaryStandardColor = PrimaryColorBox.Text,
                DifferentialStandardColor = DifferentialColorBox.Text
            };
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

        #region clickevents

        private void TransformButton_Click(object sender, EventArgs e)
        {
            Utility.ButtonRed(TransformButton);
            isAnalyzing = true;
            if (TargetPickListBox.SelectedItem == null)
            {
                LogIt("Cancelling transformation. No target selected");
                Utility.ButtonGreen(TransformButton);
                isAnalyzing = false;
                return;
            }
            //Verify that filters and colors are set to something
            if (!AreFiltersSet())
            {
                LogIt("Cancelling transformation.  Improper standard colors or filters selected.");
                Utility.ButtonGreen(TransformButton);
                isAnalyzing = false;
                return;
            }
            ResetTargetData(TargetPickListBox.SelectedItem.ToString());
            CurrentTargetData.SessionDate = Convert.ToDateTime(TargetDateSelectBox.Text);
            TargetColorIndex = new ColorIndexing();

            TransformTargetImageSet();
            Utility.ButtonGreen(TransformButton);
            isAnalyzing = false;
            return;
        }

        private void ClearDateButton_Click(object sender, EventArgs e)
        {
            //Remove all entries for the specified session date
            Utility.ButtonRed(ClearDateButton);
            isAnalyzing = true;
            CurrentTargetData.SessionDate = CollectionSessionDateBox.Value;
            if (UseGaiaBox.Checked)
                Starchive.ClearStarchiveSession(CurrentTargetData.SessionDate, "Gaia");
            else
                Starchive.ClearStarchiveSession(CurrentTargetData.SessionDate, "APASS");
            Utility.ButtonGreen(ClearDateButton);
            isAnalyzing = false;
        }

        private void ClearAllSessionsButton_Click(object sender, EventArgs e)
        {
            //Remove all entries for the specified session date
            Utility.ButtonRed(ClearAllSessionsButton);
            isAnalyzing = true;
            Starchive.ClearStarchive();
            Utility.ButtonGreen(ClearAllSessionsButton);
            isAnalyzing = false;
        }

        private void ClearTargetButton_Click(object sender, EventArgs e)
        {
            //Remove all entries for the specified catalog
            Utility.ButtonRed(ClearTargetButton);
            isAnalyzing = true;
            if (UseGaiaBox.Checked)
                Starchive.ClearStarchiveCatalog("Gaia");
            else
                Starchive.ClearStarchiveCatalog("APASS");
            Utility.ButtonGreen(ClearTargetButton);
            isAnalyzing = false;
        }

        private void ClearCatButton_Click(object sender, EventArgs e)
        {
            //Remove all entries for the specified target and catalog
            Utility.ButtonRed(ClearTargetButton);
            isAnalyzing = true;
            string catName;
            if (UseGaiaBox.Checked)
                catName = "Gaia";
            else
                catName = "APASS";
            Starchive.ClearStarchiveTarget(CurrentTargetData.TargetName, catName);
            Utility.ButtonGreen(ClearTargetButton);
            isAnalyzing = false;

        }

        private void ScanImagesButton_Click(object sender, EventArgs e)
        {
            Utility.ButtonRed(ScanImagesButton);
            isAnalyzing = true;
            //Verify that filters and colors are set to something
            if (!AreFiltersSet())
            {
                LogIt("Cancelling scan.  Improper standard colors or filters selected.");
                Utility.ButtonGreen(ScanImagesButton);
                return;
            }
            //Initialize a color index for saving transform data
            TargetColorIndex = new ColorIndexing();
            //for each target in the target list
            List<string> tdirs = VariScanFileManager.GetVaultList();
            foreach (string tpath in tdirs)
            {
                //Find the earliest session date from the file list and 
                string tname = VariScanFileManager.StripPath(tpath);
                //find and set target list item
                ResetTargetData(tname);
                TargetedNameBox.Text = CurrentTargetData.TargetName;
                TargetedRABox.Text = CurrentTargetData.TargetRA.ToString("0.00000");
                TargetedDecBox.Text = CurrentTargetData.TargetDec.ToString("0.00000");
                int tidx = TargetPickListBox.FindString(tname);
                TargetPickListBox.SelectedIndex = tidx;
                foreach (DateTime dt in VariScanFileManager.SessionDates(tname))
                {
                    CurrentTargetData.SessionDate = dt;
                    LogIt("Looking for images in " + tname + " on " + CurrentTargetData.SessionDate.ToShortDateString());
                    LogIt("Looking for prior plots in Starchives");
                    if (!Starchive.HasMatchingPhotometryRecord(CurrentTargetData))
                    {
                        LogIt("No prior plots found");
                        //Build lightsource
                        TransformTargetImageSet();
                    }
                }
            }
            Utility.ButtonGreen(ScanImagesButton);
            isAnalyzing = false;
            return;
        }

        private void OnTopCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //Sets the window to stay on top... or not
            Configuration cfg = new Configuration();
            cfg.AnalysisFormOnTop = OnTopCheckBox.Checked.ToString();
            this.TopMost = OnTopCheckBox.Checked;
        }

        private void TargetPickListBox_Click(object sender, EventArgs e)
        {
            currentPickIndex = TargetPickListBox.SelectedIndex;
            return;
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

        private void TargetPickListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isInitializing || isAnalyzing)
                return;
            //Get list of targets
            ResetTargetData(TargetPickListBox.SelectedItem.ToString());            //Get list of targets

            TargetedNameBox.Text = CurrentTargetData.TargetName;
            TargetedRABox.Text = CurrentTargetData.TargetRA.ToString("0.00000");
            TargetedDecBox.Text = CurrentTargetData.TargetDec.ToString("0.00000");
            FitsNameBox.Items.Clear();
            foreach (string fName in VariScanFileManager.TargetImageList(CurrentTargetData.TargetName))
                FitsNameBox.Items.Add(Path.GetFileNameWithoutExtension(fName));
            FitsNameBox.SelectedIndex = 0;

            TargetDateSelectBox.Items.Clear();
            SampleManager sm = new SampleManager(TargetPickListBox.SelectedItem.ToString());
            foreach (DateTime dt in sm.GetTargetSessions())
                TargetDateSelectBox.Items.Add(dt.ToShortDateString());
            TargetDateSelectBox.SelectedIndex = 0;

            PrimaryFilterBox.Items.Clear();
            DifferentialFilterBox.Items.Clear();
            foreach (string fn in sm.GetTargetSessionFilters(Convert.ToDateTime(TargetDateSelectBox.SelectedItem)))
            {
                PrimaryFilterBox.Items.Add(fn);
                DifferentialFilterBox.Items.Add(fn);
            }
            if (PrimaryFilterBox.Items.Count >= 1)
                PrimaryFilterBox.SelectedIndex = 0;
            else
                PrimaryFilterBox.SelectedIndex = 0;
            if (DifferentialFilterBox.Items.Count >= 2)
                DifferentialFilterBox.SelectedIndex = 1;
            else
                PrimaryFilterBox.SelectedIndex = 0;
            return;
        }

        private void LogIt(string logLine)
        {
            //Copies logLine into LogBox and eventually into a log file
            LogBox.AppendText(logLine + Environment.NewLine);
            Show();
            System.Windows.Forms.Application.DoEvents();
            return;
        }

        private void CurveButton_Click(object sender, EventArgs e)
        {
            Utility.ButtonRed(CurveButton);
            PlotPhotometryHistory(CurrentTargetData);
            Utility.ButtonGreen(CurveButton);
            return;
        }

        private void FitsReadButton_Click(object sender, EventArgs e)
        {
            Utility.ButtonRed(FitsReadButton);
            Configuration cfg = new Configuration();
            string fitsName = FitsNameBox.SelectedItem.ToString();
            string fitsPath = cfg.ImageBankFolder + "\\" + CurrentTargetData.TargetName + "\\" + fitsName + ".fit";
            if (File.Exists(fitsPath))
                AnalyzeImage(fitsPath);
            Utility.ButtonGreen(FitsReadButton);
            return;
        }

        private void CollectionSessionDateBox_ValueChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                //Pick another collection to analyze
                if (!isInitializing)
                {
                    Configuration cfg = new Configuration();
                    cfg.TargetListPath = CollectionManagement.OpenCollection(CollectionSelectionBox.SelectedItem.ToString());

                    isInitializing = true;
                    InitializeCollectionData();
                    isInitializing = false;
                }
                return;
            }
            return;
        }

        private void PrimaryFilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
                CurrentTargetData.PrimaryImageFilter = PrimaryFilterBox.Text;
            return;
        }

        private void DifferentialFilterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
                CurrentTargetData.DifferentialImageFilter = DifferentialFilterBox.Text;
            return;
        }

        private void PrimaryColorBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
                CurrentTargetData.PrimaryStandardColor = PrimaryColorBox.Text;
            return;
        }

        private void DifferentialColorBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
                CurrentTargetData.DifferentialStandardColor = DifferentialColorBox.Text;
            return;
        }

        private void UseGaiaBox_CheckedChanged(object sender, EventArgs e)
        {
            //Sets flag to use Gaia rather than APASS catalog magnitudes
            Configuration cfg = new Configuration();
            if (!isInitializing)
                cfg.UseGaia = UseGaiaBox.Checked.ToString();
            return;
        }

        private void StepTransformsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            //Sets flag to pause execution after updating each transform graph
            Configuration cfg = new Configuration();
            if (!isInitializing)
                cfg.StepTransforms = StepTransformsCheckbox.Checked.ToString();
            return;
        }

        private void CollectionSelectionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Pick another collection to analyze
            if (!isInitializing)
            {
                Configuration cfg = new Configuration();
                cfg.TargetListPath = CollectionManagement.OpenCollection(CollectionSelectionBox.SelectedItem.ToString());
                isInitializing = true;
                InitializeCollectionData();
                isInitializing = false;
            }
            return;
        }

        private void TargetDateSelectBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isInitializing)
            {
                DateTime sessionDate = Convert.ToDateTime(TargetDateSelectBox.SelectedItem.ToString());
                CurrentTargetData.SessionDate = sessionDate;
                SampleManager ss = new SampleManager(CurrentTargetData.TargetName);
                foreach (string fn in ss.GetTargetSessionFilters(sessionDate))
                {
                    PrimaryFilterBox.Items.Add(fn);
                    DifferentialFilterBox.Items.Add(fn);
                }
                if (PrimaryFilterBox.Items.Count >= 1)
                    PrimaryFilterBox.SelectedIndex = 0;
                if (DifferentialFilterBox.Items.Count >= 2)
                    DifferentialFilterBox.SelectedIndex = 1;
            }
            return;

        }

        #endregion

    }
}



