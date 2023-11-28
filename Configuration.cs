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
using System.Xml.Linq;

namespace VariScan
{
    public class Configuration
    {
        public const string TargetListFilename = "VariScanList.xml";
        public const string ColorListFilename = "ColorList.xml";

        //Private data
        const string ScanFolderName = "VariScan";
        const string ScanImageBankFoldername = "Image Bank";
        const string ScanLogFoldername = "Logs";
        const string ScanConfigurationFilename = "Configuration.xml";
        const string ScanCollectionFolderName = "Collection";
        const string StarchiveFileName = "Starchive.xml";

        const string ScanConfigurationRootX = "VariScan";
        const string StarchiveFilePathX = "StarchiveFilePath";
        const string ScanCollectionFolderPathX = "CollectionFolderPath";

        const string ScanFolderPathX = "VariScanFoldername";
        const string ScanTargetListPathX = "TargetListPath";
        const string ScanColorListPathX = "ColorListPath";
        const string ScanImageBankFoldernameX = "ImageBankFoldername";
        const string ScanLogFoldernameX = "LogFoldername";
        const string ScanExposureX = "Exposure";
        const string ScanMinimumAltitudeX = "MinimumAltitude";
        const string ScanMinimumRetakeIntervalX = "MinimumRetakeInterval";
        const string ScanImagesPerSampleX = "ImagesPerSample";

        const string ScanCCDTempX = "CCDTemp";
        const string ScanUseFocusPresetX = "UseFocusPreset";
        const string ScanAutoFocusX = "AutoFocus";
        const string ScanFocusFilterX = "FocusFilter";
        const string ScanAutoStartX = "AutoStart";
        const string ScanAutoExtinctionX = "AutoExtinction";
        const string ScanStageSystemOnX = "StageSystemOn";
        const string ScanStartUpOnX = "StartUpOn";
        const string ScanShutDownOnX = "ShutDownOn";
        const string ScanStageSystemTimeX = "StageSystemTime";
        const string ScanStartUpTimeX = "StartUpTime";
        const string ScanShutDownTimeX = "ShutDownTime";
        const string ScanStageSystemPathX = "StageSystemPath";
        const string ScanStartUpPathX = "StartUpPath";
        const string ScanShutDownPathX = "ShutDownPath";
        const string ScanWatchWeatherX = "WatchWeather";
        const string ScanUsesDomeX = "UsesDome";
        const string ScanFormOnTopX = "SurveyFormOnTop";
        const string ScanRefreshTargetsX = "RefreshTargets";
        const string ReductionTypeX = "ReductionType";
        const string ScanVarAnalysisFormOnTopX = "AnalysisFormOnTop";
        const string ScanUseGaiaX = "UseGaia";
        const string ScanStepTransformsX = "StepTransforms";

        const string AutoADUX = "AutoADU";
        const string ADUMaxX = "ADUMax";

        const string StepsPerDegreeX = "FocuserStepsPerDegree";
        const string PositionAtZeroX = "FocuserPositionAtZero";

        const string UseCLSX = "UseCLS";
        const string CLSReductionX = "CLSReduction";

        const string ApassColorTransformX = "ApassColorTransform";
        const string GaiaColorTransformX = "GaiaColorTransform";
        const string ApassMagnitudeTransformX = "ApassMagnitudeTransform";
        const string GaiaMagnitudeTransformX = "GaiaMagnitudeTransform";

        const string CurrentSessionSetX = "CurrentSessionSet";
        const string RotateToPA0EastX = "RotateToZeroEast";

        string ssdir;  //VariScan Root directory
        //string cmdir;  //Current Collection directory

