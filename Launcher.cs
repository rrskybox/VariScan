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
using System.Diagnostics;

namespace VariScan
{
    public static class Launcher
    {

        public static void WaitStage()
        {
            // Wait method gets the staging time from the AGNSurvey configuration file,
            //  then runs a one second sleep loop until the current time is greater than the
            //  staging time.

            Configuration cfg = new Configuration();
            //Check to see if Staging executable has been enabled
            //  If so, then wait until the current time is greater than stage system time
            if (Convert.ToBoolean(cfg.StageSystemOn))
            {
                DateTime stageTime = DateTime.Parse(cfg.StageSystemTime);

                while (stageTime > System.DateTime.Now)
                {
                    System.Threading.Thread.Sleep(1000);
                    //AGNSurveyForm.ActiveForm.Show();
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            return;
        }

        public static void WaitStart()
        {
            // Wait method gets the start up time from the AGNSurvey configuration file,
            //  then runs a one second sleep loop until the current time is greater than the
            //  start up time.
            Configuration cfg = new Configuration();
            if (Convert.ToBoolean(cfg.StartUpOn))
            {
                DateTime startTime = DateTime.Parse(cfg.StartUpTime);

                while (startTime > System.DateTime.Now)
                {
                    System.Threading.Thread.Sleep(1000);
                    //AGNSurveyForm.ActiveForm.Show();
                    System.Windows.Forms.Application.DoEvents();
                }
                return;
            }
        }

        public static bool CheckEnd()
        {
            // CheckEnd gets the configured end time and returns true
            //   if the datetime exceeds the end time
            Configuration cfg = new Configuration();
            if (Convert.ToBoolean(cfg.ShutDownOn))
            {
                DateTime startTime = DateTime.Parse(cfg.StartUpTime);
                DateTime endTime = DateTime.Parse(cfg.ShutDownTime);
                //if (startTime > endTime)
                //{
                //    endTime = endTime.AddDays(1);
                //}

                if (endTime < System.DateTime.Now)
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            else
            {
                return (false);
            }
        }

        public static void RunStageSystem()
        {
            //If StageSystemOn is set, then RunStageSystem gets the StageSystem filepath from the AGNSurvey config file, if any
            //  then launches it and waits for completion.

            Configuration cfg = new Configuration();
            if (Convert.ToBoolean(cfg.StageSystemOn))
            {
                Process stageSystemExe = new Process();
                if (cfg.StageSystemPath != "")
                {
                    stageSystemExe.StartInfo.FileName = cfg.StageSystemPath;
                    stageSystemExe.Start();
                    stageSystemExe.WaitForExit();
                }
            }
            return;
        }

        public static void RunStartUp()
        {
            //If StageSystemOn is set, then RunStageSystem gets the StageSystem filepath from the AGNSurvey config file, if any
            //  then launches it and waits for completion.

            Configuration cfg = new Configuration();
            if (Convert.ToBoolean(cfg.StartUpOn))
            {
                if (cfg.StartUpPath != "")
                {
                    Process startUpExe = new Process();
                    startUpExe.StartInfo.FileName = cfg.StartUpPath;
                    startUpExe.Start();
                    startUpExe.WaitForExit();
                }
            }
            return;
        }

        public static void RunShutDown()
        {
            //If ShutDownOn is set, then RunShutDown gets the postscan filepath from the AGNSurvey config file, if any
            //  then launches it and waits for completion.

            Configuration cfg = new Configuration();
            if (Convert.ToBoolean(cfg.ShutDownOn))
            {
                if (cfg.ShutDownPath != "")
                {
                    Process shutDownExe = new Process();
                    shutDownExe.StartInfo.FileName = cfg.ShutDownPath;
                    shutDownExe.Start();
                    shutDownExe.WaitForExit();
                }
            }
            return;
        }

    }
}
