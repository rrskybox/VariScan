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

        public Transformation(List<StarField.FieldLightSource> mLS)
        {
            masterLightSources = mLS;
        }

        private static List<double> Residuals(List<double> xPoint, List<double> yPoint, double slope, double intercept)
        {
            List<double> residual = new List<double>();
            for (int i = 0; i < xPoint.Count; i++)
            {
                double deltaX = Math.Abs((xPoint[i] * slope) - intercept) - yPoint[i];
                residual.Add(deltaX);
            }
            return residual;
        }


        public static (double intercept, double slope) ColorTransform(ref List<double> instrumentColorIndex, ref List<double> standardColorIndex)
        {
            //Tp_rp 	=> Color Transform is the inverse of the regressed slope of mean (r-p)/(R-P) over images
            double colorTransformation;
            (double intercept, double slope) = MathNet.Numerics.Fit.Line(instrumentColorIndex.ToArray(), standardColorIndex.ToArray());
            colorTransformation = 1 / slope;
            return (intercept, colorTransformation);
        }

        public static (double intercept, double slope) MagnitudeTransform(double[] instrumentColorIndex, double[] standardColorIndex)
        {
            //Trp 	=> Magnitude Transform is the regressed slope of mean (R-p)/(R-P) over images
            double magnitudeTransformation;
            (double intercept, double slope) = MathNet.Numerics.Fit.Line(instrumentColorIndex, standardColorIndex);
            magnitudeTransformation = slope;
            return (intercept, magnitudeTransformation);
        }

        public static (double primaryMag, double differentialMag) FormColorIndex(int regIndex, ColorIndexing.ColorDataSource diffColor,
                                                                                 StarField.FieldLightSource[] differentialFLS,
                                                                                 ColorIndexing.ColorDataSource priColor,
                                                                                 StarField.FieldLightSource[] primaryFLS,
                                                                                 bool useGaia)
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
                return (GetCatalogedMagnitude(priColor, pFLS, useGaia), GetCatalogedMagnitude(diffColor, dFLS, useGaia));
        }

        public static double GetCatalogedMagnitude(ColorIndexing.ColorDataSource cds, StarField.FieldLightSource fls, bool useGaia)
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
                        if (useGaia)
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
                        if (useGaia)
                            mag = ColorIndexing.GaiaToJohnson(ColorIndexing.StandardColors.Vj,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeG,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeGbp,
                                                              fls.StandardMagnitudes.Value.GAIACatalogMagnitudeGrp);
                        else
                            mag = fls.StandardMagnitudes.Value.APASSCatalogMagnitudeV; break;
                    }
                case ColorIndexing.ColorDataSource.Rc:
                    {
                        if (useGaia)
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

        public static double GetCatalogedMagnitude(ColorIndexing.ColorDataSource cds, StarField.CatalogData fls, bool useGaia)
        {
            double mag = 0;

            switch (cds)
            {
                case ColorIndexing.ColorDataSource.Bj:
                    {
                        if (useGaia)
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
                        if (useGaia)
                            mag = ColorIndexing.GaiaToJohnson(ColorIndexing.StandardColors.Vj,
                                                              fls.GAIACatalogMagnitudeG,
                                                              fls.GAIACatalogMagnitudeGbp,
                                                              fls.GAIACatalogMagnitudeGrp);
                        else
                            mag = fls.APASSCatalogMagnitudeV; break;
                    }
                case ColorIndexing.ColorDataSource.Rc:
                    {
                        if (useGaia)
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

        public static (List<double> xOut, List<double> yOut) ClusterFinder(List<double> xIn, List<double> yIn)
        {
            //Removes all points which do not have a neighbor within the mean proximity distance for all points
            //
            List<double> xOut = new List<double>();
            List<double> yOut = new List<double>();
            //Compute mean proximity
            //For each xIn/XOut find the distance to the nearest other xIn/xOut 
            //  add to total
            //const double lowX = 50;
            //const double highX = 400;
            //const double lowProx = 1;
            //const double highProx = .25;

            //double cGradient = ((lowProx - highProx) / (lowX - highX));
            //double cIntercept = -(cGradient * lowX) + lowProx;
            //double closeAs = cGradient * xIn.Count + cIntercept;
            double proximity = 0;
            List<double> proximityList = new List<double>();

            //Make a list of the distance of each source for every othersource
            for (int i = 0; i < xIn.Count; i++)
            {
                double shortestDistance = double.MaxValue;
                for (int j = 0; j < xIn.Count; j++)
                {
                    proximity = DistanceXY(xIn[i], yIn[i], xIn[j], yIn[j]);
                    if ((i != j) && (proximity < shortestDistance))
                    {
                        shortestDistance = proximity;
                    }
                }
                proximityList.Add(shortestDistance);
            }
            //Find the average value for nearness of each source to another
            double meanProximity = proximityList.Average();

            double closeAs = .5;

            //Build output list x, y from all sources that are at least as close as the average
            for (int i = 0; i < proximityList.Count; i++)
            {
                if (proximityList[i] < (meanProximity * closeAs))
                {
                    xOut.Add(xIn[i]);
                    yOut.Add(yIn[i]);
                }
            }
            //if clustered points are found, then return that set,
            //  otherwise just return the set of input points
            if (xOut.Count > 0 && yOut.Count > 0)
                return (xOut, yOut);
            else
                return (xIn, yIn);
        }

        public static (List<double> xOut, List<double> yOut) ClusterFinder2(List<double> xIn, List<double> yIn)
        {
            //Removes all points whose average longest distance to all other points exceeds the mean longest distance
            //
            List<double> xOut = new List<double>();
            List<double> yOut = new List<double>();
            //Compute mean proximity
            //For each xIn/XOut find the distance to the nearest other xIn/xOut 
            //  add to total

#pragma warning disable CS0219 // The variable 'closestNode' is assigned but its value is never used
            int closestNode = 0;
#pragma warning restore CS0219 // The variable 'closestNode' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'proximity' is assigned but its value is never used
            double proximity = 0;
#pragma warning restore CS0219 // The variable 'proximity' is assigned but its value is never used
            List<double> proximityList = new List<double>();

            for (int i = 0; i < xIn.Count; i++)
            {
                double distanceSum = 0;
                for (int j = 0; j < xIn.Count; j++)
                {
                    distanceSum += DistanceXY(xIn[i], yIn[i], xIn[j], yIn[j]);
                }
                proximityList.Add(distanceSum);
            }
            //Take mean
            double meanDistanceSum = proximityList.Average();

            //Build output list x, y from all input x,y closer than the mean
            for (int i = 0; i < proximityList.Count; i++)
            {
                if (proximityList[i] < meanDistanceSum)
                {
                    xOut.Add(xIn[i]);
                    yOut.Add(yIn[i]);
                }
            }
            return (xOut, yOut);
        }

        public static (List<double> xs, List<double> ys) TwoPassSlope(List<double> xp, List<double> yp)
        {
            //First pass is low density to get an approximate slope
            //Then use standard deviation to delete all points outside
            //Second pass is for new slope/intercept
            (List<double> lowXP, List<double> lowYP) = ClusterFinder(xp, yp);
            (double interceptLow, double slopeLow) = MathNet.Numerics.Fit.Line(lowXP.ToArray(), lowYP.ToArray());
            List<double> lowModelY = new List<double>();
            foreach (double x in xp)
                lowModelY.Add(x * slopeLow + interceptLow);
            double xpDevSum = 0;
            for (int i = 0; i < xp.Count; i++)
                xpDevSum += Math.Abs(yp[i] - ((xp[i] * slopeLow) + interceptLow));
            xpDevSum /= xp.Count;
            List<double> pass2X = new List<double>();
            List<double> pass2Y = new List<double>();
            for (int i = 0; i < xp.Count; i++)
            {
                if (Math.Abs(yp[i] - lowModelY[i]) < xpDevSum)
                {
                    pass2X.Add(xp[i]);
                    pass2Y.Add(yp[i]);
                }
            }
            return (pass2X, pass2Y);
        }

        public static List<double> OutlierRemover(List<double> arrayIn)
        {
            //Removes all members of arrayIn whose  exceed the mean of the set
            double arrayMean = MathNet.Numerics.Statistics.ArrayStatistics.Mean(arrayIn.ToArray());
            double arrayStdDev = MathNet.Numerics.Statistics.ArrayStatistics.StandardDeviation(arrayIn.ToArray());
            List<double> arrayOut = (from a in arrayIn
                                     where (Math.Abs(a - arrayMean) <= arrayStdDev)
                                     select a).ToList();
            return arrayOut;
        }

        private static double DistanceXY(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        private static double MeanDistanceToNearestStars(double x, double y, List<double> xp, List<double> yp, int meanCount)
        {
            //Find the nearest number of meanCount points in xp,yp and average their distance to x,y
            List<double> proximityList = new List<double>();
            for (int i = 0; i < xp.Count; i++)
                for (int j = 0; j < yp.Count; j++)
                    proximityList.Add(DistanceXY(x, y, xp[i], yp[j]));
            proximityList.Sort();
            if (meanCount == 1)
                return proximityList[0];
            else
                return proximityList.GetRange(0, meanCount).Average();
        }

        public static List<StarField.FieldLightSource> SortByMagnitude(List<StarField.FieldLightSource> lsList, int brightCount)
        {
            //Find the ten brightest stars
            if (lsList.Count < brightCount)
                brightCount = lsList.Count;
            List<StarField.FieldLightSource> listOut = lsList.OrderByDescending(a => a.InstrumentMagnitude, new SpecialComparer()).ToList();
            return listOut.GetRange(0, brightCount);
        }

    }

}

