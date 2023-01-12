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

namespace VariScan
{
    public partial class ColorIndexing
    {
        const string ColorConfigurationRootX = "ColorConfiguration";
        const string ColorIndexX = "ColorIndex";
        const string FilterIndexX = "FilterIndex";

        const string ImageFilterX = "ImageFilter";

        const string ColorTransformMeanX = "ColorTransformMean";
        const string MagnitudeTransformMeanX = "MagnitudeTransformMean";

        const string BjX = "Bj";
        const string VjX = "Vj";
        const string RcX = "Rc";
        const string IcX = "Ic";
        const string UcX = "Uc";
        const string GpX = "Gp";
        const string GRpX = "GRp";
        const string GBpX = "GBp";

        const string FilterNameX = "FilterName";

        private XElement colorListRecordX;

        public enum ColorDataSource
        {
            Instrument,
            Bj,
            Vj,
            Rc,
            Ic,
            Uc,
            Gp,
            GRp,
            GBp
        }

        public enum StandardColors
        {
            Uc,
            Bj,
            Vj,
            Rc,
            Ic,
            G,
            GBp,
            GRp
        }

        private List<double> ColorTransformList = new List<double>();
        private List<double> MagnitudeTransformList = new List<double>();

        public ColorIndexing()
        {
            Configuration cfg = new Configuration();

            //Check for an existing xml file.  If so, then load it.
            //  Otherwise create an empty file.

            if (File.Exists(cfg.ColorListPath))
                colorListRecordX = XElement.Load(cfg.ColorListPath);
            else
            {
                XElement cfgXf = new XElement(ColorConfigurationRootX);
                cfgXf.Save(cfg.ColorListPath);
            }
            return;
        }

        public double AddColorTransform(List<double> colorXformList)
        {
            //Adds the list of color transforms to the existing list and saves the mode to the configuration file and returns the mode
            ColorTransformList.AddRange(colorXformList);
            MathNet.Numerics.Statistics.Histogram histBuckets = new MathNet.Numerics.Statistics.Histogram(ColorTransformList, (int)Math.Ceiling((double)ColorTransformList.Count / 4));
            MathNet.Numerics.Statistics.Bucket bigBucket = Utility.FullestBucket(histBuckets);
            List<double> bucketList = ColorTransformList.FindAll(x => x <= bigBucket.UpperBound && x >= bigBucket.LowerBound);
            double bucketAverage = MathNet.Numerics.Statistics.ArrayStatistics.Mean(bucketList.ToArray());
            SetConfig(ColorTransformMeanX, bucketAverage.ToString());
            return bucketAverage;
        }

        public double AddMagnitudeTransform(List<double> magXformList)
        {
            //Adds the list of magnitude transforms to the existing list and saves the mode to the configuration file and returns the mode
            MagnitudeTransformList.AddRange(magXformList);
            MathNet.Numerics.Statistics.Histogram histBuckets = new MathNet.Numerics.Statistics.Histogram(MagnitudeTransformList, (int)Math.Ceiling((double)MagnitudeTransformList.Count / 4));
            MathNet.Numerics.Statistics.Bucket bigBucket = Utility.FullestBucket(histBuckets);
            List<double> bucketList = MagnitudeTransformList.FindAll(x => x <= bigBucket.UpperBound && x >= bigBucket.LowerBound);
            double bucketAverage = MathNet.Numerics.Statistics.ArrayStatistics.Mean(bucketList.ToArray());
            SetConfig(MagnitudeTransformMeanX, bucketAverage.ToString());
            return bucketAverage;
        }

        private void SetConfig(string elementName, string data)
        {
            Configuration cfg = new Configuration();
            XElement cfgXf = XElement.Load(cfg.ColorListPath);
            XElement cfgXel = cfgXf.Element(elementName);
            XElement dataX = new XElement(elementName, data);
            if (cfgXel == null)
                cfgXf.Add(new XElement(dataX));
            else
                cfgXel.ReplaceWith(new XElement(dataX));
            cfgXf.Save(cfg.ColorListPath);
            return;
        }

