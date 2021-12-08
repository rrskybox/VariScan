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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace VariScan
{
    public static class TSX_Process
    {
        public static void DeleteSRC(string folder)
        {
            //Bug in TSX causes hard stop if SRC file is locked by another process
            //  so delete all .SRC files in folder
            string[] srcFiles = Directory.GetFiles(folder, "*.SRC", SearchOption.AllDirectories);
            foreach (string f in srcFiles)
                try { File.Delete(f); }
                catch { }
            return;
        }

        public static void MinimizeTSX()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName.Contains("Sky"))
                {
                    IDictionary<IntPtr, string> windows = List_Windows_By_PID(process.Id);
                    foreach (KeyValuePair<IntPtr, string> pair in windows)
                    {
                        ShowWindowAsync(pair.Key, SW_SHOWMINIMIZED);
                    }
                }
            }
        }

        public static void NormalizeTSX()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName.Contains("Sky"))
                {
                    IDictionary<IntPtr, string> windows = List_Windows_By_PID(process.Id);
                    foreach (KeyValuePair<IntPtr, string> pair in windows)
                    {
                        ShowWindowAsync(pair.Key, SW_SHOWNORMAL);
                    }
                }
            }
        }

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        private struct WINDOWPLACEMENT
        {
#pragma warning disable CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.length' is never assigned to, and will always have its default value 0
            public int length;
#pragma warning restore CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.length' is never assigned to, and will always have its default value 0
#pragma warning disable CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.flags' is never assigned to, and will always have its default value 0
            public int flags;
#pragma warning restore CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.flags' is never assigned to, and will always have its default value 0
#pragma warning disable CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.showCmd' is never assigned to, and will always have its default value 0
            public int showCmd;
#pragma warning restore CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.showCmd' is never assigned to, and will always have its default value 0
#pragma warning disable CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.ptMinPosition' is never assigned to, and will always have its default value
            public System.Drawing.Point ptMinPosition;
#pragma warning restore CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.ptMinPosition' is never assigned to, and will always have its default value
#pragma warning disable CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.ptMaxPosition' is never assigned to, and will always have its default value
            public System.Drawing.Point ptMaxPosition;
#pragma warning restore CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.ptMaxPosition' is never assigned to, and will always have its default value
#pragma warning disable CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.rcNormalPosition' is never assigned to, and will always have its default value
            public System.Drawing.Rectangle rcNormalPosition;
#pragma warning restore CS0649 // Field 'TSX_Process.WINDOWPLACEMENT.rcNormalPosition' is never assigned to, and will always have its default value
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        [DllImport("USER32.DLL")]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public static IDictionary<IntPtr, string> List_Windows_By_PID(int processID)
        {
            IntPtr hShellWindow = GetShellWindow();
            Dictionary<IntPtr, string> dictWindows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                //ignore the shell window
                if (hWnd == hShellWindow)
                {
                    return true;
                }

                //ignore non-visible windows
                if (!IsWindowVisible(hWnd))
                {
                    return true;
                }

                //ignore windows with no text
                int length = GetWindowTextLength(hWnd);
                if (length == 0)
                {
                    return true;
                }

                uint windowPid;
                GetWindowThreadProcessId(hWnd, out windowPid);

                //ignore windows from a different process
                if (windowPid != processID)
                {
                    return true;
                }

                StringBuilder stringBuilder = new StringBuilder(length);
                GetWindowText(hWnd, stringBuilder, length + 1);
                dictWindows.Add(hWnd, stringBuilder.ToString());

                return true;

            }, 0);

            return dictWindows;
        }
    }
}
