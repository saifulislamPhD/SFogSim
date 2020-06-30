using Amib.Threading;
using Newtonsoft.Json;
using SFog.Business.Utilities;
using SFog.Business.Utilities.Cloud;
using SFog.Business.Utilities.Fog;
using SFog.Business.Utilities.Json;
using SFog.Models;
using SFog.Models.Cache;
using SFog.Models.Links;
using SFog.Models.Nodes;
using SFog.Models.Utility;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Tuple = SFog.Models.Tuple;

namespace SFog.Business
{
    public class FogSimulator
    {
        #region inputs
        public static List<Results> resultList = new List<Results>();
        private static List<FogDevice> fogList = new List<FogDevice>();
        public static List<Models.Tuple> tupleList = new List<Models.Tuple>();
        public static List<Models.Tuple> initial_tupleList = new List<Models.Tuple>();
        public static List<Models.Tuple> final_tupleList = new List<Models.Tuple>();
        public static ConcurrentBag<FogTimes> FogTimings = new ConcurrentBag<FogTimes>();
        public static ConcurrentBag<TupleTimes> TupleTimings = new ConcurrentBag<TupleTimes>();
        public static ConcurrentBag<CollectionLinks> CompleteLinkPath = new ConcurrentBag<CollectionLinks>();
        private static Object Lock = new Object();
        public static bool WithGateway = false;
        public static bool WithCoo = false;
        public static bool IsCreateCache = false;
        private static int FogSize = 20;
        public static int MaxFogCapacity = 0;
        #endregion
        public enum EnumDataType
        {
            Multimedia,
            Bulk,
            Adrupt,
            SmallTextual,
            Large,
            Medical,
            LocationBased
        }

        public enum EnumNodeType
        {
            Sensor,
            Acuator,
            DumbObjects,
            Mobile,
            Nodes
        }

