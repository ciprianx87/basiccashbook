using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TaxCalculator.Common
{
    public static class DbHelper
    {
        public static void CopyDBIfNeeded(string emptyDbName, string dbName, string emptyDbDirName, string dbDirName)
        {
            try
            {
                string currentLocation = System.Reflection.Assembly.GetCallingAssembly().Location;
                FileInfo currentApp = new FileInfo(currentLocation);
                DirectoryInfo dbDir = currentApp.Directory;// new DirectoryInfo(+"\\Database");
                if (dbDir.Exists)
                {
                    DirectoryInfo emptyDbDir = new DirectoryInfo(dbDir.FullName + "\\" + emptyDbDirName);
                    DirectoryInfo databaseDbDir = new DirectoryInfo(dbDir.FullName + "\\" + dbDirName);
                    FileInfo fi = new FileInfo(databaseDbDir.FullName + "\\" + dbName);
                    if (!fi.Exists)
                    {
                        if (!databaseDbDir.Exists)
                        {
                            databaseDbDir.Create();
                        }
                        //copy the empty DB
                        FileInfo emptyFi = new FileInfo(emptyDbDir.FullName + "\\" + emptyDbName);
                        File.Copy(emptyFi.FullName, databaseDbDir.FullName + "\\" + dbName);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogException(ex);
            }
        }

    }
}
