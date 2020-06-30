using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SFog.Business.Utilities.Json
{
    public class SimJson
    {
        public static void BindFile(string Data, string Path)
        {
            try
            {
                File.WriteAllText(Path, Data);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string ReadJsonFile(string Path)
        {
            try
            {
                string JsonText = File.ReadAllText(Path);
                return JsonText;
            }
            catch (Exception ex)
            {
                return ex.InnerException.ToString();
            }
        }
        public static string ReadJsonFile(string Folder, string FileName)
        {
            try
            {
                var pathInfo = new FileInfo(Path.Combine(FileInformation.GetDirectory(), Folder + "/" + FileName + ".txt"));

                string JsonText = File.ReadAllText(pathInfo.ToString());
                return JsonText;
            }
            catch (Exception ex)
            {
                return ex.InnerException.ToString();
            }
        }
        public static void WriteJson(object List, string Folder, string FileName)
        {
            try
            {
                string JsonFog = JsonConvert.SerializeObject(List);
                var pathInfo = new FileInfo(Path.Combine(FileInformation.GetDirectory(), Folder + "/" + FileName));
                BindFile(JsonFog, pathInfo.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}