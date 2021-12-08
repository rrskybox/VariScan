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
using TheSky64Lib;

namespace VariScan
{
    public class Extinction
    {

        const string ObservingListPath = "Software Bisque\\TheSky Professional Edition 64\\Database Queries";
        const string StandardFieldQueryFile = "VariScanStandardFields.dbq";

        public double KayPrime { get; set; }
        public TargetData ZenithTarget { get; set; }
        public TargetData LowerTarget { get; set; }

        public Extinction() { }


        public bool GetStandardFields()
        {
            //Query TSX for standard field locations
            //  Find the field target nearest the zenith
            const double TargetLower = 40;
            //Set the starchart FOV to full
            sky6StarChart tsxsc = new sky6StarChart();
            tsxsc.FieldOfView = 180;
            //Load the standard field observation query "VarScanStandardFields" with uses VSOStandardField custom catalog
            string ssdir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + ObservingListPath;
            string stdFieldQueryPath = ssdir + "\\" + StandardFieldQueryFile;
            sky6DataWizard tsxdw = new sky6DataWizard();
            sky6ObjectInformation tsxoi = new sky6ObjectInformation();
            tsxdw.Path = stdFieldQueryPath;
            tsxdw.Open();
            tsxoi = tsxdw.RunQuery;
            //Check for no return -- abort if so
            if (tsxoi.Count == 0)
                return false;
            //Find standard field closest to zenith (highest altitude and store as target
            int zenithIndex = 0;
            double maxAlt = 0;
            int lowerIndex = 0;
            double nearestLower = int.MaxValue;
            double alt = 0;
            for (int i = 0; i < tsxoi.Count; i++)
            {
                tsxoi.Index = i;
                tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_ALT);
                alt = tsxoi.ObjInfoPropOut;
                if (alt > maxAlt)
                {
                    zenithIndex = i;
                    maxAlt = alt;
                }
                if (Math.Abs(TargetLower - alt) < nearestLower)
                {
                    lowerIndex = i;
                    nearestLower = Math.Abs(TargetLower - alt);
                }
            }
            //Load target data for zenith and lower

            ZenithTarget = new TargetData();
            tsxoi.Index = zenithIndex;
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_NAME1);
            ZenithTarget.TargetName = "XRef_" + tsxoi.ObjInfoPropOut;
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_RA_2000);
            ZenithTarget.TargetRA = tsxoi.ObjInfoPropOut;
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_DEC_2000);
            ZenithTarget.TargetDec = tsxoi.ObjInfoPropOut;
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_AIR_MASS);
            ZenithTarget.AirMass = tsxoi.ObjInfoPropOut;
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_ALT);
            double zAlt = tsxoi.ObjInfoPropOut;

            LowerTarget = new TargetData();
            tsxoi.Index = lowerIndex;
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_NAME1);
            LowerTarget.TargetName = "XRef_" + tsxoi.ObjInfoPropOut.ToString();
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_RA_2000);
            LowerTarget.TargetRA = tsxoi.ObjInfoPropOut;
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_DEC_2000);
            LowerTarget.TargetDec = tsxoi.ObjInfoPropOut;
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_AIR_MASS);
            LowerTarget.AirMass = tsxoi.ObjInfoPropOut;
            tsxoi.Property(Sk6ObjectInformationProperty.sk6ObjInfoProp_ALT);
            double lAlt = tsxoi.ObjInfoPropOut;

            return true;
        }


    }
}
