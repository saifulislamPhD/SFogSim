using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SFog.Business.Utilities
{
    public class FileInformation
    {
        public static string GetDirectory()
        {
            var appDataPath = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();        
            return appDataPath;
        }

        public static string ExcelFileName => ConfigurationManager.AppSettings["ExcelFileName"];
    }
}