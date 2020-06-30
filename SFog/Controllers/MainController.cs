using Newtonsoft.Json;
using SFog.Business;
using SFog.Business.Utilities;
using SFog.Business.Utilities.Json;
using SFog.Models;
using SFog.Models.Nodes;
using SFog.Models.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace SFog.Controllers
{
    [RoutePrefix("api/main")]
    public class MainController : ApiController
    {
        public enum EnumDataType
        {
            Multimedia,
            Abrupt,
            SmallTextual,
            Large,
            Medical,
            LocationBased,
            Bulk,
        }
        public enum EnumDataTypeEdge
        {
            Multimedia,
            Abrupt,
            SmallTextual,
            Large,
            Medical,
            LocationBased,
        }
        public enum EnumNodeType
        {
            Sensor,
            Acuator,
            DumbObjects,
            Mobile,
            Nodes
        }
        public enum EdgeNodeType
        {
            Mobile,
            Tablet,
            Laptop,
            Desktop,
            NanoDataCenter
        }//GetEdgeFile

        [Route("GetFogFile")]
        [HttpGet]
        public HttpResponseMessage GetFog()
        {
            try
            {
                string filename = "Results_Fog.xlsx";
                var filePath = Path.Combine(FileInformation.GetDirectory(), filename);
                if (!File.Exists(filePath)) throw new ArgumentException("Report template not found.");
                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        stream.CopyTo(ms);

                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StreamContent(new MemoryStream(ms.ToArray())),
                            Headers = { { "FileName", filename } }
                        };
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = filename
                        };
                        result.Headers.Add("title", filename);
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        result.Content.Headers.Add("name", filename);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [Route("GetEdgeFile")]
        [HttpGet]
        public HttpResponseMessage GetEdge()
        {
            try
            {
                string filename = "Results_Edge.xlsx";
                var filePath = Path.Combine(FileInformation.GetDirectory(), filename);
                if (!File.Exists(filePath)) throw new ArgumentException("Report template not found.");
                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        stream.CopyTo(ms);

                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StreamContent(new MemoryStream(ms.ToArray())),
                            Headers = { { "FileName", filename } }
                        };
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = filename
                        };
                        result.Headers.Add("title", filename);
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        result.Content.Headers.Add("name", filename);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [Route("GetCloudFile")]
        [HttpGet]
        public HttpResponseMessage GetCloud()
        {
            try
            {
                string filename = "Results_Cloud.xlsx";
                var filePath = Path.Combine(FileInformation.GetDirectory(), filename);
                if (!File.Exists(filePath)) throw new ArgumentException("Report template not found.");
                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        stream.CopyTo(ms);

                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StreamContent(new MemoryStream(ms.ToArray())),
                            Headers = { { "FileName", filename } }
                        };
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = filename
                        };
                        result.Headers.Add("title", filename);
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        result.Content.Headers.Add("name", filename);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        //------------- Broker/ Datacenter/ Host/ VM implementation -------------------//

        [Route("cloud")]
        [HttpPost]
        public IHttpActionResult cloud(CloudSimulationPost model)
        {
            try
            {
                CloudSimulator.CloudSimulation
                    (
                    model.TuplePost,
                    model.PolicyType,
                    model.Service,
                    model.DataCenter
                    );
                return Ok();
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }

        [Route("fog")]
        [HttpPost]
        public IHttpActionResult fog(FogSimulationPost model)
        {
            try
            {
                FogSimulator.FogSimulation
                    (
                    model.FogPost,
                    model.TuplePost,
                    model.PolicyType,
                    model.GatewayPolicyType,
                    model.NodeLevelPolicyTypes,
                    model.CommunicationType,
                    model.Service,
                    model.DataCenter,
                    model.Gateway,
                    model.Cooperation,
                    model.FogType
                    );
                return Ok("ok");
            }
            catch (Exception ex)
            {
                return Ok(ex);
            }
        }
        [Route("edge")]
        [HttpPost]
        public IHttpActionResult edge(FogSimulationPost model)
        {
            try
            {
                EdgeSimulator.EdgeSimulation
                    (
                    model.FogPost,
                    model.TuplePost,
                    model.PolicyType,
                    model.CommunicationType,
                    model.Service,
                    model.DataCenter,
                    model.Gateway,
                    model.Cooperation,
                    model.FogType
                    );
                return Ok("ok");
            }
            catch (Exception ex)
            {
                return Ok(ex);
                throw;
            }
        }

        //[Route("multipath")]
        //[HttpPost]
        //public IHttpActionResult multipathrouting(MemoryParams model)
        //{
        //    try
        //    {
        //        MultiPathRouting.MPRSimulation
        //             (
        //             model.FogPost,
        //             model.TuplePost,
        //             model.PolicyType,
        //             model.CommunicationType,
        //             model.Service,
        //             model.DataCenter,
        //             model.Gateway,
        //             model.Cooperation,
        //             model.FogType
        //             );
        //        return Ok("ok");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ex);
        //        throw;
        //    }
        //}

        [Route("createDS")]
        [HttpPost]
        public IHttpActionResult createDS(FogSimulationPost model)
        {
            try
            {

                Random rnd = new Random();

                FogPost fog = model.FogPost;
                Models.TuplePost tuple = model.TuplePost;
                bool F_homo_hetro = true;//true means Homogenious Fogs
                bool T_homo_hetro = true;//true mean Homogenious  Tuples
                long FogSize = fog.FogSize;
                List<FogDevice> fogList = new List<FogDevice>();
                List<Models.Tuple> tupleList = new List<Models.Tuple>();

                #region create Fog dataSet
                if (fog.FogDevice.RAM != 0)
                {
                    #region Fog
                    F_homo_hetro = true;
                    for (int i = 0; i < FogSize; i++)
                    {
                        bool[] bit = { true, false };
                        var b = rnd.Next(bit.Length);

                        Array NodeTypevalues = Enum.GetValues(typeof(EnumNodeType));
                        EnumNodeType randomDataType = (EnumNodeType)NodeTypevalues.GetValue(rnd.Next(NodeTypevalues.Length));

                        fogList.Add(new FogDevice(
                            Guid.NewGuid(),
                            1,
                            fog.FogDevice.MIPS,
                            fog.FogDevice.NumberOfPes,
                            fog.FogDevice.RAM,
                            fog.FogDevice.UpBW,
                            fog.FogDevice.DownBW,
                            fog.FogDevice.Size,
                            fog.FogDevice.Storage,
                            fog.FogDevice.Name + "-" + i,
                            randomDataType.ToString(),
                            new CloudletScheduler(),
                            GeoDistance.RandomGeoLocation(rnd),
                            bit[b],
                            0,
                            PowerUtility.SetIdlePower(),15)
                            );
                    }

                    #endregion Fog
                }
                else
                {
                    #region Fog
                    F_homo_hetro = false;
                    for (int i = 0; i < FogSize; i++)
                    {
                        bool[] bit = { true };
                        var b = rnd.Next(bit.Length);

                        int[] randomRam = { 4096, 6144, 8192, 10240, 12288, 16384 };
                        //  var randomRamIndex = rnd.Next(randomRam.Length);
                        var index = rnd.Next(randomRam.Length);
                        ///commented original values
                        int[] randomMips = { 25000, 50000, 75000, 85000, 95000, 110000 };

                        //these smaller mips were added by ali for testing
                        //int[] randomMips = { 4000, 5000, 10000, 12000, 15000,3000 };


                        var randomMipsIndex = rnd.Next(randomMips.Length);

                        int[] randomPe = { 1, 1, 2, 2, 3, 4 };
                        var randomPeIndex = rnd.Next(randomPe.Length);

                        int[] randomSize = { 7000, 10000, 12000, 15000, 20000, 25000 };
                        var randomSizeIndex = rnd.Next(randomSize.Length);

                        int[] randomDownBW = { 500, 700, 1000, 1200, 1500, 1700 };
                        var randomDownBWIndex = rnd.Next(randomDownBW.Length);

                        int[] randomUpBW = { 700, 1000, 1200, 1500, 2000, 2500 };
                        var randomUpBWIndex = rnd.Next(randomUpBW.Length);

                        int[] randomStorage = { 5000, 10000, 12000, 15000, 20000, 25000 };
                        var randomStorageIndex = rnd.Next(randomStorage.Length);

                        //processor burst time is random since it is OS defined and processor defined and it is not calculated
                        int[] timeSlice = { 15,20,25,30 };
                        var timeSliceIndex = rnd.Next(timeSlice.Length);

                        Array NodeTypevalues = Enum.GetValues(typeof(EnumNodeType));
                        EnumNodeType randomDataType = (EnumNodeType)NodeTypevalues.GetValue(rnd.Next(NodeTypevalues.Length));

                        fogList.Add(new FogDevice(
                            Guid.NewGuid(),
                            1,
                            randomMips[index],
                            randomPe[index],
                            randomRam[index],
                            randomUpBW[index],
                            randomDownBW[index],
                            randomSize[index],
                            randomStorage[index],
                            fog.FogDevice.Name + "-" + i,
                            randomDataType.ToString(),
                            new CloudletScheduler(),
                            GeoDistance.RandomGeoLocation(rnd),
                            bit[b],
                            0,
                            PowerUtility.SetIdlePower(), timeSlice[timeSliceIndex])
                            );
                    }
                    // for writing Json file

                    #endregion Fog

                }
                #endregion

                #region Create Tuple data Set

                if (tuple.TupleData.RAM != 0)
                {
                    T_homo_hetro = true;
                    for (int i = 0; i < tuple.TupleSize; i++)
                    {
                        Array values = Enum.GetValues(typeof(EnumDataType));
                        EnumDataType randomDataType = (EnumDataType)values.GetValue(rnd.Next(values.Length));

                        Array NodeTypevalues = Enum.GetValues(typeof(EnumNodeType));
                        EnumNodeType randomNodeType = (EnumNodeType)NodeTypevalues.GetValue(rnd.Next(NodeTypevalues.Length));

                        tupleList.Add(new Models.Tuple(
                            Guid.NewGuid(),
                            1,
                            tuple.TupleData.MIPS,
                            tuple.TupleData.NumberOfPes,
                            tuple.TupleData.RAM,
                            tuple.TupleData.BW,
                            tuple.TupleData.Size,
                            "T-" + i,
                            randomDataType.ToString(),
                            100,
                            0.0,
                            "Medium",//its medium temporarly
                            new CloudletScheduler(),
                            GeoDistance.RandomGeoLocation(rnd),
                            false,
                            randomNodeType.ToString(),20,0)
                            );
                    }

                }
                else
                {

                    T_homo_hetro = false;
                    for (int i = 0; i < tuple.TupleSize; i++)
                    {
                        Array values = Enum.GetValues(typeof(EnumDataType));
                        EnumDataType randomDataType = (EnumDataType)values.GetValue(rnd.Next(values.Length));
                        var randomMipsIndex = 0;
                        bool BulkOrLarge = false;
                        List<int> randomMips = new List<int>();
                        bool MedORLB = randomDataType.ToString() == "Medical" || randomDataType.ToString() == "LocationBased";
                        switch (randomDataType.ToString())
                        {
                            case "Bulk":
                                randomMips.Add(700);
                                randomMips.Add(900);
                                randomMips.Add(1000);
                                randomMips.Add(1200);
                                BulkOrLarge = true;
                                randomMipsIndex = rnd.Next(randomMips.Count) + 2;
                                break;

                            case "Large":
                                randomMips.Add(500);
                                randomMips.Add(700);
                                randomMips.Add(900);
                                randomMips.Add(1000);
                                BulkOrLarge = true;
                                randomMipsIndex = rnd.Next(randomMips.Count) + 2;
                                break;

                            default:
                                randomMips.Add(50);
                                randomMips.Add(75);
                                randomMips.Add(100);
                                randomMips.Add(150);
                                randomMips.Add(200);
                                randomMips.Add(250);

                                randomMipsIndex = rnd.Next(randomMips.Count);
                                break;
                        }
                        //MB
                        int[] randomRam = { 100, 150, 200, 300, 500, 1024 };
                        // var randomRamIndex = rnd.Next(randomRam.Length);
                        var index = rnd.Next(randomRam.Length);
                        int[] randomPe = { 1, 1, 1, 1, 1, 1 };
                        var randomPeIndex = rnd.Next(randomPe.Length);

                        //MB
                        int[] randomSize = { 80, 120, 170, 220, 270, 300 };
                        var randomSizeIndex = rnd.Next(randomSize.Length);

                        //bit/sec
                        int[] randomBW = { 20, 50, 80, 90, 100, 150 };
                        var randomBWIndex = rnd.Next(randomBW.Length);

                        string[] _priority = { "medium", "low" };
                        var _priorityIndex = rnd.Next(_priority.Length);

                        double[] randomTupleBurstTimes = { 20, 40, 60 };
                        var randomBusrtIndex = rnd.Next(randomTupleBurstTimes.Length);

                        Array NodeTypevalues = Enum.GetValues(typeof(EnumNodeType));
                        EnumNodeType randomNodeType = (EnumNodeType)NodeTypevalues.GetValue(rnd.Next(NodeTypevalues.Length));
                        //if(BulkOrLarge) randomMipsIndex

                        var tupleMips = BulkOrLarge == true ? randomMips[randomMipsIndex - 2] : randomMips[randomMipsIndex];
                        double burstTimeForTuple=0.0;
                        if(tupleMips<=100)
                        {
                            burstTimeForTuple = 10;
                        }

                        else if(tupleMips>100 && tupleMips<=300)
                        {
                            burstTimeForTuple = 15;
                       
                        }

                        else if(tupleMips >300 && tupleMips <500)
                        {
                            burstTimeForTuple = 20;

                        }

                        else if(tupleMips > 500 && tupleMips < 700)
                        {
                            burstTimeForTuple = 25;

                        }
                        else if(tupleMips > 700 && tupleMips <900)
                        {

                            burstTimeForTuple = 30;
                        }

                        else if(tupleMips > 900 && tupleMips <= 1200)
                        {

                            burstTimeForTuple = 35;
                        }

                        tupleList.Add(new Models.Tuple(Guid.NewGuid(),
                            1,
                            BulkOrLarge == true ? randomMips[randomMipsIndex - 2] : randomMips[randomMipsIndex],
                            randomPe[randomMipsIndex],
                            randomRam[randomMipsIndex],
                            randomBW[randomMipsIndex],
                            randomSize[randomMipsIndex],
                            "T-" + i,
                            randomDataType.ToString(),
                            100,
                            0.0,
                            //adding medical, location based and shortest jobs to high priority
                            MedORLB == true ? "high" : BulkOrLarge == true?"low":tupleMips<=100?"high": _priority[_priorityIndex],
                            new CloudletScheduler(),
                            GeoDistance.RandomGeoLocation(rnd),
                            false,
                            randomNodeType.ToString(), burstTimeForTuple, 0)
                            );
                    }
                }

                #endregion

                if (fogList != null && tupleList != null)
                {
                    if (F_homo_hetro)
                        SimJson.WriteJson(fogList, "Homo", "JsonFog.txt");
                    else
                        SimJson.WriteJson(fogList, "Hetro", "JsonFog.txt");
                    if (T_homo_hetro)
                        SimJson.WriteJson(tupleList, "Homo", "JsonTuple.txt");
                    else
                        SimJson.WriteJson(tupleList, "Hetro", "JsonTuple.txt");
                }
                return Ok("ok");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message.ToString());
            }
        }

        [Route("CreateEdgeDS")]
        [HttpPost]
        public IHttpActionResult CreateEdgeDS(FogSimulationPost model)
        {
            try
            {
                /// We are using Same classes in Edge computing  
                /// but changing the specifications for Edge..

                Random rnd = new Random();

                FogPost edge = model.FogPost;
                Models.TuplePost tuple = model.TuplePost;
                bool F_homo_hetro = true; // true means Homogenious Fogs
                bool T_homo_hetro = true; // true mean Homogenious  Tuples
                long EdgeSize = edge.FogSize;
                List<FogDevice> edgeList = new List<FogDevice>();
                List<Models.Tuple> tupleList = new List<Models.Tuple>();

                #region create Edge dataSet
                if (edge.FogDevice.RAM != 0)
                {
                    #region Edge Homogenious
                    F_homo_hetro = true;
                    for (int i = 0; i < EdgeSize; i++)
                    {
                        bool[] bit = { true, false };
                        var b = rnd.Next(bit.Length);

                        Array NodeTypevalues = Enum.GetValues(typeof(EnumNodeType));
                        EnumNodeType randomDataType = (EnumNodeType)NodeTypevalues.GetValue(rnd.Next(NodeTypevalues.Length));

                        edgeList.Add(new FogDevice(
                            Guid.NewGuid(),
                            1,
                            edge.FogDevice.MIPS,
                            edge.FogDevice.NumberOfPes,
                            edge.FogDevice.RAM,
                            edge.FogDevice.UpBW,
                            edge.FogDevice.DownBW,
                            edge.FogDevice.Size,
                            edge.FogDevice.Storage,
                            edge.FogDevice.Name + "-" + i,
                            randomDataType.ToString(),
                            new CloudletScheduler(),
                            GeoDistance.RandomGeoLocation(rnd),
                            bit[b],
                            0,
                            PowerUtility.SetIdlePower(),15)
                            );
                    }

                    #endregion Edge
                }
                else
                {
                    #region Edge Hetrogenious
                    F_homo_hetro = false;
                    for (int i = 0; i < EdgeSize; i++)
                    {
                        bool[] bit = { true };
                        var b = rnd.Next(bit.Length);

                        int[] randomRam = { 512, 1024, 2048, 3072, 3584, 4096 };
                        //  var randomRamIndex = rnd.Next(randomRam.Length);
                        var index = rnd.Next(randomRam.Length);
                        int[] randomMips = { 2000, 3500, 7000, 10000, 15000, 18000 };
                        var randomMipsIndex = rnd.Next(randomMips.Length);

                        int[] randomPe = { 1, 1, 2, 2, 3, 4 };
                        var randomPeIndex = rnd.Next(randomPe.Length);

                        int[] randomSize = { 4000, 5000, 7000, 10000, 12000, 15000 };
                        var randomSizeIndex = rnd.Next(randomSize.Length);

                        int[] randomDownBW = { 400, 500, 700, 1000, 1200, 1500 };
                        var randomDownBWIndex = rnd.Next(randomDownBW.Length);

                        int[] randomUpBW = { 500, 700, 1000, 1200, 1500, 2000 };
                        var randomUpBWIndex = rnd.Next(randomUpBW.Length);

                        int[] randomStorage = { 4000, 5000, 7000, 10000, 12000, 15000 };
                        var randomStorageIndex = rnd.Next(randomStorage.Length);

                        int[] timeSlice = { 15, 20, 25, 30 };
                        var timeSliceIndex = rnd.Next(timeSlice.Length);



                        Array NodeTypevalues = Enum.GetValues(typeof(EnumNodeType));
                        EnumNodeType randomDataType = (EnumNodeType)NodeTypevalues.GetValue(rnd.Next(NodeTypevalues.Length));

                        edgeList.Add(new FogDevice(
                            Guid.NewGuid(),
                            1,
                            randomMips[index],
                            randomPe[index],
                            randomRam[index],
                            randomUpBW[index],
                            randomDownBW[index],
                            randomSize[index],
                            randomStorage[index],
                            edge.FogDevice.Name + "-" + i,
                            randomDataType.ToString(),
                            new CloudletScheduler(),
                            GeoDistance.RandomGeoLocation(rnd),
                            bit[b],
                            0,
                            PowerUtility.SetIdlePower(), timeSlice[timeSliceIndex])
                            );
                    }
                    // for writing Json file

                    #endregion Fog
                }
                #endregion

                #region Create Tuple data Set

                if (tuple.TupleData.RAM != 0)
                {
                    T_homo_hetro = true;
                    for (int i = 0; i < tuple.TupleSize; i++)
                    {
                        Array values = Enum.GetValues(typeof(EnumDataTypeEdge));
                        EnumDataTypeEdge randomDataType = (EnumDataTypeEdge)values.GetValue(rnd.Next(values.Length));

                        Array NodeTypevalues = Enum.GetValues(typeof(EnumNodeType));
                        EnumNodeType randomNodeType = (EnumNodeType)NodeTypevalues.GetValue(rnd.Next(NodeTypevalues.Length));

                        tupleList.Add(new Models.Tuple(
                            Guid.NewGuid(),
                            1,
                            tuple.TupleData.MIPS,
                            tuple.TupleData.NumberOfPes,
                            tuple.TupleData.RAM,
                            tuple.TupleData.BW,
                            tuple.TupleData.Size,
                            "T-" + i,
                            randomDataType.ToString(),
                            100,
                           0.0,
                           "medium",
                            new CloudletScheduler(),
                            GeoDistance.RandomGeoLocation(rnd),
                            false,
                            randomNodeType.ToString(), 20,0)
                            );
                    }

                }
                else
                {

                    T_homo_hetro = false;
                    for (int i = 0; i < tuple.TupleSize; i++)
                    {
                        Array values = Enum.GetValues(typeof(EnumDataTypeEdge));
                        EnumDataTypeEdge randomDataType = (EnumDataTypeEdge)values.GetValue(rnd.Next(values.Length));
                        bool MedORLB = randomDataType.ToString() == "Medical" || randomDataType.ToString() == "LocationBased";

                        var randomMipsIndex = 0;
                        bool datatypecheck = false;
                        List<int> randomMips = new List<int>();
                        switch (randomDataType.ToString())
                        {
                            //case "Bulk":
                            //    randomMips.Add(500);
                            //    randomMips.Add(700);
                            //    randomMips.Add(900);
                            //    randomMips.Add(1000);
                            //    BulkOrLarge = true;
                            //    randomMipsIndex = rnd.Next(randomMips.Count) + 2;
                            //    break;
                            case "Abrupt":
                                randomMips.Add(350);
                                randomMips.Add(375);
                                randomMips.Add(400);
                                randomMips.Add(450);
                                datatypecheck = true;
                                randomMipsIndex = rnd.Next(randomMips.Count) + 2;
                                break;
                            case "Large":
                                randomMips.Add(400);
                                randomMips.Add(450);
                                randomMips.Add(500);
                                randomMips.Add(700);
                                datatypecheck = true;
                                randomMipsIndex = rnd.Next(randomMips.Count) + 2;
                                break;
                            case "Medical":
                                randomMips.Add(250);
                                randomMips.Add(270);
                                randomMips.Add(300);
                                randomMips.Add(350);
                                datatypecheck = true;
                                randomMipsIndex = rnd.Next(randomMips.Count) + 2;
                                break;
                            case "LocationBased":
                                randomMips.Add(300);
                                randomMips.Add(350);
                                randomMips.Add(375);
                                randomMips.Add(400);
                                datatypecheck = true;
                                randomMipsIndex = rnd.Next(randomMips.Count) + 2;
                                break;
                            //LocationBased
                            case "Multimedia":
                                randomMips.Add(325);
                                randomMips.Add(350);
                                randomMips.Add(375);
                                randomMips.Add(425);
                                datatypecheck = true;
                                randomMipsIndex = rnd.Next(randomMips.Count) + 2;
                                break;
                            default:
                                randomMips.Add(75);
                                randomMips.Add(100);
                                randomMips.Add(120);
                                randomMips.Add(150);
                                randomMips.Add(150); randomMips.Add(175);
                                randomMipsIndex = rnd.Next(randomMips.Count);
                                break;
                        }
                        //MB
                        int[] randomRam = { 100, 150, 200, 300, 400, 512 };
                        // var randomRamIndex = rnd.Next(randomRam.Length);
                        var index = rnd.Next(randomRam.Length);
                        int[] randomPe = { 1, 1, 1, 1, 1, 1 };
                        var randomPeIndex = rnd.Next(randomPe.Length);

                        //MB
                        int[] randomSize = { 80, 120, 170, 220, 270, 300 };
                        var randomSizeIndex = rnd.Next(randomSize.Length);

                        //bit/sec
                        int[] randomBW = { 20, 50, 80, 90, 100, 150 };
                        var randomBWIndex = rnd.Next(randomBW.Length);

                        string[] _priority = { "medium", "low" };
                        var _priorityIndex = rnd.Next(_priority.Length);

                        double[] randomTupleBurstTimes = { 20, 40, 60 };
                        var randomBusrtIndex = rnd.Next(randomTupleBurstTimes.Length);


                        var tupleMips = datatypecheck == true ? randomMips[randomMipsIndex - 2] : randomMips[randomMipsIndex];
                        double burstTimeForTuple = 0.0;
                        if (tupleMips <= 100)
                        {
                            burstTimeForTuple = 10;
                        }

                        else if (tupleMips > 100 && tupleMips <= 300)
                        {
                            burstTimeForTuple = 15;

                        }

                        else if (tupleMips > 300 && tupleMips < 500)
                        {
                            burstTimeForTuple = 20;

                        }

                        else if (tupleMips > 500 && tupleMips < 700)
                        {
                            burstTimeForTuple = 25;

                        }
                        else if (tupleMips > 700 && tupleMips < 900)
                        {

                            burstTimeForTuple = 30;
                        }

                        else if (tupleMips > 900 && tupleMips <= 1200)
                        {

                            burstTimeForTuple = 35;
                        }
                        Array NodeTypevalues = Enum.GetValues(typeof(EnumNodeType));
                        EnumNodeType randomNodeType = (EnumNodeType)NodeTypevalues.GetValue(rnd.Next(NodeTypevalues.Length));
                        //if(BulkOrLarge) randomMipsIndex
                        tupleList.Add(new Models.Tuple(Guid.NewGuid(),
                            1,
                            datatypecheck == true ? randomMips[randomMipsIndex - 2] : randomMips[randomMipsIndex],
                            randomPe[randomMipsIndex],
                            randomRam[randomMipsIndex],
                            randomBW[randomMipsIndex],
                            randomSize[randomMipsIndex],
                            "T-" + i,
                            randomDataType.ToString(),
                            100,
                             0.0,
                            MedORLB == true ? "high" : _priority[_priorityIndex],
                            new CloudletScheduler(),
                            GeoDistance.RandomGeoLocation(rnd),
                            false,
                            randomNodeType.ToString(), burstTimeForTuple, 0)
                            );
                    }
                }

                #endregion

                if (edgeList != null && tupleList != null)
                {
                    if (F_homo_hetro)
                        SimJson.WriteJson(edgeList, "EHomo", "JsonEdge.txt");
                    else
                        SimJson.WriteJson(edgeList, "EHetro", "JsonEdge.txt");
                    if (T_homo_hetro)
                        SimJson.WriteJson(tupleList, "EHomo", "JsonTuple.txt");
                    else
                        SimJson.WriteJson(tupleList, "EHetro", "JsonTuple.txt");
                }
                return Ok("ok");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message.ToString());
            }
        }

        [Route("download")]
        [HttpGet]
        public HttpResponseMessage download()
        {
            try
            {
                List<Results> resultlist = new List<Results>();
                List<FogTimes> fogtime = new List<FogTimes>();
                List<TupleTimes> TupleTime = new List<TupleTimes>();
                string Text = "";
                Text = SimJson.ReadJsonFile("Result", "FRList");//FRList for Fogserverdlist conmplete
                resultlist = JsonConvert.DeserializeObject<List<Results>>(Text);

                Text = "";
                Text = SimJson.ReadJsonFile("Result", "FTiming");//FTiming for Fogserverdlist conmplete
                fogtime = JsonConvert.DeserializeObject<List<FogTimes>>(Text);

                Text = "";
                Text = SimJson.ReadJsonFile("Result", "TTuple");//TTuple for Fogserverdlist conmplete
                TupleTime = JsonConvert.DeserializeObject<List<TupleTimes>>(Text);

                if (resultlist != null)
                {
                    //Excel.CreateExcelSheetForFog(resultlist, fogtime, TupleTime);

                }

                string filename = "Results_Fog.xlsx";
                var filePath = Path.Combine(FileInformation.GetDirectory(), filename);
                if (!File.Exists(filePath)) throw new ArgumentException("Report template not found.");
                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        stream.CopyTo(ms);

                        var result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new StreamContent(new MemoryStream(ms.ToArray())),
                            Headers = { { "FileName", filename } }
                        };
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = filename
                        };
                        result.Headers.Add("title", filename);
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                        result.Content.Headers.Add("name", filename);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}