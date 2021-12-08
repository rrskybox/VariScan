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
using TheSky64Lib;

namespace VariScan
{
    public static class AutoFocus
    {
        //private DateTime afStartTime;
        public static double LastTemp { get; set; } = -100;
        //Autofocus manages the TSX functions to refocus the camera
        // every change of 1 degree in temperature.
        //The first fime autofocus is called, the telescope is slewed to 
        // a position with Az = 90, Alt = 80.  Then @Focus2 is called with
        // TSX providing the star to use.  the temperature at that time is recorded.
        //Subsequent calls to autofocus check to see if the current focuser temperature
        //  is more than a degree celsius different from the last @autofocus2 time.
        //  if so, @autofocus2 is called again, although the telescope is not slewed.  And so on.

        public static string Check(bool isAF2, int focFilter)
        {
            //check to see if current temperature is a degree different from last temperature
            //  If so, then set up and run @focus2
            //AtFocus2 chooses to use a 15 degree x 15 degree field of view to choose a focus star
            //  If the current position is close to the meridian then a focus star on the other
            //  side of the meridian can be choosen and the mount will flip trying to get to it
            //  and, if using a dome, the slew does not wait for the dome slit to catch up (CLS flaw)
            //  so not only will an exception be thrown (Dome command in progress Error 123) the first image
            //   will be crap and the focus fail (as of DB 11360).  So, this method will point the mount to a
            //  altitude that is no more than 80 degrees at the same azimuth of the current position in order
            //  to avoid a flip and subsequent bullshit happening

            ccdsoftCamera tsxc = new ccdsoftCamera();
            tsxc.Connect();
            double currentTemp = tsxc.focTemperature;
            if (Math.Abs(currentTemp - LastTemp) > 1)
            {
                //Going to have to refocus.  

                //Move to altitude away from meridian, if need be
                sky6RASCOMTele tsxt = new sky6RASCOMTele();
                tsxt.GetAzAlt();
                double tAlt = tsxt.dAlt;
                if (tAlt > 80)
                {
                    double tAz = tsxt.dAz;
                    tAlt = 80.0;
                    //turn off tracking to avoid dome error
                    //DeviceControl dctl = new DeviceControl();
                    //dctl.DomeTrackingOff();
                    tsxt.SlewToAzAlt(tAz, tAlt, "AtFocus2ReadyPosition");
                    while (tsxt.IsSlewComplete == 0) System.Threading.Thread.Sleep(1000);

                    //dctl.DomeTrackingOn();
                }

                //reset last temp
                LastTemp = currentTemp;
                int syncSave = tsxc.Asynchronous;
                tsxc.Asynchronous = 0;
                if (isAF2)
                {
                    try
                    {
                        //set filter to focus filter
                        tsxc.FilterIndexZeroBased = focFilter;
                        //launch atfocus2
                        int focStat = tsxc.AtFocus2();
                        //save position vrs temp data
                        double position = tsxc.focPosition;
                        double degrees = tsxc.focTemperature;
                        RecalculateFocuserValues(position, degrees);
                        return ("@Focus2 Successful @ " + degrees.ToString("0.0") + " C => " + position.ToString("0") + " Steps");
                    }
                    catch (Exception e)
                    {
                        tsxc.Asynchronous = syncSave;
                        return ("@Focus2 Failed: " + e.Message);
                    }
                }
                else
                {
                    try
                    {
                        //set filter to focus filter
                        tsxc.FilterIndexZeroBased = focFilter;
                        //launch atfocus2
                        int focStat = tsxc.AtFocus3(3, true);
                        double position = tsxc.focPosition;
                        double degrees = tsxc.focTemperature;
                        RecalculateFocuserValues(position, degrees);
                        return ("@Focus3 Successful @ " + degrees.ToString("0.0") + " C => " + position.ToString("0") + " Steps");
                    }
                    catch (Exception e)
                    {
                        tsxc.Asynchronous = syncSave;
                        return ("@Focus3 Failed: " + e.Message);
                    }
                }
            }
            return ("Focus Check: Temperature change less than 1 degree");
        }

        private static List<Tuple<double, double>> PositionAndTemp = new List<Tuple<double, double>>();

        public static string PresetFocus()
        {
            //Sets the focuser to the position set by the StepsPerDegree and StepAtZero (slope and intercept)
            //  as accumulated for the focuser.  If StepsPerDegree is zero, then no movement is made
            Configuration cfg = new Configuration();

            ccdsoftCamera tsxc = new ccdsoftCamera();
            tsxc.Connect();
            double currentTemp = tsxc.focTemperature;
            int currentPosition = tsxc.focPosition;
            double stepsPerDegree = Convert.ToDouble(cfg.StepsPerDegree);
            double positionAtZero = Convert.ToDouble(cfg.PositionAtZero);
            string logUpdate;
            if (stepsPerDegree != 0.0)
            {
                int newPosition = (int)(currentTemp * stepsPerDegree + positionAtZero);
                int move = (newPosition - currentPosition);
                logUpdate = "Moving Focuser to initial position @ " + currentTemp.ToString("0.0") + " C => " + newPosition.ToString("0") + " Steps";
                try
                {
                    if (move > 0)
                        tsxc.focMoveOut(move);
                    else
                        tsxc.focMoveIn(-move);
                }
                catch (Exception ex)
                {
                    logUpdate += " -- FAILED: " + ex.Message;
                }
            }
            else
                logUpdate = "Cannot determine preset position for focuser";
            return logUpdate;
        }

        private static void RecalculateFocuserValues(double position, double degrees)
        {
            //Determines new values for steps per degree and zero position during this run
            //
            //If the list is empty, just add the current position and temperature
            Configuration cfg = new Configuration();
            if (PositionAndTemp.Count == 0)
                PositionAndTemp.Add(new Tuple<double, double>(position, degrees));
            else
            {
                PositionAndTemp.Add(new Tuple<double, double>(position, degrees));
                double[] sPos = new double[PositionAndTemp.Count];
                double[] sDeg = new double[PositionAndTemp.Count];
                for (int i = 0; i < PositionAndTemp.Count; i++)
                {
                    sPos[i] = PositionAndTemp[i].Item1;
                    sDeg[i] = PositionAndTemp[i].Item2;
                }
                (double intercept, double slope) = MathNet.Numerics.Fit.Line(sDeg, sPos);
                cfg.StepsPerDegree = slope.ToString();
                cfg.PositionAtZero = intercept.ToString();
            }

        }

        public static string ListFocusResults()
        {
            //Outputs the current contents of the position and temp list for diagnostic purposes
            string pt = "Session Focus Position and Temp:  ";
            for (int i = 0; i < PositionAndTemp.Count; i++)
            {
                pt += "\r\n" + "   Position: " + PositionAndTemp[i].Item1.ToString("0") + " Temp: " + PositionAndTemp[i].Item2.ToString("0.00");
            }
            return pt;
        }
    }
}
