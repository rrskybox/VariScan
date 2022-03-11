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
using System.Xml.Linq;
using TheSky64Lib;

namespace VariScan
{
    public class TargetXList
    {
        const string TargetListRootX = "VariScan_List";
        const string TargetListRecordX = "VariScan_Target";
        const string DifferentialRecordX = "VariScan_Differential";

        const string NameX = "Name";
        const string RAX = "RA";
        const string DecX = "Dec";
        //const string FilterX = "Filter";
        const string ExposureX = "Exposure";
        const string LastDateX = "LastImagingDate";

        private XElement targetListRecordX;

        public XElement CollectionDefinition { get; set; }

        public TargetXList()
        {
            //Upon instantiation...
            //Open empty working XML file for AGN list
            //Create an XML datastructure for the Observing List and load it with Observing LIst entries
            //Get the path to the AGN List
            //For each entry in AGN List, convert RA/Dec to Altitude and Azimuth at start time
            //Alt/Az acceptable, then add target to XML data list
            //Replace the current working target List file with the new XML data list
            //Close the file

            {
                Configuration cfg = new Configuration();

                //Check for an existing xml file.  If so, then load it.
                //  Otherwise log an error and return nothing.

                if (File.Exists(cfg.TargetListPath))
                {
                    targetListRecordX = XElement.Load(cfg.TargetListPath);
                    NameSortTargetXList();
                }
                else
                {
                    targetListRecordX = null;
                }
            }
        }

        public List<TargetXDescriptor> GetTargetXList()
        {
            List<TargetXDescriptor> varTargets = new List<TargetXDescriptor>();
            IEnumerable<XElement> tListX = targetListRecordX.Elements(TargetListRecordX);
            foreach (XElement agnt in tListX)
            {
                varTargets.Add(new TargetXDescriptor(agnt));
            }
            return varTargets;
        }

        public int TargetXCount
        {
            //Public Property VariableTargetCount
            //
            // Returns the current number of targets remaining in the target list
            //

            get
            {
                Configuration cfg = new Configuration();
                int count = 0;
                try
                {
                    XElement acnX = XElement.Load(cfg.TargetListPath);
                    count = acnX.Elements(TargetListRecordX).Count();
                }
                catch { count = 0; }
                return (count);
            }
        }

        public static void CreateXList(string textFilePath, string xmlFilePath)
        {
            //Generages the XML target file from the filePath .txt file
            //The assumed format for the AGN text file is |<name>|<ra>|<dec>|<magnitude>
            char[] remChar = { 'h', 'm', 's', 'd' };

            string textLine;
            string[] lineElements;
            XElement tgtXFile;
            tgtXFile = new XElement(TargetListRootX);

            TextReader tgtTextFile = File.OpenText(textFilePath);
            textLine = tgtTextFile.ReadLine();  //Header line -- skip

            while (tgtTextFile.Peek() != -1) //skip non entry lines
            {
                //next entry
                textLine = tgtTextFile.ReadLine();
                if (textLine.Contains(","))
                {
                    lineElements = textLine.Split(',');
                    //Calculate RA and Dec -> decimal values
                    string nameString = Utility.ParseNameString(lineElements[0].Replace(":", " "));
                    double raDouble = Utility.ParseRADecString(lineElements[1]);
                    double decDouble = Utility.ParseRADecString(lineElements[2]);
                    //Load up xelement
                    XElement varTarget = new XElement(TargetListRecordX,
                        new XElement(NameX, nameString),
                        new XElement(RAX, raDouble.ToString()),
                        new XElement(DecX, decDouble.ToString()),
                        new XElement(LastDateX, DateTime.MinValue));
                    tgtXFile.Add(varTarget);
                }
            }
            //Save file
            tgtXFile.Save(xmlFilePath);
        }

        public static void AddDifferentialToTargetXList(TargetXDescriptor refX)
        {
            //Adds a Differential type of target into the the target list
            var newRef = new XElement(DifferentialRecordX,
                   new XElement(NameX, refX.Name,
                   new XElement(RAX, refX.RA.ToString()),
                   new XElement(DecX, refX.Dec.ToString()),
                   //new XElement(FilterX, varTgt.Filter.ToString()),
                   new XElement(LastDateX, refX.LastImagingDate.ToString())));
            Configuration cfg = new Configuration();
            //Load xml list and update data on target
            XElement acnL = XElement.Load(cfg.TargetListPath);
            acnL.Add(newRef);
            acnL.Save(cfg.TargetListPath);
            return;
        }


        public void UpdateTargetXDate(TargetXDescriptor varTgt)
        {
            var newTgt = new XElement(TargetListRecordX,
                   new XElement(NameX, varTgt.Name,
                   new XElement(RAX, varTgt.RA.ToString()),
                   new XElement(DecX, varTgt.Dec.ToString()),
                   //new XElement(FilterX, varTgt.Filter.ToString()),
                   new XElement(LastDateX, varTgt.LastImagingDate.ToString())));
            Configuration cfg = new Configuration();
            //Load xml list and update data on target
            XElement acnL = XElement.Load(cfg.TargetListPath);
            acnL.Elements(TargetListRecordX).FirstOrDefault(t => ((string)t.Element(NameX) == varTgt.Name)).SetElementValue(LastDateX, DateTime.Now.ToString());
            //acnT.Element(VariableTargetLastDateX).ReplaceWith(new XElement(VariableTargetLastDateX, DateTime.Now.ToString()));
            acnL.Save(cfg.TargetListPath);
            return;
        }

