using Amib.Threading;
using Newtonsoft.Json;
using SFog.Business.Utilities;
using SFog.Business.Utilities.Cloud;
using SFog.Business.Utilities.Fog;
using SFog.Business.Utilities.Json;
using SFog.Models;
using SFog.Models.Cache;
using SFog.Models.Nodes;
using SFog.Models.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SFog.Business
{
    public enum EnumDataType
    {
        Multimedia,
        Adrupt,
        SmallTextual,
        Large,
        Medical,
        LocationBased,
        Bulk,
    }

    public enum EnumNodeType
    {
        Sensor,
        Acuator,
        DumbObjects,
        Mobile,
        Nodes
    }
    public class EdgeSimulator
    {
        public static List<Results> edgeResultList = new List<Results>();
        public static ConcurrentBag<FogCache> EdgeCahce = new ConcurrentBag<FogCache>();
        public static List<FogDevice> edgeList, fogServers = new List<FogDevice>();
        public static List<Models.Tuple> tupleList = new List<Models.Tuple>();

        public static List<Models.Tuple> initial_tupleList, final_tupleList = new List<Models.Tuple>();

        public static ConcurrentBag<FogTimes> FogTimings = new ConcurrentBag<FogTimes>();
        public static ConcurrentBag<TupleTimes> TupleTimings = new ConcurrentBag<TupleTimes>();

        public static ConcurrentBag<Models.Tuple> DropedtupleList = new ConcurrentBag<Models.Tuple>();

        private static Object Lock = new Object();

        public static bool WithCoo = false;
        public static bool IsCreateCache = false;
        private static int EdgeSize = 20;

        public static void EdgeSimulation(FogPost edge, TuplePost tuple, string policy, int CommunicationType, int Service, List<string> DataCenter, string gateway, string cooperation, string edgeType)
        {

            if (cooperation == "0")
            {
                WithCoo = true;
            }
            Random rnd = new Random();
            Stopwatch watch = new Stopwatch();
            var result = new Results();
            edgeResultList = new List<Results>();
            edgeList = new List<FogDevice>();
            tupleList = new List<Models.Tuple>();
            PowerUtility.FillNumRange();
            string JsonReturn, path;

            #region DataSet population
            if (edgeType == "0")
            {
                #region Edge Homogenious

                path = (new FileInfo(Path.Combine(FileInformation.GetDirectory(), "EHomo\\JsonEdge.txt"))).ToString();
                JsonReturn = SimJson.ReadJsonFile(path);
                edgeList = JsonConvert.DeserializeObject<List<FogDevice>>(JsonReturn);
                EdgeSize = edgeList.Count;

                #endregion Edge

                #region Tuple homogenious

                path = (new FileInfo(Path.Combine(FileInformation.GetDirectory(), "EHomo\\JsonTuple.txt"))).ToString();
                JsonReturn = SimJson.ReadJsonFile(path);
                tupleList = JsonConvert.DeserializeObject<List<Models.Tuple>>(JsonReturn);

                #endregion Tuple

            }
            else
            {
                #region Edge Hetrogenous

                path = (new FileInfo(Path.Combine(FileInformation.GetDirectory(), "EHetro\\JsonEdge.txt"))).ToString();
                JsonReturn = SimJson.ReadJsonFile(path);
                edgeList = JsonConvert.DeserializeObject<List<FogDevice>>(JsonReturn);
                EdgeSize = edgeList.Count;

                #endregion Edge

                #region Tuple Hetrogenious

                path = (new FileInfo(Path.Combine(FileInformation.GetDirectory(), "EHetro\\JsonTuple.txt"))).ToString();
                JsonReturn = SimJson.ReadJsonFile(path);
                tupleList = JsonConvert.DeserializeObject<List<Models.Tuple>>(JsonReturn);
                #endregion Tuple
            }

            #region create fog for Edge-fog cloud
            if (CommunicationType == 1)
            {
                int fCount = Convert.ToInt32(EdgeSize / 2);
                for (int i = 0; i < fCount; i++)
                {
                    bool[] bit = { true };
                    var b = rnd.Next(bit.Length);

                    int[] randomRam = { 512, 1024, 2048, 3072, 3584, 4096 };
                    //  var randomRamIndex = rnd.Next(randomRam.Length);
                    var index = rnd.Next(randomRam.Length);
                    int[] randomMips = { 2000, 4000, 8000, 12000, 18000, 20000 };
                    var randomMipsIndex = rnd.Next(randomMips.Length);

                    int[] randomPe = { 1, 1, 2, 2, 3, 4 };
                    var randomPeIndex = rnd.Next(randomPe.Length);

                    int[] randomSize = { 4000, 5000, 7000, 10000, 12000, 15000 };
                    var randomSizeIndex = rnd.Next(randomSize.Length);

                    int[] randomDownBW = { 400, 500, 700, 1000, 1200, 1500 };
                    var randomDownBWIndex = rnd.Next(randomDownBW.Length);

                    int[] randomUpBW = { 500, 700, 1000, 1200, 1500, 2000 };
                    var randomUpBWIndex = rnd.Next(randomUpBW.Length);

                    int[] randomStorage = { 2500, 4500, 5000, 7000, 10000, 12000 };
                    var randomStorageIndex = rnd.Next(randomStorage.Length);

                    int[] timeSlice = { 15, 20, 25, 30 };
                    var timeSliceIndex = rnd.Next(timeSlice.Length);

                    Array NodeTypevalues = Enum.GetValues(typeof(EnumNodeType));
                    EnumNodeType randomDataType = (EnumNodeType)NodeTypevalues.GetValue(rnd.Next(NodeTypevalues.Length));

                    fogServers.Add(new FogDevice(
                        Guid.NewGuid(),
                        1,
                        randomMips[index],
                        randomPe[index],
                        randomRam[index],
                        randomUpBW[index],
                        randomDownBW[index],
                        randomSize[index],
                        randomStorage[index],
                        "fog" + "-" + i,
                        randomDataType.ToString(),
                        new CloudletScheduler(),
                        GeoDistance.RandomGeoLocation(rnd),
                        bit[b],
                        0,
                        PowerUtility.SetIdlePower(), 
                        timeSlice[timeSliceIndex])
                        );
                }
            }

            #endregion

            #endregion
            SmartThreadPool s = new SmartThreadPool();
            s.MaxThreads = 1000;
            s.MinThreads = 1000;

            List<Task> myTaskList = new List<Task>();
            if (policy == "1")
            {
                //FCFS
                #region P1 FCFS

                s = new SmartThreadPool();
                watch.Start();
                foreach (var item in tupleList)
                {
                   // item.QueueDelay = watch.Elapsed.Milliseconds;
                   // FogUtility.EdgeSim(item, edgeList, CommunicationType);
                    s.QueueWorkItem(o => FogUtility.EdgeSim(item, edgeList, CommunicationType), new object());
                }
                watch.Stop();
                try
                {
                    s.WaitForIdle();
                    s.Shutdown();
                }
                catch { };
                #endregion
            }
            else if (policy == "2")
            {
                #region P2 SJF
                var localtupleList = tupleList.OrderBy(x => x.MIPS).ToList();

                s = new SmartThreadPool();
                watch.Start();
                foreach (var item in localtupleList)
                {
                    s.QueueWorkItem(o => FogUtility.EdgeSim(item, edgeList, CommunicationType), new object());
                }
                watch.Stop();
                try
                {
                    s.WaitForIdle();
                    s.Shutdown();
                }
                catch { };
                #endregion
            }
            else if (policy == "3")
            {
                #region P3 LJF
                var localtupleList = tupleList.OrderByDescending(x => x.MIPS).ToList();

                s = new SmartThreadPool();
                watch.Start();
                foreach (var item in localtupleList)
                {
                    s.QueueWorkItem(o => FogUtility.EdgeSim(item, edgeList, CommunicationType), new object());
                }
                watch.Stop();
                try
                {
                    s.WaitForIdle();
                    s.Shutdown();
                }
                catch { };
                #endregion
            }
            //inserted new policy #change 
            //LBFC
            else if (policy == "4")
            {
                try
                {
                    IsCreateCache = true;
                    s = new SmartThreadPool();
                    ///*getting 5% to inital execute*/
                    double initialRandomTuplesCount = Math.Ceiling((double)((tupleList.Count() * 5) / 100));
                    initial_tupleList = tupleList.Take(Convert.ToInt32(initialRandomTuplesCount)).ToList();
                    final_tupleList = tupleList.Skip(Convert.ToInt32(initialRandomTuplesCount)).ToList();

                    myTaskList = new List<Task>();
                    var split = LinqExtensions.Split(initial_tupleList, 5).ToList();
                    for (int j = 0; j < split.Count(); j++)
                    {
                        foreach (var item in split[j])
                        {
                            int[] ranIndex = { 0, 1, 0, 1 };
                            var randCloudIndex = rnd.Next(ranIndex.Length);

                            watch.Start();

                            if (randCloudIndex == 0)
                            {
                                myTaskList.Add(Task.Factory.StartNew(() =>
                                {
                                    var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                                    lock (Lock)
                                        EdgeSimulator.TupleTimings.Add(tupleTime);
                                    FogUtility.EdgeSim(item, edgeList, CommunicationType);
                                }));
                            }
                            else
                            {

                                myTaskList.Add(Task.Factory.StartNew(() =>
                                {
                                    var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                                    lock (Lock)
                                        FogSimulator.TupleTimings.Add(tupleTime);
                                    FogUtility.FogSimulationForEdge(item, fogServers);
                                }));
                            }
                        }
                    }
                    Task.WaitAll(myTaskList.ToArray());
                    #region threadpool base
                    try
                    {

                        FogCache F_cache, C_cache;
                        ///both are same for edge also 
                        foreach (var item in final_tupleList)
                        {
                            lock (Lock)
                            {
                                F_cache = FogUtility.fogCahce.Where(x => x.DataType == item.DataType).OrderBy(x => x.InternalProcessingTime).OrderBy(x => x.link.Propagationtime).FirstOrDefault();
                                C_cache = EdgeCahce.Where(x => x.DataType == item.DataType).OrderBy(x => x.InternalProcessingTime).OrderBy(x => x.link.Propagationtime).FirstOrDefault();
                            }
                            watch.Start();
                            if (F_cache == null || C_cache == null)
                            {
                                int[] ranIndex = { 0, 1, 0, 1 };
                                var randCloudIndex = rnd.Next(ranIndex.Length);
                                watch.Start();
                                if (randCloudIndex == 0)
                                {
                                    s.QueueWorkItem(o => FogUtility.EdgeSim(item, edgeList, CommunicationType), new object());
                                }
                                else
                                {
                                    s.QueueWorkItem(o => FogUtility.FogSimulationForEdge(item, fogServers), new object());
                                }
                            }
                            else
                            {
                                //for predication base
                                double _CTime = C_cache.InternalProcessingTime + C_cache.link.Propagationtime;
                                double _FTime = F_cache.InternalProcessingTime + F_cache.link.Propagationtime;

                                if (_CTime >= _FTime)
                                {
                                    s.QueueWorkItem(o => FogUtility.EdgeSim(item, edgeList, CommunicationType), new object());
                                }
                                else
                                {
                                    s.QueueWorkItem(o => FogUtility.FogSimulationForEdge(item, fogServers), new object());

                                }
                            }
                        }
                        try
                        {
                            s.WaitForIdle();
                            s.Shutdown();
                        }
                        catch { };
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(ex.Message);
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(ex.Message);
                }
            }
            else
            {
                #region random
                try
                {
                    var split = LinqExtensions.Split(tupleList, 16).ToList();
                    watch.Start();
                    s = new SmartThreadPool();
                    for (int j = 0; j < split.Count(); j++)
                    {
                        foreach (var item in split[j])
                        {
                            s.QueueWorkItem(o => FogUtility.EdgeSim(item, edgeList, CommunicationType), new object());
                        }
                    }

                    try
                    {
                        s.WaitForIdle();
                        s.Shutdown();
                    }
                    catch { };
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                #endregion
            }
            watch.Stop();
            if (policy == "4")
            {
                if (edgeResultList != null)
                {
                    Excel.CreateExcelSheetEdgeFog(edgeResultList, FogTimings.ToList(), TupleTimings.ToList(), DropedtupleList.ToList());
                }
            }
            else

            {
                Excel.CreateExcelSheetEdgeFog(edgeResultList, FogTimings.ToList(), TupleTimings.ToList(), DropedtupleList.ToList());
                if (CommunicationType == 1)
                    Excel.CreateExcelSheetForFog(FogSimulator.resultList, FogSimulator.FogTimings.ToList(), FogSimulator.TupleTimings.ToList());

            }
        }


    }
}