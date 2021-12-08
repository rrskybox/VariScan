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
    public class WCSReader
    {

        enum ccdsoftWCSIndex
        {
            cdWCSRA,                //0
            cdWCSDec,               //1 
            cdWCSX,                 //2
            cdWCSY,                 //3
            cdWCSPositionError,     //4 
            cdWCSResidual,          //5 
            cdWCSCatalogID,         //6 
            cdActive                //7
        }

        public struct AstroSolution
        {
            public bool Used;
            public string StarName;
            public double PositionError;
            public double RA;
            public double Dec;
            public double ImageX;
            public double ImageY;
            public double Residual;
        }

        public static List<AstroSolution> ReadWCS(ccdsoftImage tsxi)
        {
            List<AstroSolution> astList = new List<AstroSolution>();
            int wcsCount;
            try { wcsCount = tsxi.InsertWCS(true); }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex) { return astList; }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            //Read in WCS Array
            object[] wcsRA = tsxi.WCSArray(0);
            object[] wcsDec = tsxi.WCSArray(1);
            object[] wcsX = tsxi.WCSArray(2);
            object[] wcsY = tsxi.WCSArray(3);
            object[] wcsErr = tsxi.WCSArray(4);
            object[] wcsRes = tsxi.WCSArray(5);
            object[] wcsID = tsxi.WCSArray(6);
            object[] wcsActive = tsxi.WCSArray(7);
            for (int i = 0; i < wcsRA.Length; i++)
            {
                if ((double)wcsActive[i] == 1)
                {
                    AstroSolution ast = new AstroSolution();

                    double ra = (double)wcsRA[i];
                    double dec = (double)wcsDec[i];
                    ast.RA = ra;
                    ast.Dec = dec;
                    ast.ImageX = (double)wcsX[i];
                    ast.ImageY = (double)wcsY[i];
                    ast.Residual = (double)wcsRes[i];
                    ast.PositionError = (double)wcsErr[i];
                    ast.StarName = wcsID[i].ToString();
                    object a = tsxi.FindInventoryAtRADec(ra, dec);
                    astList.Add(ast);
                }
            }

            return astList;

        }
    }
}
