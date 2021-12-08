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

using TheSky64Lib;

namespace VariScan
{
    public static class DomeControl
    {

        public static bool DomeStartUp()
        {
            //Method for connecting and initializing the TSX dome, if any
            // use exception handlers to check for dome commands, opt out if none
            //  couple the dome to telescope if everything works out
            sky6Dome tsxd = new sky6Dome();
            try { tsxd.Connect(); }
            catch { return false; }
            //If a connection is set, then make sure the dome is coupled to the telescope slews
            IsDomeCoupled = true;
            //Wait until the dome stops trying to catch up with the mount
            return true;
        }

        /// <summary>
        /// //Property for coupling the TSX dome, if any
        // use exception handlers to check for dome commands, opt out if none
        //  couple the dome to telescope if everything works out
        /// </summary>
        public static bool IsDomeCoupled
        {
            get
            {
                sky6Dome tsxd = new sky6Dome();
                try { tsxd.Connect(); }
                catch { return false; }
                int cState = tsxd.IsCoupled;
                if (cState == 0)
                    return false;
                else
                    return (true); ;
            }
            set
            {
                sky6Dome tsxd = new sky6Dome();
                try { tsxd.Connect(); }
                catch { return; }
                //If a connection is set, then make sure the dome is coupled to the telescope slews
                if (value)
                    tsxd.IsCoupled = 1;
                else
                    tsxd.IsCoupled = 0;
                return;
            }
        }

        public static void ReliableDomeHome()
        {
            //Method for homing my dome that ensures that wipers for power to the dome
            //  are lined up correctly at the home position so power can flow!
            // use exception handlers to check for dome commands, opt out if none
            //  couple the dome to telescope if everything works out
            //Method for opening the TSX dome 
            sky6Dome tsxd = new sky6Dome();
            //Decouple the dome from the telescope
            IsDomeCoupled = false;
            //Move dome to 20 degrees short of home position
            //  then come back to make sure the home switch is activated accurately
            //Get the az of the current postion -- probably home
            tsxd.GetAzEl();
            double azHome = tsxd.dAz;
            //Move to 20 degrees CCW of current position
            tsxd.GotoAzEl(azHome - 20, 0);
            System.Threading.Thread.Sleep(1000);
            //Wait until complete (Sync mode doesn//t seem to work)
            while (tsxd.IsGotoComplete == 0) { System.Threading.Thread.Sleep(1000); };
            //Home the dome,wait for the command to propogate, then wait until the dome reports it is homed
            System.Threading.Thread.Sleep(6000);
            tsxd.FindHome();
            System.Threading.Thread.Sleep(1000);
            while (tsxd.IsFindHomeComplete == 0) { System.Threading.Thread.Sleep(1000); };
            return;
        }

        public static bool OpenDome()
        {
            //Method to open dome
            //Assume the dome is properly positioned for power
            //Position the dome with at home (wipers on pads)
            // open the dome shutter
            sky6RASCOMTele tsxt = new sky6RASCOMTele();
            //Make sure dome is connected and decoupled
            IsDomeCoupled = false;
            //Disconnect the mount
            tsxt.Disconnect();

            sky6Dome tsxd = new sky6Dome();
            //Stop whatever the dome might have been doing, if any and wait a few seconds for it to clear
            try { tsxd.Abort(); }
            catch { }
            System.Threading.Thread.Sleep(5000);  //Wait for abort command to clear TSX and ASCOM driver
                                                  //TSX throws error for some reason with this code, so maybe bypass it and hope
            tsxd.FindHome();
            System.Threading.Thread.Sleep(1000);
            while (tsxd.IsFindHomeComplete == 0) { System.Threading.Thread.Sleep(1000); };

            tsxd.OpenSlit();
            System.Threading.Thread.Sleep(1000);  //Wait for close command to clear TSX and ASCOM driver
            while (tsxd.IsOpenComplete == 0)
            { System.Threading.Thread.Sleep(1000); } //one second wait loop
            IsDomeCoupled = true;
            return true;
        }

        public static void CloseDome()
        {
            //Method for closing the TSX dome
            // use exception handlers to check for dome commands, opt out if none
            //Park Mount, if not parked already
            sky6RASCOMTele tsxt = new sky6RASCOMTele();
            //Connect dome and decouple the dome from the mount position
            IsDomeCoupled = false;
            //Disconnect the mount
            tsxt.Disconnect();

            sky6Dome tsxd = new sky6Dome();
            try { tsxd.Connect(); }
            catch { return; }
            //Stop whatever the dome is doing, if any and wait a few seconds for it to clear
            try { tsxd.Abort(); }
            catch { return; }
            //Wait for a second for the command to clear
            System.Threading.Thread.Sleep(1000);
            //Close up the dome:  Connect, Home (so power is to the dome), Close the slit
            if (tsxd.IsConnected == 1)
            {
                tsxd.FindHome();
                System.Threading.Thread.Sleep(1000);
                while (tsxd.IsFindHomeComplete == 0) { System.Threading.Thread.Sleep(5000); };
                //Close slit
                //Standard false stop avoidance code
                bool slitClosed = false;
                try
                {
                    tsxd.CloseSlit();
                    System.Threading.Thread.Sleep(1000);
                    while (tsxd.IsCloseComplete == 0) { System.Threading.Thread.Sleep(5000); }
                    //Report success  
                    slitClosed = true;
                }
                catch
                {
                    slitClosed = false;
                }

                //Check to see if slit got closed, if not, then try one more time
                if (!slitClosed)
                {
                    tsxd.CloseSlit();
                    System.Threading.Thread.Sleep(10000);
                    try
                    {
                        while (tsxd.IsCloseComplete == 0) { System.Threading.Thread.Sleep(5000); }
                    }
                    catch { }
                }
            }
            //disconnect dome controller
            tsxd.Disconnect();
        }
    }
}