        private string GetConfig(string elementName)
        {
            Configuration cfg = new Configuration();
            XElement cfgXf = XElement.Load(cfg.ColorListPath);
            XElement cfgXel = cfgXf.Element(elementName);
            if (cfgXel == null)
                return "0";
            else
                return cfgXel.Value.ToString();
        }

        public void SaveActiveFilters(List<Filters.ActiveFilter> filterSet)
        {
            //Rewrite filter list with new filter set
            Configuration cfg = new Configuration();
            XElement cfgXf = new XElement(ColorConfigurationRootX);
            foreach (Filters.ActiveFilter af in filterSet)
            {
                XElement filter = new XElement(ImageFilterX,
                                        new XElement(FilterNameX, af.FilterName),
                                        new XElement(FilterIndexX, af.FilterIndex.ToString()));

                cfgXf.Add(filter);
                cfgXf.Save(cfg.ColorListPath);
            }
            return;
        }

        public (double, double) GetAverageTransforms()
        {
            //Read the transform averages from the colorindex.xml
            double cTrans = Convert.ToDouble(GetConfig(ColorTransformMeanX));
            double mTrans = Convert.ToDouble(GetConfig(MagnitudeTransformMeanX));
            return (cTrans, mTrans);
        }

        public List<Filters.ActiveFilter> GetIndexFilters()
        {
            List<Filters.ActiveFilter> flist = new List<Filters.ActiveFilter>();
            //Read in filter list
            Configuration cfg = new Configuration();
            //Reload color list filters
            colorListRecordX = XElement.Load(cfg.ColorListPath);
            foreach (XElement xp in colorListRecordX.Elements(ImageFilterX))
            {
                Filters.ActiveFilter ap = new Filters.ActiveFilter()
                {
                    FilterName = xp.Element(FilterNameX).Value,
                    FilterIndex = Convert.ToInt32(xp.Element(FilterIndexX).Value)
                };
                flist.Add(ap);
            };
            IEnumerable<Filters.ActiveFilter> iList = flist;
            return iList.Distinct().ToList();
        }

        public List<string> GetSessionFilters()
        {
            Configuration cfg = new Configuration();
            ColorIndexing cL = new ColorIndexing();
            List<Filters.ActiveFilter> afList = cL.GetIndexFilters();
            if (afList != null)
            {
                var filterList = from af in afList
                                 orderby af.FilterIndex
                                 select af.FilterName;
                return filterList.ToList();
            }
            else
                return null;
        }


        public static ColorIndexing.ColorDataSource ConvertColorEnum(string colorName)
        {
            //Translates a string color name from JSAssign to the ColorDataSource enumeration
            switch (colorName)
            {
                case BjX: { return ColorIndexing.ColorDataSource.Bj; }
                case VjX: { return ColorIndexing.ColorDataSource.Vj; }
                case RcX: { return ColorIndexing.ColorDataSource.Rc; }
                case IcX: { return ColorIndexing.ColorDataSource.Ic; }
                case UcX: { return ColorIndexing.ColorDataSource.Uc; }
                case GpX: { return ColorIndexing.ColorDataSource.Gp; }
                case GBpX: { return ColorIndexing.ColorDataSource.GBp; }
                case GRpX: { return ColorIndexing.ColorDataSource.GRp; }
                default: return ColorIndexing.ColorDataSource.Gp;
            }
        }

        public static ColorIndexing.StandardColors ConvertStandardsEnum(string filterName)
        {
            //Translates a string color name from JSAssign to the ColorDataSource enumeration
            switch (filterName)
            {
                case BjX: { return ColorIndexing.StandardColors.Bj; }
                case VjX: { return ColorIndexing.StandardColors.Vj; }
                case RcX: { return ColorIndexing.StandardColors.Rc; }
                case IcX: { return ColorIndexing.StandardColors.Ic; }
                case UcX: { return ColorIndexing.StandardColors.Uc; }
                case GpX: { return ColorIndexing.StandardColors.G; }
                case GBpX: { return ColorIndexing.StandardColors.GBp; }
                case GRpX: { return ColorIndexing.StandardColors.GRp; }
                default: return ColorIndexing.StandardColors.G;
            }
        }

