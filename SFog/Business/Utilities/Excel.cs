using OfficeOpenXml;
using SFog.Business.Utilities.Cloud;
using SFog.Models;
using SFog.Models.Archives;
using SFog.Models.Brokers;
using SFog.Models.Nodes;
using SFog.Models.Nodes.Cloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SFog.Business.Utilities
{
    public static class Excel
    {
        public static void CreateExcelSheet(List<Results> list)
        {
            //Move Excel file

            var file = new FileInfo(Path.Combine(FileInformation.GetDirectory(), ""));
            if (list[0].FogDevice != null)
            {
                MoveFogExcelFile();
                file = new FileInfo(Path.Combine(FileInformation.GetDirectory(), FileInformation.ExcelFileName + "_Fog.xlsx"));
            }
            else
            {
                MoveCloudExcelFile();
                file = new FileInfo(Path.Combine(FileInformation.GetDirectory(), FileInformation.ExcelFileName + "_Cloud.xlsx"));
            }
            // Create the file using the FileInfo object

            // Create the package and make sure you wrap it in a using statement
            using (var package = new ExcelPackage(file))
            {
                // add a new worksheet to the empty workbook
                ExcelWorksheet worksheet;
                if (list[0].FogDevice != null)
                {
                    worksheet = package.Workbook.Worksheets.Add("Results_Fog");
                }
                else
                {
                    worksheet = package.Workbook.Worksheets.Add("Results_Cloud");
                }

                ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add("InActive_Fog");

                // --------- Data and styling goes here -------------- //

                // First of all the first row
                worksheet.Cells[1, 1].Value = "Tuple_ID";
                worksheet.Cells[1, 2].Value = "Tuple_Name";
                worksheet.Cells[1, 3].Value = "Tuple_GeoLocation";
                worksheet.Cells[1, 4].Value = "Tuple_DataType";
                worksheet.Cells[1, 5].Value = "Tuple_DataPercentage";
                worksheet.Cells[1, 6].Value = "Tuple_ElapsedTime";
                worksheet.Cells[1, 7].Value = "Tuple_InitiatesTime";
                worksheet.Cells[1, 8].Value = "Fog_ID";
                worksheet.Cells[1, 9].Value = "Fog_Name";
                worksheet.Cells[1, 10].Value = "Fog_GeoLocation";
                worksheet.Cells[1, 11].Value = "Tuple_Reversed";
                worksheet.Cells[1, 12].Value = "Tuple_IsServerFound";
                worksheet.Cells[1, 13].Value = "Fog_DistanceFromServer";
                worksheet.Cells[1, 14].Value = "Fog_DistanceFromTuple";
                worksheet.Cells[1, 15].Value = "TimeTakenByEachTuple";
                worksheet.Cells[1, 16].Value = "DeviceType";

                int rowNumber = 2;

                if (list[0].FogDevice != null)
                {
                    //Add existing users
                    foreach (var item in list)
                    {
                        if (item != null)
                        {
                            worksheet.Cells[rowNumber, 1].Value = item.Tuple.ID;
                            worksheet.Cells[rowNumber, 2].Value = item.Tuple.Name;
                            worksheet.Cells[rowNumber, 3].Value = item.Tuple.GeoLocation.getLongitude() + "," + item.Tuple.GeoLocation.getLatitude();
                            worksheet.Cells[rowNumber, 4].Value = item.Tuple.DataType;
                            worksheet.Cells[rowNumber, 5].Value = item.Tuple.DataPercentage;
                            worksheet.Cells[rowNumber, 6].Value = item.ElapsedTime;
                            worksheet.Cells[rowNumber, 7].Value = item.InitiatesTime;
                            worksheet.Cells[rowNumber, 8].Value = item.FogDevice != null ? item.FogDevice.ID : new Guid();
                            worksheet.Cells[rowNumber, 9].Value = item.FogDevice != null ? item.FogDevice.Name : null;
                            worksheet.Cells[rowNumber, 10].Value = item.FogDevice != null ? item.FogDevice.GeoLocation.getLongitude() + "," + item.FogDevice.GeoLocation.getLatitude() : null;
                            worksheet.Cells[rowNumber, 11].Value = item.Tuple.IsReversed;
                            worksheet.Cells[rowNumber, 12].Value = item.Tuple.IsServerFound;
                            worksheet.Cells[rowNumber, 13].Value = item.FogDevice != null ? item.FogDevice.DistanceFromFogServer : 0.0;
                            worksheet.Cells[rowNumber, 14].Value = item.FogDevice != null ? item.FogDevice.DistanceFromTuple : 0.0;
                            worksheet.Cells[rowNumber, 15].Value = item.ElapsedTime - Convert.ToDouble(item.InitiatesTime) < 0 ? 0 : item.ElapsedTime - Convert.ToDouble(item.InitiatesTime);
                            worksheet.Cells[rowNumber, 16].Value = item.Tuple.DeviceType;
                            rowNumber++;
                        }
                    }
                }
                else
                {
                    foreach (var item in list)
                    {
                        if (item != null)
                        {
                            worksheet.Cells[rowNumber, 1].Value = item.Tuple.ID;
                            worksheet.Cells[rowNumber, 2].Value = item.Tuple.Name;
                            worksheet.Cells[rowNumber, 3].Value = item.Tuple.GeoLocation.getLongitude() + "," + item.Tuple.GeoLocation.getLatitude();
                            worksheet.Cells[rowNumber, 4].Value = item.Tuple.DataType;
                            worksheet.Cells[rowNumber, 5].Value = item.Tuple.DataPercentage;
                            worksheet.Cells[rowNumber, 6].Value = item.ElapsedTime;
                            worksheet.Cells[rowNumber, 7].Value = item.InitiatesTime;
                            worksheet.Cells[rowNumber, 8].Value = item.CloudDevice != null ? item.CloudDevice.ID : new Guid();
                            worksheet.Cells[rowNumber, 9].Value = item.CloudDevice != null ? item.CloudDevice.Name : null;
                            worksheet.Cells[rowNumber, 10].Value = item.CloudDevice != null ? item.CloudDevice.GeoLocation.getLongitude() + "," + item.CloudDevice.GeoLocation.getLatitude() : null;
                            worksheet.Cells[rowNumber, 11].Value = item.Tuple.IsReversed;
                            worksheet.Cells[rowNumber, 12].Value = item.Tuple.IsServerFound;
                            worksheet.Cells[rowNumber, 13].Value = item.CloudDevice != null ? item.CloudDevice.DistanceFromFogServer : 0.0;
                            worksheet.Cells[rowNumber, 14].Value = item.CloudDevice != null ? item.CloudDevice.DistanceFromTuple : 0.0;
                            worksheet.Cells[rowNumber, 15].Value = item.ElapsedTime - Convert.ToDouble(item.InitiatesTime) < 0 ? 0 : item.ElapsedTime - Convert.ToDouble(item.InitiatesTime);
                            worksheet.Cells[rowNumber, 16].Value = item.Tuple.DeviceType;

                            rowNumber++;
                        }
                    }
                }

                worksheet2.Cells[1, 1].Value = "Fog_Name";
                int _rowNumber = 2;
                if (list[0].InActiveFogDecives != null)
                {
                    //Add existing users
                    foreach (var item in list)
                    {
                        if (item.InActiveFogDecives != null)
                        {
                            foreach (var item1 in item.InActiveFogDecives)
                            {
                                worksheet.Cells[_rowNumber, 1].Value = item1.Name;
                                _rowNumber++;
                            }
                        }
                    }
                }

                // Fit the columns according to its content
                worksheet.Column(1).AutoFit();
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit();
                worksheet.Column(5).AutoFit();
                worksheet.Column(6).AutoFit();
                worksheet.Column(7).AutoFit();
                worksheet.Column(8).AutoFit();
                worksheet.Column(9).AutoFit();
                worksheet.Column(10).AutoFit();
                worksheet.Column(11).AutoFit();
                worksheet.Column(12).AutoFit();
                worksheet.Column(13).AutoFit();
                worksheet.Column(14).AutoFit();
                worksheet.Column(15).AutoFit();
                worksheet.Column(16).AutoFit();

                worksheet2.Column(1).AutoFit();

                // Set some document properties
                if (list[0].FogDevice != null)
                {
                    package.Workbook.Properties.Title = "Results_Fog";
                }
                else
                {
                    package.Workbook.Properties.Title = "Results_Cloud";
                }

                package.Workbook.Properties.Author = "Salman";
                package.Workbook.Properties.Company = "Simulation";

                // save our new workbook and we are done!
                package.Save();
            }
        }
        public static void MoveFogExcelFile()
        {
            try
            {
                var deletedExcelFolderPath = Path.Combine(FileInformation.GetDirectory(), "DeletedExcel");
                var sourceFilePath = Path.Combine(FileInformation.GetDirectory(), FileInformation.ExcelFileName + "_Fog.xlsx");

                if (!Directory.Exists(deletedExcelFolderPath))
                    Directory.CreateDirectory(deletedExcelFolderPath);
                var newFileName = deletedExcelFolderPath + "\\Results_Fog" + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".xlsx";
                if (File.Exists(sourceFilePath))
                {
                    File.Move(sourceFilePath, newFileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static void MoveEdgeExcelFile()
        {
            try
            {
                var deletedExcelFolderPath = Path.Combine(FileInformation.GetDirectory(), "DeletedEdgeExcel");
                var sourceFilePath = Path.Combine(FileInformation.GetDirectory(), FileInformation.ExcelFileName + "_Edge.xlsx");

                if (!Directory.Exists(deletedExcelFolderPath))
                    Directory.CreateDirectory(deletedExcelFolderPath);
                var newFileName = deletedExcelFolderPath + "\\Results_Edge" + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".xlsx";
                if (File.Exists(sourceFilePath))
                {
                    File.Move(sourceFilePath, newFileName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static void MoveCloudExcelFile()
        {
            try
            {
                var deletedExcelFolderPath = Path.Combine(FileInformation.GetDirectory(), "DeletedExcel");
                var sourceFilePath = Path.Combine(FileInformation.GetDirectory(), FileInformation.ExcelFileName + "_Cloud.xlsx");

                if (!Directory.Exists(deletedExcelFolderPath))
                    Directory.CreateDirectory(deletedExcelFolderPath);
                var newFileName = deletedExcelFolderPath + "\\Results_Cloud" + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".xlsx";
                if (File.Exists(sourceFilePath))
                {
                    File.Move(sourceFilePath, newFileName);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public static void CreateExcelSheetForCloud(List<Results> list)
        {
            try
            {
                MoveCloudExcelFile();
                var file = new FileInfo(Path.Combine(FileInformation.GetDirectory(), FileInformation.ExcelFileName + "_Cloud.xlsx"));

                // Create the file using the FileInfo object

                // Create the package and make sure you wrap it in a using statement
                using (var package = new ExcelPackage(file))
                {
                    // add a new worksheet to the empty workbook
                    #region CloudSimulation

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Results_Cloud");

                    // First of all the first row
                    worksheet.Cells[1, 1].Value = "Tuple_ID";
                    worksheet.Cells[1, 2].Value = "Tuple_Name";
                    worksheet.Cells[1, 3].Value = "Tuple_GeoLocation";
                    worksheet.Cells[1, 4].Value = "Tuple_DataType";
                    worksheet.Cells[1, 5].Value = "Tuple_ElapsedTime";
                    worksheet.Cells[1, 6].Value = "Tuple_InitiatesTime";
                    worksheet.Cells[1, 7].Value = "TotalTimeTaken";
                    worksheet.Cells[1, 8].Value = "Tuple_Reversed";
                    worksheet.Cells[1, 9].Value = "Tuple_IsServerFound";
                    worksheet.Cells[1, 10].Value = "DeviceType";
                    worksheet.Cells[1, 11].Value = "DataCenter_Name";
                    worksheet.Cells[1, 12].Value = "DataCenter_GeoLocation";
                    worksheet.Cells[1, 13].Value = "Cloud_DistanceFromServer";
                    worksheet.Cells[1, 14].Value = "DataCenter_DistanceFromTuple";
                    worksheet.Cells[1, 15].Value = "Tuple_PropogationTime";
                    worksheet.Cells[1, 16].Value = "InternalProcessingTime";
                    worksheet.Cells[1, 17].Value = "DataCenter";

                    int rowNumber = 2;

                    if (list[0].CloudBroker != null)
                    {
                        //Add existing users
                        foreach (var item in list)
                        {
                            if (item != null)
                            {
                                worksheet.Cells[rowNumber, 1].Value = item.Tuple.ID;
                                worksheet.Cells[rowNumber, 2].Value = item.Tuple.Name;
                                worksheet.Cells[rowNumber, 3].Value = item.Tuple.GeoLocation.getLongitude() + "," + item.Tuple.GeoLocation.getLatitude();
                                worksheet.Cells[rowNumber, 4].Value = item.Tuple.DataType;
                                worksheet.Cells[rowNumber, 5].Value = item.ElapsedTime;
                                worksheet.Cells[rowNumber, 6].Value = item.InitiatesTime;
                                worksheet.Cells[rowNumber, 7].Value = item.ElapsedTime - Convert.ToDouble(item.InitiatesTime) < 0 ? 0 : item.ElapsedTime - Convert.ToDouble(item.InitiatesTime);
                                worksheet.Cells[rowNumber, 8].Value = item.Tuple.IsReversed;
                                worksheet.Cells[rowNumber, 9].Value = item.Tuple.IsServerFound;
                                worksheet.Cells[rowNumber, 10].Value = item.Tuple.DeviceType;
                                worksheet.Cells[rowNumber, 11].Value = item.CloudBroker.SelectedDataCenter.Name != null ? item.CloudBroker.SelectedDataCenter.Name : "Null";
                                worksheet.Cells[rowNumber, 12].Value = item.CloudBroker.SelectedDataCenter.DatacenterCharacteristics.GeoLocation.getLongitude() + "," +
                                                                        item.CloudBroker.SelectedDataCenter.DatacenterCharacteristics.GeoLocation.getLatitude();
                                worksheet.Cells[rowNumber, 13].Value = 0.0;
                                worksheet.Cells[rowNumber, 14].Value = item.CloudBroker.SelectedDataCenter.DatacenterCharacteristics.DistanceFromTuple;
                                worksheet.Cells[rowNumber, 15].Value = item.Link.Propagationtime;
                                worksheet.Cells[rowNumber, 16].Value = item.CloudBroker.Tuple.InternalProcessingTime;
                                worksheet.Cells[rowNumber, 17].Value = item.CloudBroker.SelectedDataCenter.Name;
                                rowNumber++;
                            }
                        }
                    }

                    // Fit the columns according to its content
                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();
                    worksheet.Column(5).AutoFit();
                    worksheet.Column(6).AutoFit();
                    worksheet.Column(7).AutoFit();
                    worksheet.Column(8).AutoFit();
                    worksheet.Column(9).AutoFit();
                    worksheet.Column(10).AutoFit();
                    worksheet.Column(11).AutoFit();
                    worksheet.Column(12).AutoFit();
                    worksheet.Column(13).AutoFit();
                    worksheet.Column(14).AutoFit();
                    worksheet.Column(15).AutoFit();
                    worksheet.Column(16).AutoFit();
                    worksheet.Column(17).AutoFit();

                    #endregion CloudSimulation

                    #region Cloud_Characteristics

                    ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add("Cloud_Characteristics");

                    // --------- Data and styling goes here -------------- //

                    worksheet2.Cells[1, 1].Value = "Cloud_Service";
                    worksheet2.Cells[1, 2].Value = "Ram";
                    worksheet2.Cells[1, 3].Value = "MIPS";
                    worksheet2.Cells[1, 4].Value = "Bandwidth";
                    worksheet2.Cells[1, 5].Value = "Number of Cores";
                    worksheet2.Cells[1, 6].Value = "Size";
                    worksheet2.Cells[1, 7].Value = "GeoLocation_Longitute";
                    worksheet2.Cells[1, 8].Value = "GeoLocation_Latitude";
                    worksheet2.Cells[1, 9].Value = "VM_Id";
                    worksheet2.Cells[1, 10].Value = "DataCenter_Name";
                    worksheet2.Cells[1, 11].Value = "Host_Id";

                    int _rowNumber = 2;

                    //Add existing users
                    worksheet2.Cells[_rowNumber, 1].Value = list[0].CloudBroker.SelectedVM.Name;
                    worksheet2.Cells[_rowNumber, 2].Value = list[0].CloudBroker.SelectedVM.RAM;
                    worksheet2.Cells[_rowNumber, 3].Value = list[0].CloudBroker.SelectedVM.MIPS;
                    worksheet2.Cells[_rowNumber, 4].Value = list[0].CloudBroker.SelectedVM.BW;
                    worksheet2.Cells[_rowNumber, 5].Value = list[0].CloudBroker.SelectedVM.NumberOfPes;
                    worksheet2.Cells[_rowNumber, 6].Value = list[0].CloudBroker.SelectedVM.Size;
                    worksheet2.Cells[_rowNumber, 7].Value = list[0].CloudBroker.SelectedVM.GeoLocation.getLongitude();
                    worksheet2.Cells[_rowNumber, 8].Value = list[0].CloudBroker.SelectedVM.GeoLocation.getLatitude();
                    worksheet2.Cells[_rowNumber, 9].Value = list[0].CloudBroker.SelectedVM.ID;
                    worksheet2.Cells[_rowNumber, 10].Value = list[0].CloudBroker.SelectedDataCenter.Name;
                    worksheet2.Cells[_rowNumber, 11].Value = list[0].CloudBroker.SelectedHost.ID;

                    worksheet2.Column(1).AutoFit();
                    worksheet2.Column(2).AutoFit();
                    worksheet2.Column(3).AutoFit();
                    worksheet2.Column(4).AutoFit();
                    worksheet2.Column(5).AutoFit();
                    worksheet2.Column(6).AutoFit();
                    worksheet2.Column(7).AutoFit();
                    worksheet2.Column(8).AutoFit();
                    worksheet2.Column(9).AutoFit();
                    worksheet2.Column(10).AutoFit();
                    worksheet2.Column(11).AutoFit();

                    #endregion Cloud_Characteristics

                    #region Cloud_Characteristics

                    ExcelWorksheet worksheet3 = package.Workbook.Worksheets.Add("Resources_of_Host");

                    // --------- Data and styling goes here -------------- //

                    worksheet3.Cells[1, 1].Value = "Host_Id";
                    worksheet3.Cells[1, 2].Value = "Ram";
                    worksheet3.Cells[1, 3].Value = "MIPS";
                    worksheet3.Cells[1, 4].Value = "Number of Cores";
                    worksheet3.Cells[1, 5].Value = "Storage";
                    worksheet3.Cells[1, 6].Value = "DataCenter";
                    worksheet3.Cells[1, 7].Value = "Number_Of_VMs";

                    int ResourceRowNumber = 2;

                    foreach (var item in CloudSimulator.ResourceMonitoring)
                    {
                        worksheet3.Cells[ResourceRowNumber, 1].Value = "";
                        worksheet3.Cells[ResourceRowNumber, 2].Value = item.RamManager.AvailableRam;
                        worksheet3.Cells[ResourceRowNumber, 3].Value = item.MIPSManager.AvailableMIPS;
                        worksheet3.Cells[ResourceRowNumber, 4].Value = item.PeManager.AvailablePE;
                        worksheet3.Cells[ResourceRowNumber, 5].Value = item.StorageManager.AvailableStorage;
                        worksheet3.Cells[ResourceRowNumber, 6].Value = "";
                        worksheet3.Cells[ResourceRowNumber, 7].Value = "";
                        ResourceRowNumber++;
                    }

                    //Add existing users

                    worksheet3.Column(1).AutoFit();
                    worksheet3.Column(2).AutoFit();
                    worksheet3.Column(3).AutoFit();
                    worksheet3.Column(4).AutoFit();
                    worksheet3.Column(5).AutoFit();
                    worksheet3.Column(6).AutoFit();
                    worksheet3.Column(7).AutoFit();

                    #endregion Cloud_Characteristics

                    // Set some document properties
                    if (list[0].FogDevice != null)
                    {
                        package.Workbook.Properties.Title = "Results_Fog";
                    }
                    else
                    {
                        package.Workbook.Properties.Title = "Results_Cloud";
                    }

                    package.Workbook.Properties.Author = "Salman And Faizan";
                    package.Workbook.Properties.Company = "Simulation";

                    // save our new workbook and we are done!
                    package.Save();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        public static void CreateExcelSheetForFog(List<Results> list, List<FogTimes> fogTimings, List<TupleTimes> tupleTiming)
        { //, List<ActingServer> ActingFog
            try
            {
                if (list.Count > 0)
                {
                    MoveFogExcelFile();
                    var file = new FileInfo(Path.Combine(FileInformation.GetDirectory(), FileInformation.ExcelFileName + "_Fog.xlsx"));
                    List<FinalResults> finalResults = new List<FinalResults>();
                    CloudBroker CloudBroker = list.Where(a => a.CloudBroker != null).Select(a => a.CloudBroker).FirstOrDefault();// .CloudBroker;
                    FogBroker fogBroker = list.Where(a => a.FogBroker != null).Select(a => a.FogBroker).FirstOrDefault();

                    // Create the file using the FileInfo object

                    // Create the package and make sure you wrap it in a using statement
                    using (var package = new ExcelPackage(file))
                    {
                        #region Results_Fog

                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Results_Fog");

                        // First of all the first row
                        worksheet.Cells[1, 1].Value = "Tuple_ID";
                        worksheet.Cells[1, 2].Value = "Tuple_Name";
                        worksheet.Cells[1, 3].Value = "Tuple_GeoLocation";
                        worksheet.Cells[1, 4].Value = "Tuple_DataType";
                        worksheet.Cells[1, 5].Value = "Tuple_ElapsedTime";
                        worksheet.Cells[1, 6].Value = "Tuple_InitiatesTime";
                        worksheet.Cells[1, 7].Value = "TotalTimeTaken";
                        worksheet.Cells[1, 8].Value = "Tuple_Reversed";
                        worksheet.Cells[1, 9].Value = "Tuple_IsServerFound";
                        worksheet.Cells[1, 10].Value = "DeviceType";
                        worksheet.Cells[1, 11].Value = "DataCenter_Name";
                        worksheet.Cells[1, 12].Value = "DataCenter_GeoLocation_Longitute";
                        worksheet.Cells[1, 13].Value = "DataCenter_GeoLocation_Latitute";
                        worksheet.Cells[1, 14].Value = "Cloud_DistanceFromServer";
                        worksheet.Cells[1, 15].Value = "DataCenter_DistanceFromTuple";
                        worksheet.Cells[1, 16].Value = "IsCloudServed";
                        worksheet.Cells[1, 17].Value = "DataCenter";
                        worksheet.Cells[1, 18].Value = "Host";
                        worksheet.Cells[1, 19].Value = "Tuple_PropogationTime";
                        worksheet.Cells[1, 20].Value = "InternalProcessingTime";
                        worksheet.Cells[1, 21].Value = "Serving_Fog_Level";
                        worksheet.Cells[1, 22].Value = "IsServedByFC_Cloud";

                        int rowNumber = 2;
                        FinalResults Fr;
                        //Add existing users
                        foreach (var item in list)
                        {
                            if (item != null)
                            {
                                try
                                {
                                    Fr = new FinalResults();

                                    worksheet.Cells[rowNumber, 1].Value = item.Tuple.ID;
                                    worksheet.Cells[rowNumber, 2].Value = item.Tuple.Name;
                                    worksheet.Cells[rowNumber, 3].Value = item.Tuple.GeoLocation.getLongitude() + "," + item.Tuple.GeoLocation.getLatitude();
                                    Fr.DataType = item.Tuple.DataType;
                                    worksheet.Cells[rowNumber, 4].Value = Fr.DataType;

                                    worksheet.Cells[rowNumber, 5].Value = item.ElapsedTime;
                                    worksheet.Cells[rowNumber, 6].Value = item.InitiatesTime;
                                    Fr.TimeTaken = item.ElapsedTime - Convert.ToDouble(item.InitiatesTime) < 0 ? 0 : item.ElapsedTime - Convert.ToDouble(item.InitiatesTime);
                                    worksheet.Cells[rowNumber, 7].Value = Fr.TimeTaken;
                                    worksheet.Cells[rowNumber, 8].Value = item.Tuple.IsReversed;
                                    worksheet.Cells[rowNumber, 9].Value = item.Tuple.IsServerFound;
                                    worksheet.Cells[rowNumber, 10].Value = item.Tuple.DeviceType;
                                    worksheet.Cells[rowNumber, 11].Value = item.FogBroker != null && item.FogBroker.SelectedFogDevice.Name != null ? item.FogBroker.SelectedFogDevice.Name : "Null";
                                    worksheet.Cells[rowNumber, 12].Value = item.FogBroker != null ? item.FogBroker.SelectedFogDevice.GeoLocation.getLongitude() :
                                                           item.CloudBroker != null ? item.CloudBroker.SelectedDataCenter.DatacenterCharacteristics.GeoLocation.getLongitude() : -1;
                                    worksheet.Cells[rowNumber, 13].Value = item.FogBroker != null ? item.FogBroker.SelectedFogDevice.GeoLocation.getLatitude() :
                                                           item.CloudBroker != null ? item.CloudBroker.SelectedDataCenter.DatacenterCharacteristics.GeoLocation.getLatitude() : -1;
                                    worksheet.Cells[rowNumber, 14].Value = 0.0;
                                    worksheet.Cells[rowNumber, 15].Value = item.FogBroker != null ? item.FogBroker.SelectedFogDevice.DistanceFromTuple : -1;

                                    Fr.IsServedByCloud = item.FogBroker != null ? item.FogBroker.Tuple.IsCloudServed : item.CloudBroker != null ? true : false;
                                    worksheet.Cells[rowNumber, 16].Value = Fr.IsServedByCloud;
                                    worksheet.Cells[rowNumber, 17].Value = item.CloudBroker != null ? item.CloudBroker.SelectedDataCenter.Name : null;
                                    worksheet.Cells[rowNumber, 18].Value = item.CloudBroker != null ? item.CloudBroker.SelectedHost.ID : -1;

                                    Fr.PropagationTime = item.Link != null ? item.Link.Propagationtime : -1;
                                    worksheet.Cells[rowNumber, 19].Value = Fr.PropagationTime;

                                    Fr.InternalProcessingTime = item.Tuple.InternalProcessingTime;
                                    worksheet.Cells[rowNumber, 20].Value = Fr.InternalProcessingTime;
                                    worksheet.Cells[rowNumber, 21].Value = item.Tuple.FogLevelServed;
                                    Fr.IsServedByFC_Cloud = item.Tuple.IsServedByFC_Cloud;
                                    worksheet.Cells[rowNumber, 22].Value = Fr.IsServedByFC_Cloud;
                                    rowNumber++;

                                    finalResults.Add(Fr);
                                }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                                catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                                {
                                    //throw;
                                }

                            }
                        }

                        // Fit the columns according to its content
                        worksheet.Column(1).AutoFit();
                        worksheet.Column(2).AutoFit();
                        worksheet.Column(3).AutoFit();
                        worksheet.Column(4).AutoFit();
                        worksheet.Column(5).AutoFit();
                        worksheet.Column(6).AutoFit();
                        worksheet.Column(7).AutoFit();
                        worksheet.Column(8).AutoFit();
                        worksheet.Column(9).AutoFit();
                        worksheet.Column(10).AutoFit();
                        worksheet.Column(11).AutoFit();
                        worksheet.Column(12).AutoFit();
                        worksheet.Column(13).AutoFit();
                        worksheet.Column(14).AutoFit();
                        worksheet.Column(15).AutoFit();
                        worksheet.Column(16).AutoFit();
                        worksheet.Column(17).AutoFit();
                        worksheet.Column(18).AutoFit();
                        worksheet.Column(19).AutoFit();
                        worksheet.Column(20).AutoFit();
                        worksheet.Column(21).AutoFit();
                        worksheet.Column(22).AutoFit();

                        #endregion Results_Fog

                        // add a new worksheet to the empty workbook

                        #region Fog_Characteristics

                        ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add("Fog_Characteristics");


                        // --------- Data and styling goes here -------------- //

                        worksheet2.Cells[1, 1].Value = "Fog_Name";
                        worksheet2.Cells[1, 2].Value = "Ram";
                        worksheet2.Cells[1, 3].Value = "MIPS";
                        worksheet2.Cells[1, 4].Value = "UpBandwidth";
                        worksheet2.Cells[1, 5].Value = "DownBandwidth";
                        worksheet2.Cells[1, 6].Value = "Number of Cores";
                        worksheet2.Cells[1, 7].Value = "Size";
                        worksheet2.Cells[1, 8].Value = "Storage";
                        worksheet2.Cells[1, 9].Value = "Status";
                        worksheet2.Cells[1, 10].Value = "DataType";
                        worksheet2.Cells[1, 11].Value = "GeoLocation_Longitute";
                        worksheet2.Cells[1, 12].Value = "GeoLocation_Latitute";
                        worksheet2.Cells[1, 13].Value = "Id";
                        int _rowNumber = 2;

                        if (fogBroker != null)
                        {
                            foreach (var item1 in fogBroker.FogList)
                            {
                                try
                                {
                                    worksheet2.Cells[_rowNumber, 1].Value = item1.Name;
                                    worksheet2.Cells[_rowNumber, 2].Value = item1.RAM;
                                    worksheet2.Cells[_rowNumber, 3].Value = item1.MIPS;
                                    worksheet2.Cells[_rowNumber, 4].Value = item1.UpBW;
                                    worksheet2.Cells[_rowNumber, 5].Value = item1.DownBW;
                                    worksheet2.Cells[_rowNumber, 6].Value = item1.NumberOfPes;
                                    worksheet2.Cells[_rowNumber, 7].Value = item1.Size;
                                    worksheet2.Cells[_rowNumber, 8].Value = item1.Storage;
                                    worksheet2.Cells[_rowNumber, 9].Value = item1.IsActive;
                                    worksheet2.Cells[_rowNumber, 10].Value = item1.DataType;
                                    worksheet2.Cells[_rowNumber, 11].Value = item1.GeoLocation.getLongitude();
                                    worksheet2.Cells[_rowNumber, 12].Value = item1.GeoLocation.getLatitude();
                                    worksheet2.Cells[_rowNumber, 13].Value = item1.ID;
                                    _rowNumber++;
                                }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                                catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                                {
                                }

                            }
                        }
                        if (CloudBroker != null)
                        {
                            worksheet2.Cells[_rowNumber, 1].Value = CloudBroker.SelectedDataCenter.Name;
                            worksheet2.Cells[_rowNumber, 2].Value = CloudBroker.SelectedVM.RAM;
                            worksheet2.Cells[_rowNumber, 3].Value = CloudBroker.SelectedVM.MIPS;
                            worksheet2.Cells[_rowNumber, 4].Value = CloudBroker.SelectedVM.BW;
                            worksheet2.Cells[_rowNumber, 5].Value = "";
                            worksheet2.Cells[_rowNumber, 6].Value = CloudBroker.SelectedVM.NumberOfPes;
                            worksheet2.Cells[_rowNumber, 7].Value = CloudBroker.SelectedVM.Size;
                            worksheet2.Cells[_rowNumber, 8].Value = "";
                            worksheet2.Cells[_rowNumber, 9].Value = true;
                            worksheet2.Cells[_rowNumber, 10].Value = CloudBroker.SelectedVM.DataType;
                            worksheet2.Cells[_rowNumber, 11].Value = CloudBroker.SelectedVM.GeoLocation.getLongitude();
                            worksheet2.Cells[_rowNumber, 12].Value = CloudBroker.SelectedVM.GeoLocation.getLatitude();
                            worksheet2.Cells[_rowNumber, 13].Value = CloudBroker.SelectedVM.ID;
                            _rowNumber++;
                        }

                        worksheet2.Column(1).AutoFit();
                        worksheet2.Column(2).AutoFit();
                        worksheet2.Column(3).AutoFit();
                        worksheet2.Column(4).AutoFit();
                        worksheet2.Column(5).AutoFit();
                        worksheet2.Column(6).AutoFit();
                        worksheet2.Column(7).AutoFit();
                        worksheet2.Column(8).AutoFit();
                        worksheet2.Column(9).AutoFit();
                        worksheet2.Column(10).AutoFit();
                        worksheet2.Column(11).AutoFit();
                        worksheet2.Column(12).AutoFit();
                        worksheet2.Column(13).AutoFit();

                        #endregion Fog_Characteristics

                        #region Tuple_Characteristics

                        ExcelWorksheet worksheet3 = package.Workbook.Worksheets.Add("Tuple_Characteristics");

                        // --------- Data and styling goes here -------------- //

                        worksheet3.Cells[1, 1].Value = "Tuple_Name";
                        worksheet3.Cells[1, 2].Value = "Ram";
                        worksheet3.Cells[1, 3].Value = "MIPS";
                        worksheet3.Cells[1, 4].Value = "Bandwidth";
                        worksheet3.Cells[1, 5].Value = "Number of Cores";
                        worksheet3.Cells[1, 6].Value = "Size";
                        worksheet3.Cells[1, 7].Value = "DataType";
                        worksheet3.Cells[1, 8].Value = "GeoLocation_Longitute";
                        worksheet3.Cells[1, 9].Value = "GeoLocation_Latitude";
                        worksheet3.Cells[1, 10].Value = "Id";

                        int __rowNumber = 2;
                        try
                        {
                            foreach (var item1 in list)
                            {
                                if (item1.Tuple != null)
                                {
                                    try
                                    {
                                        worksheet3.Cells[__rowNumber, 1].Value = string.IsNullOrEmpty(item1.Tuple.Name) ? "Tuple" : item1.Tuple.Name;
                                        worksheet3.Cells[__rowNumber, 2].Value = item1.Tuple.RAM;
                                        worksheet3.Cells[__rowNumber, 3].Value = item1.Tuple.MIPS;
                                        worksheet3.Cells[__rowNumber, 4].Value = item1.Tuple.BW;
                                        worksheet3.Cells[__rowNumber, 5].Value = item1.Tuple.NumberOfPes;
                                        worksheet3.Cells[__rowNumber, 6].Value = item1.Tuple.Size;
                                        worksheet3.Cells[__rowNumber, 7].Value = item1.Tuple.DataType;
                                        worksheet3.Cells[__rowNumber, 8].Value = item1.Tuple.GeoLocation.getLongitude();
                                        worksheet3.Cells[__rowNumber, 9].Value = item1.Tuple.GeoLocation.getLatitude();
                                        worksheet3.Cells[__rowNumber, 10].Value = item1.Tuple.ID;
                                        __rowNumber++;
                                    }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                                    catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                                    {
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                        worksheet3.Column(1).AutoFit();
                        worksheet3.Column(2).AutoFit();
                        worksheet3.Column(3).AutoFit();
                        worksheet3.Column(4).AutoFit();
                        worksheet3.Column(5).AutoFit();
                        worksheet3.Column(6).AutoFit();
                        worksheet3.Column(7).AutoFit();
                        worksheet3.Column(8).AutoFit();
                        worksheet3.Column(9).AutoFit();
                        worksheet3.Column(10).AutoFit();

                        #endregion Tuple_Characteristics

                        #region Fog_Power_Values

                        ExcelWorksheet worksheet4 = package.Workbook.Worksheets.Add("Fog_Power_Values");
                        //used to find the average value of fog consumption
                        List<Powervalues> pwrVal = pwrVal = new List<Powervalues>();
                        Powervalues pval;
                        // --------- Data and styling goes here -------------- //

                        for (int i = 0; i < fogBroker.FogList.Count(); i++)
                        {
                            worksheet4.Cells[1, i + 1].Value = fogBroker.FogList[i].Name;
                        }

                        for (int i = 0; i < fogBroker.FogList.Count; i++)
                        {
                            try
                            {
                                int powerRowNumber = 2;
                                int numFS = 0;
                                double value = 0;
                                for (int j = 0; j < fogBroker.FogList[i].PowerConsumption.Count; j++)
                                {
                                    worksheet4.Cells[powerRowNumber, i + 1].Value = fogBroker.FogList[i].PowerConsumption[j].PowerValue;
                                    value += fogBroker.FogList[i].PowerConsumption[j].PowerValue;
                                    powerRowNumber++;
                                    numFS++;
                                }
                                pval = new Powervalues();
                                NodeConsumption cNcons = new NodeConsumption();

                                pval.Pval = value;
                                pval.ServerName = fogBroker.FogList[i].Name;
                                pval.count = numFS;
                                pval.average = value != 0 ? Math.Round((value / numFS), 3) : 0;
                                pwrVal.Add(pval);
                                worksheet4.Column(i + 1).AutoFit();
                            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                            {
                            }
                        }

                        #endregion Fog_Power_Values

                        #region Fog_Timings

                        ExcelWorksheet worksheet5 = package.Workbook.Worksheets.Add("Fog_Timings");

                        worksheet5.Cells[1, 1].Value = "Server Name";
                        worksheet5.Cells[1, 2].Value = "Job Name";
                        worksheet5.Cells[1, 3].Value = "Task_Arrival_Time";
                        worksheet5.Cells[1, 4].Value = "Free_From_Task";
                        worksheet5.Cells[1, 5].Value = "TimeDifference in MS";
                        worksheet5.Cells[1, 6].Value = "Fog Consumption";
                        worksheet5.Cells[1, 7].Value = "Fog Consumption Percentage";


                        int rowNumberForTiming = 2;
                        string previousName = "";
                        PowerUtility.FillNumRange();
                        var fgT = fogTimings.OrderBy(x => x.FogName.Split('-')[1]).OrderBy(a => a.TaskArrival);
                        foreach (var item in fgT)
                        {
                            try
                            {
                                if (item.FogName != previousName && rowNumberForTiming > 2)
                                {
                                    previousName = item.FogName;
                                    rowNumberForTiming++;
                                }
                                worksheet5.Cells[rowNumberForTiming, 1].Value = item.FogName;
                                worksheet5.Cells[rowNumberForTiming, 2].Value = item.TupleName;
                                worksheet5.Cells[rowNumberForTiming, 3].Value = item.TaskArrival;
                                worksheet5.Cells[rowNumberForTiming, 4].Value = item.FreeTime;
                                worksheet5.Cells[rowNumberForTiming, 5].Value = item.TimeDifference;
                                worksheet5.Cells[rowNumberForTiming, 6].Value = item.Consumption;
                                worksheet5.Cells[rowNumberForTiming, 7].Value = item.ConsumptionPer;
                                rowNumberForTiming++;
                            }
                            catch (Exception ex)
                            {
                                throw new ArgumentException(ex.Message);
                            }
                        }

                        worksheet5.Column(1).AutoFit();
                        worksheet5.Column(2).AutoFit();
                        worksheet5.Column(3).AutoFit();
                        worksheet5.Column(4).AutoFit();
                        worksheet5.Column(5).AutoFit();
                        worksheet5.Column(6).AutoFit();
                        worksheet5.Column(7).AutoFit();

                        #endregion Fog_Timings

                        #region Tuple_Timings

                        ExcelWorksheet worksheet6 = package.Workbook.Worksheets.Add("Tuple_Timings");

                        worksheet6.Cells[1, 1].Value = "Tuple_Name";
                        worksheet6.Cells[1, 2].Value = "Task_Arrival_Time";
                        worksheet6.Cells[1, 3].Value = "Task_Departure_Time";


                        int rowNumberForTimingT = 2;
                        foreach (var item in tupleTiming.OrderBy(x => x.Name.Split('-')[1]))
                        {
                            try
                            {
                                worksheet6.Cells[rowNumberForTimingT, 1].Value = item.Name;
                                worksheet6.Cells[rowNumberForTimingT, 2].Value = item.TupleArrival;
                                worksheet6.Cells[rowNumberForTimingT, 3].Value = item.TupleDeparture;
                                rowNumberForTimingT++;
                            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                            {
                            }

                        }

                        worksheet6.Column(1).AutoFit();
                        worksheet6.Column(2).AutoFit();
                        worksheet6.Column(3).AutoFit();

                        #endregion Tuple_Timings

                        #region Servers and Stats

                        ExcelWorksheet worksheet7 = package.Workbook.Worksheets.Add("Acting Servers");
                        worksheet7.Cells[1, 1].Value = "Server Name";
                        worksheet7.Cells[1, 2].Value = "Total Jobs";
                        worksheet7.Cells[1, 3].Value = "StartTime";
                        worksheet7.Cells[1, 4].Value = "EndTime";
                        worksheet7.Cells[1, 5].Value = "FogConsumption";
                        worksheet7.Cells[1, 6].Value = "FogConsPercent";
                        worksheet7.Cells[1, 7].Value = "TimeKey";

                        int rowNumberActingServer = 2;
                        List<IlistResult> ilistResult = new List<IlistResult>();
                        long FogCount = fogTimings.OrderBy(a => a.FogName).GroupBy(a => a.FogName).ToList().Count();
                        var Servers = fogTimings.GroupBy(a => a.FogName);
                        List<FogTimes> AFdata = fogTimings.OrderBy(a => a.FogName.Split('-')[1]).ToList();//.OrderBy(a => a.TaskArrival);
                                                                                                          // string check = AFdata[0].FogName.Split('-')[0];

                        foreach (var item in Servers)
                        {
                            IlistResult iLresult = new IlistResult();
                            string serverName = item.Key.ToString();
                            iLresult.iListFog = AFdata.Where(a => a.FogName == serverName).Select(a => a).ToList();
                            ilistResult.Add(iLresult);
                        };
                        try
                        {
                            foreach (var item in ilistResult)
                            {
                                List<IFogTimeCons> query = item.iListFog.GroupBy(x => new { Convert.ToDateTime(x.TaskArrival).Hour, Convert.ToDateTime(x.TaskArrival).Minute, Convert.ToDateTime(x.TaskArrival).Second })
                                         .Select(g => new IFogTimeCons
                                         {
                                             TimeKey = g.Key.ToString(),
                                             JobCount = g.Select(a => a.TupleName).Count(),
                                             JobsList = g.Select(a => a.TupleName).ToList(),
                                             FogConsumption = g.Select(a => a.Consumption).ToList().Sum(),
                                             FogConsPercent = g.Select(a => a.ConsumptionPer).Sum(),
                                             ServerName = g.Select(a => a.FogName).FirstOrDefault().ToString(),
                                             StartTime = g.Select(a => a.TaskArrival).FirstOrDefault(),
                                             EndTime = g.Select(a => a.FreeTime).LastOrDefault(),
                                         }).OrderBy(a => a.TimeKey).ToList();
                                foreach (var _item in query)
                                {
                                    worksheet7.Cells[rowNumberActingServer, 1].Value = _item.ServerName;
                                    worksheet7.Cells[rowNumberActingServer, 2].Value = _item.JobCount;
                                    worksheet7.Cells[rowNumberActingServer, 3].Value = _item.StartTime;
                                    worksheet7.Cells[rowNumberActingServer, 4].Value = _item.EndTime;
                                    worksheet7.Cells[rowNumberActingServer, 5].Value = _item.FogConsumption;
                                    worksheet7.Cells[rowNumberActingServer, 6].Value = _item.FogConsPercent;
                                    worksheet7.Cells[rowNumberActingServer, 7].Value = _item.TimeKey;
                                    rowNumberActingServer++;
                                }
                            }
                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        {
                        }

                        worksheet7.Column(1).AutoFit();
                        worksheet7.Column(2).AutoFit();
                        worksheet7.Column(3).AutoFit();
                        worksheet7.Column(4).AutoFit();
                        worksheet7.Column(5).AutoFit();
                        worksheet7.Column(6).AutoFit();
                        worksheet7.Column(7).AutoFit();

                        #endregion

                        #region Filtered results
                        ExcelWorksheet worksheet8 = package.Workbook.Worksheets.Add("Filtered Results");
                        worksheet8.Cells[1, 1].Value = "TotalSimulationTime(Seconds)";
                        worksheet8.Cells[1, 2].Value = "ToTalServerUsed";
                        worksheet8.Cells[1, 3].Value = "StartTime";
                        worksheet8.Cells[1, 4].Value = "EndTime";
                        worksheet8.Cells[1, 5].Value = "TotalAverageTimeTaken(MS)";
                        worksheet8.Cells[1, 6].Value = "TotalAveragePropagationTaken";
                        worksheet8.Cells[1, 7].Value = "TotalAverageInternalProccTime";
                        worksheet8.Cells[1, 8].Value = "JobsServedbyCloud";
                        worksheet8.Cells[1, 9].Value = "JobsServedbyEdge";
                        worksheet8.Cells[1, 10].Value = "GrandPowerConsumption";
                        worksheet8.Cells[1, 11].Value = "TotalReversedJobs";
                        worksheet8.Cells[1, 12].Value = "DroppedJobs";
                        worksheet8.Cells[1, 13].Value = "DataType";
                        worksheet8.Cells[1, 14].Value = "DataType_AvgInternalProcessingTime";
                        worksheet8.Cells[1, 15].Value = "DataType_AvgPropagationTime";
                        worksheet8.Cells[1, 16].Value = "DataType_TotalJobsServed";
                        worksheet8.Cells[1, 17].Value = "DataType_TotlaServerUsedBy";
                        worksheet8.Cells[1, 18].Value = "DataType_Avg_TimeTaken";
                        worksheet8.Cells[1, 19].Value = "DataType_Avg_QueueDelay";

                        List<string> servers = null;
                        try
                        {
                            int worksheetNo = 19;
                            for (int i = 1; i <= pwrVal.Select(a => a.ServerName).ToList().Count(); i++)
                            {
                                servers = pwrVal.Select(a => a.ServerName).Take(i).ToList();

                                worksheet8.Cells[1, i + worksheetNo].Value = servers[i - 1];
                            }
                            int nextRow = worksheetNo + pwrVal.Select(a => a.ServerName).ToList().Count();
                            List<NodeConsumption> nodCons = new List<NodeConsumption>();
                            NodeConsumption nc;
                            foreach (var item in fogBroker.FogList)
                            {
                                nc = new NodeConsumption();
                                nc.DataType = item.DataType;
                                nc.powrCons = item.PowerConsumption;
                                nc.average = item.PowerConsumption.Count() != 0 ? item.PowerConsumption.Where(a => a.PowerValue > 0).Select(a => a.PowerValue).Sum() / item.PowerConsumption.Where(a => a.PowerValue > 0).Select(a => a.PowerValue).Count() : 0;
                                nodCons.Add(nc);
                            }

                            var group = nodCons.GroupBy(a => a.DataType).ToList();
                            List<NodeConsumption_sub> NodeConsSub = new List<NodeConsumption_sub>();
                            NodeConsumption_sub sub;
                            foreach (var item in group)
                            {
                                sub = new NodeConsumption_sub();
                                sub.DataType = item.Key.ToString();
                                sub.average = nodCons.Where(a => a.DataType == sub.DataType && a.average != 0).Select(a => a.average).Sum() / nodCons.Where(a => a.DataType == sub.DataType && a.average != 0).Select(a => a.average).Count();
                                NodeConsSub.Add(sub);
                            }
                            for (int i = 1; i <= NodeConsSub.Count; i++)
                            {
                                worksheet8.Cells[1, i + nextRow].Value = NodeConsSub[i - 1].DataType;
                            }
                            var tupleDT = list.GroupBy(x => x.Tuple.DataType)
                                 .Select(g => new TupleDT
                                 {
                                     DataType = g.Key.ToString(),
                                     DataType_InternalProcessingTime = Math.Round(g.Select(a => a.Tuple.InternalProcessingTime).Sum() / g.Select(a => a.Tuple.InternalProcessingTime).Count(), 3),
                                     DataType_PropagationTime = Math.Round(g.Select(a => a.Link.Propagationtime).Sum() / g.Select(a => a.Link.Propagationtime).Count(), 3),
                                     DataType_TotalJobsServed = g.Select(a => a.Tuple.ID).Count(),
                                     DataType_TotalServerUsedBy = g.Where(a => a.FogBroker != null).Select(a => a.FogBroker.SelectedFogDevice).Count(),
                                     DataType_TimeTaken = Math.Round(finalResults.Where(a => a.DataType == g.Key.ToString()).Select(a => a.TimeTaken).Sum() / finalResults.Where(a => a.DataType == g.Key.ToString()).Select(a => a.TimeTaken).Count(), 3),
                                     DataType_QueueDelay = Math.Round(list.Where(a => a.Tuple.DataType == g.Key.ToString()).Select(a => a.Tuple.QueueDelay).Sum() / list.Where(a => a.Tuple.DataType == g.Key.ToString()).Select(a => a.Tuple.QueueDelay).Count(), 3),
                                 }).OrderBy(a => a.DataType).ToList();
                            List<TupleDT> querytupleDT = tupleDT;


                            var tupleNT = list.GroupBy(x => x.Tuple.DeviceType)
                                .Select(g => new TupleNT
                                {
                                    NodeType = g.Key.ToString(),
                                    NodeType_InternalProcessingTime = Math.Round(g.Select(a => a.Tuple.InternalProcessingTime).Sum() / g.Select(a => a.Tuple.InternalProcessingTime).Count(), 3),
                                    NodeType_PropagationTime = Math.Round(g.Select(a => a.Link.Propagationtime).Sum() / g.Select(a => a.Link.Propagationtime).Count(), 3),
                                    NodeType_TotalJobsServed = g.Select(a => a.Tuple.ID).Count(),
                                    NodeType_TotalServerUsedBy = g.Where(a => a.FogBroker != null).Select(a => a.FogBroker.SelectedFogDevice).Count(),
                                }).OrderBy(a => a.NodeType).ToList();

                            List<TupleNT> querytupleNT = tupleNT;

                            int rowNumberFiltered = 2;
                            // Simulation time ?
                            string StartTime = fogTimings.OrderBy(a => a.TaskArrival).Select(a => a.TaskArrival).FirstOrDefault();
                            string EndTime = fogTimings.OrderByDescending(a => a.FreeTime).Select(a => a.FreeTime).FirstOrDefault();

                            worksheet8.Cells[2, 1].Value = (Convert.ToDateTime(EndTime) - Convert.ToDateTime(StartTime)).TotalSeconds;
                            worksheet8.Cells[2, 2].Value = FogCount;
                            worksheet8.Cells[2, 3].Value = StartTime;
                            worksheet8.Cells[2, 4].Value = EndTime;
                            worksheet8.Cells[2, 5].Value = Math.Round(finalResults.Select(a => a.TimeTaken).Sum() / finalResults.Select(a => a.TimeTaken).Count(), 3);//finding averge;
                            worksheet8.Cells[2, 6].Value = Math.Round(finalResults.Select(a => a.PropagationTime).Sum() / finalResults.Select(a => a.PropagationTime).Count(), 3);
                            worksheet8.Cells[2, 7].Value = Math.Round(finalResults.Select(a => a.InternalProcessingTime).Sum() / finalResults.Select(a => a.InternalProcessingTime).Count(), 3);
                            worksheet8.Cells[2, 8].Value = finalResults.Where(a => a.IsServedByCloud == true).Count();
                            worksheet8.Cells[2, 9].Value = finalResults.Where(a => a.IsServedByCloud == false).Count();
                            worksheet8.Cells[2, 10].Value = pwrVal != null ? Math.Round(pwrVal.Where(a => a.average != 0).Select(a => a.average).Sum() / pwrVal.Where(a => a.average != 0).Select(a => a.average).Count(), 3) : 0;
                            worksheet8.Cells[2, 11].Value = list.Where(a => a.Tuple.IsReversed == true && a.Tuple.IsCloudServed == false).Count() + " IsReversed false " + list.Where(a => a.Tuple.IsReversed == true).Count();
                            worksheet8.Cells[2, 12].Value = 0;
                            int countNum = querytupleDT.Count();

                            foreach (var item in querytupleDT)
                            {
                                worksheet8.Cells[rowNumberFiltered, 13].Value = item.DataType;
                                worksheet8.Cells[rowNumberFiltered, 14].Value = item.DataType_InternalProcessingTime;
                                worksheet8.Cells[rowNumberFiltered, 15].Value = item.DataType_PropagationTime;
                                worksheet8.Cells[rowNumberFiltered, 16].Value = item.DataType_TotalJobsServed;
                                worksheet8.Cells[rowNumberFiltered, 17].Value = item.DataType_TotalServerUsedBy;
                                worksheet8.Cells[rowNumberFiltered, 18].Value = item.DataType_TimeTaken;
                                worksheet8.Cells[rowNumberFiltered, 19].Value = item.DataType_QueueDelay;

                                rowNumberFiltered++;
                            }
                            for (int i = 1; i <= pwrVal.Select(a => a.ServerName).ToList().Count; i++)
                            {
                                List<double> avg = pwrVal.Select(a => a.average).Take(i).ToList();
                                worksheet8.Cells[2, i + worksheetNo].Value = avg[i - 1];
                            }

                            for (int i = 1; i <= NodeConsSub.Count; i++)
                            {
                                worksheet8.Cells[rowNumberFiltered, i + nextRow].Value = Math.Round(NodeConsSub[i - 1].average, 3);
                            }

                            rowNumberFiltered++;
                        }
                        catch (Exception)
                        { }


                        worksheet8.Column(1).AutoFit();
                        worksheet8.Column(2).AutoFit();
                        worksheet8.Column(3).AutoFit();
                        worksheet8.Column(4).AutoFit();
                        worksheet8.Column(5).AutoFit();
                        worksheet8.Column(6).AutoFit();
                        worksheet8.Column(7).AutoFit();
                        worksheet8.Column(8).AutoFit();
                        worksheet8.Column(9).AutoFit();
                        worksheet8.Column(10).AutoFit();
                        #endregion

                        // Set some document properties

                        package.Workbook.Properties.Title = "Results_Fog";

                        package.Workbook.Properties.Author = "Salman and Faizan";
                        package.Workbook.Properties.Company = "Simulation";

                        // save our new workbook and we are done!
                        package.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            //Move Excel file

        }
        public static void CreateExcelSheetEdgeFog(List<Results> edgelist, List<FogTimes> fogTimings, List<TupleTimes> tupleTiming, List<Models.Tuple> DropedtupleList)
        { //, List<ActingServer> ActingFog
            try
            {
                MoveEdgeExcelFile();
                var file = new FileInfo(Path.Combine(FileInformation.GetDirectory(), FileInformation.ExcelFileName + "_Edge.xlsx"));
                List<FinalResults> finalResults = new List<FinalResults>();
                CloudBroker CloudBroker = edgelist.Where(a => a.CloudBroker != null).Select(a => a.CloudBroker).FirstOrDefault();// .CloudBroker;
                FogBroker fogBroker = edgelist.Where(a => a.FogBroker != null).Select(a => a.FogBroker).FirstOrDefault();
                var locTup = edgelist.Select(a => a.FogBroker.Tuple).ToList();
                // Create the file using the FileInfo object

                // Create the package and make sure you wrap it in a using statement
                using (var package = new ExcelPackage(file))
                {
                    #region Results_Edge

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Results_Edge");

                    // First of all the first row
                    worksheet.Cells[1, 1].Value = "Tuple_ID";
                    worksheet.Cells[1, 2].Value = "Tuple_Name";
                    worksheet.Cells[1, 3].Value = "Tuple_GeoLocation";
                    worksheet.Cells[1, 4].Value = "Tuple_DataType";
                    worksheet.Cells[1, 5].Value = "Tuple_ElapsedTime";
                    worksheet.Cells[1, 6].Value = "Tuple_InitiatesTime";
                    worksheet.Cells[1, 7].Value = "TotalTimeTaken";
                    worksheet.Cells[1, 8].Value = "Tuple_Reversed";
                    worksheet.Cells[1, 9].Value = "Tuple_IsServerFound";
                    worksheet.Cells[1, 10].Value = "TupleDeviceType";
                    worksheet.Cells[1, 11].Value = "DataCenter_Name";
                    worksheet.Cells[1, 12].Value = "DataCenter_GeoLocation_Longitute";
                    worksheet.Cells[1, 13].Value = "DataCenter_GeoLocation_Latitute";
                    worksheet.Cells[1, 14].Value = "Cloud_DistanceFromServer";
                    worksheet.Cells[1, 15].Value = "DataCenter_DistanceFromTuple";
                    worksheet.Cells[1, 16].Value = "IsCloudServed";
                    worksheet.Cells[1, 17].Value = "DataCenter";
                    worksheet.Cells[1, 18].Value = "Host";
                    worksheet.Cells[1, 19].Value = "Tuple_PropogationTime";
                    worksheet.Cells[1, 20].Value = "InternalProcessingTime";
                    worksheet.Cells[1, 21].Value = "Serving_Fog_Level";
                    worksheet.Cells[1, 22].Value = "IsServedByFC_Cloud";
                    worksheet.Cells[1, 23].Value = "JobQueueDelay";
                    int rowNumber = 2;
                    FinalResults Fr;
                    //Add existing users
                    foreach (var item in edgelist)
                    {
                        if (item != null)
                        {
                            try
                            {
                                Fr = new FinalResults();

                                worksheet.Cells[rowNumber, 1].Value = item.Tuple.ID;
                                worksheet.Cells[rowNumber, 2].Value = item.Tuple.Name;
                                worksheet.Cells[rowNumber, 3].Value = item.Tuple.GeoLocation.getLongitude() + "," + item.Tuple.GeoLocation.getLatitude();
                                Fr.DataType = item.Tuple.DataType;

                                worksheet.Cells[rowNumber, 4].Value = Fr.DataType;
                                worksheet.Cells[rowNumber, 5].Value = item.ElapsedTime;
                                worksheet.Cells[rowNumber, 6].Value = item.InitiatesTime;
                                Fr.TimeTaken = item.ElapsedTime - Convert.ToDouble(item.InitiatesTime) < 0 ? 0 : item.ElapsedTime - Convert.ToDouble(item.InitiatesTime);
                                worksheet.Cells[rowNumber, 7].Value = Fr.TimeTaken;
                                worksheet.Cells[rowNumber, 8].Value = item.Tuple.IsReversed;
                                worksheet.Cells[rowNumber, 9].Value = item.Tuple.IsServerFound;
                                worksheet.Cells[rowNumber, 10].Value = item.Tuple.DeviceType;
                                worksheet.Cells[rowNumber, 11].Value = item.FogBroker == null ? "Null" : item.FogBroker.SelectedFogDevice != null ? item.FogBroker.SelectedFogDevice.Name : "Null";
                                worksheet.Cells[rowNumber, 12].Value = item.FogBroker != null ? item.FogBroker.SelectedFogDevice.GeoLocation.getLongitude() :
                                                       item.CloudBroker != null ? item.CloudBroker.SelectedDataCenter.DatacenterCharacteristics.GeoLocation.getLongitude() : -1;
                                worksheet.Cells[rowNumber, 13].Value = item.FogBroker != null ? item.FogBroker.SelectedFogDevice.GeoLocation.getLatitude() :
                                                       item.CloudBroker != null ? item.CloudBroker.SelectedDataCenter.DatacenterCharacteristics.GeoLocation.getLatitude() : -1;
                                worksheet.Cells[rowNumber, 14].Value = 0.0;
                                worksheet.Cells[rowNumber, 15].Value = item.FogBroker != null ? item.FogBroker.SelectedFogDevice.DistanceFromTuple : -1;

                                Fr.IsServedByCloud = item.FogBroker != null ? item.FogBroker.Tuple.IsCloudServed : item.CloudBroker != null ? true : false;
                                worksheet.Cells[rowNumber, 16].Value = Fr.IsServedByCloud;
                                worksheet.Cells[rowNumber, 17].Value = item.CloudBroker != null ? item.CloudBroker.SelectedDataCenter.Name : null;
                                worksheet.Cells[rowNumber, 18].Value = item.CloudBroker != null ? item.CloudBroker.SelectedHost.ID : -1;

                                Fr.PropagationTime = item.Link != null ? item.Link.Propagationtime : -1;
                                worksheet.Cells[rowNumber, 19].Value = Fr.PropagationTime;

                                Fr.InternalProcessingTime = item.Tuple.InternalProcessingTime;
                                worksheet.Cells[rowNumber, 20].Value = Fr.InternalProcessingTime;
                                worksheet.Cells[rowNumber, 21].Value = item.Tuple.FogLevelServed;
                                Fr.IsServedByFC_Cloud = item.Tuple.IsServedByFC_Cloud;
                                worksheet.Cells[rowNumber, 22].Value = Fr.IsServedByFC_Cloud;
                                worksheet.Cells[rowNumber, 23].Value = item.Tuple.QueueDelay;
                                rowNumber++;

                                finalResults.Add(Fr);
                            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                            {
                            }
                        }
                    }

                    // Fit the columns according to its content
                    worksheet.Column(1).AutoFit();
                    worksheet.Column(2).AutoFit();
                    worksheet.Column(3).AutoFit();
                    worksheet.Column(4).AutoFit();
                    worksheet.Column(5).AutoFit();
                    worksheet.Column(6).AutoFit();
                    worksheet.Column(7).AutoFit();
                    worksheet.Column(8).AutoFit();
                    worksheet.Column(9).AutoFit();
                    worksheet.Column(10).AutoFit();
                    worksheet.Column(11).AutoFit();
                    worksheet.Column(12).AutoFit();
                    worksheet.Column(13).AutoFit();
                    worksheet.Column(14).AutoFit();
                    worksheet.Column(15).AutoFit();
                    worksheet.Column(16).AutoFit();
                    worksheet.Column(17).AutoFit();
                    worksheet.Column(18).AutoFit();
                    worksheet.Column(19).AutoFit();
                    worksheet.Column(20).AutoFit();
                    worksheet.Column(21).AutoFit();
                    worksheet.Column(22).AutoFit();

                    #endregion Results_Fog

                    // add a new worksheet to the empty workbook

                    #region Edge_Characteristics

                    ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add("Edge_Characteristics");


                    // --------- Data and styling goes here -------------- //

                    worksheet2.Cells[1, 1].Value = "Edge_Name";
                    worksheet2.Cells[1, 2].Value = "Ram";
                    worksheet2.Cells[1, 3].Value = "MIPS";
                    worksheet2.Cells[1, 4].Value = "UpBandwidth";
                    worksheet2.Cells[1, 5].Value = "DownBandwidth";
                    worksheet2.Cells[1, 6].Value = "Number of Cores";
                    worksheet2.Cells[1, 7].Value = "Size";
                    worksheet2.Cells[1, 8].Value = "Storage";
                    worksheet2.Cells[1, 9].Value = "Status";
                    worksheet2.Cells[1, 10].Value = "DataType";
                    worksheet2.Cells[1, 11].Value = "GeoLocation_Longitute";
                    worksheet2.Cells[1, 12].Value = "GeoLocation_Latitute";
                    worksheet2.Cells[1, 13].Value = "Id";
                    int _rowNumber = 2;

                    if (fogBroker != null)
                    {
                        foreach (var item1 in fogBroker.FogList)
                        {
                            try
                            {
                                worksheet2.Cells[_rowNumber, 1].Value = item1.Name;
                                worksheet2.Cells[_rowNumber, 2].Value = item1.RAM;
                                worksheet2.Cells[_rowNumber, 3].Value = item1.MIPS;
                                worksheet2.Cells[_rowNumber, 4].Value = item1.UpBW;
                                worksheet2.Cells[_rowNumber, 5].Value = item1.DownBW;
                                worksheet2.Cells[_rowNumber, 6].Value = item1.NumberOfPes;
                                worksheet2.Cells[_rowNumber, 7].Value = item1.Size;
                                worksheet2.Cells[_rowNumber, 8].Value = item1.Storage;
                                worksheet2.Cells[_rowNumber, 9].Value = item1.IsActive;
                                worksheet2.Cells[_rowNumber, 10].Value = item1.DataType;
                                worksheet2.Cells[_rowNumber, 11].Value = item1.GeoLocation.getLongitude();
                                worksheet2.Cells[_rowNumber, 12].Value = item1.GeoLocation.getLatitude();
                                worksheet2.Cells[_rowNumber, 13].Value = item1.ID;
                                _rowNumber++;
                            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                            {
                            }

                        }
                    }
                    worksheet2.Column(1).AutoFit();
                    worksheet2.Column(2).AutoFit();
                    worksheet2.Column(3).AutoFit();
                    worksheet2.Column(4).AutoFit();
                    worksheet2.Column(5).AutoFit();
                    worksheet2.Column(6).AutoFit();
                    worksheet2.Column(7).AutoFit();
                    worksheet2.Column(8).AutoFit();
                    worksheet2.Column(9).AutoFit();
                    worksheet2.Column(10).AutoFit();
                    worksheet2.Column(11).AutoFit();
                    worksheet2.Column(12).AutoFit();
                    worksheet2.Column(13).AutoFit();

                    #endregion Fog_Characteristics

                    #region Tuple_Characteristics

                    ExcelWorksheet worksheet3 = package.Workbook.Worksheets.Add("Tuple_Characteristics");

                    // --------- Data and styling goes here -------------- //

                    worksheet3.Cells[1, 1].Value = "Tuple_Name";
                    worksheet3.Cells[1, 2].Value = "Ram";
                    worksheet3.Cells[1, 3].Value = "MIPS";
                    worksheet3.Cells[1, 4].Value = "Bandwidth";
                    worksheet3.Cells[1, 5].Value = "Number of Cores";
                    worksheet3.Cells[1, 6].Value = "Size";
                    worksheet3.Cells[1, 7].Value = "DataType";
                    worksheet3.Cells[1, 8].Value = "GeoLocation_Longitute";
                    worksheet3.Cells[1, 9].Value = "GeoLocation_Latitude";
                    worksheet3.Cells[1, 10].Value = "Id";
                    worksheet3.Cells[1, 11].Value = "IsReversed";
                    worksheet3.Cells[1, 12].Value = "IsServed";
                    //IsServerFound
                    int __rowNumber = 2;

                    foreach (var item1 in edgelist)
                    {
                        if (item1.Tuple != null)
                        {
                            try
                            {
                                worksheet3.Cells[__rowNumber, 1].Value = string.IsNullOrEmpty(item1.Tuple.Name) ? "Tuple" : item1.Tuple.Name;
                                worksheet3.Cells[__rowNumber, 2].Value = item1.Tuple.RAM;
                                worksheet3.Cells[__rowNumber, 3].Value = item1.Tuple.MIPS;
                                worksheet3.Cells[__rowNumber, 4].Value = item1.Tuple.BW;
                                worksheet3.Cells[__rowNumber, 5].Value = item1.Tuple.NumberOfPes;
                                worksheet3.Cells[__rowNumber, 6].Value = item1.Tuple.Size;
                                worksheet3.Cells[__rowNumber, 7].Value = item1.Tuple.DataType;
                                worksheet3.Cells[__rowNumber, 8].Value = item1.Tuple.GeoLocation.getLongitude();
                                worksheet3.Cells[__rowNumber, 9].Value = item1.Tuple.GeoLocation.getLatitude();
                                worksheet3.Cells[__rowNumber, 10].Value = item1.Tuple.ID;
                                worksheet3.Cells[__rowNumber, 11].Value = item1.Tuple.IsReversed;
                                worksheet3.Cells[__rowNumber, 12].Value = item1.Tuple.IsServed;
                                __rowNumber++;
                            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                            { }
                        }
                    }

                    worksheet3.Column(1).AutoFit();
                    worksheet3.Column(2).AutoFit();
                    worksheet3.Column(3).AutoFit();
                    worksheet3.Column(4).AutoFit();
                    worksheet3.Column(5).AutoFit();
                    worksheet3.Column(6).AutoFit();
                    worksheet3.Column(7).AutoFit();
                    worksheet3.Column(8).AutoFit();
                    worksheet3.Column(9).AutoFit();
                    worksheet3.Column(10).AutoFit();
                    worksheet3.Column(11).AutoFit();
                    worksheet3.Column(12).AutoFit();

                    #endregion Tuple_Characteristics

                    #region Edge_Power_Values

                    ExcelWorksheet worksheet4 = package.Workbook.Worksheets.Add("Edge_Power_Values");
                    List<Powervalues> pwrVal = pwrVal = new List<Powervalues>();
                    Powervalues pval;
                    try
                    {

                        for (int i = 0; i < fogBroker.FogList.Count(); i++)
                        {
                            worksheet4.Cells[1, i + 1].Value = fogBroker.FogList[i].Name;
                        }

                        for (int i = 0; i < fogBroker.FogList.Count; i++)
                        {
                            int powerRowNumber = 2;
                            int numFS = 0;
                            double value = 0;
                            for (int j = 0; j < fogBroker.FogList[i].PowerConsumption.Count; j++)
                            {
                                worksheet4.Cells[powerRowNumber, i + 1].Value = fogBroker.FogList[i].PowerConsumption[j].PowerValue;
                                value += fogBroker.FogList[i].PowerConsumption[j].PowerValue;
                                powerRowNumber++;
                                numFS++;
                            }
                            pval = new Powervalues();
                            NodeConsumption cNcons = new NodeConsumption();

                            pval.Pval = value;
                            pval.ServerName = fogBroker.FogList[i].Name;
                            pval.count = numFS;
                            pval.average = value != 0 ? Math.Round((value / numFS), 3) : 0;
                            pwrVal.Add(pval);
                            worksheet4.Column(i + 1).AutoFit();
                        }
                    }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                    catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                    { }


                    #endregion Fog_Power_Values

                    #region Edge_Timings

                    ExcelWorksheet worksheet5 = package.Workbook.Worksheets.Add("Edge_Timings");

                    worksheet5.Cells[1, 1].Value = "Server Name";
                    worksheet5.Cells[1, 2].Value = "Job Name";
                    worksheet5.Cells[1, 3].Value = "Task_Arrival_Time";
                    worksheet5.Cells[1, 4].Value = "Free_From_Task";
                    worksheet5.Cells[1, 5].Value = "TimeDifference in MS";
                    worksheet5.Cells[1, 6].Value = "Edge Consumption";
                    worksheet5.Cells[1, 7].Value = "Edge Consumption Percentage";


                    int rowNumberForTiming = 2;
                    string previousName = "";
                    PowerUtility.FillNumRange();

                    var fgT = fogTimings.OrderBy(x => x.FogName.Split('-')[1]).OrderBy(a => a.TaskArrival);
                    foreach (var item in fgT)
                    {
                        try
                        {
                            if (item.FogName != previousName && rowNumberForTiming > 2)
                            {
                                previousName = item.FogName;
                                rowNumberForTiming++;
                            }
                            worksheet5.Cells[rowNumberForTiming, 1].Value = item.FogName;
                            worksheet5.Cells[rowNumberForTiming, 2].Value = item.TupleName;
                            worksheet5.Cells[rowNumberForTiming, 3].Value = item.TaskArrival;
                            worksheet5.Cells[rowNumberForTiming, 4].Value = item.FreeTime;
                            worksheet5.Cells[rowNumberForTiming, 5].Value = item.TimeDifference;
                            worksheet5.Cells[rowNumberForTiming, 6].Value = item.Consumption;
                            worksheet5.Cells[rowNumberForTiming, 7].Value = item.ConsumptionPer;

                            rowNumberForTiming++;
                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        { }
                    }
                    worksheet5.Column(1).AutoFit();
                    worksheet5.Column(2).AutoFit();
                    worksheet5.Column(3).AutoFit();
                    worksheet5.Column(4).AutoFit();
                    worksheet5.Column(5).AutoFit();
                    worksheet5.Column(6).AutoFit();
                    worksheet5.Column(7).AutoFit();

                    #endregion Fog_Timings

                    #region Tuple_Timings

                    ExcelWorksheet worksheet6 = package.Workbook.Worksheets.Add("Tuple_Timings");

                    worksheet6.Cells[1, 1].Value = "Tuple_Name";
                    worksheet6.Cells[1, 2].Value = "Task_Arrival_Time";
                    worksheet6.Cells[1, 3].Value = "Task_Departure_Time";


                    int rowNumberForTimingT = 2;
                    foreach (var item in tupleTiming.OrderBy(x => x.Name.Split('-')[1]))
                    {
                        try
                        {
                            worksheet6.Cells[rowNumberForTimingT, 1].Value = item.Name;
                            worksheet6.Cells[rowNumberForTimingT, 2].Value = item.TupleArrival;
                            worksheet6.Cells[rowNumberForTimingT, 3].Value = item.TupleDeparture;
                            rowNumberForTimingT++;
                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        { }
                    }

                    worksheet6.Column(1).AutoFit();
                    worksheet6.Column(2).AutoFit();
                    worksheet6.Column(3).AutoFit();

                    #endregion Tuple_Timings

                    #region Servers and Stats

                    ExcelWorksheet worksheet7 = package.Workbook.Worksheets.Add("Acting Servers");
                    worksheet7.Cells[1, 1].Value = "Server Name";
                    worksheet7.Cells[1, 2].Value = "Total Jobs";
                    worksheet7.Cells[1, 3].Value = "StartTime";
                    worksheet7.Cells[1, 4].Value = "EndTime";
                    worksheet7.Cells[1, 5].Value = "FogConsumption";
                    worksheet7.Cells[1, 6].Value = "FogConsPercent";
                    worksheet7.Cells[1, 7].Value = "TimeKey";

                    int rowNumberActingServer = 2;
                    List<IlistResult> ilistResult = new List<IlistResult>();
                    long FogCount = fogTimings.OrderBy(a => a.FogName).GroupBy(a => a.FogName).ToList().Count();
                    var Servers = fogTimings.GroupBy(a => a.FogName);
                    List<FogTimes> AFdata = fogTimings.OrderBy(a => a.FogName.Split('-')[1]).ToList();//.OrderBy(a => a.TaskArrival);
                    try
                    {
                        foreach (var item in Servers)
                        {
                            IlistResult iLresult = new IlistResult();
                            string serverName = item.Key.ToString();
                            iLresult.iListFog = AFdata.Where(a => a.FogName == serverName).Select(a => a).ToList();
                            ilistResult.Add(iLresult);
                        };
                        foreach (var item in ilistResult)
                        {
                            List<IFogTimeCons> query = item.iListFog.GroupBy(x => new { Convert.ToDateTime(x.TaskArrival).Hour, Convert.ToDateTime(x.TaskArrival).Minute, Convert.ToDateTime(x.TaskArrival).Second })
                                     .Select(g => new IFogTimeCons
                                     {
                                         TimeKey = g.Key.ToString(),
                                         JobCount = g.Select(a => a.TupleName).Count(),
                                         JobsList = g.Select(a => a.TupleName).ToList(),
                                         FogConsumption = g.Select(a => a.Consumption).ToList().Sum(),
                                         FogConsPercent = g.Select(a => a.ConsumptionPer).Sum(),
                                         ServerName = g.Select(a => a.FogName).FirstOrDefault().ToString(),
                                         StartTime = g.Select(a => a.TaskArrival).FirstOrDefault(),
                                         EndTime = g.Select(a => a.FreeTime).LastOrDefault(),
                                     }).OrderBy(a => a.TimeKey).ToList();
                            foreach (var _item in query)
                            {
                                worksheet7.Cells[rowNumberActingServer, 1].Value = _item.ServerName;
                                worksheet7.Cells[rowNumberActingServer, 2].Value = _item.JobCount;
                                worksheet7.Cells[rowNumberActingServer, 3].Value = _item.StartTime;
                                worksheet7.Cells[rowNumberActingServer, 4].Value = _item.EndTime;
                                worksheet7.Cells[rowNumberActingServer, 5].Value = _item.FogConsumption;
                                worksheet7.Cells[rowNumberActingServer, 6].Value = _item.FogConsPercent;
                                worksheet7.Cells[rowNumberActingServer, 7].Value = _item.TimeKey;
                                rowNumberActingServer++;
                            }
                        }
                    }
                    catch (Exception)
                    { }

                    worksheet7.Column(1).AutoFit();
                    worksheet7.Column(2).AutoFit();
                    worksheet7.Column(3).AutoFit();
                    worksheet7.Column(4).AutoFit();
                    worksheet7.Column(5).AutoFit();
                    worksheet7.Column(6).AutoFit();
                    worksheet7.Column(7).AutoFit();

                    #endregion

                    #region Filtered results
                    ExcelWorksheet worksheet8 = package.Workbook.Worksheets.Add("Filtered Results");
                    worksheet8.Cells[1, 1].Value = "TotalSimulationTime(Seconds)";
                    worksheet8.Cells[1, 2].Value = "ToTalServerUsed";
                    worksheet8.Cells[1, 3].Value = "StartTime";
                    worksheet8.Cells[1, 4].Value = "EndTime";
                    worksheet8.Cells[1, 5].Value = "TotalAverageTimeTaken(MS)";
                    worksheet8.Cells[1, 6].Value = "TotalAveragePropagationTaken";
                    worksheet8.Cells[1, 7].Value = "TotalAverageInternalProccTime";
                    worksheet8.Cells[1, 8].Value = "JobsServedbyCloud";
                    worksheet8.Cells[1, 9].Value = "JobsServedbyEdge";
                    worksheet8.Cells[1, 10].Value = "GrandPowerConsumption";
                    worksheet8.Cells[1, 11].Value = "TotalReversedJobs";
                    worksheet8.Cells[1, 12].Value = "DroppedJobs";
                    worksheet8.Cells[1, 13].Value = "DataType";
                    worksheet8.Cells[1, 14].Value = "DataType_AvgInternalProcessingTime";
                    worksheet8.Cells[1, 15].Value = "DataType_AvgPropagationTime";
                    worksheet8.Cells[1, 16].Value = "DataType_TotalJobsServed";
                    worksheet8.Cells[1, 17].Value = "DataType_TotlaServerUsedBy";
                    worksheet8.Cells[1, 18].Value = "DataType_Avg_TimeTaken";
                    worksheet8.Cells[1, 19].Value = "DataType_Avg_QueueDelay";

                    List<string> servers = null;
                    try
                    {
                        int worksheetNo = 19;
                        for (int i = 1; i <= pwrVal.Select(a => a.ServerName).ToList().Count(); i++)
                        {
                            servers = pwrVal.Select(a => a.ServerName).Take(i).ToList();

                            worksheet8.Cells[1, i + worksheetNo].Value = servers[i - 1];
                        }
                        int nextRow = worksheetNo + pwrVal.Select(a => a.ServerName).ToList().Count();
                        List<NodeConsumption> nodCons = new List<NodeConsumption>();
                        NodeConsumption nc;
                        foreach (var item in fogBroker.FogList)
                        {
                            nc = new NodeConsumption();
                            nc.DataType = item.DataType;
                            nc.powrCons = item.PowerConsumption;
                            nc.average = item.PowerConsumption.Count() != 0 ? item.PowerConsumption.Where(a => a.PowerValue > 0).Select(a => a.PowerValue).Sum() / item.PowerConsumption.Where(a => a.PowerValue > 0).Select(a => a.PowerValue).Count() : 0;
                            nodCons.Add(nc);
                        }

                        var group = nodCons.GroupBy(a => a.DataType).ToList();
                        List<NodeConsumption_sub> NodeConsSub = new List<NodeConsumption_sub>();
                        NodeConsumption_sub sub;
                        foreach (var item in group)
                        {
                            sub = new NodeConsumption_sub();
                            sub.DataType = item.Key.ToString();
                            sub.average = nodCons.Where(a => a.DataType == sub.DataType && a.average != 0).Select(a => a.average).Sum() / nodCons.Where(a => a.DataType == sub.DataType && a.average != 0).Select(a => a.average).Count();
                            NodeConsSub.Add(sub);
                        }
                        for (int i = 1; i <= NodeConsSub.Count; i++)
                        {
                            worksheet8.Cells[1, i + nextRow].Value = NodeConsSub[i - 1].DataType;
                        }
                        var tupleDT = edgelist.GroupBy(x => x.Tuple.DataType)
                             .Select(g => new TupleDT
                             {
                                 DataType = g.Key.ToString(),
                                 DataType_InternalProcessingTime = Math.Round(g.Select(a => a.Tuple.InternalProcessingTime).Sum() / g.Select(a => a.Tuple.InternalProcessingTime).Count(), 3),
                                 DataType_PropagationTime = Math.Round(g.Select(a => a.Link.Propagationtime).Sum() / g.Select(a => a.Link.Propagationtime).Count(), 3),
                                 DataType_TotalJobsServed = g.Select(a => a.Tuple.ID).Count(),
                                 DataType_TotalServerUsedBy = g.Where(a => a.FogBroker != null).Select(a => a.FogBroker.SelectedFogDevice).Count(),
                                 DataType_TimeTaken = Math.Round(finalResults.Where(a => a.DataType == g.Key.ToString()).Select(a => a.TimeTaken).Sum() / finalResults.Where(a => a.DataType == g.Key.ToString()).Select(a => a.TimeTaken).Count(), 3),
                                 DataType_QueueDelay = Math.Round(edgelist.Where(a => a.Tuple.DataType == g.Key.ToString()).Select(a => a.Tuple.QueueDelay).Sum() / edgelist.Where(a => a.Tuple.DataType == g.Key.ToString()).Select(a => a.Tuple.QueueDelay).Count(), 3),
                             }).OrderBy(a => a.DataType).ToList();
                        List<TupleDT> querytupleDT = tupleDT;


                        var tupleNT = edgelist.GroupBy(x => x.Tuple.DeviceType)
                            .Select(g => new TupleNT
                            {
                                NodeType = g.Key.ToString(),
                                NodeType_InternalProcessingTime = Math.Round(g.Select(a => a.Tuple.InternalProcessingTime).Sum() / g.Select(a => a.Tuple.InternalProcessingTime).Count(), 3),
                                NodeType_PropagationTime = Math.Round(g.Select(a => a.Link.Propagationtime).Sum() / g.Select(a => a.Link.Propagationtime).Count(), 3),
                                NodeType_TotalJobsServed = g.Select(a => a.Tuple.ID).Count(),
                                NodeType_TotalServerUsedBy = g.Where(a => a.FogBroker != null).Select(a => a.FogBroker.SelectedFogDevice).Count(),
                            }).OrderBy(a => a.NodeType).ToList();

                        List<TupleNT> querytupleNT = tupleNT;

                        int rowNumberFiltered = 2;
                        // Simulation time ?
                        string StartTime = fogTimings.OrderBy(a => a.TaskArrival).Select(a => a.TaskArrival).FirstOrDefault();
                        string EndTime = fogTimings.OrderByDescending(a => a.FreeTime).Select(a => a.FreeTime).FirstOrDefault();

                        worksheet8.Cells[2, 1].Value = (Convert.ToDateTime(EndTime) - Convert.ToDateTime(StartTime)).TotalSeconds;
                        worksheet8.Cells[2, 2].Value = FogCount;
                        worksheet8.Cells[2, 3].Value = StartTime;
                        worksheet8.Cells[2, 4].Value = EndTime;
                        worksheet8.Cells[2, 5].Value = Math.Round(finalResults.Select(a => a.TimeTaken).Sum() / finalResults.Select(a => a.TimeTaken).Count(), 3);//finding averge;
                        worksheet8.Cells[2, 6].Value = Math.Round(finalResults.Select(a => a.PropagationTime).Sum() / finalResults.Select(a => a.PropagationTime).Count(), 3);
                        worksheet8.Cells[2, 7].Value = Math.Round(finalResults.Select(a => a.InternalProcessingTime).Sum() / finalResults.Select(a => a.InternalProcessingTime).Count(), 3);
                        worksheet8.Cells[2, 8].Value = finalResults.Where(a => a.IsServedByCloud == true).Count();
                        worksheet8.Cells[2, 9].Value = finalResults.Where(a => a.IsServedByCloud == false).Count();
                        worksheet8.Cells[2, 10].Value = pwrVal != null ? Math.Round(pwrVal.Where(a => a.average != 0).Select(a => a.average).Sum() / pwrVal.Where(a => a.average != 0).Select(a => a.average).Count(), 3) : 0;
                        worksheet8.Cells[2, 11].Value = edgelist.Where(a => a.Tuple.IsReversed == true && a.Tuple.IsCloudServed == false).Count() + " IsReversed false " + edgelist.Where(a => a.Tuple.IsReversed == true).Count();
                        worksheet8.Cells[2, 12].Value = DropedtupleList.Count > 0 ? DropedtupleList.Count() : 0;
                        int countNum = querytupleDT.Count();

                        foreach (var item in querytupleDT)
                        {
                            worksheet8.Cells[rowNumberFiltered, 13].Value = item.DataType;
                            worksheet8.Cells[rowNumberFiltered, 14].Value = item.DataType_InternalProcessingTime;
                            worksheet8.Cells[rowNumberFiltered, 15].Value = item.DataType_PropagationTime;
                            worksheet8.Cells[rowNumberFiltered, 16].Value = item.DataType_TotalJobsServed;
                            worksheet8.Cells[rowNumberFiltered, 17].Value = item.DataType_TotalServerUsedBy;
                            worksheet8.Cells[rowNumberFiltered, 18].Value = item.DataType_TimeTaken;
                            worksheet8.Cells[rowNumberFiltered, 19].Value = item.DataType_QueueDelay;

                            rowNumberFiltered++;
                        }
                        for (int i = 1; i <= pwrVal.Select(a => a.ServerName).ToList().Count; i++)
                        {
                            List<double> avg = pwrVal.Select(a => a.average).Take(i).ToList();
                            worksheet8.Cells[2, i + worksheetNo].Value = avg[i - 1];
                        }

                        for (int i = 1; i <= NodeConsSub.Count; i++)
                        {
                            worksheet8.Cells[rowNumberFiltered, i + nextRow].Value = Math.Round(NodeConsSub[i - 1].average, 3);
                        }

                        rowNumberFiltered++;
                    }
                    catch (Exception)
                    { }


                    worksheet8.Column(1).AutoFit();
                    worksheet8.Column(2).AutoFit();
                    worksheet8.Column(3).AutoFit();
                    worksheet8.Column(4).AutoFit();
                    worksheet8.Column(5).AutoFit();
                    worksheet8.Column(6).AutoFit();
                    worksheet8.Column(7).AutoFit();
                    worksheet8.Column(8).AutoFit();
                    worksheet8.Column(9).AutoFit();
                    worksheet8.Column(10).AutoFit();
                    #endregion

                    #region droppedTupleList

                    if (DropedtupleList.Count() > 0)
                    {
                        ExcelWorksheet worksheet9 = package.Workbook.Worksheets.Add("DroppedTupleList");

                        // --------- Data and styling goes here -------------- //

                        worksheet9.Cells[1, 1].Value = "Tuple_Name";
                        worksheet9.Cells[1, 2].Value = "Ram";
                        worksheet9.Cells[1, 3].Value = "MIPS";
                        worksheet9.Cells[1, 4].Value = "Bandwidth";
                        worksheet9.Cells[1, 5].Value = "Number of Cores";
                        worksheet9.Cells[1, 6].Value = "Size";
                        worksheet9.Cells[1, 7].Value = "DataType";
                        worksheet9.Cells[1, 8].Value = "GeoLocation_Longitute";
                        worksheet9.Cells[1, 9].Value = "GeoLocation_Latitude";
                        worksheet9.Cells[1, 10].Value = "Id";
                        worksheet9.Cells[1, 11].Value = "IsReversed";
                        worksheet9.Cells[1, 12].Value = "IsServed";
                        //IsServerFound
                        int __rowNumberDt = 2;

                        foreach (var item1 in DropedtupleList)
                        {
                            if (item1 != null)
                            {
                                try
                                {
                                    worksheet9.Cells[__rowNumberDt, 1].Value = string.IsNullOrEmpty(item1.Name) ? "Tuple" : item1.Name;
                                    worksheet9.Cells[__rowNumberDt, 2].Value = item1.RAM;
                                    worksheet9.Cells[__rowNumberDt, 3].Value = item1.MIPS;
                                    worksheet9.Cells[__rowNumberDt, 4].Value = item1.BW;
                                    worksheet9.Cells[__rowNumberDt, 5].Value = item1.NumberOfPes;
                                    worksheet9.Cells[__rowNumberDt, 6].Value = item1.Size;
                                    worksheet9.Cells[__rowNumberDt, 7].Value = item1.DataType;
                                    worksheet9.Cells[__rowNumberDt, 8].Value = item1.GeoLocation.getLongitude();
                                    worksheet9.Cells[__rowNumberDt, 9].Value = item1.GeoLocation.getLatitude();
                                    worksheet9.Cells[__rowNumberDt, 10].Value = item1.ID;
                                    worksheet9.Cells[__rowNumberDt, 11].Value = item1.IsReversed;
                                    worksheet9.Cells[__rowNumberDt, 12].Value = item1.IsServed;
                                    __rowNumberDt++;
                                }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                                catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                                { }
                            }
                        }

                        worksheet9.Column(1).AutoFit();
                        worksheet9.Column(2).AutoFit();
                        worksheet9.Column(3).AutoFit();
                        worksheet9.Column(4).AutoFit();
                        worksheet9.Column(5).AutoFit();
                        worksheet9.Column(6).AutoFit();
                        worksheet9.Column(7).AutoFit();
                        worksheet9.Column(8).AutoFit();
                        worksheet9.Column(9).AutoFit();
                        worksheet9.Column(10).AutoFit();
                        worksheet9.Column(11).AutoFit();
                        worksheet9.Column(12).AutoFit();

                    }
                    #endregion
                    // Set some document properties

                    package.Workbook.Properties.Title = "Results_Edge";
                    package.Workbook.Properties.Author = "Salman and Faizan";
                    package.Workbook.Properties.Company = "Simulation";

                    // save our new workbook and we are done!
                    package.Save();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
            //Move Excel file

        }
    }
    public class IlistResult
    {
        public List<FogTimes> iListFog { get; set; }
    }
    public class IFogTimeCons
    {
        public string TimeKey { get; set; }
        public double JobCount { get; set; }
        public List<string> JobsList { get; set; }
        public double FogConsumption { get; set; }
        public double FogConsPercent { get; set; }
        public string ServerName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }


    }
    //this will set some final extrated values from different places for Final results.
    public class FinalResults
    {
        public string DataType { get; set; }
        public int QueueDelay { get; set; }
        public double TimeTaken { get; set; }
        public double PropagationTime { get; set; }
        public double InternalProcessingTime { get; set; }
        public bool IsServedByCloud { get; set; }
        public bool IsServedByFC_Cloud { get; set; }

    }
    //Tuple data type all details
    public class TupleDT
    {
        public string DataType { get; set; }
        public int DataType_TotalJobsServed { get; set; }
        public double DataType_PropagationTime { get; set; }
        public double DataType_InternalProcessingTime { get; set; }
        public int DataType_TotalServerUsedBy { get; set; }
        public double DataType_TimeTaken { get; set; }
        public double DataType_QueueDelay { get; set; }

    }
    public class TupleNT
    {
        public string NodeType { get; set; }
        public int NodeType_TotalJobsServed { get; set; }
        public double NodeType_PropagationTime { get; set; }
        public double NodeType_InternalProcessingTime { get; set; }
        public int NodeType_TotalServerUsedBy { get; set; }

    }
    public class Powervalues
    {
        public double Pval { get; set; }
        public string ServerName { get; set; }
        public int count { get; set; }
        public double average { get; set; }

    }
    public class NodeConsumption
    {
        public string DataType { get; set; }
        public IEnumerable<PowerConsumption> powrCons { get; set; }
        public double average { get; set; }
    }
    public class NodeConsumption_sub
    {
        public string DataType { get; set; }
        public double average { get; set; }
    }
}