        public Configuration()
        {
            //Check to see if VariScan directory exists, if not, create the VariScan folder path for the base folder.
            ssdir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + ScanFolderName;
            if (!Directory.Exists(ssdir + "\\" + ScanFolderName))
            {
                Directory.CreateDirectory(ssdir);
            }

            //The configuration file is set at in the VariScan directory, if not there than create it with defaults.
            if (!File.Exists(ssdir + "\\" + ScanConfigurationFilename))
            {
                //New set up
                XElement cDefaultX = new XElement(ScanConfigurationRootX,
                    new XElement(ScanFolderPathX, ssdir),
                    //new XElement(ScanCollectionFolderPathX, ssdir+"\\"+ScanCollectionFolderName),
                    //new XElement(ScanTargetListPathX, ssdir + "\\" + ScanCollectionFolderName + "\\" + TargetListFilename),
                    //new XElement(ScanColorListPathX, ssdir + "\\" + ScanCollectionFolderName + "\\" + ColorListFilename),
                    //new XElement(ScanImageBankFoldernameX, ssdir + "\\" + ScanCollectionFolderName + "\\" + ScanImageBankFoldername),
                    //new XElement(StarchiveFilePathX, ssdir + "\\" + ScanCollectionFolderName + "\\" + StarchiveFileName),
                    //new XElement(ScanLogFoldernameX, ssdir + "\\" + ScanLogFoldername),
                    new XElement(ScanExposureX, "180"),
                    new XElement(ScanMinimumAltitudeX, "30"),
                    new XElement(ScanMinimumRetakeIntervalX, "12"),
                    new XElement(ScanImagesPerSampleX, "1"),
                    new XElement(ScanCCDTempX, " -30"),
                    new XElement(ScanAutoFocusX, "None"),
                    new XElement(ScanFocusFilterX, "3"),
                    new XElement(ScanAutoStartX, "False"),
                    new XElement(ScanAutoExtinctionX, "False"),
                    new XElement(ScanStageSystemOnX, "False"),
                    new XElement(ScanStartUpOnX, "False"),
                    new XElement(ScanShutDownOnX, "False"),
                    new XElement(ScanStageSystemTimeX, System.DateTime.Now.ToShortTimeString()),
                    new XElement(ScanStartUpTimeX, System.DateTime.Now.ToShortTimeString()),
                    new XElement(ScanShutDownTimeX, System.DateTime.Now.ToShortTimeString()),
                    new XElement(ScanStageSystemPathX, ""),
                    new XElement(ScanStartUpPathX, ""),
                    new XElement(ScanShutDownPathX, ""),
                    new XElement(ScanWatchWeatherX, "False"),
                    new XElement(ScanUsesDomeX, "False"),
                    new XElement(ScanFormOnTopX, "False"),
                    new XElement(ScanRefreshTargetsX, "True"),
                    new XElement(ReductionTypeX, "1"),
                    new XElement(AutoADUX, "False"),
                    new XElement(ADUMaxX, "24000"),
                    new XElement(StepsPerDegreeX, "0"),
                    new XElement(PositionAtZeroX, "0"),
                    new XElement(UseCLSX, "True"),
                    new XElement(CLSReductionX, "None"),
                    new XElement(ScanUseGaiaX, "False"),
                    new XElement(ScanStepTransformsX, "False"),
                    new XElement(ApassColorTransformX, "0.0"),
                    new XElement(GaiaColorTransformX, "0.0"),
                    new XElement(ApassMagnitudeTransformX, "0.0"),
                    new XElement(GaiaMagnitudeTransformX, "0.0"),
                    new XElement(RotateToPA0EastX, "False"),
                    new XElement(CurrentSessionSetX, "0.0"));

                cDefaultX.Save(ssdir + "\\" + ScanConfigurationFilename);
            }
            //string cmdir = ssdir + "\\" + ScanCollectionFolderName;
            ////check on collection folder, if none create it
            //if (!Directory.Exists(GetConfig(ScanCollectionFolderPathX, cmdir)))
            //    Directory.CreateDirectory(cmdir);

            ////check on Log folder, if none, create it
            //if (!Directory.Exists(cmdir + "\\" + ScanLogFoldername))
            //    Directory.CreateDirectory(cmdir + "\\" + ScanLogFoldername);

            ////check on Image Bank folder, if none, create it.
            //if (!Directory.Exists(cmdir + "\\" + ScanImageBankFoldername))
            //    Directory.CreateDirectory(cmdir + "\\" + ScanImageBankFoldername);

            return;
        }

        private string GetConfig(string elementName, string defaultValue)
        {
            string cfgfilename = ssdir + "\\" + ScanConfigurationFilename;
            XElement cfgXf = XElement.Load(cfgfilename);
            if (cfgXf.Element(elementName) == null)
            {
                cfgXf.Add(new XElement(elementName, defaultValue));
                cfgXf.Save(cfgfilename);
                return defaultValue;
            }
            else if (cfgXf.Element(elementName).Value == "")
            {
                cfgXf.Element(elementName).ReplaceWith(new XElement(elementName, defaultValue));
                cfgXf.Save(cfgfilename);
                return defaultValue;
            }
            else return (cfgXf.Element(elementName).Value);
        }