        public static string[] StandardColorsList()
        {
            return new string[] { BjX, VjX, RcX, IcX, UcX, GpX, GRpX, GBpX };
        }

        #region Coefficients

        static Coefficients GmV_VI = new Coefficients(-0.01746, 0.008092, -0.281, 0.03655, 0.0467);
        static Coefficients GbpmV_VI = new Coefficients(-0.05204, 0.483, -0.2001, 0.02186, 0.04483);
        static Coefficients GrpmV_VI = new Coefficients(0.0002428, -0.8675, -0.02866, 0, 0.04474);
        static Coefficients GbpmGrp_VI = new Coefficients(-0.04212, 1.286, -0.09494, 0, 0.02366);
        static Coefficients GmV_VR = new Coefficients(-0.02269, 0.01784, -1.016, 0.2225, 0.04895);
        static Coefficients GmV_BV = new Coefficients(-0.02907, -0.02385, -0.2297, -0.001768, 0.06285);
        static Coefficients GmV_GbpGrp = new Coefficients(-0.0176, -0.00686, -0.1732, 0, 0.045858);
        static Coefficients GmR_GbpGrp = new Coefficients(-0.003226, 0.3833, -0.1345, 0, 0.0484);
        static Coefficients GmI_GbpGrp = new Coefficients(0.02085, 0.7419, -0.09631, 0, 0.04956);

        #endregion

        public static double GaiaToJohnson(StandardColors jcFilter, double gG, double gGbp, double gGrp)
        {
            double filterValOut = 0; ;

            switch (jcFilter)
            {
                case StandardColors.Uc:
                    {
                        break;
                    }
                case StandardColors.Bj:
                    {
                        //This isn't correct, nor even doable -- dont' try to convert Gaia to Bj
                        //double gmR = GaiaMagnitudeTransform(gGbp - gGrp, GmR_GbpGrp);
                        //filterValOut = gG - gmR;
                        break;
                    }
                case StandardColors.Vj:
                    {
                        double gmV = GaiaMagnitudeTransform(gGbp - gGrp, GmV_GbpGrp);
                        filterValOut = gG - gmV;
                        break;
                    }
                case StandardColors.Rc:
                    {
                        double gmR = GaiaMagnitudeTransform(gGbp - gGrp, GmR_GbpGrp);
                        filterValOut = gG - gmR;
                        break;
                    }
                case StandardColors.Ic:
                    {
                        double gmI = GaiaMagnitudeTransform(gGbp - gGrp, GmI_GbpGrp);
                        filterValOut = gG - gmI;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return filterValOut;
        }

        private static double GaiaMagnitudeTransform(double fVal, Coefficients fCoef)
        {
            if (fVal == 0)
                return 0;
            else
                return (fCoef.Constant + fCoef.First * (fVal) + fCoef.Second * Math.Pow(fVal, 2)); //- fCoef.Sigma;
        }

        public static StandardColors FindStandard(string stdFilterLetters)
        {
            switch (stdFilterLetters)
            {
                case "Uc": { return StandardColors.Uc; }
                case "Bj": { return StandardColors.Bj; }
                case "Vj": { return StandardColors.Vj; }
                case "Rc": { return StandardColors.Rc; }
                case "Ic": { return StandardColors.Ic; }
                case "G": { return StandardColors.G; }
                case "GRp": { return StandardColors.GRp; }
                case "GBp": { return StandardColors.GBp; }
                default: { return StandardColors.Vj; }
            }
        }

        public struct Coefficients
        {
            public double Constant, First, Second, Third, Sigma;

            public Coefficients(double zero, double first, double second, double third, double sigma)
            {
                this.Constant = zero;
                this.First = first;
                this.Second = second;
                this.Third = third;
                this.Sigma = sigma;
            }
        }
    }
}
