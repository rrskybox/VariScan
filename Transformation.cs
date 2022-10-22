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
using System.Linq;


namespace VariScan
{
    public class Transformation
    {

        private List<StarField.FieldLightSource> masterLightSources;

        public static (double primaryMag, double differentialMag) FormColorIndex(int regIndex, ColorIndexing.ColorDataSource diffColor,
                                                                                StarField.FieldLightSource[] differentialFLS,
                                                                                ColorIndexing.ColorDataSource priColor,
                                                                                StarField.FieldLightSource[] primaryFLS,
                                                                                string catalog)
        {
            StarField.FieldLightSource pFLS = (from StarField.FieldLightSource p in primaryFLS
                                               where (p.RegistrationIndex == regIndex)
                                               select p).FirstOrDefault();
            StarField.FieldLightSource dFLS = (from StarField.FieldLightSource d in differentialFLS
                                               where (d.RegistrationIndex == regIndex)
                                               select d).FirstOrDefault();
            if (pFLS.StandardMagnitudes == null || dFLS.StandardMagnitudes == null)
                return (0, 0);
            else
                return (GetCatalogedMagnitude(priColor, pFLS, catalog), GetCatalogedMagnitude(diffColor, dFLS, catalog));
        }

        public static double GetCatalogedMagnitude(ColorIndexing.ColorDataSource cds, StarField.FieldLightSource fls, string catalog)
        {
            double mag = 0;

            switch (cds)
            {
                case ColorIndexing.ColorDataSource.Instrument:
                    {
                        mag = fls.InstrumentMagnitude;
                        break;
                    }
                case ColorIndexing.ColorDataSource.Bj:
                    {
                        if (catalog == "Gaia")
                            mag = ColorIndexing.GaiaToJohnson(ColorIndexing.StandardColors.Bj,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeG,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeGbp,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeGrp);
                        else
                            mag = fls.StandardMagnitudes.Value.APASSCatalogMagnitudeB;
                        break;
                    }
                case ColorIndexing.ColorDataSource.Vj:
                    {
                        if (catalog == "Gaia")
                            mag = ColorIndexing.GaiaToJohnson(ColorIndexing.StandardColors.Vj,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeG,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeGbp,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeGrp);
                        else
                            mag = fls.StandardMagnitudes.Value.APASSCatalogMagnitudeV; break;
                    }
                case ColorIndexing.ColorDataSource.Rc:
                    {
                        if (catalog == "Gaia")
                            mag = ColorIndexing.GaiaToJohnson(ColorIndexing.StandardColors.Rc,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeG,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeGbp,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeGrp);
                        else
                            mag = fls.StandardMagnitudes.Value.APASSCatalogMagnitudeR;
                        break;
                    }
                case ColorIndexing.ColorDataSource.Ic:
                    {
                        mag = fls.StandardMagnitudes.Value.APASSCatalogMagnitudeI;
                        break;
                    }
                case ColorIndexing.ColorDataSource.Uc:
                    {
                        mag = fls.StandardMagnitudes.Value.APASSCatalogMagnitudeU;
                        break;
                    }
                case ColorIndexing.ColorDataSource.Gp:
                    {
                        mag = fls.StandardMagnitudes.Value.GAIACatalogMagnitudeG;
                        break;
                    }
                case ColorIndexing.ColorDataSource.GBp:
                    {
                        mag = fls.StandardMagnitudes.Value.GAIACatalogMagnitudeGbp;
                        break;
                    }
                case ColorIndexing.ColorDataSource.GRp:
                    {
                        mag = fls.StandardMagnitudes.Value.GAIACatalogMagnitudeGrp;
                        break;
                    }
            }
            return (mag);
        }

        public static double GetCatalogedMagnitude(ColorIndexing.ColorDataSource cds, StarField.CatalogData fls, string catalog)
        {
            double mag = 0;

            switch (cds)
            {
                case ColorIndexing.ColorDataSource.Bj:
                    {
                        if (catalog == "Gaia")
                            mag = ColorIndexing.GaiaToJohnson(ColorIndexing.StandardColors.Bj,
                                                              fls.GAIACatalogMagnitudeG,
                                                              fls.GAIACatalogMagnitudeGbp,
                                                              fls.GAIACatalogMagnitudeGrp);
                        else
                            mag = fls.APASSCatalogMagnitudeB;
                        break;
                    }
                case ColorIndexing.ColorDataSource.Vj:
                    {
                        if (catalog == "Gaia")
                            mag = ColorIndexing.GaiaToJohnson(ColorIndexing.StandardColors.Vj,
                                                              fls.GAIACatalogMagnitudeG,
                                                              fls.GAIACatalogMagnitudeGbp,
                                                              fls.GAIACatalogMagnitudeGrp);
                        else
                            mag = fls.APASSCatalogMagnitudeV; break;
                    }
                case ColorIndexing.ColorDataSource.Rc:
                    {
                        if (catalog == "Gaia")
                            mag = ColorIndexing.GaiaToJohnson(ColorIndexing.StandardColors.Rc,
                                                              fls.GAIACatalogMagnitudeG,
                                                              fls.GAIACatalogMagnitudeGbp,
                                                              fls.GAIACatalogMagnitudeGrp);
                        else
                            mag = fls.APASSCatalogMagnitudeR;
                        break;
                    }
                case ColorIndexing.ColorDataSource.Ic:
                    {
                        mag = fls.APASSCatalogMagnitudeI;
                        break;
                    }
                case ColorIndexing.ColorDataSource.Uc:
                    {
                        mag = fls.APASSCatalogMagnitudeU;
                        break;
                    }
                case ColorIndexing.ColorDataSource.Gp:
                    {
                        mag = fls.GAIACatalogMagnitudeG;
                        break;
                    }
                case ColorIndexing.ColorDataSource.GBp:
                    {
                        mag = fls.GAIACatalogMagnitudeGbp;
                        break;
                    }
                case ColorIndexing.ColorDataSource.GRp:
                    {
                        mag = fls.GAIACatalogMagnitudeGrp;
                        break;
                    }
            }
            return (mag);
        }