        private void SetConfig(string elementName, string value)
        {
            string cfgfilename = ssdir + "\\" + ScanConfigurationFilename;
            XElement cfgXf = XElement.Load(cfgfilename);
            XElement cfgXel = cfgXf.Element(elementName);
            if (cfgXel == null) cfgXf.Add(new XElement(elementName, value));
            else cfgXel.ReplaceWith(new XElement(elementName, value));
            cfgXf.Save(cfgfilename);
            return;
        }

        public string VariScanFolderPath
        {
            get { return GetConfig(ScanFolderPathX, ssdir); }
            set { SetConfig(ScanFolderPathX, value); }
        }

        public string TargetListPath
        {
            get { return GetConfig(ScanTargetListPathX, ""); }
            set { SetConfig(ScanTargetListPathX, value); }
        }

        public string ColorListPath
        {
            get { return GetConfig(ScanColorListPathX, ""); }
            set { SetConfig(ScanColorListPathX, value); }
        }

        public string CollectionFolderPath
        {
            get { return GetConfig(ScanCollectionFolderPathX, ""); }
            set { SetConfig(ScanCollectionFolderPathX, value); }
        }

        public string ImageBankFolder
        {
            get { return GetConfig(ScanImageBankFoldernameX, ""); }
            set { SetConfig(ScanImageBankFoldernameX, value); }
        }

        public string StarchiveFilePath
        {
            get { return GetConfig(StarchiveFilePathX, ""); }
            set { SetConfig(StarchiveFilePathX, value); }
        }

        public string LogFolder
        {
            get { return GetConfig(ScanLogFoldernameX, ""); }
            set { SetConfig(ScanLogFoldernameX, value); }
        }

        public string Exposure
        {
            get { return GetConfig(ScanExposureX, "1"); }
            set { SetConfig(ScanExposureX, value); }
        }

        public string MinAltitude
        {
            get { return GetConfig(ScanMinimumAltitudeX, "25"); }
            set { SetConfig(ScanMinimumAltitudeX, value); }
        }

        public string MinRetakeInterval
        {
            get { return GetConfig(ScanMinimumRetakeIntervalX, "12"); }
            set { SetConfig(ScanMinimumRetakeIntervalX, value); }
        }

        public string FocusFilter
        {
            get { return GetConfig(ScanFocusFilterX, "3"); }
            set { SetConfig(ScanFocusFilterX, value); }
        }

        public string ImagesPerSample
        {
            get { return GetConfig(ScanImagesPerSampleX, "1"); }
            set { SetConfig(ScanImagesPerSampleX, value); }
        }

        public string CCDTemp
        {
            get { return GetConfig(ScanCCDTempX, "0"); }
            set { SetConfig(ScanCCDTempX, value); }
        }

        public string WatchWeather
        {
            get { return GetConfig(ScanWatchWeatherX, "False"); }
            set { SetConfig(ScanWatchWeatherX, value); }
        }

        public string UsesDome
        {
            get { return GetConfig(ScanUsesDomeX, "False"); }
            set { SetConfig(ScanUsesDomeX, value); }
        }

        public string AutoStart
        {
            get { return GetConfig(ScanAutoStartX, "False"); }
            set { SetConfig(ScanAutoStartX, value); }
        }

        public string AutoFocus
        {
            get { return GetConfig(ScanAutoFocusX, "None"); }
            set { SetConfig(ScanAutoFocusX, value); }
        }

        public string UseFocusPreset
        {
            get { return GetConfig(ScanUseFocusPresetX, "False"); }
            set { SetConfig(ScanUseFocusPresetX, value); }
        }

        public string AutoExtinction
        {
            get { return GetConfig(ScanAutoExtinctionX, "False"); }
            set { SetConfig(ScanAutoExtinctionX, value); }
        }

        public string SurveyFormOnTop
        {
            get { return GetConfig(ScanFormOnTopX, "False"); }
            set { SetConfig(ScanFormOnTopX, value); }
        }

        public string AnalysisFormOnTop
        {
            get { return GetConfig(ScanVarAnalysisFormOnTopX, "False"); }
            set { SetConfig(ScanVarAnalysisFormOnTopX, value); }
        }

