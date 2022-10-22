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
using System.Drawing;
using System.Windows.Forms;

namespace VariScan
{
    public class SpecialComparer : IComparer<double>
    {
        public int Compare(double d1, double d2)
        {
            return d1.CompareTo(d2);
        }
    }

    public class SpecialDateComparer : IComparer<string>
    {
        public int Compare(string d1, string d2)
        {
            return Convert.ToDateTime(d2).CompareTo(Convert.ToDateTime(d1));
        }

    }


    public static class Utility
    {

        //Common utilities for TSX connections
        //

        public static double ReduceTo360(double degrees)
        {
            degrees = Math.IEEERemainder(degrees, 360);
            if (degrees < 0)
            { degrees += 360; }
            return degrees;
        }

        public static void ButtonRed(Button genericButton)
        {
            genericButton.ForeColor = Color.Black;
            genericButton.BackColor = Color.LightSalmon;
            return;
        }

        public static void ButtonGreen(Button genericButton)
        {
            genericButton.ForeColor = Color.Black;
            genericButton.BackColor = Color.PaleGreen;
            return;
        }

        public static bool IsButtonRed(Button genericButton)
        {
            if (genericButton.BackColor == Color.LightSalmon)
            { return true; }
            else
            { return false; }
        }

        public static bool IsButtonGreen(Button genericButton)
        {
            if (genericButton.BackColor == Color.PaleGreen)
            { return true; }
            else
            { return false; }
        }

        public static bool CloseEnough(double testval, double targetval, double minRange)
        {
            //Cute little method for determining if a value is withing a certain percentatge of
            // another value.
            //testval is the value under consideration
            //targetval is the value to be tested against
            //minRange is how close the two need to be within to test true
            // otherwise returns false

            if (Math.Abs(targetval - testval) <= minRange)
            { return true; }
            else
            { return false; }
        }

        public static string CreateStarLabel(string catName, double RA, double Dec)
        {
            //Creates a name for a blank "Gaia" star
            return (catName + " " + RA.ToString("0.0000") + " " + Dec.ToString("0.0000"));
        }

        public static string ParsePathToFileName(string fullPath)
        {
            //return just the file or directory name from a string containing the full path
            char[] splitter = new char[] { '\\' };
            string[] allwords = fullPath.Split(splitter);
            string lastword = allwords[allwords.Length - 1];
            return lastword;
        }

        private static double MeanDifference(double[] a, double[] b)
        {
            double[] c = new double[a.Length];
            for (int i = 0; i < a.Length; i++) c[i] = a[i] - b[i];
            return MathNet.Numerics.Statistics.Statistics.Mean(c);
        }

        public static double RMS(double x, double y)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        public static double ParseRADecString(string radec)
        {
            //Converts a string in either decimal or sexidecimal format to a double
            //if the string splits because it has internal spaces, then treat as sexidecimal
            //  otherwise treat as decimal 
            char[] remChar = { 'h', 'm', 's', 'd' };
            radec = radec.Replace(':', ' ');
            for (int i = 0; i < radec.Length; i++) if (radec[i] == '\"') radec = radec.Remove(i, 1);
            string[] radecSplit = radec.Split(' ');
            if (radecSplit.Length == 1) return Convert.ToDouble(radec);
            else
            {
                if (radecSplit.Length < 3) return 0;
                for (int i = 0; i < 3; i++) radecSplit[i] = radecSplit[i].TrimEnd(remChar);
                int radecsign = 1;
                if (radecSplit[0].Contains("-")) radecsign = -1;
                double radecDouble = radecsign *
                    (Math.Abs(Convert.ToDouble(radecSplit[0])) + Convert.ToDouble(radecSplit[1]) / 60.0 + Convert.ToDouble(radecSplit[2]) / 3600.0);
                return radecDouble;
            }
        }

        public static string ParseNameString(string name)
        {
            //Converts a string in either decimal or sexidecimal format to a double
            //if the string splits because it has internal spaces, then treat as sexidecimal
            //  otherwise treat as decimal 
            for (int i = 0; i < name.Length; i++) if (name[i] == '\"') { name = name.Remove(i, 1); }
            return name;
        }

        public static string SexidecimalRADec(double radec, bool hourFlag)
        {
            //turn the double value into xxh yym zzs or xxd yym zzs
            //  depending on hourFlag -- if true then it's RA: hours
            if (radec == 0) return "";
            int sign = Math.Sign(radec);
            radec = Math.Abs(radec);
            int degreeHours = (int)radec;
            radec -= degreeHours;
            radec *= 60;
            int minutes = (int)radec;
            radec -= minutes;
            radec *= 60;
            if (hourFlag) return (sign * degreeHours).ToString("00") + "h " + minutes.ToString("00") + "m " + radec.ToString("00.0") + "s";
            else return (sign * degreeHours).ToString("00") + "d " + minutes.ToString("00") + "m " + radec.ToString("00.0") + "s";
        }

        public static bool MatchPoint(Point a, Point b)
        {
            if (a == b)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns true if b is between a and c, inclusive
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsBetween(double a, double b, double c)
        {
            if (a <= c && a <= b && b <= c) return true;
            else if (a >= c && a >= b && b >= c) return true;
            else return false;
        }

        public static string NAGenerator(int iValue, double maxNotNA)
        {
            if (iValue <= maxNotNA) return iValue.ToString("0");
            else return "N/A";
        }

        public static MathNet.Numerics.Statistics.Bucket FullestBucket(MathNet.Numerics.Statistics.Histogram hist)
        {
            MathNet.Numerics.Statistics.Bucket b = null;
            //find bucket with highest count
            double bucketCount = 0;
            for (int i = 0; i < hist.BucketCount; i++)
                if (hist[i].Count > bucketCount)
                {
                    bucketCount = hist[i].Count;
                    b = hist[i];
                }
            return b;
        }

        public static bool NightTest(DateTime sampleDT, DateTime sessionNight)
        {
            //Determine it sampleDT is within 12 hours, plus or minus of the sessionNight
            //  ssessionNight is defined as the local day after or on the time of the image
            //  which will be a datetime of date at 0000 hours that is midnight of the
            //  session
            DateTime sessionZero = sessionNight -= sessionNight.TimeOfDay;
            DateTime sampleZero = (sampleDT + TimeSpan.FromHours(6)) - (sampleDT + TimeSpan.FromHours(6)).TimeOfDay;
            if (sessionZero != sampleZero)
                return false;
            else
                return true;
        }

        public static DateTime GetImageSession(DateTime sampleDT)
        {
            //Figure out the session date for this sample
            DateTime sessionDT = sampleDT.Date;
            if (sampleDT - sessionDT > TimeSpan.FromHours(12))
                return sessionDT.AddDays(1);
            else
                return sessionDT;
        }

    }
}

