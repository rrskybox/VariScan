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

namespace VariScan
{
    class Registration
    {
        public static StarField.FieldLightSource[] RegisterLightSources(StarField.FieldLightSource[] masterList, StarField.FieldLightSource[] minionList)
        {
            //For each master light source, register a minion light source
            //  there may be more minions than masters, so some registrations will be null
            StarField.FieldLightSource[] registeredList = new StarField.FieldLightSource[minionList.Length];
            foreach (StarField.FieldLightSource masterls in masterList)
            {
                int closestMinonFLS = ClosestMinionLightSource(minionList, masterls);
                minionList[closestMinonFLS].RegistrationIndex = masterls.RegistrationIndex;
                minionList[closestMinonFLS].StandardMagnitudes = masterls.StandardMagnitudes;
            }
            registeredList = minionList;
            return registeredList;
        }

        private static int ClosestMinionLightSource(StarField.FieldLightSource[] lightsourceList, StarField.FieldLightSource masterlightsource)
        {
            //Determines the lightsource from the lightsource list that is the least distance from the master
            int closestMinonIndex = 0;
            double ldDistance = double.MaxValue;
            Point2D aP = new Point2D(masterlightsource.SourceRA, masterlightsource.SourceDec);
            for (int i = 0; i < lightsourceList.Length; i++)
            {
                Point2D bP = new Point2D(lightsourceList[i].SourceRA, lightsourceList[i].SourceDec);
                double d = aP.DistanceTo(bP);
                if (d < ldDistance)
                {
                    closestMinonIndex = i;
                    ldDistance = d;
                }
            }
            return closestMinonIndex;
        }

        public static int ClosestFieldLightSource(double ra, double dec, StarField.FieldLightSource[] lightsourceList)
        {
            //Determines the lightsource from the lightsource list that is the least distance from the master
            int ldIndex = 0;
            double ldDistance = double.MaxValue;
            Point2D aP = new Point2D(ra, dec);
            foreach (StarField.FieldLightSource lightsource in lightsourceList)
            {
                Point2D bP = new Point2D(lightsource.SourceRA, lightsource.SourceDec);
                double d = aP.DistanceTo(bP);
                if (d < ldDistance)
                {
                    ldIndex = (int)lightsource.RegistrationIndex;
                    ldDistance = d;
                }
            }
            return ldIndex;
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