        public string ReductionType
        {
            get { return GetConfig(ReductionTypeX, "False"); }
            set { SetConfig(ReductionTypeX, value); }
        }

        public string StageSystemOn
        {
            get { return GetConfig(ScanStageSystemOnX, "False"); }
            set { SetConfig(ScanStageSystemOnX, value); }
        }

        public string StartUpOn
        {
            get { return GetConfig(ScanStartUpOnX, "False"); }
            set { SetConfig(ScanStartUpOnX, value); }
        }

        public string ShutDownOn
        {
            get { return GetConfig(ScanShutDownOnX, "False"); }
            set { SetConfig(ScanShutDownOnX, value); }
        }

        public string StageSystemTime
        {
            get { return GetConfig(ScanStageSystemTimeX, DateTime.Now.ToString()); }
            set { SetConfig(ScanStageSystemTimeX, value); }
        }

        public string StartUpTime
        {
            get { return GetConfig(ScanStartUpTimeX, DateTime.Now.ToString()); }
            set { SetConfig(ScanStartUpTimeX, value); }
        }

        public string ShutDownTime
        {
            get { return GetConfig(ScanShutDownTimeX, DateTime.Now.ToString()); }
            set { SetConfig(ScanShutDownTimeX, value); }
        }

        public string StageSystemPath
        {
            get { return GetConfig(ScanStageSystemPathX, ""); }
            set { SetConfig(ScanStageSystemPathX, value); }
        }

        public string StartUpPath
        {
            get { return GetConfig(ScanStartUpPathX, ""); }
            set { SetConfig(ScanStartUpPathX, value); }
        }

        public string ShutDownPath
        {
            get { return GetConfig(ScanShutDownPathX, ""); }
            set { SetConfig(ScanShutDownPathX, value); }
        }
        public string AutoADU
        {
            get { return GetConfig(AutoADUX, "False"); }
            set { SetConfig(AutoADUX, value); }
        }

        public string ADUMax
        {
            get { return GetConfig(ADUMaxX, "24000"); }
            set { SetConfig(ADUMaxX, value); }
        }

        public string StepsPerDegree
        {
            get { return GetConfig(StepsPerDegreeX, "0.0"); }
            set { SetConfig(StepsPerDegreeX, value); }
        }

        public string PositionAtZero
        {
            get { return GetConfig(PositionAtZeroX, "0.0"); }
            set { SetConfig(PositionAtZeroX, value); }
        }
        public string UseGaia
        {
            get { return GetConfig(ScanUseGaiaX, "False"); }
            set { SetConfig(ScanUseGaiaX, value); }
        }
        public string StepTransforms
        {
            get { return GetConfig(ScanStepTransformsX, "False"); }
            set { SetConfig(ScanStepTransformsX, value); }
        }
        public string RefreshTargets
        {
            get { return GetConfig(ScanRefreshTargetsX, "True"); }
            set { SetConfig(ScanRefreshTargetsX, value); }
        }
        public string UseCLS
        {
            get { return GetConfig(UseCLSX, "True"); }
            set { SetConfig(UseCLSX, value); }
        }
        public string CLSReduction
        {
            get { return GetConfig(CLSReductionX, "None"); }
            set { SetConfig(CLSReductionX, value); }
        }
        public string ApassColorTransform
        {
            get { return GetConfig(ApassColorTransformX, "0,0"); }
            set { SetConfig(ApassColorTransformX, value); }
        }
        public string GaiaColorTransform
        {
            get { return GetConfig(GaiaColorTransformX, "0,0"); }
            set { SetConfig(GaiaColorTransformX, value); }
        }
        public string ApassMagnitudeTransform
        {
            get { return GetConfig(ApassMagnitudeTransformX, "0,0"); }
            set { SetConfig(ApassMagnitudeTransformX, value); }
        }
        public string GaiaMagnitudeTransform
        {
            get { return GetConfig(GaiaMagnitudeTransformX, "0,0"); }
            set { SetConfig(GaiaMagnitudeTransformX, value); }
        }

        public string CurrentSessionSet
        {
            get { return GetConfig(CurrentSessionSetX, "0"); }
            set { SetConfig(CurrentSessionSetX, value); }
        }

        public string IsRotateZeroEast
        {
            get { return GetConfig(RotateToPA0EastX , "False"); }
            set { SetConfig(RotateToPA0EastX, value); }
        }

    }
}
