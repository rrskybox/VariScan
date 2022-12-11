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
using System.Globalization;
using TheSky64Lib;

namespace VariScan
{
    public class FitsFileTSX
    {
        public string? FitsTarget;
        public double? PixSize;
        public double? Gain;
        public double? Aperture;
        public double? FocalLength;
        public double? Exposure;
        public string Filter;
        public int? PixBits;
        public int? ImagePixWidth;
        public int? ImagePixHeight;
        public int? BinX;
        public int? BinY;
        public int? Pedistal;
        public string FitsUTCDate;
        public string FitsUTCTime;
        public DateTime FitsUTCDateTime;
        public double? FitsAirMass;
        public double? FitsRA;
        public double? FitsDec;
        public DateTime FitsLocalTime;


        public FitsFileTSX(ccdsoftImage tsximg)
        {
            //Using open FITS file information...
            //Compute pixel scale = 206.256 * pixel size (in microns) / focal length
            FitsTarget = GetFitsString(tsximg, "OBJECT");
            string fitsUDT = GetFitsString(tsximg, "DATE-OBS").Split('.')[0];
            //DateTime utcDT = DateTime.ParseExact(fitsUDT,"yyyy-MM-ddTHH:mm:ss", CultureInfo.CurrentCulture);
            //Gregorian Calander culture doesn't work in 64 bits -- wrote my own parser
            string[] dsts = fitsUDT.Split('T');
            string[] ds = dsts[0].Split('-');
            string[] dt = dsts[1].Split(':');
            int year = Convert.ToInt16(ds[0]);
            int month = Convert.ToInt16(ds[1]);
            int day = Convert.ToInt16(ds[2]);
            int hour = Convert.ToInt16(dt[0]);
            int minute = Convert.ToInt16(dt[1]) % 60;
            int second = Convert.ToInt16(dt[2]);
            //
            DateTime utcDT = new DateTime(year, month, day, hour, minute, second);
            FitsUTCDate = utcDT.Date.ToShortDateString();
            FitsUTCTime = utcDT.TimeOfDay.ToString();
            FitsUTCDateTime = utcDT;
            FitsRA = Utility.ParseRADecString(GetFitsString(tsximg, "OBJCTRA"));
            FitsDec = Utility.ParseRADecString(GetFitsString(tsximg, "OBJCTDEC"));
            PixSize = GetFitsDouble(tsximg, "XPIXSZ");
            PixSize = GetFitsDouble(tsximg, "XPIXELSZ") ?? PixSize;
            Gain = GetFitsDouble(tsximg, "EGAIN");
            FocalLength = GetFitsDouble(tsximg, "FOCALLEN");
            Aperture = GetFitsDouble(tsximg, "APTDIA");
            Exposure = GetFitsDouble(tsximg, "EXPTIME");
            Filter = GetFitsString(tsximg, "FILTER");
            PixBits = (int)GetFitsDouble(tsximg, "BITPIX");
            ImagePixWidth = (int)(tsximg.FITSKeyword("NAXIS1") ?? 0);
            ImagePixHeight = (int)(tsximg.FITSKeyword("NAXIS2") ?? 0);
            BinX = (int)tsximg.FITSKeyword("XBINNING");
            BinY = (int)tsximg.FITSKeyword("YBINNING");
            try { Pedistal = (int)(tsximg.FITSKeyword("PEDISTAL") ?? 0); }
            catch { Pedistal = 0; }
            FitsAirMass = GetFitsDouble(tsximg, "AIRMASS");
            string fitsLocal = GetFitsString(tsximg, "LOCALTIM");
            fitsLocal = fitsLocal.Substring(0, fitsLocal.Length - 4);
            FitsLocalTime = DateTime.ParseExact(fitsLocal, "M/d/yyyy hh:mm:ss.FFF tt", CultureInfo.CurrentCulture);  //'5/18/2020 01:53:24.469 AM STD' 
        }

        private double? GetFitsDouble(ccdsoftImage tsximg, string keyWord)
        {
            double? keyValue;
            try { keyValue = tsximg.FITSKeyword(keyWord); }
            catch { return null; }
            return keyValue;
        }
        private string GetFitsString(ccdsoftImage tsximg, string keyWord)
        {
            string keyValue;
            try { keyValue = tsximg.FITSKeyword(keyWord); }
            catch { return "None"; }
            return keyValue;
        }
        private int GetFitsInteger(ccdsoftImage tsximg, string keyWord)
        {
            int keyValue;
            try { keyValue = tsximg.FITSKeyword(keyWord); }
            catch { return 0; }
            return keyValue;
        }

    }
}