        public TargetXDescriptor NextClosestTargetX(double az, double alt)
        {
            //Sets the first target for the session at alt/az and gets the closest target
            TargetXDescriptor lastTgt = null;
            Configuration cfg = new Configuration();
            sky6Utils tsxu = new sky6Utils();
            tsxu.ConvertAzAltToRADec(az, alt);
            double ra = (double)tsxu.dOut0;
            double dec = (double)tsxu.dOut1;
            double minAlt = Convert.ToDouble(cfg.MinAltitude);
            double minRetake = Convert.ToDouble(cfg.MinRetakeInterval);

            double minSeparation = double.MaxValue;

            if (targetListRecordX == null)
                return lastTgt;

            foreach (XElement tDesc in targetListRecordX.Elements())
            {
                TargetXDescriptor xTarget = new TargetXDescriptor(tDesc);
                if (IsUp(xTarget.RA, xTarget.Dec, minAlt))
                {
                    double targetSeparation = TargetXSeparation(ra, dec, xTarget.RA, xTarget.Dec);
                    if (targetSeparation < minSeparation)
                    {
                        lastTgt = xTarget;
                        minSeparation = targetSeparation;
                    }
                }
            }
            ////By now, either we have a target name or not.  Make sure the calling function checks for a null name.
            return lastTgt;
        }

        public TargetXDescriptor NextClosestTargetX(TargetXDescriptor lastTgt)
        {
            //  Gets the next target for targeting
            //  Based on finding the closest target to the last target
            //    that is above the minimum altitude

            {
                //Set access to configuration information about minimum altitude
                Configuration cfg = new Configuration();
                double minAlt = Convert.ToDouble(cfg.MinAltitude);
                double minRetake = Convert.ToDouble(cfg.MinRetakeInterval);
                double minSeparation = double.MaxValue;
                bool foundNewTarget = false;

                TargetXDescriptor nextTgt = lastTgt;
                foreach (XElement tDesc in targetListRecordX.Elements())
                {
                    TargetXDescriptor xTarget = new TargetXDescriptor(tDesc);
                    if (((DateTime.Now - xTarget.LastImagingDate) > TimeSpan.FromHours(minRetake)) && (IsUp(xTarget.RA, xTarget.Dec, minAlt)))
                    {
                        foundNewTarget = true;
                        double targetSeparation = TargetXSeparation(lastTgt.RA, lastTgt.Dec, xTarget.RA, xTarget.Dec);
                        if (targetSeparation < minSeparation)
                        {
                            nextTgt = xTarget;
                            minSeparation = targetSeparation;
                        }
                    }
                }
                ////By now, either we have a target name or not.  Make sure the calling function checks for a null name.
                if (foundNewTarget)
                    return nextTgt;
                else return null;
            }
        }

        private double TargetXSeparation(double ra1, double dec1, double ra2, double dec2)
        {
            //Computes the angular distance between two polar coordinates using TSX utility function
            //

            sky6Utils tsx_ut = new sky6Utils();
            tsx_ut.ComputeAngularSeparation(ra1, dec1, ra2, dec2);
            double dist = tsx_ut.dOut0;
            return dist;
        }

        public bool IsUp(double ra, double dec, double minAlt)
        {
            sky6Utils tsxu = new sky6Utils();
            tsxu.ConvertRADecToAzAlt(ra, dec);
            double alt = tsxu.dOut1;
            if (minAlt > alt) return false;
            else return true;
        }

        public void NameSortTargetXList()
        {
            //Sort List based on name
            IEnumerable<XElement> sortedX =
                from el in targetListRecordX.Elements(TargetListRecordX)
                let tName = el.Element(RAX).Value
                orderby tName
                select el;
            targetListRecordX = new XElement(TargetListRecordX);
            //AGNListX.RemoveAll();  
            foreach (var el in sortedX)
                targetListRecordX.Add(el);
            return;
        }

        public class TargetXDescriptor
        {
            private XElement xTarget;

            public TargetXDescriptor(string name, double ra, double dec)
            {
                xTarget = new XElement(TargetListRecordX,
                          new XElement(NameX, name),
                          new XElement(RAX, ra.ToString()),
                          new XElement(DecX, dec.ToString()),
                          //new XElement(FilterX, filter.ToString()),
                          new XElement(LastDateX, DateTime.MinValue));
            }

            public TargetXDescriptor(XElement variTarget)
            {
                xTarget = variTarget;
            }

            public string Name
            {
                get { return xTarget.Element(NameX).Value; }
                set { xTarget.Element(NameX).Value = value.ToString(); }
            }

            public double RA
            {
                get { return Convert.ToDouble(xTarget.Element(RAX).Value); }
                set { xTarget.Element(RAX).Value = value.ToString(); }
            }

            public double Dec
            {
                get { return Convert.ToDouble(xTarget.Element(DecX).Value); }
                set { xTarget.Element(DecX).Value = value.ToString(); }
            }

            public double Exposure
            {
                get { return Convert.ToDouble(xTarget.Element(ExposureX).Value); }
                set
                {
                    if (xTarget.Element(ExposureX) == null)
                        xTarget.Add(new XElement(ExposureX, value.ToString()));
                    else xTarget.Element(ExposureX).Value = value.ToString();
                }
            }

            public DateTime LastImagingDate
            {
                get { return Convert.ToDateTime(xTarget.Element("LastImagingDate").Value); }
                set
                {
                    if (xTarget.Element(LastDateX) == null)
                        xTarget.Add(new XElement(LastDateX, value.ToString()));
                    else xTarget.Element(LastDateX).Value = value.ToString();
                }
            }
        }
    }
}

