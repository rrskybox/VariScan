﻿// --------------------------------------------------------------------------------
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
    class DeviceControl
    {
        public bool TelescopeStartUp()
        {
            //Method for connecting and unparking the TSX mount,
            // leaving it connected
            sky6RASCOMTele tsxm = new sky6RASCOMTele();
            if (tsxm.IsConnected == 0)
                tsxm.Connect();
            //If parked, try to unpark, if fails return false
            try
            { if (tsxm.IsParked()) tsxm.Unpark(); }
            catch { return false; }
            //Otherwise return true;
            return true;
        }

        public void TelescopePrePosition(string side)
        {
            // Directs the mount to point either to the "East" or "West" side of the 
            // meridian at a location of 80 degrees altitude.  Used for autofocus routine
            // and for starting off the target search
            sky6RASCOMTele tsxm = new sky6RASCOMTele();
            tsxm.Asynchronous = 0;
            tsxm.Connect();
            if (side == "East")
            {
                tsxm.SlewToAzAlt(90.0, 80.0, "");
                while (tsxm.IsSlewComplete == 0) System.Threading.Thread.Sleep(1000);
            }
            else
            {
                tsxm.SlewToAzAlt(270.0, 80.0, "");
                while (tsxm.IsSlewComplete == 0) System.Threading.Thread.Sleep(1000);
            }
            return;
        }

        public bool TelescopeShutDown()
        //Method for connecting and parking and disconnecting the TSX mount
        {
            sky6RASCOMTele tsxm = new sky6RASCOMTele();
            if (tsxm.IsConnected == 0) { tsxm.Connect(); }
            try
            {
                tsxm.Park();
                tsxm.Disconnect();
            }
            catch { return false; }
            return true;
        }

        public bool CameraStartUp()
        {
            //Method for connecting and initializing the TSX camera
            ccdsoftCamera tsxc = new ccdsoftCamera();
            try { tsxc.Connect(); }
            catch { return false; }
            return true;
        }

        public bool FocuserStartUp()
        {
            //Method for connecting and initializing the TSX camera
            ccdsoftCamera tsxc = new ccdsoftCamera();
            try { tsxc.focConnect(); }
            catch { return false; }
            return true;
        }

        public bool RotatorStartUp()
        {
            //Method for connecting and initializing the TSX camera
            ccdsoftCamera tsxc = new ccdsoftCamera();
            try { tsxc.rotatorConnect(); }
            catch { return false; }
            return true;
        }

        public bool HasRotator()
        {
            //Returns true if a rotator is connected, else false
            ccdsoftCamera tsxc = new ccdsoftCamera();
            if (tsxc.rotatorIsConnected() == 1) //rotator is present and powered up
                return true;
            else return false;
        }

        public void SetCameraTemperature(double settemp)
        {
            //Method for setting TSX camera temp
            const int temperatureSettlingRange = 1;
            ccdsoftCamera tsxc = new ccdsoftCamera();
            tsxc.TemperatureSetPoint = settemp;
            tsxc.RegulateTemperature = 1;
            while (!Utility.CloseEnough(tsxc.Temperature, settemp, temperatureSettlingRange))
            {
                System.Threading.Thread.Sleep(1000);
            };
            return;
        }

        public void ReliableRADecSlew(double RA, double Dec, string name, bool hasDome)
        {
            //
            //Checks for dome tracking underway, waits half second if so -- doesn't solve race condition, but may avoid 
            sky6RASCOMTele tsxt = new sky6RASCOMTele();
            if (hasDome)
            {
                while (IsDomeTrackingUnderway()) System.Threading.Thread.Sleep(500);
                int result = -1;
                while (result != 0)
                {
                    result = 0;
                    try { tsxt.SlewToRaDec(RA, Dec, name); }
                    catch (Exception ex) { result = ex.HResult - 1000; }
                }
            }
            else tsxt.SlewToRaDec(RA, Dec, name);
            return;
        }

        public int ReliableClosedLoopSlew(double RA, double Dec, string name, bool hasDome, string reductionType)
        {
            //Tries to perform CLS without running into dome tracking race condition
            //
            //First set camera for image reduction
            ccdsoftCamera tsxc = new ccdsoftCamera();
            switch (reductionType)
            {
                case "None":
                    { tsxc.ImageReduction = ccdsoftImageReduction.cdNone; break; }
                case "2":
                    { tsxc.ImageReduction = ccdsoftImageReduction.cdAutoDark; break; }
                case "3":
                    { tsxc.ImageReduction = ccdsoftImageReduction.cdBiasDarkFlat; break; }
            }

            ReliableRADecSlew(RA, Dec, name, hasDome);
            ClosedLoopSlew tsx_cl = new ClosedLoopSlew();
            int clsStatus = 123;
            //If dome, Turn off tracking
            if (hasDome)
            {
                DomeCouplingOff();
                while (clsStatus == 123)
                {
                    try { clsStatus = tsx_cl.exec(); }
                    catch (Exception ex)
                    {
                        clsStatus = ex.HResult - 1000;
                    };
                    if (clsStatus == 123) System.Threading.Thread.Sleep(500);
                }
                DomeCouplingOn();
            }
            else
            {
                try { clsStatus = tsx_cl.exec(); }
                catch (Exception ex)
                {
                    clsStatus = ex.HResult - 1000;
                };
            }
            return clsStatus;
        }

        private bool IsDomeTrackingUnderway()
        {
            //Test to see if a dome tracking operation is underway.
            // If so, doing a IsGotoComplete will throw an Error 212.
            // return true
            // otherwise return false
            return false;

            //sky6Dome tsxd = new sky6Dome();
            //int testDomeTrack;
            //try { testDomeTrack = tsxd.IsGotoComplete; }
            //catch { return true; }
            //if (testDomeTrack == 0) return true;
            //else return false;
        }

        public void ToggleDomeCoupling()
        {
            //Uncouple dome tracking, then recouple dome tracking (synchronously)
            sky6Dome tsxd = new sky6Dome();
            tsxd.IsCoupled = 0;
            System.Threading.Thread.Sleep(1000);
            tsxd.IsCoupled = 1;
            //Wait for all dome activity to stop
            while (IsDomeTrackingUnderway()) { System.Threading.Thread.Sleep(1000); }
            return;
        }

        public void DomeCouplingOn()
        {
            //Uncouple dome tracking, then recouple dome tracking (synchronously)

            sky6Dome tsxd = new sky6Dome();
            tsxd.IsCoupled = 1;
            System.Threading.Thread.Sleep(500);
            while (IsDomeTrackingUnderway())
            { System.Threading.Thread.Sleep(1000); }
            return;
        }

        public void DomeCouplingOff()
        {
            //Uncouple dome tracking, then recouple dome tracking (synchronously)
            sky6Dome tsxd = new sky6Dome();
            tsxd.IsCoupled = 0;
            System.Threading.Thread.Sleep(500);
            while (IsDomeTrackingUnderway()) { System.Threading.Thread.Sleep(1000); }
            return;
        }

        public double? CurrentIPA()
        {
            //Will return Image PA of most recent image link
            double? currentImagePA = null;
            if (!HasRotator())
                return currentImagePA;
            ImageLinkResults ilr = new ImageLinkResults();
            try
            { currentImagePA = ilr.imagePositionAngle; }
            catch
            { return currentImagePA; }
            return currentImagePA;
        }

        public bool RotateToImagePA(double destinationIPA)
        {
            //Move the rotator to a position that gives an image position angle of tImagePA
            //  Assumes that the rotator position angle variables are current
            //Returns false if failure, true if good

            if (!HasRotator())
                return false;
            ccdsoftCamera tsxc = new ccdsoftCamera();
            //target rotation PA = current image PA + current rotator PA - target image PA 

            double rotatorPA = tsxc.rotatorPositionAngle();
            double destRotationPA = ((double)CurrentIPA() - destinationIPA) + rotatorPA;
            double destRotationPAnormalized = AstroMath.Transform.NormalizeDegreeRange(destRotationPA);
            tsxc.rotatorGotoPositionAngle(destRotationPAnormalized);
            return true;
        }

    }
}

