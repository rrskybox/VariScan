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

using MathNet.Spatial.Euclidean;
using System.Collections.Generic;

namespace VariScan
{
    class Registration
    {
        public static StarField.FieldLightSource[] RegisterLightSources(StarField.FieldLightSource[] masterList, StarField.FieldLightSource[] minionList, double minDistanceArcSec)
        {
            //For each minion light source, find a registered master light source
            //  there may be more minions than masters, so some registrations will be null
            List<StarField.FieldLightSource> registeredList = new List<StarField.FieldLightSource>();
            foreach (StarField.FieldLightSource minion in minionList)
            {
                int? closestMasterFLS = ClosestLightSource(masterList, minion, minDistanceArcSec);
                if (closestMasterFLS != null)
                {
                    StarField.FieldLightSource minionStarField = minion;
                    minionStarField.RegistrationIndex = (int)closestMasterFLS;
                    minionStarField.StandardMagnitudes = masterList[(int)closestMasterFLS].StandardMagnitudes;
                    registeredList.Add(minionStarField);
               }
            }
            return registeredList.ToArray();
        }

        private static int? ClosestLightSource(StarField.FieldLightSource[] lightsourceList, StarField.FieldLightSource lightsource, double minDistanceArcSec)
        {
            //Determines the lightsource on the lightsource list that is the least distance from the light source
            int closestIndex = 0;
            double ldDistance = double.MaxValue;
            Point2D aP = new Point2D(lightsource.SourceRA, lightsource.SourceDec);
            for (int i = 0; i < lightsourceList.Length; i++)
            {
                Point2D bP = new Point2D(lightsourceList[i].SourceRA, lightsourceList[i].SourceDec);
                double d = aP.DistanceTo(bP);
                if (d < ldDistance)
                {
                    closestIndex = i;
                    ldDistance = d;
                }
            }
            if (ldDistance <= minDistanceArcSec)
                return closestIndex;
            else return null;
        }

        public static int? ClosestLightSource(StarField.FieldLightSource[] lightsourceList, double ra, double dec, double minDistanceArcSec)
        {
            //Determines the lightsource from the lightsource list that is the least distance from RA, dec
            int closestIndex = 0;
            double closestDistance = double.MaxValue;
            double currentDistance = 0;
            Point2D aP = new Point2D(ra, dec);
            for (int i = 0; i< lightsourceList.Length; i++)
            {
                Point2D bP = new Point2D(lightsourceList[i].SourceRA, lightsourceList[i].SourceDec);
                currentDistance = aP.DistanceTo(bP);
                if (currentDistance < closestDistance)
                {
                    closestIndex = i;
                    closestDistance = currentDistance;
                }
            }
            if (closestDistance <= minDistanceArcSec)
                return closestIndex;
            else return null;
        }

        public static int? FindRegisteredLightSource(StarField.FieldLightSource[] lightSourceArray, int index)
        {
            for (int i = 0; i < lightSourceArray.Length; i++)
                if (lightSourceArray[i].RegistrationIndex == index)
                    return i;
            return null;
        }

 

    }
}