        public static void FogSimulation(FogPost fog, TuplePost tuple, string policy,string GatewaypolicyType,string NodeLevelPolicyType, int CommunicationType, int Service, List<string> DataCenter, string gateway, string cooperation, string fogType)
        {
            if (gateway == "0")
            {
                WithGateway = true;
            }
            if (cooperation == "0")
            {
                WithCoo = true;
            }
            Random rnd = new Random();
            Stopwatch watch = new Stopwatch();
            var result = new Results();
            resultList = new List<Results>();
            fogList = new List<FogDevice>();
            tupleList = new List<Models.Tuple>();
            PowerUtility.FillNumRange();
            string JsonReturn, path;

            #region DataSet reading 
            if (fogType == "0")
            {
                #region Fog Homogenious

                path = (new FileInfo(Path.Combine(FileInformation.GetDirectory(), "Homo\\JsonFog.txt"))).ToString();
                JsonReturn = SimJson.ReadJsonFile(path);
                fogList = JsonConvert.DeserializeObject<List<FogDevice>>(JsonReturn);
                FogSize = fogList.Count;

                #endregion Fog

                #region Tuple homogenious

                path = (new FileInfo(Path.Combine(FileInformation.GetDirectory(), "Homo\\JsonTuple.txt"))).ToString();
                JsonReturn = SimJson.ReadJsonFile(path);
                tupleList = JsonConvert.DeserializeObject<List<Models.Tuple>>(JsonReturn);

                #endregion Tuple

            }
            else
            {
                #region Fog Hetrogenous

                path = (new FileInfo(Path.Combine(FileInformation.GetDirectory(), "Hetro\\JsonFog.txt"))).ToString();
                JsonReturn = SimJson.ReadJsonFile(path);
                fogList = JsonConvert.DeserializeObject<List<FogDevice>>(JsonReturn);
                FogSize = fogList.Count;

                #endregion Fog

                #region Tuple Hetrogenious

                path = (new FileInfo(Path.Combine(FileInformation.GetDirectory(), "Hetro\\JsonTuple.txt"))).ToString();
                JsonReturn = SimJson.ReadJsonFile(path);
                tupleList = JsonConvert.DeserializeObject<List<Models.Tuple>>(JsonReturn);
                #endregion Tuple
            }

            

            #endregion

            SmartThreadPool s = new SmartThreadPool();
            //s.MaxThreads = 1000;
            //s.MinThreads = 1000;

            List<Task> myTaskList = new List<Task>();
           

            
            if (policy == "1")
            {

                 //tupleList = tupleList.Take(10).ToList();
                tupleList = tupleList.ToList();
                //FCFS
                #region P1 FCFS
                if (WithGateway)
                {
                    GlobalGateway.GatewayPathDecider(tupleList, fogList, CommunicationType, Service, DataCenter, true);
                }
                else
                {
                    s = new SmartThreadPool();
                    watch.Start();
                    foreach (var item in tupleList)
                    {
                        item.QueueDelay = watch.Elapsed.Milliseconds;
                       // FogUtility.FogSim(item, fogList, CommunicationType, Service, DataCenter);
                        s.QueueWorkItem(o => FogUtility.FogSim(item, fogList, CommunicationType, Service, DataCenter), new object());
                    }
                    watch.Stop();
                    try
                    {
                        s.WaitForIdle();
                        s.Shutdown();
                    }
                    catch { };
                }
                #endregion
            }
            else if (policy == "2")
            {
                #region P2 SJF
                var localtupleList = tupleList.OrderBy(x => x.MIPS).ToList();
                if (WithGateway)
                {
                    GlobalGateway.GatewayPathDecider(localtupleList, fogList, CommunicationType, Service, DataCenter, true);
                }
                else
                {
                    s = new SmartThreadPool();
                    s.MaxThreads = 1000;
                    s.MinThreads = 1000;
                    watch.Start();
                    foreach (var item in localtupleList)
                    {
                        item.QueueDelay = watch.Elapsed.Milliseconds;
                        s.QueueWorkItem(o => FogUtility.FogSim(item, fogList, CommunicationType, Service, DataCenter), new object());
                    }
                    watch.Stop();
                    try
                    {
                        s.WaitForIdle();
                        s.Shutdown();
                    }
                    catch { };
                }
                #endregion
            }
            else if (policy == "3")
            {
                #region P3 LJF
                var localtupleList = tupleList.OrderByDescending(x => x.MIPS).ToList();
                if (WithGateway)
                {
                    GlobalGateway.GatewayPathDecider(localtupleList, fogList, CommunicationType, Service, DataCenter, true);
                }
                else
                {
                    s = new SmartThreadPool();
                    s.MaxThreads = 1000;
                    s.MinThreads = 1000;
                    watch.Start();
                    foreach (var item in localtupleList)
                    {
                        item.QueueDelay = watch.Elapsed.Milliseconds;
                        //FogUtility.FogSim(item, fogList, CommunicationType, Service, DataCenter);
                        s.QueueWorkItem(o => FogUtility.FogSim(item, fogList, CommunicationType, Service, DataCenter), new object());
                    }
                    watch.Stop();
                    try
                    {
                        s.WaitForIdle();
                        s.Shutdown();
                    }
                    catch { };
                }
                #endregion
            }
            //inserted new policy #change 
            //LBFC Learning based fog cloud
            else if (policy == "4")
            {
                //Learning based cloud fog
                #region policy 4

                try
                {
                    IsCreateCache = true;
                    s = new SmartThreadPool();
                    ///*getting 10% to inital execute*/
                    double initialRandomTuplesCount = Math.Ceiling((double)((tupleList.Count() * 10) / 100));

                    initial_tupleList = tupleList.Take(Convert.ToInt32(initialRandomTuplesCount)).ToList();
                    final_tupleList = tupleList.Skip(Convert.ToInt32(initialRandomTuplesCount)).ToList();

                    myTaskList = new List<Task>();
                    var split = LinqExtensions.Split(initial_tupleList, 5).ToList();
                    watch.Start();

                    for (int j = 0; j < split.Count(); j++)
                    {
                        foreach (var item in split[j])
                        {
                            int[] ranIndex = { 0, 1, 0, 1 };
                            var randCloudIndex = rnd.Next(ranIndex.Length);
                            if (randCloudIndex == 0)
                            {
                                //myTaskList.Add(Task.Factory.StartNew(() =>
                                //          {
                                              var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                                              FogSimulator.TupleTimings.Add(tupleTime);
                                              FogUtility.FogSim_LR(item, fogList, CommunicationType, Service, DataCenter, "0");
                                         // }));
                            }
                            else
                            {
                                //comment threading code for debugging
                                // create service
                                var service = ServicesUtility.GetServices()[Service];
                                myTaskList.Add(Task.Factory.StartNew(() =>
                                {
                                    CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter);
                                }));
                            }
                        }
                    }
                    Task.WaitAll(myTaskList.ToArray());
                    #region threadpool base

                    try
                    {
                        FogCache F_cache; CloudCache C_cache;
                        foreach (var item in final_tupleList)
                        {
                            lock (Lock)
                            {
                                F_cache = FogUtility.fogCahce.Where(x => x.DataType == item.DataType).OrderBy(x => x.InternalProcessingTime).OrderBy(x => x.link.Propagationtime).FirstOrDefault();
                                C_cache = CloudUtility.cloudCahce.Where(x => x.DataType == item.DataType).OrderBy(x => x.InternalProcessingTime).OrderBy(x => x.link.Propagationtime).FirstOrDefault();
                            }
                            bool f, c;
                            f = F_cache == null ? true : false;
                            c = C_cache == null ? true : false;
                            if (F_cache == null || C_cache == null)
                            {
                                if (f)
                                {
                                    var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                                    FogSimulator.TupleTimings.Add(tupleTime);
                                    s.QueueWorkItem(o => FogUtility.FogSim_LR(item, fogList, CommunicationType, Service, DataCenter, "0"), new object());
                                }
                                if (c)
                                {
                                    // create service
                                    var service = ServicesUtility.GetServices()[Service];
                                    s.QueueWorkItem(o => CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter), new object());
                                }
                            }
                            else
                            {
                                //for predication base
                                double _CTime = C_cache.InternalProcessingTime + C_cache.link.Propagationtime;
                                double _FTime = F_cache.InternalProcessingTime + F_cache.link.Propagationtime;

                                if (_CTime >= _FTime)
                                {
                                    var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                                    FogSimulator.TupleTimings.Add(tupleTime);
                                    try
                                    {
                                        Debug.WriteLine(" Fog Serving ID" + F_cache.FogServer);
                                        s.QueueWorkItem(o => FogUtility.FogSim_LR(item, fogList, CommunicationType, Service, DataCenter, F_cache.FogServer.ToString()), new object());
                                        //FogUtility.FogSim_LR(item, fogList, CommunicationType, Service, DataCenter, F_cache.FogServer.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        throw ex;
                                    }
                                }
                                else
                                {
                                    #region Cloud

                                    // create service
                                    var service = ServicesUtility.GetServices()[Service];
                                    #endregion
                                    s.QueueWorkItem(o => CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter), new object());

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

                    #endregion
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(ex.Message);
                }
                //end
            }

            ////policy 5

            else if (policy == "5")
            {
                //In memory allocation
                #region policy 5
                try
                {
                    IsCreateCache = true;
                    var localtupleList = tupleList.OrderBy(x => x.MIPS).ToList();
                    if (WithGateway)
                    {
                        GlobalGateway.MemoryGatewayPathDecider(localtupleList, fogList, CommunicationType, Service, DataCenter, true);
                    }
                    else
                    {
                        //commented by ali for testing 
                        s = new SmartThreadPool();
                        s.MaxThreads = 1000;
                        s.MinThreads = 1000;
                        watch.Start();
                        foreach (var item in localtupleList)
                        {
                            item.QueueDelay = watch.Elapsed.Milliseconds;
                            s.QueueWorkItem(o => FogUtility.Memory(item, fogList, CommunicationType, Service, DataCenter), new object());
                             //FogUtility.Memory(item, fogList, CommunicationType, Service, DataCenter);
                        }
                        watch.Stop();
                        try
                        {
                            s.WaitForIdle();
                            s.Shutdown();
                        }
                        catch { };
                    }
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(ex.Message);
                }
                #endregion

            }
            // Policy 6
            else if (policy == "6")
            {
                #region policy 6
                try
                {
                    //s = new SmartThreadPool();
                    double[] nodes = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
                    var prepathLink = AppDomain.CurrentDomain.BaseDirectory;
                    List<NodeLinkList> linklist;
                    using (StreamReader r = new StreamReader(prepathLink + "//" + "INodes.json"))
                    {
                        string json = r.ReadToEnd();
                        fogList = JsonConvert.DeserializeObject<List<FogDevice>>(json);
                    }

                    // fetching nodes
                    // path = (new FileInfo(Path.Combine(FileInformation.GetDirectory(), "LinkSeq.json"))).ToString();
                    using (StreamReader r = new StreamReader(prepathLink + "//" + "LinkSeq.json"))
                    {
                        string json = r.ReadToEnd();
                        linklist = JsonConvert.DeserializeObject<List<NodeLinkList>>(json);
                    }

                    // getting 100 jobs
                    List<Tuple> Tuples = tupleList;//.Take(100).ToList();
                    // setting the Source and Destinations
                    foreach (var job in Tuples)
                    {
                        string S = nodes[rnd.Next(nodes.Length)].ToString();
                        string D = nodes[rnd.Next(nodes.Length)].ToString();
                        job.Source = S;
                        if (D != S)//define destination
                            job.Destination = D;
                        else
                        {
                            if (Convert.ToInt32(D) == 15)
                                job.Destination = (Convert.ToInt32(D) - 1).ToString();
                            else if (Convert.ToInt32(D) != 1)
                                job.Destination = (Convert.ToInt32(D) - 1).ToString();
                            else
                                job.Destination = (Convert.ToInt32(D) + 1).ToString();
                        }

                    }
                    List<NodeLinkList> FinalLinks = FogUtility.LinkRouting(fogList, Tuples, linklist).ToList();

                    watch.Start();
                    foreach (var item in tupleList.Take(100).ToList())
                    {
                        // Message
                        // Relibility Maximum

                        item.QueueDelay = watch.Elapsed.Milliseconds;
                        //s.QueueWorkItem(o => FogUtility.MPRouting(item, FinalLinks), new object());
                        FogUtility.MPRouting(item, FinalLinks);
                    }
                    watch.Stop();
                    //try
                    //{
                    //    s.WaitForIdle();
                    //    s.Shutdown();
                    //}
                    //catch { };
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(ex.Message);
                }
                #endregion
            }
            else if (policy == "7")
            {
                //Round Robin
                #region Policy 7
                try
                {
                    //tupleList = tupleList.ToList().GetRange(6186,10);
                    tupleList = tupleList.ToList();
                   

                        //O means non agent based on gatway level
                        if (GatewaypolicyType == "0" )
                        {

                            //if nodel level policy is simple round robin call this function
                            //Do Simple Round Robin
                            if (NodeLevelPolicyType == "0")
                            {
                                FogUtility.GenerateNodeLevelQueuesSimpleRoundRobin(tupleList, fogList, CommunicationType, Service, DataCenter, WithGateway);


                            }

                            //Do Mean Round Robin
                            else
                            {
                            // FogUtility.GenerateNodeLevelQueuesOnMeanRoundRRobinBasis(tupleList, fogList, CommunicationType, Service, DataCenter, WithGateway);
                            FogUtility.GenerateNodeLevelQueuesOnMeanRoundRRobinBasisWithMultiThreading(tupleList, fogList, CommunicationType, Service, DataCenter, WithGateway);


                        }

                        }

                        else
                        {

                            var hPTuples = new Queue<Tuple>(tupleList.Where(x => x.Priority == "high" && x.IsServed == false).ToList());
                            var mPTuples = new Queue<Tuple>(tupleList.Where(x => x.Priority == "medium" && x.IsServed == false).ToList());
                            var lpTuples = new Queue<Tuple>(tupleList.Where(x => x.Priority != "medium" && x.Priority != "high" && x.IsServed == false).ToList());





                            if (NodeLevelPolicyType == "0")
                            {
                                FogUtility.GenerateNodeLevelQueuesSimpleRoundRobin(hPTuples, fogList, CommunicationType, Service, DataCenter, WithGateway);

                                mPTuples = new Queue<Tuple>(tupleList.Where(x => x.Priority == "medium" && x.IsServed == false).ToList());
                                lpTuples = new Queue<Tuple>(tupleList.Where(x => x.Priority != "medium" && x.Priority != "high" && x.IsServed == false).ToList());
                                FogUtility.GenerateNodeLevelQueuesSimpleRoundRobin(mPTuples, fogList, CommunicationType, Service, DataCenter, WithGateway);
                                FogUtility.GenerateNodeLevelQueuesSimpleRoundRobin(lpTuples, fogList, CommunicationType, Service, DataCenter, WithGateway);

                            }

                            else
                            {
                                FogUtility.GenerateNodeLevelQueuesOnMeanRoundRRobinBasis(hPTuples, fogList, CommunicationType, Service, DataCenter, WithGateway);

                                //foreach (var item in fogList)
                                //{

                                //    item.MaxCapacity = MaxFogCapacity;
                                //}
                                FogUtility.GenerateNodeLevelQueuesOnMeanRoundRRobinBasis(mPTuples, fogList, CommunicationType, Service, DataCenter, WithGateway);
                                //foreach (var item in fogList)
                                //{

                                //    item.MaxCapacity = MaxFogCapacity;
                                //}
                                FogUtility.GenerateNodeLevelQueuesOnMeanRoundRRobinBasis(lpTuples, fogList, CommunicationType, Service, DataCenter, WithGateway);

                            }


                        }

                    

                   



              
                }
                catch (Exception ex)
                {

                    throw new ArgumentException(ex.Message);
                }
                #endregion
            }
            else
            {
                #region random
                try
                {
                    var split = LinqExtensions.Split(tupleList, 16).ToList();
                    watch.Start();
                    if (WithGateway)
                    {
                        GlobalGateway.GatewayPathDecider(tupleList, fogList, CommunicationType, Service, DataCenter, false);
                    }
                    else
                    {
                        s = new SmartThreadPool();
                        s.MaxThreads = 1000;
                        s.MinThreads = 1000;
                        for (int j = 0; j < split.Count(); j++)
                        {
                            foreach (var item in split[j])
                            {
                                var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                                s.QueueWorkItem(o => FogUtility.FogSim(item, fogList, CommunicationType, Service, DataCenter), new object());
                                //FogUtility.FogSim(item, fogList, CommunicationType, Service, DataCenter);
                            }
                        }

                        try
                        {
                            s.WaitForIdle();
                            s.Shutdown();
                        }
                        catch { };
                    }
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
                if (resultList != null)
                {
                    Excel.CreateExcelSheetForFog(resultList, FogTimings.ToList(), TupleTimings.ToList());
                }
                if (CloudSimulator.resultList.ToList().Count() > 0)
                    Excel.CreateExcelSheetForCloud(CloudSimulator.resultList.ToList());
            }
            else
            {
                Excel.CreateExcelSheetForFog(resultList, FogTimings.ToList(), TupleTimings.ToList());
            }
        }
        public static FogDevice FindAvailableDevice(List<FogDevice> fogListwithDistance, SFog.Models.Tuple tuple)
        {
            List<FogDevice> newfogListwithDistance = new List<FogDevice>();
            lock (Lock)
            {



                foreach (var device in fogList)
                {
                    var newDevice = device;
                    newDevice.DistanceFromTuple = fogListwithDistance.FirstOrDefault(x => x.ID.Equals(device.ID)).DistanceFromTuple;
                    newfogListwithDistance.Add(newDevice);
                }
                var availableFogDevices = newfogListwithDistance.Where(x => (x.MIPS - x.CurrentAllocatedMips) > tuple.MIPS).ToList();

                var availableFogDevice = newfogListwithDistance.Where(x => (x.MIPS - x.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();//.OrderBy(x => rnd.Next()).Take(1).First();// fogList[rnd.Next(fogList.Count)]; 
                if (availableFogDevice != null)
                {
                    var fogDevice = fogList.FirstOrDefault(x => x.ID.Equals(availableFogDevice.ID));
                    if (fogDevice != null)
                    {
                        //   if (fogDevice.CurrentAllocatedBw <= fogDevice.UpBW &&
                        //   fogDevice.CurrentAllocatedMips <= fogDevice.MIPS &&
                        //   fogDevice.CurrentAllocatedRam <= fogDevice.RAM &&
                        //   fogDevice.CurrentAllocatedSize <= fogDevice.Size)

                        var availableMips = fogDevice.MIPS - fogDevice.CurrentAllocatedMips;
                        if (availableMips > tuple.MIPS)
                        {
                            fogDevice.CurrentAllocatedMips = fogDevice.CurrentAllocatedMips + tuple.MIPS;
                            availableFogDevice.CurrentAllocatedMips = fogDevice.CurrentAllocatedMips;
                            return availableFogDevice;
                        }
                        else { }
                    }
                    else { }
                }
                else { }
                return availableFogDevice;
            }
        }
        public static List<FogDevice> getList() { return fogList; }
    }
}