        public static (double intercept, double slope) ColorTransform(List<double> xp, List<double> yp)
        {
            const int thetaCount = 180;
            const int rangeCount = 1000;
            (double intercept, double slope) = HoughTransform(xp, yp, thetaCount, rangeCount);
            return (intercept, slope);
        }

        public static (double intercept, double slope) MagnitudeTransform(List<double> xp, List<double> yp)
        {
            //Trp 	=> Magnitude Transform is the regressed slope of mean (R-p)/(R-P) over images
            const int thetaCount = 180;
            const int rangeCount = 1000;
            (double intercept, double slope) = HoughTransform(xp, yp, thetaCount, rangeCount);
            return (intercept, slope);
        }

        public static (double intercept, double slope) HoughTransform(List<double> xP, List<double> yP, int thetaCount, int rangeCount)
        {
            //Runs Hough transform on list of diagram points
            //

            //Theta will increment from zero to PI (180 degrees)
            double thetaIncrement = Math.PI / thetaCount;
            //The maximum range (r) can be no greater than largest abs(x) + abs(y) value, e.g. r = x cos theta + y sin theta
            double rangeMax = 0;
            for (int i = 0; i < xP.Count; i++)
            {
                double r = Math.Abs(xP[i]) + Math.Abs(yP[i]);
                if (r > rangeMax)
                    rangeMax = r;
            }
            double rangeMin = -rangeMax; //may chnage this later
            //Calculate the range value for each incremental index
            double rangeIncrement = (rangeMax - rangeMin) / rangeCount;
            //Set accumlator range to +/- maxRange
            //Create accumulator array size 
            int[,] accumulator = new int[thetaCount, rangeCount];

            //Convert diagramXY to NormalPoint array
            for (int p = 0; p < xP.Count; p++)
            {
                for (int t = 0; t < thetaCount; t++)
                {
                    double theta = t * thetaIncrement;
                    double rangePoint = xP[p] * Math.Cos(theta) + yP[p] * Math.Sin(theta);
                    //the range runs from - rangeMax to + rangeMax
                    //  the index will be 2 * range/max 
                    int rangeBucket = Convert.ToInt32((rangePoint - rangeMin) / rangeIncrement);
                    //Add vote to range/theta
                    accumulator[t, rangeBucket]++;
                }
            }
            //Find max voted in normal space
            int maxVote = 0;
            int currentVote;
            int votedRangeIndex = 0;
            int votedThetaIndex = 0;
            for (int r = 0; r < rangeCount; r++)
            {
                for (int t = 1; t < thetaCount; t++) //Ignor omega = 0 to avoid infinite slope
                {
                    currentVote = accumulator[t, r];
                    if (currentVote >= maxVote)
                    {
                        maxVote = currentVote;
                        votedRangeIndex = r;
                        votedThetaIndex = t;
                    }
                }
            }
            //y = (-cotθ)*x + (r*cosecθ)
            //  m = (-cotθ)
            //  b = (r * cosecθ)
            if (maxVote == 0)
                return (1, 1);
            double votedTheta = (votedThetaIndex * thetaIncrement);  //in radians
            double slope = -1.0 / Math.Tan(votedTheta);
            double votedRange = votedRangeIndex * rangeIncrement + rangeMin;
            double intercept = votedRange * (1.0 / Math.Sin(votedTheta));
            return (intercept, slope);
        }

        public static List<StarField.FieldLightSource> SortByMagnitude(List<StarField.FieldLightSource> lsList, int brightCount)
        {
            //Find the ten brightest stars
            if (lsList.Count < brightCount)
                brightCount = lsList.Count;
            List<StarField.FieldLightSource> listOut = lsList.OrderByDescending(a => a.InstrumentMagnitude, new SpecialComparer()).ToList();
            return listOut.GetRange(0, brightCount);
        }

        public static (List<double>, List<double>) CullOutliers(List<double> xP, List<double> yP, double intercept, double slope)
        {
            //Compute standard deviation of x,y datapoints
            //Build new list of all points that are within the standard deviation of line y = slope X + intercept
            //Compute the mean deviation for all points w/r/t line defined by slope/intercept
            List<double> distanceList = new List<double>();
            List<double> xPout = new List<double>();
            List<double> yPout = new List<double>();

            double mean = 0;
            for (int i = 0; i < xP.Count; i++)
            {
                double distance = Math.Abs(-slope * xP[i] + yP[i] + (-intercept)) / (Math.Sqrt((-slope * -slope) + 1));
                distanceList.Add(distance);
                mean += distance;

            }
            mean /= xP.Count;
            for (int i = 0; i < xP.Count; i++)
            {
                if (distanceList[i] <= mean)
                {
                    xPout.Add(xP[i]);
                    yPout.Add(yP[i]);
                }
            }
            return (xPout, yPout);
        }
    }
}

