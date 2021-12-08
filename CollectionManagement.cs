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

using System.Collections.Generic;
using System.IO;

namespace VariScan
{
    class CollectionManagement
    {
        public static void CreateCollection(string collectionPath)
        {
            //If the collection isn't already initialized, create a new collection file structure
            Configuration cfg = new Configuration();
            if (!Directory.Exists(cfg.CollectionFolderPath))
            {
                Directory.CreateDirectory(cfg.CollectionFolderPath);
                cfg.TargetListPath = cfg.CollectionFolderPath + "\\" + Configuration.TargetListFilename;
                cfg.ColorListPath = cfg.CollectionFolderPath + "\\" + Configuration.ColorListFilename;
                cfg.ImageBankFolder = cfg.CollectionFolderPath + "\\" + "Image Bank";
                Directory.CreateDirectory(cfg.ImageBankFolder);
                cfg.StarchiveFilePath = cfg.CollectionFolderPath + "\\" + "Starchive.xml";
                cfg.LogFolder = cfg.CollectionFolderPath + "\\" + "Logs";
                Directory.CreateDirectory(cfg.LogFolder);
            }
        }

        public static string OpenCollection(string collectionName)
        {
            //Change collection to an existing collection
            //  Return the path to the collection's target list)
            Configuration cfg = new Configuration();
            cfg.CollectionFolderPath = cfg.VariScanFolderPath + "\\" + collectionName;
            cfg.TargetListPath = cfg.CollectionFolderPath + "\\" + Configuration.TargetListFilename;
            cfg.ColorListPath = cfg.CollectionFolderPath + "\\" + Configuration.ColorListFilename;
            cfg.ImageBankFolder = cfg.CollectionFolderPath + "\\" + "Image Bank";
            cfg.StarchiveFilePath = cfg.CollectionFolderPath + "\\" + "Starchive.xml";
            cfg.LogFolder = cfg.CollectionFolderPath + "\\" + "Logs";
            return cfg.TargetListPath;
        }

        public static List<string> ListCollections()
        {
            //Produces a list of collection names (not paths)
            Configuration cfg = new Configuration();
            List<string> cList = new List<string>();
            string basepath = cfg.VariScanFolderPath;
            foreach (string fd in Directory.EnumerateDirectories(basepath))
            {
                Path.GetFileName(fd);
                cList.Add(Path.GetFileName(fd));
            }
            return cList;
        }

    }
}
