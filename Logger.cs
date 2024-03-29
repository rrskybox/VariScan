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
using System.IO;

namespace VariScan
{
    public class Logger
    {
        public Logger()
        {
            Configuration cfg = new Configuration();
            if (cfg.LogFolder == "")
                return;
            string logfilename = DateTime.Now.ToString("yyyy_MM_dd") + ".log";
            string logfilepath = cfg.LogFolder + "\\" + logfilename;
            if (!Directory.Exists(cfg.LogFolder))
                Directory.CreateDirectory(cfg.LogFolder);
            if (!File.Exists(logfilename))
                File.CreateText(logfilename);
            return;
        }

        public void LogEntry(string upd)
        {
            Configuration cfg = new Configuration();
            if (cfg.LogFolder == "")
                return;
            string logfilename = DateTime.Now.ToString("yyyy_MM_dd") + ".log";
            string logfilepath = cfg.LogFolder + "\\" + logfilename;
            System.IO.StreamWriter sys_sw;
            if (!File.Exists(logfilepath))
                sys_sw = File.CreateText(logfilepath);
            else
                sys_sw = new System.IO.StreamWriter(logfilepath, true);
            sys_sw.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " :: " + upd);
            sys_sw.Close();
            return;
        }

    }
}
