using Newtonsoft.Json;
using SFog.Models;
using SFog.Models.Archives;
using SFog.Models.Brokers;
using SFog.Models.Cache;
using SFog.Models.Links;
using SFog.Models.Nodes;
using SFog.Models.Queues;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using Amib.Threading;
using System.Threading.Tasks;
namespace SFog.Business.Utilities.Fog
{
    public static class FogUtility
    {
        public static List<CollectionLinks> tempLInkList = new List<CollectionLinks>();
        public static double cooperSpeed = 197863.022;
        public static double transmitionRate = 100;
        public static ConcurrentBag<Link> linksList = new ConcurrentBag<Link>();
        public static ConcurrentBag<FogCache> fogCahce = new ConcurrentBag<FogCache>();
        public static bool IsRoute = false;
        private static Object Lock = new Object();
        //  public static ConcurrentBag<Models.Tuple> Buffer = new ConcurrentBag<Models.Tuple>();
        public static void FogSim(Models.Tuple tuple, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter)
        {
            Debug.WriteLine("Fog Sim" + tuple.ID);
            try
            {
                Stopwatch watch = new Stopwatch();
                var result = new Results();
                watch.Start();
                result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();
                var Broker = new FogBroker()
                {
                    FogList = fogList,
                    Tuple = tuple
                };

                Broker.SelectedFogDevice = FogBrokerUtility.GetValidFogDevice(Broker.Tuple, Broker.FogList);
                if (Broker.SelectedFogDevice != null)
                {
                    var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = Broker.SelectedFogDevice.Name, TupleName = Broker.Tuple.Name };
                    // var actingServer = new ActingServer() { ServerName = Broker.SelectedFogDevice.Name, TupleName = tuple.Name, StartTime = fogTime.TaskArrival };
                    lock (Lock)
                    {
                        result.Link = linksList.Where(x => x.Source == tuple.Name && x.Destination == Broker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                        result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                        result.FogBroker = Broker;
                    }
                    tuple.IsServed = true;
                    tuple.IsServerFound = true;
                    string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                    var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = tuple.Name };
                    // it will execute on policy 4
                    if (FogSimulator.IsCreateCache)
                    {
                        lock (fogCahce)
                        {
                            fogCahce.Add(new FogCache() { DataType = tuple.DataType, FogServer = Broker.SelectedFogDevice.ID, InternalProcessingTime = tuple.InternalProcessingTime, TupleGuid = tuple.ID, link = result.Link });
                        }
                    }
                    #region fog utilization
                    //finding fog utilization
                    Debug.Write("Tuple processig Time " + tuple.Name + " " + tuple.InternalProcessingTime);
                    lock (Lock)
                    {
                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, tuple);
                        fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                        fogTime.FreeTime = tupleTime.TupleDeparture;
                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                        FogSimulator.FogTimings.Add(fogTime);
                        FogSimulator.TupleTimings.Add(tupleTime);


                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, tuple);
                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, Broker.Tuple, fogList);
                        result.ElapsedTime = (result.Link.Propagationtime + tuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                    }
                    #endregion
                    FogSimulator.resultList.Add(result);
                 
                }
                else
                {
                    Debug.WriteLine("missed by fog");
                    // because it these values are already set in GetValidFogDevice()
                    // tuple.IsReversed = true;
                    // tuple.IsServerFound = false;
                    if (CommunicationType == 1)
                    {
                        lock (Lock)
                        {
                            // communicate with cloud if fog device is not able to serve.
                            if (CloudSimulator.CloudSimulationForFog(tuple, false, Service, DataCenter))
                            {
                                tuple.IsReversed = true;
                                tuple.IsCloudServed = true;
                                tuple.IsServedByFC_Cloud = true;
                            }
                        }
                    }
                    else
                    {
                        tuple.IsCloudServed = false;
                        tuple.IsReversed = true; //even cloud could not served the request.
                    }
                }
                watch.Stop();
                result.Tuple = tuple;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

        }
        public static void FogSim_LR(Models.Tuple tuple, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter, string SERVER_ID)
        {
            Debug.WriteLine("Fog Sim" + tuple.ID);
            try
            {
                // if (FogSimulator.WithGateway)
                //  {
                //    if (GlobalGateway.SendToCloud(tuple) && CommunicationType == 1)
                //    {
                //        var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = tuple.Name };
                //        lock (Lock)
                //            FogSimulator.TupleTimings.Add(tupleTime);
                //        CloudSimulator.CloudSimulationForFog(tuple, false, Service, DataCenter);
                //        tuple.IsServedByFC_Cloud = false;
                //    }
                // }

                Stopwatch watch = new Stopwatch();
                var result = new Results();
                watch.Start();
                result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();
                var Broker = new FogBroker()
                {
                    FogList = fogList,
                    Tuple = tuple
                };

                Broker.SelectedFogDevice = FogBrokerUtility.GetValidFogDevice_LR(Broker.Tuple, Broker.FogList, SERVER_ID);
                if (Broker.SelectedFogDevice != null)
                {
                    var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = Broker.SelectedFogDevice.Name, TupleName = Broker.Tuple.Name };
                    lock (Lock)
                    {
                        result.Link = linksList.Where(x => x.Source == tuple.Name && x.Destination == Broker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                        result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                        result.FogBroker = Broker;
                    }
                    tuple.IsServed = true;
                    tuple.IsServerFound = true;
                    string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                    var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = tuple.Name };
                    // it will execute on policy 4
                    if (FogSimulator.IsCreateCache)
                    {
                        lock (fogCahce)
                        {
                            fogCahce.Add(new FogCache() { DataType = tuple.DataType, FogServer = Broker.SelectedFogDevice.ID, InternalProcessingTime = tuple.InternalProcessingTime, TupleGuid = tuple.ID, link = result.Link });
                        }
                    }
                    #region fog utilization
                    //finding fog utilization
                    Debug.Write("Tuple processig Time " + tuple.InternalProcessingTime);
                    lock (Lock)
                    {
                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, tuple);
                        fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                        fogTime.FreeTime = tupleTime.TupleDeparture;
                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                        FogSimulator.FogTimings.Add(fogTime);
                        FogSimulator.TupleTimings.Add(tupleTime);
                        #endregion
                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, tuple);
                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, Broker.Tuple, fogList);
                        result.ElapsedTime = (result.Link.Propagationtime + tuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                        FogSimulator.resultList.Add(result);
                    }
                }
                else
                {
                    Debug.WriteLine("missed by fog using LRFC");
                    // because it these values are already set in GetValidFogDevice()
                    // tuple.IsReversed = true;
                    // tuple.IsServerFound = false;
                    if (CommunicationType == 1)
                    {
                        lock (Lock)
                        {
                            // communicate with cloud if fog device is not able to serve.
                            if (CloudSimulator.CloudSimulationForFog(tuple, false, Service, DataCenter))
                            {
                                tuple.IsReversed = true;
                                tuple.IsCloudServed = true;
                                tuple.IsServedByFC_Cloud = true;
                            }
                        }
                    }
                    else
                    {
                        tuple.IsCloudServed = false;
                        tuple.IsReversed = true; //even cloud could not served the request.
                    }
                }
                watch.Stop();
                result.Tuple = tuple;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

        }

        /// <summary>
        /// Edge simulation
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="edgeList same class used as fog"></param>
        /// <param name="CommunicationType"></param>
        /// <param name="Service"></param>
        /// <param name="DataCenter"></param>
        public static void EdgeSim(Models.Tuple tuple, List<FogDevice> edgeList, int CommunicationType)
        {
            Debug.WriteLine("Edge Sim" + tuple.ID);
            try
            {
                Stopwatch watch = new Stopwatch();
                var result = new Results();
                // List<Models.Tuple> buffer = null;

                watch.Start();
                result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();
                var EdgeBroker = new FogBroker()
                {
                    FogList = edgeList,
                    Tuple = tuple,// T1,T2,T3,T4
                };
                EdgeBroker.SelectedFogDevice = FogBrokerUtility.GetValidEdgeDevice(tuple, EdgeBroker.FogList);

                if (EdgeBroker.SelectedFogDevice != null)
                {
                    // lock (Lock)
                    {
                        var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = EdgeBroker.SelectedFogDevice.Name, TupleName = tuple.Name };
                        lock (Lock)
                        {
                            result.Link = linksList.Where(x => x.Source == tuple.Name && x.Destination == EdgeBroker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                            result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                            result.FogBroker = EdgeBroker;
                        }
                        tuple.IsServed = true;
                        tuple.IsServerFound = true;
                        string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                        var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = tuple.Name };
                        // it will execute on policy 4
                        if (EdgeSimulator.IsCreateCache)
                        {
                            lock (EdgeSimulator.EdgeCahce)
                            {
                                EdgeSimulator.EdgeCahce.Add(new FogCache() { DataType = tuple.DataType, FogServer = EdgeBroker.SelectedFogDevice.ID, InternalProcessingTime = tuple.InternalProcessingTime, TupleGuid = tuple.ID, link = result.Link });
                            }
                        }
                        #region Edge utilization
                        //finding fog utilization
                        Debug.Write("Tuple processig Time " + tuple.InternalProcessingTime);
                        lock (Lock)
                        {
                            fogTime.Consumption = PowerUtility.Consumption(EdgeBroker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, tuple);
                            fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                            fogTime.FreeTime = tupleTime.TupleDeparture;
                            fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(EdgeBroker.SelectedFogDevice, edgeList);

                            EdgeSimulator.FogTimings.Add(fogTime);
                            EdgeSimulator.TupleTimings.Add(tupleTime);

                            #endregion
                            PowerUtility.ReleasePower(EdgeBroker.SelectedFogDevice, tuple);
                            ResourceUtility.ResourceReleaseChanged(EdgeBroker.SelectedFogDevice, tuple, edgeList);
                            result.ElapsedTime = (result.Link.Propagationtime + tuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                        }
                        EdgeSimulator.edgeResultList.Add(result);
                        // result.Tuple = tuple;
                    }
                }
                else
                {
                    Debug.WriteLine("missed by edge");

                    if (CommunicationType == 1)
                    {
                        lock (Lock)
                        {
                            // communicate with  fog device  if edge  is not able to serve.
                            if (FogSimulationForEdge(tuple, EdgeSimulator.fogServers))
                            {
                                tuple.IsReversed = false;
                                tuple.IsCloudServed = true;
                                tuple.IsServedByFC_Cloud = true;
                                tuple.IsServed = true;
                            }
                            else EdgeSimulator.DropedtupleList.Add(tuple);
                        }
                    }
                    else
                    {
                        Debug.Write("Adding in Dropped");
                        tuple.IsCloudServed = false;
                        tuple.IsServedByFC_Cloud = false;

                        tuple.IsReversed = true; //fog act as cloud in the request.
                        tuple.IsServed = false;
                        EdgeSimulator.DropedtupleList.Add(tuple);
                    }

                }
                watch.Stop();
                result.Tuple = tuple;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

        }
        private static void Execute(Models.Tuple tuple, List<FogDevice> edgeList, Stopwatch watch, Results result, FogBroker EdgeBroker)
        {
            var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = EdgeBroker.SelectedFogDevice.Name, TupleName = tuple.Name };
            lock (Lock)
            {
                result.Link = linksList.Where(x => x.Source == tuple.Name && x.Destination == EdgeBroker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                result.FogBroker = EdgeBroker;
            }
            tuple.IsServed = true;
            tuple.IsServerFound = true;
            string endtime = watch.Elapsed.TotalMilliseconds.ToString();
            var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = tuple.Name };
            // it will execute on policy 4
            if (EdgeSimulator.IsCreateCache)
            {
                lock (EdgeSimulator.EdgeCahce)
                {
                    EdgeSimulator.EdgeCahce.Add(new FogCache() { DataType = tuple.DataType, FogServer = EdgeBroker.SelectedFogDevice.ID, InternalProcessingTime = tuple.InternalProcessingTime, TupleGuid = tuple.ID, link = result.Link });
                }
            }
            #region Edge utilization
            //finding fog utilization
            Debug.Write("Tuple processig Time " + tuple.InternalProcessingTime);
            lock (Lock)
            {
                fogTime.Consumption = PowerUtility.Consumption(EdgeBroker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, tuple);
                fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                fogTime.FreeTime = tupleTime.TupleDeparture;
                fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(EdgeBroker.SelectedFogDevice, edgeList);
                EdgeSimulator.FogTimings.Add(fogTime);
                EdgeSimulator.TupleTimings.Add(tupleTime);

                #endregion
                PowerUtility.ReleasePower(EdgeBroker.SelectedFogDevice, tuple);
                ResourceUtility.ResourceReleaseChanged(EdgeBroker.SelectedFogDevice, tuple, edgeList);
                result.ElapsedTime = (result.Link.Propagationtime + tuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
            }
            EdgeSimulator.edgeResultList.Add(result);
        }
        public static bool FogSimulationForEdge(Models.Tuple tuple, List<FogDevice> fogList)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                var result = new Results();
                bool returnValue = false;
                watch.Start();
                result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();
                var Broker = new FogBroker()
                {
                    FogList = fogList,
                    Tuple = tuple
                };

                Broker.SelectedFogDevice = FogBrokerUtility.GetValidFogDeviceFoEdgeFog(Broker.Tuple, Broker.FogList);
                if (Broker.SelectedFogDevice != null)
                {
                    var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = Broker.SelectedFogDevice.Name, TupleName = Broker.Tuple.Name };
                    lock (Lock)
                    {
                        result.Link = linksList.Where(x => x.Source == tuple.Name && x.Destination == Broker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                        result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                        result.FogBroker = Broker;
                    }
                    tuple.IsServed = true;
                    tuple.IsServerFound = true;
                    tuple.IsReversed = false;
                    // in this scenario Fog act as cloud
                    tuple.IsCloudServed = true;
                    tuple.IsServedByFC_Cloud = true;
                    //end here
                    string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                    var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = tuple.Name };
                    // it will execute on policy 4
                    if (EdgeSimulator.IsCreateCache)
                    {
                        lock (fogCahce)
                        {
                            fogCahce.Add(new FogCache() { DataType = tuple.DataType, FogServer = Broker.SelectedFogDevice.ID, InternalProcessingTime = tuple.InternalProcessingTime, TupleGuid = tuple.ID, link = result.Link });
                        }
                    }
                    #region fog utilization
                    //finding fog utilization
                    Debug.Write("Tuple processig Time " + tuple.InternalProcessingTime);
                    lock (Lock)
                    {
                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, tuple);
                        fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                        fogTime.FreeTime = tupleTime.TupleDeparture;
                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                        FogSimulator.FogTimings.Add(fogTime);
                        FogSimulator.TupleTimings.Add(tupleTime);

                        #endregion
                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, tuple);
                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, Broker.Tuple, fogList);
                        result.ElapsedTime = (result.Link.Propagationtime + tuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                    }
                    FogSimulator.resultList.Add(result);
                    returnValue = true;
                }
                else
                {
                    Debug.WriteLine("missed by fog EdgeSimulator.DropedtupleList.Add");
                    tuple.IsCloudServed = false;
                    tuple.IsReversed = true; //even cloud could not served the request.
                    tuple.IsServed = false;
                }
                watch.Stop();
                result.Tuple = tuple;
                return returnValue;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message.ToString());
            }
        }
        public static void Memory(Models.Tuple tuple, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                var result = new Results();
                watch.Start();
                result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();
                var Broker = new FogBroker()
                {
                    FogList = fogList,
                    Tuple = tuple
                };

                //at first iteration there is no device in cache and cache is empty after first iteration it adds fog device to cache and when next job comes
                //it first checks if required device is in cache and serves the job from device in cache
                if (fogCahce.Where(x => x.DataType == Broker.Tuple.DataType).OrderBy(x => x.InternalProcessingTime).OrderBy(x => x.link.Propagationtime).Count() > 0)
                {
                    //work here
                    Broker.SelectedFogDevice = FogBrokerUtility.ValidMemoryDevice(Broker.Tuple, Broker.FogList, fogCahce.Where(x => x.DataType == Broker.Tuple.DataType).OrderBy(x => x.InternalProcessingTime).OrderBy(x => x.link.Propagationtime).FirstOrDefault());
                }
                else
                {
                    Broker.SelectedFogDevice = FogBrokerUtility.GetValidMemoryDevice(Broker.Tuple, Broker.FogList);
                }


                if (Broker.SelectedFogDevice != null)
                {
                    var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = Broker.SelectedFogDevice.Name, TupleName = Broker.Tuple.Name };
                    // var actingServer = new ActingServer() { ServerName = Broker.SelectedFogDevice.Name, TupleName = tuple.Name, StartTime = fogTime.TaskArrival };
                    try
                    {
                        lock (Lock)
                        {
                            result.Link = linksList.Where(x => x.Source == tuple.Name && x.Destination == Broker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                            result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                            result.FogBroker = Broker;
                        }

                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                    tuple.IsServed = true;
                    tuple.IsServerFound = true;
                    string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                    var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = tuple.Name };
                    // it will execute on policy 4/5 both
                    if (FogSimulator.IsCreateCache)
                    {
                        lock (fogCahce)
                        {
                            fogCahce.Add(new FogCache() { DataType = tuple.DataType, FogServer = Broker.SelectedFogDevice.ID, InternalProcessingTime = tuple.InternalProcessingTime, TupleGuid = tuple.ID, link = result.Link });
                        }
                    }
                    #region fog utilization
                    //finding fog utilization
                    Debug.Write("Tuple processig Time " + tuple.InternalProcessingTime);
                    lock (Lock)
                    {
                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, tuple);
                        fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                        fogTime.FreeTime = tupleTime.TupleDeparture;
                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                        FogSimulator.FogTimings.Add(fogTime);
                        FogSimulator.TupleTimings.Add(tupleTime);


                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, tuple);
                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, Broker.Tuple, fogList);
                        result.ElapsedTime = (result.Link.Propagationtime + tuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                    }
                    #endregion
                    FogSimulator.resultList.Add(result);
                }
                else
                {
                    Debug.WriteLine("missed by fog");
                    // because it these values are already set in GetValidFogDevice()
                    // tuple.IsReversed = true;
                    // tuple.IsServerFound = false;
                    if (CommunicationType == 1)
                    {
                        lock (Lock)
                        {
                            // communicate with cloud if fog device is not able to serve.
                            if (CloudSimulator.CloudSimulationForFog(tuple, false, Service, DataCenter))
                            {
                                tuple.IsReversed = true;
                                tuple.IsCloudServed = true;
                                tuple.IsServedByFC_Cloud = true;
                            }
                        }
                    }
                    else
                    {
                        tuple.IsCloudServed = false;
                        tuple.IsReversed = true; //even cloud could not served the request.
                    }
                }
                watch.Stop();
                result.Tuple = tuple;
            }
            catch (Exception ex)
            {

                throw new ArgumentException(ex.Message);
            }
        }
        public static void MPRouting(Models.Tuple tuple, List<NodeLinkList> finallinks)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                var result = new Results();
                List<CurrentPaths> matchingLinks = new List<CurrentPaths>();
                List<int> vN = new List<int>();
                watch.Start();
                result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();
                NodeLinkList LinkedList = finallinks.Where(a => a.Head == tuple.Source).FirstOrDefault();

                string _Source = tuple.Source;//3 direct links of three 7 4 12 15 1
                string _Destination = tuple.Destination; //12

                // get topology paths
                vN.Add(Convert.ToInt32(tuple.Source));
                AddPaths(matchingLinks, LinkedList, Convert.ToInt32(tuple.Source));

                foreach (var node in LinkedList.Nodes)
                {
                    AddPaths(matchingLinks, LinkedList, node);
                    vN.Add(Convert.ToInt32(node));
                    if (LinkedList.Nodes.Contains(Convert.ToInt32(tuple.Destination)))
                    {
                        AddPaths(matchingLinks, LinkedList, Convert.ToInt32(tuple.Destination));
                        var link = new CollectionLinks();
                        link.Paths = matchingLinks;
                        Debug.WriteLine("Insertion value" + node);

                        FogSimulator.CompleteLinkPath.Add(link);
                        tempLInkList.Add(link);
                        matchingLinks = null;

                    }
                    else
                        RecurionPath(tuple, finallinks, matchingLinks, vN, node);

                    //RecurionPath(tuple, finallinks, matchingLinks, vN, node);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        private static bool RecurionPath(Models.Tuple tuple, List<NodeLinkList> finallinks, List<CurrentPaths> matchingLinks, List<int> vN, int node)
        {
            NodeLinkList Llt = finallinks.Where(a => a.Head == node.ToString()).FirstOrDefault();
            if (Llt.Nodes.Contains(Convert.ToInt32(tuple.Source)))
            {
                Llt.Nodes.Remove(Convert.ToInt32(tuple.Source));
            }
            if (node == Convert.ToInt32(tuple.Destination))
            {
                AddPaths(matchingLinks, Llt, Convert.ToInt32(Llt.Head));
                var link = new CollectionLinks();
                link.Paths = matchingLinks;
                FogSimulator.CompleteLinkPath.Add(link);
                Debug.WriteLine("Insertion value" + node);

                tempLInkList.Add(link);

                matchingLinks = null;

            }
            else
            {
                foreach (var item in Llt.Nodes)
                {
                    if (!vN.Contains(Convert.ToInt32(item)))
                    {
                        AddPaths(matchingLinks, Llt, item);
                        Debug.WriteLine("item.Head" + item);
                        vN.Add(Convert.ToInt32(item));
                        NodeLinkList innerLinkedList = finallinks.Where(a => a.Head == item.ToString()).FirstOrDefault();
                        if (innerLinkedList.Nodes.Contains(Convert.ToInt32(tuple.Destination)))
                        {
                            AddPaths(matchingLinks, innerLinkedList, Convert.ToInt32(tuple.Destination));
                            var link = new CollectionLinks();
                            link.Paths = matchingLinks;
                            Debug.WriteLine("Insertion value" + item);
                            FogSimulator.CompleteLinkPath.Add(link);

                            tempLInkList.Add(link);

                            matchingLinks = null;
                            break;

                        }
                        else
                        {
                            RecurionPath(tuple, finallinks, matchingLinks, vN, item);
                        }
                        vN.Add(Convert.ToInt32(innerLinkedList.Head));

                    }
                }
                return false;
            }
            return false;
        }

        private static void AddPaths(List<CurrentPaths> matchingLinks, NodeLinkList LinkedList, int node)
        {
            CurrentPaths _mL = new CurrentPaths();
            _mL.Node = node.ToString();
            _mL.PLink = LinkedList.Paths.Where(a => a.PLink.Destination == node.ToString()).Select(a => a.PLink).FirstOrDefault();
            matchingLinks.Add(_mL);
        }

        public static ConcurrentBag<NodeLinkList> LinkRouting(List<FogDevice> foglist, List<Models.Tuple> tuples, List<NodeLinkList> linklist)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                Link link;
                CurrentPaths path;
                ConcurrentBag<CurrentPaths> CurrentPathsList;
                ConcurrentBag<NodeLinkList> NewNodeList = new ConcurrentBag<NodeLinkList>();

                foreach (var node in foglist)
                {
                    //Debug.WriteLine("NodeName" + node.Name);
                    NodeLinkList Nlinklst = linklist.Where(a => a.Head == node.Name).FirstOrDefault();
                    //Debug.WriteLine(": Nlinklst cOUNT" + Nlinklst.Nodes.Count + " Nlinklst Head" + Nlinklst.Head);
                    double addowntime = (2566368 + 25920);
                    // node.Availability = Math.Round((2566368 / addowntime), 3); //30 days // downtime=25920s,uptime=2566368s =//uptime/(uptime+downtime)
                    // node.Relibility = Math.Abs(node.Availability / Nlinklst.Nodes.Count);

                    CurrentPathsList = new ConcurrentBag<CurrentPaths>();
                    // foreach (var tuple in tuples)
                    //{
                    foreach (var _link in Nlinklst.Nodes)
                    {
                        FogDevice linkedNode = foglist.Where(a => a.Name == _link.ToString()).FirstOrDefault();
                        var dis = GeoDistance.CalcNodeDistance(linkedNode.GeoLocation.getLongitude(), linkedNode.GeoLocation.getLatitude(),
                                                            node.GeoLocation.getLongitude(), node.GeoLocation.getLatitude(),
                                                            GeoCodeCalcMeasurement.Kilometers);

                        node.DistanceFromTuple = 0;
                        dis = dis * 1000; //in meters
                        double PGT = LinkUtility.CalculateLatency(dis, cooperSpeed, 200, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)]);//(dis / cooperSpeed);//meters/
                                                                                                                                                   // double PGT1 = dis / PGT;
                                                                                                                                                   //  double weight = PGT1 + (200 / Nlinklst.Nodes.Count);
                                                                                                                                                   // double weggit2 = ((dis / PGT) + (200 / (node.UpBW)));
                        link = new Link()
                        {
                            Availabilty = Math.Round((2566368 / addowntime), 3),//99%
                            Reliability = Math.Abs(Math.Round((2566368 / addowntime), 3) / Nlinklst.Nodes.Count),
                            Source = node.Name,
                            Destination = _link.ToString(),
                            SDDistance = dis,
                            Propagationtime = PGT,
                            //LinkUtility.CalculateLatency(dis, cooperSpeed, 200, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)]),
                            Weight = (dis / PGT) + (200 / Nlinklst.Nodes.Count),
                            //((dis / (LinkUtility.CalculateLatency(dis, cooperSpeed, 200, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)]))) + (200 / (node.UpBW)))

                        };
                        path = new CurrentPaths
                        {
                            Node = _link.ToString(),
                            PLink = link
                        };
                        CurrentPathsList.Add(path);
                    }
                    // }
                    Nlinklst.Paths = CurrentPathsList.ToList();
                    NewNodeList.Add(Nlinklst);
                }
                return NewNodeList;
            }
            catch (Exception ex)
            {

                throw new ArgumentException(ex.Message);
            }
        }
        public static bool GenerateNodeQueues(Models.Tuple tuple, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                var result = new Results();
                watch.Start();
                result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();

                // var Broker = new FogBroker()
                // {
                //    FogList = fogList,
                //  Tuple = tuple
                //  };
                List<AllocatedServer> Aserver = new List<AllocatedServer>();

                // find the server by the nearest distance
                //  Aserver.Add(new AllocatedServer(tuple,))


                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        
        //This method is added by Ali for simple round robin on shortest job first basis
        public static bool GenerateNodeLevelQueuesSimpleRoundRobin(Queue<SFog.Models.Tuple> tuple, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter, bool WithGateway)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                var result = new Results();
                int QueueIndex = 0;
                watch.Start();
                result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();


                var NodeLevelQueue = new Queue<SFog.Models.Tuple>[fogList.Count];
                var NodeLevelQueueFinal = new Queue<SFog.Models.Tuple>[fogList.Count];
                var JobListForCloud = new List<SFog.Models.Tuple>();
                for (int i = 0; i < fogList.Count; i++)
                {

                    NodeLevelQueue[i] = new Queue<SFog.Models.Tuple>();
                    NodeLevelQueueFinal[i]= new Queue<SFog.Models.Tuple>();
                }
                foreach (var item in tuple)
                {
                    var Broker = new FogBroker()
                    {
                        FogList = fogList,
                        Tuple = item
                    };

                    Broker.SelectedFogDevice = FogBrokerUtility.GetValidFogDevice(Broker.Tuple, Broker.FogList);

                    if (Broker.SelectedFogDevice != null)
                    {
                       
                        QueueIndex = Convert.ToInt32(Broker.SelectedFogDevice.Name.Substring(Broker.SelectedFogDevice.Name.IndexOf('-') + 1));
                       
                        NodeLevelQueue[QueueIndex].Enqueue(item);

                    }

                    else
                    {

                        JobListForCloud.Add(Broker.Tuple);
                    }



                }

                //Now Sort same priority Jobs based on MIPS
                var SortedNodeLevelTupleList = new List<SFog.Models.Tuple>[fogList.Count];
                for (int i = 0; i < fogList.Count; i++)
                {

                    SortedNodeLevelTupleList[i] = NodeLevelQueue[i].OrderBy(x => x.MIPS).ToList();
                }


                for (int i = 0; i < fogList.Count; i++)
                {
                    foreach (var localTuple in SortedNodeLevelTupleList[i])
                        NodeLevelQueueFinal[i].Enqueue(localTuple);
                }

                #region JobsAllocation

                for (int i = 0; i < fogList.Count; i++)
                {
                    var Broker = new FogBroker();
                    Broker.FogList = fogList;
                    Broker.SelectedFogDevice = fogList[i];
                    //now server each tuple to his own fog device on burst timediffeerence basis

                    //List<Task> myTaskList = new List<Task>();
                    //myTaskList.Add(Task.Factory.StartNew(() =>


                    if (NodeLevelQueueFinal[i].Count > 0)
                    {
                        for (int j = 0; j <= NodeLevelQueueFinal[i].Count; j++)
                        {
                            result = new Results();
                            var localTuple = NodeLevelQueueFinal[i].Dequeue();
                            if (j > 0)
                                j--;
                            if (Broker.SelectedFogDevice != null)
                            {

                                Broker.Tuple = localTuple;
                                var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = Broker.SelectedFogDevice.Name, TupleName = localTuple.Name };
                                // var actingServer = new ActingServer() { ServerName = Broker.SelectedFogDevice.Name, TupleName = tuple.Name, StartTime = fogTime.TaskArrival };
                                lock (Lock)
                                {
                                    result.Link = linksList.Where(x => x.Source == localTuple.Name && x.Destination == Broker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                                    result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                                    result.FogBroker = Broker;
                                }

                                if (localTuple.BurstTime < Broker.SelectedFogDevice.ProcessorBurstTime)
                                    localTuple.BurstTime = 0;
                                else
                                    localTuple.BurstTime = localTuple.BurstTime - Broker.SelectedFogDevice.ProcessorBurstTime;
                                if (localTuple.BurstTime == 0)
                                {
                                    localTuple.IsServed = true;
                                    localTuple.IsServerFound = true;
                                    string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                                    var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = localTuple.Name };
                                    // it will execute on policy 4
                                    if (FogSimulator.IsCreateCache)
                                    {
                                        lock (fogCahce)
                                        {
                                            fogCahce.Add(new FogCache() { DataType = localTuple.DataType, FogServer = Broker.SelectedFogDevice.ID, InternalProcessingTime = localTuple.InternalProcessingTime, TupleGuid = localTuple.ID, link = result.Link });
                                        }
                                    }
                                    #region fog utilization
                                    //finding fog utilization
                                    Debug.Write("Tuple processig Time " + localTuple.Name + " " + localTuple.InternalProcessingTime);
                                    lock (Lock)
                                    {
                                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);
                                        fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                                        fogTime.FreeTime = tupleTime.TupleDeparture;
                                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                        FogSimulator.FogTimings.Add(fogTime);
                                        FogSimulator.TupleTimings.Add(tupleTime);


                                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                        result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                                    }
                                    #endregion
                                    FogSimulator.resultList.Add(result);
                                }
                                else
                                {
                                    lock (Lock)
                                    {
                                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);

                                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                        FogSimulator.FogTimings.Add(fogTime);



                                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                        result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                                    }

                                    //if job still not finished in his time add back to end of queue for next execution
                                    NodeLevelQueueFinal[i].Enqueue(localTuple);

                                }
                            }

                            else
                            {
                                Debug.WriteLine("missed by fog");
                                // because it these values are already set in GetValidFogDevice()
                                // tuple.IsReversed = true;
                                // tuple.IsServerFound = false;
                                if (CommunicationType == 1)
                                {
                                    lock (Lock)
                                    {
                                        // communicate with cloud if fog device is not able to serve.
                                        if (CloudSimulator.CloudSimulationForFog(localTuple, false, Service, DataCenter))
                                        {
                                            localTuple.IsReversed = true;
                                            localTuple.IsCloudServed = true;
                                            localTuple.IsServedByFC_Cloud = true;
                                        }
                                    }
                                }
                                else
                                {
                                    localTuple.IsCloudServed = false;
                                    localTuple.IsReversed = true; //even cloud could not served the request.
                                }
                            }

                            watch.Stop();
                            //only assign it to result object if its burst time is 0 and it is served
                            if (localTuple.BurstTime == 0)
                                result.Tuple = localTuple;

                        }
                        //}));



                    }

                }
                #endregion

                // List<AllocatedServer> Aserver = new List<AllocatedServer>();

                // find the server by the nearest distance
                //  Aserver.Add(new AllocatedServer(tuple,))


                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }


        //This method have 2 overloads one is for agent based and other is for simple list based on gatway level
        public static bool GenerateNodeLevelQueuesSimpleRoundRobin(List<SFog.Models.Tuple> tuple, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter, bool WithGateway)
        {
            try
            {

                if (WithGateway)
                {
                    List<Models.Tuple> listJobsSendToCloud = tuple.Where(x => x.DataType == FogSimulator.EnumDataType.Bulk.ToString() || x.DataType == FogSimulator.EnumDataType.Large.ToString()).ToList();
                    foreach (var item in listJobsSendToCloud)
                        if (GlobalGateway.SendToCloud(item) && CommunicationType == 1)
                        {
                            var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                            lock (Lock)
                                FogSimulator.TupleTimings.Add(tupleTime);
                            CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter);
                            item.IsServedByFC_Cloud = false;
                        }
                }

                if (WithGateway)
                    tuple = tuple.Where(x => x.DataType != FogSimulator.EnumDataType.Bulk.ToString() || x.DataType != FogSimulator.EnumDataType.Large.ToString()).ToList();
                Stopwatch watch = new Stopwatch();
                var result = new Results();
                int QueueIndex = 0;
                watch.Start();

                result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();


                var NodeLevelQueue = new Queue<SFog.Models.Tuple>[fogList.Count];
                var NodeLevelQueueFinal = new Queue<SFog.Models.Tuple>[fogList.Count];
                var JobListForCloud = new List<SFog.Models.Tuple>();
                for (int i = 0; i < fogList.Count; i++)
                {

                    NodeLevelQueue[i] = new Queue<SFog.Models.Tuple>();
                    NodeLevelQueueFinal[i] = new Queue<SFog.Models.Tuple>();
                }
                foreach (var item in tuple)
                {
                    var Broker = new FogBroker()
                    {
                        FogList = fogList,
                        Tuple = item
                    };

                    Broker.SelectedFogDevice = FogBrokerUtility.GetValidFogDevice(Broker.Tuple, Broker.FogList);

                    if (Broker.SelectedFogDevice != null)
                    {

                        QueueIndex = Convert.ToInt32(Broker.SelectedFogDevice.Name.Substring(Broker.SelectedFogDevice.Name.IndexOf('-') + 1));

                        NodeLevelQueue[QueueIndex].Enqueue(item);

                    }

                    else
                    {

                        JobListForCloud.Add(Broker.Tuple);
                    }



                }

                //Now Sort same priority Jobs based on MIPS
                var SortedNodeLevelTupleList = new List<SFog.Models.Tuple>[fogList.Count];
                for (int i = 0; i < fogList.Count; i++)
                {

                    SortedNodeLevelTupleList[i] = NodeLevelQueue[i].OrderBy(x => x.MIPS).ToList();
                }


                for (int i = 0; i < fogList.Count; i++)
                {
                    foreach (var localTuple in SortedNodeLevelTupleList[i])
                        NodeLevelQueueFinal[i].Enqueue(localTuple);
                }

                #region JobsAllocation

                for (int i = 0; i < fogList.Count; i++)
                {
                    var Broker = new FogBroker();
                    Broker.FogList = fogList;
                    Broker.SelectedFogDevice = fogList[i];
                    //now server each tuple to his own fog device on burst timediffeerence basis

                    //List<Task> myTaskList = new List<Task>();
                    //myTaskList.Add(Task.Factory.StartNew(() =>


                    if (NodeLevelQueueFinal[i].Count > 0)
                    {
                        for (int j = 0; j <= NodeLevelQueueFinal[i].Count; j++)
                        {
                            result = new Results();
                            var localTuple = NodeLevelQueueFinal[i].Dequeue();
                            if (j > 0)
                                j--;
                            if (Broker.SelectedFogDevice != null)
                            {

                                Broker.Tuple = localTuple;
                                var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = Broker.SelectedFogDevice.Name, TupleName = localTuple.Name };
                                // var actingServer = new ActingServer() { ServerName = Broker.SelectedFogDevice.Name, TupleName = tuple.Name, StartTime = fogTime.TaskArrival };
                                lock (Lock)
                                {
                                    result.Link = linksList.Where(x => x.Source == localTuple.Name && x.Destination == Broker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                                    result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                                    result.FogBroker = Broker;
                                }

                                if (localTuple.BurstTime < Broker.SelectedFogDevice.ProcessorBurstTime)
                                    localTuple.BurstTime = 0;
                                else
                                    localTuple.BurstTime = localTuple.BurstTime - Broker.SelectedFogDevice.ProcessorBurstTime;
                                if (localTuple.BurstTime == 0)
                                {
                                    localTuple.IsServed = true;
                                    localTuple.IsServerFound = true;
                                    string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                                    var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = localTuple.Name };
                                    // it will execute on policy 4
                                    if (FogSimulator.IsCreateCache)
                                    {
                                        lock (fogCahce)
                                        {
                                            fogCahce.Add(new FogCache() { DataType = localTuple.DataType, FogServer = Broker.SelectedFogDevice.ID, InternalProcessingTime = localTuple.InternalProcessingTime, TupleGuid = localTuple.ID, link = result.Link });
                                        }
                                    }
                                    #region fog utilization
                                    //finding fog utilization
                                    Debug.Write("Tuple processig Time " + localTuple.Name + " " + localTuple.InternalProcessingTime);
                                    lock (Lock)
                                    {
                                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);
                                        fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                                        fogTime.FreeTime = tupleTime.TupleDeparture;
                                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                        FogSimulator.FogTimings.Add(fogTime);
                                        FogSimulator.TupleTimings.Add(tupleTime);


                                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                        result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                                    }
                                    #endregion
                                    FogSimulator.resultList.Add(result);
                                }
                                else
                                {
                                    lock (Lock)
                                    {
                                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);

                                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                        FogSimulator.FogTimings.Add(fogTime);



                                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                        result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                                    }

                                    //if job still not finished in his time add back to end of queue for next execution
                                    NodeLevelQueueFinal[i].Enqueue(localTuple);

                                }
                            }

                            else
                            {
                                Debug.WriteLine("missed by fog");
                                // because it these values are already set in GetValidFogDevice()
                                // tuple.IsReversed = true;
                                // tuple.IsServerFound = false;
                                if (CommunicationType == 1)
                                {
                                    lock (Lock)
                                    {
                                        // communicate with cloud if fog device is not able to serve.
                                        if (CloudSimulator.CloudSimulationForFog(localTuple, false, Service, DataCenter))
                                        {
                                            localTuple.IsReversed = true;
                                            localTuple.IsCloudServed = true;
                                            localTuple.IsServedByFC_Cloud = true;
                                        }
                                    }
                                }
                                else
                                {
                                    localTuple.IsCloudServed = false;
                                    localTuple.IsReversed = true; //even cloud could not served the request.
                                }
                            }

                            watch.Stop();
                            //only assign it to result object if its burst time is 0 and it is served
                            if (localTuple.BurstTime == 0)
                                result.Tuple = localTuple;

                        }
                        //}));



                    }

                }
                #endregion

                // List<AllocatedServer> Aserver = new List<AllocatedServer>();

                // find the server by the nearest distance
                //  Aserver.Add(new AllocatedServer(tuple,))


                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }


        //This method is added by Ali for mean round robin scheduling
        public static bool GenerateNodeLevelQueuesOnMeanBasesRoundRobin(Queue<SFog.Models.Tuple> tuple, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter, bool WithGateway)
        {
            try
            {
                
                var result = new Results();
                int QueueIndex = 0;
              
               
                //global burst time for processor to assign to each job
               

                var NodeLevelQueue = new Queue<SFog.Models.Tuple>[fogList.Count];
                var NodeLevelQueueFinal= new Queue<SFog.Models.Tuple>[fogList.Count];
                var JobListForCloud = new List<SFog.Models.Tuple>();
                for (int i = 0; i < fogList.Count; i++)
                {

                    NodeLevelQueue[i] = new Queue<SFog.Models.Tuple>();
                    NodeLevelQueueFinal[i]= new Queue<SFog.Models.Tuple>();
                }
                foreach (var item in tuple)
                {
                    var Broker = new FogBroker()
                    {
                        FogList = fogList,
                        Tuple = item
                    };

                    Broker.SelectedFogDevice = FogBrokerUtility.GetValidFogDeviceMRR(Broker.Tuple, Broker.FogList);

                    if (Broker.SelectedFogDevice != null)
                    {
                        //both approaches to get substring are fine
                        //int startIndex = Broker.SelectedFogDevice.Name.IndexOf('-')+1;
                        //int endIndex = Broker.SelectedFogDevice.Name.Length - startIndex;
                        //QueueIndex = Convert.ToInt32(Broker.SelectedFogDevice.Name.Substring(startIndex, endIndex));
                        QueueIndex = Convert.ToInt32(Broker.SelectedFogDevice.Name.Substring(Broker.SelectedFogDevice.Name.IndexOf('-') + 1));
                        //NodeLevelQueue[QueueIndex] = new Queue<SFog.Models.Tuple>();
                        NodeLevelQueue[QueueIndex].Enqueue(item);

                    }

                    else
                    {

                        JobListForCloud.Add(Broker.Tuple);
                    }



                }

               
                var SortedNodeLevelTupleList = new List<SFog.Models.Tuple>[fogList.Count];
                
                for (int i = 0; i < fogList.Count; i++)
                {

                    
                    SortedNodeLevelTupleList[i] = NodeLevelQueue[i].OrderBy(x => x.MIPS).ToList();
                }

                //Now calculate mean burst time for every Queue

                //Now add mean differnces

                for (int i = 0; i < fogList.Count; i++)
                {
                    double  MeanDifferenceOfQueues = 0;
                    foreach (var jobs in SortedNodeLevelTupleList[i])
                    {

                        MeanDifferenceOfQueues = MeanDifferenceOfQueues + jobs.BurstTime;
                    }

                    //to get mean divide by number of jobs
                    MeanDifferenceOfQueues =Math.Round( MeanDifferenceOfQueues / SortedNodeLevelTupleList[i].Count(),0);
                    foreach (var jobs in SortedNodeLevelTupleList[i])
                    {

                        jobs.BurstTimeDifference = MeanDifferenceOfQueues - jobs.BurstTime;
                    }

                    //sort the list on basis of mean difference

                    SortedNodeLevelTupleList[i]= NodeLevelQueue[i].OrderByDescending(x => x.Priority).OrderByDescending(x => x.BurstTimeDifference).ToList();

                }
               
                //Now put items back to queues for assigning jobs to fogs
                for (int i = 0; i < fogList.Count; i++)
                {
                    foreach (var localTuple in SortedNodeLevelTupleList[i])
                        NodeLevelQueueFinal[i].Enqueue(localTuple);
                }


                    #region JobsAllocation

                    for (int i = 0; i < fogList.Count; i++)
                {
                    var Broker = new FogBroker();
                    Broker.FogList = fogList;
                    Broker.SelectedFogDevice = fogList[i];
                    //now server each tuple to his own fog device on burst timediffeerence basis

                    //List<Task> myTaskList = new List<Task>();
                    //myTaskList.Add(Task.Factory.StartNew(() =>


                    if (NodeLevelQueueFinal[i].Count > 0)
                    {
                        for (int j = 0; j <= NodeLevelQueueFinal[i].Count; j++)
                        {
                            result = new Results();
                            //initiate times should be when tuple initiated
                            Stopwatch watch = new Stopwatch();
                            watch.Start();
                            result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();
                            var localTuple = NodeLevelQueueFinal[i].Dequeue();
                            if(j>0)
                            j--;
                            Broker.SelectedFogDevice = FogBrokerUtility.GetValidFogDevice(localTuple, Broker.FogList);
                  
                            if (Broker.SelectedFogDevice != null)
                            {

                                Broker.Tuple = localTuple;
                                var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = Broker.SelectedFogDevice.Name, TupleName = localTuple.Name };
                                // var actingServer = new ActingServer() { ServerName = Broker.SelectedFogDevice.Name, TupleName = tuple.Name, StartTime = fogTime.TaskArrival };
                                lock (Lock)
                                {
                                    result.Link = linksList.Where(x => x.Source == localTuple.Name && x.Destination == Broker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                                    result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                                    result.FogBroker = Broker;
                                }

                                if (localTuple.BurstTime < Broker.SelectedFogDevice.ProcessorBurstTime)
                                    localTuple.BurstTime = 0;
                                else
                                    localTuple.BurstTime = localTuple.BurstTime - Broker.SelectedFogDevice.ProcessorBurstTime;
                                if (localTuple.BurstTime == 0)
                                {
                                    localTuple.IsServed = true;
                                    localTuple.IsServerFound = true;
                                    string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                                    var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = localTuple.Name };
                                    // it will execute on policy 4
                                    if (FogSimulator.IsCreateCache)
                                    {
                                        lock (fogCahce)
                                        {
                                            fogCahce.Add(new FogCache() { DataType = localTuple.DataType, FogServer = Broker.SelectedFogDevice.ID, InternalProcessingTime = localTuple.InternalProcessingTime, TupleGuid = localTuple.ID, link = result.Link });
                                        }
                                    }
                                    #region fog utilization
                                    //finding fog utilization
                                    Debug.Write("Tuple processig Time " + localTuple.Name + " " + localTuple.InternalProcessingTime);
                                    lock (Lock)
                                    {
                                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);
                                        fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                                        fogTime.FreeTime = tupleTime.TupleDeparture;
                                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                        FogSimulator.FogTimings.Add(fogTime);
                                        FogSimulator.TupleTimings.Add(tupleTime);


                                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                        result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                                    }
                                    #endregion
                                    FogSimulator.resultList.Add(result);
                                }
                                else
                                {
                                    lock (Lock)
                                    {
                                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);

                                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                        FogSimulator.FogTimings.Add(fogTime);



                                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                        result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                                    }

                                    //if job still not finished in his time add back to end of queue for next execution
                                    NodeLevelQueueFinal[i].Enqueue(localTuple);
                                   
                                }
                            }

                            else
                            {
                                Debug.WriteLine("missed by fog");
                                // because it these values are already set in GetValidFogDevice()
                                // tuple.IsReversed = true;
                                // tuple.IsServerFound = false;
                                if (CommunicationType == 1)
                                {
                                    lock (Lock)
                                    {
                                        // communicate with cloud if fog device is not able to serve.
                                        if (CloudSimulator.CloudSimulationForFog(localTuple, false, Service, DataCenter))
                                        {
                                            localTuple.IsReversed = true;
                                            localTuple.IsCloudServed = true;
                                            localTuple.IsServedByFC_Cloud = true;
                                        }
                                    }
                                }
                                else
                                {
                                    localTuple.IsCloudServed = false;
                                    localTuple.IsReversed = true; //even cloud could not served the request.
                                }

                                result.Tuple = localTuple;
                            }

                            watch.Stop();
                            //only assign it to result object if its burst time is 0 and it is served
                            if(localTuple.BurstTime==0)
                            result.Tuple = localTuple;

                        }
                    //}));



            }

                }
                    //Jobs that didnt got any fog device will be sent to cloud 
                //    foreach(var job in JobListForCloud)
                //{
                //    if (CommunicationType == 1)
                //    {
                //        lock (Lock)
                //        {
                //            // communicate with cloud if fog device is not able to serve.
                //            if (CloudSimulator.CloudSimulationForFog(job, false, Service, DataCenter))
                //            {
                //                job.IsReversed = true;
                //                job.IsCloudServed = true;
                //                job.IsServedByFC_Cloud = true;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        job.IsCloudServed = false;
                //        job.IsReversed = true; //even cloud could not served the request.
                //    }


                //    result.Tuple = job;
                //}

                #endregion

                // List<AllocatedServer> Aserver = new List<AllocatedServer>();

                // find the server by the nearest distance
                //  Aserver.Add(new AllocatedServer(tuple,))


                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        //This method is for Queue size limitation
        /// <summary>
        /// 
        /// </summary>SetValidFogDeviceMRR
        /// <param name="tuple"></param>
        /// <param name="fogList"></param>
        /// <param name="CommunicationType"></param>
        /// <param name="Service"></param>
        /// <param name="DataCenter"></param>
        /// <returns></returns>
        /// 
        //This method have 2 overloads one for agent based and other for non agent based list based on gatway level
        public static bool GenerateNodeLevelQueuesOnMeanRoundRRobinBasis(Queue<SFog.Models.Tuple> tuple, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter, bool WithGateway)
        {
            try
            {
               
                var result = new Results();
                int QueueIndex = 0;

               
                //global burst time for processor to assign to each job
                //initiate times should be when tuple initiated


                var NodeLevelQueue = new Queue<SFog.Models.Tuple>[fogList.Count];
                var NodeLevelQueueFinal = new Queue<SFog.Models.Tuple>[fogList.Count];
                var JobListForCloud = new List<SFog.Models.Tuple>();
                for (int i = 0; i < fogList.Count; i++)
                {

                    NodeLevelQueue[i] = new Queue<SFog.Models.Tuple>();
                    NodeLevelQueueFinal[i] = new Queue<SFog.Models.Tuple>();
                }
                foreach (var item in tuple)
                {
                    var Broker = new FogBroker()
                    {
                        FogList = fogList,
                        Tuple = item
                    };

                    Broker.SelectedFogDevice = FogBrokerUtility.GetValidFogDeviceMRR(Broker.Tuple, Broker.FogList);

                    if (Broker.SelectedFogDevice != null)
                    {
                        //both approaches to get substring are fine
                        //int startIndex = Broker.SelectedFogDevice.Name.IndexOf('-')+1;
                        //int endIndex = Broker.SelectedFogDevice.Name.Length - startIndex;
                        //QueueIndex = Convert.ToInt32(Broker.SelectedFogDevice.Name.Substring(startIndex, endIndex));
                        QueueIndex = Convert.ToInt32(Broker.SelectedFogDevice.Name.Substring(Broker.SelectedFogDevice.Name.IndexOf('-') + 1));
                        //NodeLevelQueue[QueueIndex] = new Queue<SFog.Models.Tuple>();
                        NodeLevelQueue[QueueIndex].Enqueue(item);
                        //Broker.SelectedFogDevice.MaxCapacity --;

                    }

                    else
                    {

                        JobListForCloud.Add(Broker.Tuple);
                    }



                }


                var SortedNodeLevelTupleList = new List<SFog.Models.Tuple>[fogList.Count];

                for (int i = 0; i < fogList.Count; i++)
                {


                    SortedNodeLevelTupleList[i] = NodeLevelQueue[i].OrderBy(x => x.MIPS).ToList();
                }

                //Now calculate mean burst time for every Queue

                //Now add mean differnces

                for (int i = 0; i < fogList.Count; i++)
                {
                    double MeanDifferenceOfQueues = 0;
                    foreach (var jobs in SortedNodeLevelTupleList[i])
                    {

                        MeanDifferenceOfQueues = MeanDifferenceOfQueues + jobs.BurstTime;
                    }

                    //to get mean divide by number of jobs
                    MeanDifferenceOfQueues = Math.Round(MeanDifferenceOfQueues / SortedNodeLevelTupleList[i].Count(), 0);
                    foreach (var jobs in SortedNodeLevelTupleList[i])
                    {

                        jobs.BurstTimeDifference = MeanDifferenceOfQueues - jobs.BurstTime;
                    }

                    //sort the list on basis of mean difference

                    SortedNodeLevelTupleList[i] = NodeLevelQueue[i].OrderByDescending(x => x.Priority).OrderByDescending(x => x.BurstTimeDifference).ToList();

                }

                //Now put items back to queues for assigning jobs to fogs
                for (int i = 0; i < fogList.Count; i++)
                {
                    foreach (var localTuple in SortedNodeLevelTupleList[i])
                        NodeLevelQueueFinal[i].Enqueue(localTuple);
                }


                #region JobsAllocation

                for (int i = 0; i < fogList.Count; i++)
                {
                    var Broker = new FogBroker();
                    Broker.FogList = fogList;
                    Broker.SelectedFogDevice = fogList[i];
                    //now server each tuple to his own fog device on burst timediffeerence basis

                    //List<Task> myTaskList = new List<Task>();
                    //myTaskList.Add(Task.Factory.StartNew(() =>


                    if (NodeLevelQueueFinal[i].Count > 0)
                    {
                        for (int j = 0; j <= NodeLevelQueueFinal[i].Count; j++)
                        {
                            result = new Results();
                            Stopwatch watch = new Stopwatch();
                            watch.Start();
                            result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();
                            var localTuple = NodeLevelQueueFinal[i].Dequeue();
                            if (j > 0)
                                j--;
                            Broker.SelectedFogDevice = FogBrokerUtility.SetValidFogDeviceMRR(localTuple, Broker.SelectedFogDevice,fogList);

                            if (Broker.SelectedFogDevice != null)
                            {

                                Broker.Tuple = localTuple;
                                var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = Broker.SelectedFogDevice.Name, TupleName = localTuple.Name };
                                // var actingServer = new ActingServer() { ServerName = Broker.SelectedFogDevice.Name, TupleName = tuple.Name, StartTime = fogTime.TaskArrival };
                                lock (Lock)
                                {
                                    result.Link = linksList.Where(x => x.Source == localTuple.Name && x.Destination == Broker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                                    result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                                    result.FogBroker = Broker;
                                }

                                if (localTuple.BurstTime < Broker.SelectedFogDevice.ProcessorBurstTime)
                                    localTuple.BurstTime = 0;
                                else
                                    localTuple.BurstTime = localTuple.BurstTime - Broker.SelectedFogDevice.ProcessorBurstTime;
                                if (localTuple.BurstTime == 0)
                                {
                                    localTuple.IsServed = true;
                                    localTuple.IsServerFound = true;
                                    string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                                    var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = localTuple.Name };
                                    // it will execute on policy 4
                                    if (FogSimulator.IsCreateCache)
                                    {
                                        lock (fogCahce)
                                        {
                                            fogCahce.Add(new FogCache() { DataType = localTuple.DataType, FogServer = Broker.SelectedFogDevice.ID, InternalProcessingTime = localTuple.InternalProcessingTime, TupleGuid = localTuple.ID, link = result.Link });
                                        }
                                    }
                                    #region fog utilization
                                    //finding fog utilization
                                    Debug.Write("Tuple processig Time " + localTuple.Name + " " + localTuple.InternalProcessingTime);
                                    lock (Lock)
                                    {
                                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);
                                        fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                                        fogTime.FreeTime = tupleTime.TupleDeparture;
                                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                        FogSimulator.FogTimings.Add(fogTime);
                                        FogSimulator.TupleTimings.Add(tupleTime);


                                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                        result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                                    }
                                    #endregion
                                    FogSimulator.resultList.Add(result);
                                }
                                else
                                {
                                    lock (Lock)
                                    {
                                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);

                                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                        FogSimulator.FogTimings.Add(fogTime);



                                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                        result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                                    }

                                    //if job still not finished in his time add back to end of queue for next execution
                                    NodeLevelQueueFinal[i].Enqueue(localTuple);

                                }
                            }

                            else
                            {
                                Debug.WriteLine("missed by fog");
                                // because it these values are already set in GetValidFogDevice()
                                // tuple.IsReversed = true;
                                // tuple.IsServerFound = false;
                                if (CommunicationType == 1)
                                {
                                    lock (Lock)
                                    {
                                        // communicate with cloud if fog device is not able to serve.
                                        if (CloudSimulator.CloudSimulationForFog(localTuple, false, Service, DataCenter))
                                        {
                                            localTuple.IsReversed = true;
                                            localTuple.IsCloudServed = true;
                                            localTuple.IsServedByFC_Cloud = true;
                                        }
                                    }
                                }
                                else
                                {
                                    localTuple.IsCloudServed = false;
                                    localTuple.IsReversed = true; //even cloud could not served the request.
                                }

                                result.Tuple = localTuple;
                            }

                            watch.Stop();
                            //only assign it to result object if its burst time is 0 and it is served
                            if (localTuple.BurstTime == 0)
                                result.Tuple = localTuple;

                        }
                        //}));



                    }

                }
              

                #endregion

               


                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }


        
        public static bool GenerateNodeLevelQueuesOnMeanRoundRRobinBasis(List<SFog.Models.Tuple> tuple, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter,bool WithGateway)
        {
            try
            {

                var result = new Results();
                int QueueIndex = 0;
                //List<SFog.Models.Tuple> tupleAfterCloud = new List<SFog.Models.Tuple>();

                //global burst time for processor to assign to each job
                //initiate times should be when tuple initiated
                if (WithGateway)
                {
                    List<Models.Tuple> listJobsSendToCloud = tuple.Where(x => x.DataType == FogSimulator.EnumDataType.Bulk.ToString() || x.DataType == FogSimulator.EnumDataType.Large.ToString()).ToList();
                    foreach(var item in listJobsSendToCloud)
                    if (GlobalGateway.SendToCloud(item) && CommunicationType == 1)
                    {
                        var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                        lock (Lock)
                            FogSimulator.TupleTimings.Add(tupleTime);
                        CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter);
                        item.IsServedByFC_Cloud = false;
                    }
                }

                if(WithGateway)
                    tuple = tuple.Where(x => x.DataType != FogSimulator.EnumDataType.Bulk.ToString() && x.DataType != FogSimulator.EnumDataType.Large.ToString()).ToList();
                var NodeLevelQueue = new Queue<SFog.Models.Tuple>[fogList.Count];
                var NodeLevelQueueFinal = new Queue<SFog.Models.Tuple>[fogList.Count];
                var JobListForCloud = new List<SFog.Models.Tuple>();
                for (int i = 0; i < fogList.Count; i++)
                {

                    NodeLevelQueue[i] = new Queue<SFog.Models.Tuple>();
                    NodeLevelQueueFinal[i] = new Queue<SFog.Models.Tuple>();
                }
                foreach (var item in tuple)
                {
                    var Broker = new FogBroker()
                    {
                        FogList = fogList,
                        Tuple = item
                    };

                    Broker.SelectedFogDevice = FogBrokerUtility.GetValidFogDeviceMRR(Broker.Tuple, Broker.FogList);

                    if (Broker.SelectedFogDevice != null)
                    {
                        //both approaches to get substring are fine
                        //int startIndex = Broker.SelectedFogDevice.Name.IndexOf('-')+1;
                        //int endIndex = Broker.SelectedFogDevice.Name.Length - startIndex;
                        //QueueIndex = Convert.ToInt32(Broker.SelectedFogDevice.Name.Substring(startIndex, endIndex));
                        QueueIndex = Convert.ToInt32(Broker.SelectedFogDevice.Name.Substring(Broker.SelectedFogDevice.Name.IndexOf('-') + 1));
                        //NodeLevelQueue[QueueIndex] = new Queue<SFog.Models.Tuple>();
                        NodeLevelQueue[QueueIndex].Enqueue(item);
                        //Broker.SelectedFogDevice.MaxCapacity --;

                    }

                    else
                    {

                        JobListForCloud.Add(Broker.Tuple);
                    }



                }


                var SortedNodeLevelTupleList = new List<SFog.Models.Tuple>[fogList.Count];

                for (int i = 0; i < fogList.Count; i++)
                {


                    SortedNodeLevelTupleList[i] = NodeLevelQueue[i].OrderBy(x => x.MIPS).ToList();
                }

                //Now calculate mean burst time for every Queue

                //Now add mean differnces

                for (int i = 0; i < fogList.Count; i++)
                {
                    double MeanDifferenceOfQueues = 0;
                    foreach (var jobs in SortedNodeLevelTupleList[i])
                    {

                        MeanDifferenceOfQueues = MeanDifferenceOfQueues + jobs.BurstTime;
                    }

                    //to get mean divide by number of jobs
                    MeanDifferenceOfQueues = Math.Round(MeanDifferenceOfQueues / SortedNodeLevelTupleList[i].Count(), 0);
                    foreach (var jobs in SortedNodeLevelTupleList[i])
                    {

                        jobs.BurstTimeDifference = MeanDifferenceOfQueues - jobs.BurstTime;
                    }

                    //sort the list on basis of mean difference

                    SortedNodeLevelTupleList[i] = NodeLevelQueue[i].OrderByDescending(x => x.Priority).OrderByDescending(x => x.BurstTimeDifference).ToList();

                }

                //Now put items back to queues for assigning jobs to fogs
                for (int i = 0; i < fogList.Count; i++)
                {
                    foreach (var localTuple in SortedNodeLevelTupleList[i])
                        NodeLevelQueueFinal[i].Enqueue(localTuple);
                }


                #region JobsAllocation

                for (int i = 0; i < fogList.Count; i++)
                {
                    var Broker = new FogBroker();
                    Broker.FogList = fogList;
                    Broker.SelectedFogDevice = fogList[i];
                    //now server each tuple to his own fog device on burst timediffeerence basis

                    //List<Task> myTaskList = new List<Task>();
                    //myTaskList.Add(Task.Factory.StartNew(() =>


                    if (NodeLevelQueueFinal[i].Count > 0)
                    {
                        for (int j = 0; j <= NodeLevelQueueFinal[i].Count; j++)
                        {
                            result = new Results();
                            Stopwatch watch = new Stopwatch();
                            watch.Start();
                            result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();
                            var localTuple = NodeLevelQueueFinal[i].Dequeue();
                            if (j > 0)
                                j--;
                            Broker.SelectedFogDevice = FogBrokerUtility.SetValidFogDeviceMRR(localTuple, Broker.SelectedFogDevice,fogList);

                            if (Broker.SelectedFogDevice != null)
                            {

                                Broker.Tuple = localTuple;
                                var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = Broker.SelectedFogDevice.Name, TupleName = localTuple.Name };
                                // var actingServer = new ActingServer() { ServerName = Broker.SelectedFogDevice.Name, TupleName = tuple.Name, StartTime = fogTime.TaskArrival };
                                lock (Lock)
                                {
                                    result.Link = linksList.Where(x => x.Source == localTuple.Name && x.Destination == Broker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                                    result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                                    result.FogBroker = Broker;
                                }

                                if (localTuple.BurstTime < Broker.SelectedFogDevice.ProcessorBurstTime)
                                    localTuple.BurstTime = 0;
                                else
                                    localTuple.BurstTime = localTuple.BurstTime - Broker.SelectedFogDevice.ProcessorBurstTime;
                                if (localTuple.BurstTime == 0)
                                {
                                    localTuple.IsServed = true;
                                    localTuple.IsServerFound = true;
                                    string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                                    var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = localTuple.Name };
                                    // it will execute on policy 4
                                    if (FogSimulator.IsCreateCache)
                                    {
                                        lock (fogCahce)
                                        {
                                            fogCahce.Add(new FogCache() { DataType = localTuple.DataType, FogServer = Broker.SelectedFogDevice.ID, InternalProcessingTime = localTuple.InternalProcessingTime, TupleGuid = localTuple.ID, link = result.Link });
                                        }
                                    }
                                    #region fog utilization
                                    //finding fog utilization
                                    Debug.Write("Tuple processig Time " + localTuple.Name + " " + localTuple.InternalProcessingTime);
                                    lock (Lock)
                                    {
                                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);
                                        fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                                        fogTime.FreeTime = tupleTime.TupleDeparture;
                                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                        FogSimulator.FogTimings.Add(fogTime);
                                        FogSimulator.TupleTimings.Add(tupleTime);


                                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                        result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                                    }
                                    #endregion
                                    FogSimulator.resultList.Add(result);
                                }
                                else
                                {
                                    lock (Lock)
                                    {
                                        fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);

                                        fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                        FogSimulator.FogTimings.Add(fogTime);



                                        PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                        ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                        result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                                    }

                                    //if job still not finished in his time add back to end of queue for next execution
                                    NodeLevelQueueFinal[i].Enqueue(localTuple);

                                }
                            }

                            else
                            {
                                Debug.WriteLine("missed by fog");
                                // because it these values are already set in GetValidFogDevice()
                                // tuple.IsReversed = true;
                                // tuple.IsServerFound = false;
                                if (CommunicationType == 1)
                                {
                                    lock (Lock)
                                    {
                                        // communicate with cloud if fog device is not able to serve.
                                        if (CloudSimulator.CloudSimulationForFog(localTuple, false, Service, DataCenter))
                                        {
                                            localTuple.IsReversed = true;
                                            localTuple.IsCloudServed = true;
                                            localTuple.IsServedByFC_Cloud = true;
                                        }
                                    }
                                }
                                else
                                {
                                    localTuple.IsCloudServed = false;
                                    localTuple.IsReversed = true; //even cloud could not served the request.
                                }

                                result.Tuple = localTuple;
                            }

                            watch.Stop();
                            //only assign it to result object if its burst time is 0 and it is served
                            if (localTuple.BurstTime == 0)
                                result.Tuple = localTuple;

                        }
                        //}));



                    }

                }


                #endregion




                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }



        public  static bool GenerateNodeLevelQueuesOnMeanRoundRRobinBasisWithMultiThreading(List<SFog.Models.Tuple> tuple, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter, bool WithGateway)
        {
            try
            {
                if (WithGateway)
                {
                    List<Models.Tuple> listJobsSendToCloud = tuple.Where(x => x.DataType == FogSimulator.EnumDataType.Bulk.ToString() || x.DataType == FogSimulator.EnumDataType.Large.ToString()).ToList();
                    foreach (var item in listJobsSendToCloud)
                        if (GlobalGateway.SendToCloud(item) && CommunicationType == 1)
                        {
                            var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                            lock (Lock)
                                FogSimulator.TupleTimings.Add(tupleTime);
                            CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter);
                            item.IsServedByFC_Cloud = false;
                        }
                }

                if (WithGateway)
                    tuple = tuple.Where(x => x.DataType != FogSimulator.EnumDataType.Bulk.ToString() && x.DataType != FogSimulator.EnumDataType.Large.ToString()).ToList();
                

                var result = new Results();
                int QueueIndex = 0;
               
                Stopwatch watch = new Stopwatch();
                bool isJobFinished;
               


                var NodeLevelQueue = new Queue<SFog.Models.Tuple>[fogList.Count];
                var NodeLevelQueueFinal = new Queue<SFog.Models.Tuple>[fogList.Count];
                var JobListForCloud = new List<SFog.Models.Tuple>();
                for (int i = 0; i < fogList.Count; i++)
                {
                    
                    NodeLevelQueue[i] = new Queue<SFog.Models.Tuple>();
                    NodeLevelQueueFinal[i] = new Queue<SFog.Models.Tuple>();
                }
                foreach (var item in tuple)
                {
                    var Broker = new FogBroker()
                    {
                        FogList = fogList,
                        Tuple = item
                    };

                    Broker.SelectedFogDevice = FogBrokerUtility.GetValidFogDeviceMRR(Broker.Tuple, Broker.FogList);

                    if (Broker.SelectedFogDevice != null)
                    {
                     
                        QueueIndex = Convert.ToInt32(Broker.SelectedFogDevice.Name.Substring(Broker.SelectedFogDevice.Name.IndexOf('-') + 1));
                        
                        NodeLevelQueue[QueueIndex].Enqueue(item);
                     

                    }

                    else
                    {

                        JobListForCloud.Add(Broker.Tuple);
                    }



                }


                var SortedNodeLevelTupleList = new List<SFog.Models.Tuple>[fogList.Count];

                for (int i = 0; i < fogList.Count; i++)
                {


                    SortedNodeLevelTupleList[i] = NodeLevelQueue[i].OrderBy(x => x.MIPS).ToList();
                }

                //Now calculate mean burst time for every Queue

                //Now add mean differnces

                for (int i = 0; i < fogList.Count; i++)
                {
                    double MeanDifferenceOfQueues = 0;
                    foreach (var jobs in SortedNodeLevelTupleList[i])
                    {

                        MeanDifferenceOfQueues = MeanDifferenceOfQueues + jobs.BurstTime;
                    }

                    //to get mean divide by number of jobs
                    MeanDifferenceOfQueues = Math.Round(MeanDifferenceOfQueues / SortedNodeLevelTupleList[i].Count(), 0);
                    foreach (var jobs in SortedNodeLevelTupleList[i])
                    {

                        jobs.BurstTimeDifference = MeanDifferenceOfQueues - jobs.BurstTime;
                    }

                    //sort the list on basis of mean difference

                    SortedNodeLevelTupleList[i] = NodeLevelQueue[i].OrderByDescending(x => x.Priority).OrderByDescending(x => x.BurstTimeDifference).ToList();

                }
               
                //Now put items back to queues for assigning jobs to fogs
                for (int i = 0; i < fogList.Count; i++)
                {
                    foreach (var localTuple in SortedNodeLevelTupleList[i])
                    {
                        NodeLevelQueueFinal[i].Enqueue(localTuple);
                        
                    }
                }


                #region JobsAllocation
                
               
                    for (int i = 0; i < fogList.Count; i++)
                    {

                        var Broker = new FogBroker();
                        Broker.FogList = fogList;
                        Broker.SelectedFogDevice = fogList[i];

                        if (NodeLevelQueueFinal[i].Count > 0)
                        {
                            SmartThreadPool s = new SmartThreadPool();
                            s.MaxThreads = 100;
                            s.MinThreads = 100;
                            watch.Start();
                            for (int j = 0; j <= NodeLevelQueueFinal[i].Count; j++)
                            {
                                if (i > 0)
                                {
                                    if (NodeLevelQueueFinal[i - 1].Count > 0)
                                    {
                                        i--;
                                    }
                                }
                                lock (Lock)
                                {
                                    Broker.Tuple = NodeLevelQueueFinal[i].Dequeue();
                                }
                                Broker.Tuple.QueueDelay = watch.Elapsed.Milliseconds;
                                s.QueueWorkItem(o => ServeJobsOnNodeLevel(Broker.Tuple, fogList[i], result, Broker, fogList, CommunicationType, Service, DataCenter, NodeLevelQueueFinal[i]), new object());
                                
                                lock (Lock)
                                {
                                    if (j > 0)
                                        j--;
                                }

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
                   
                   
              







                #endregion




                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

       
        public static void ServeJobsOnNodeLevel(SFog.Models.Tuple NodeLevelQueueFinal, FogDevice fogNode, Results result, FogBroker Broker, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter, Queue<SFog.Models.Tuple> NodeLevelQueue)
        {
           
                result = new Results();
                Stopwatch watch = new Stopwatch();
                watch.Start();
                result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();

                var localTuple = NodeLevelQueueFinal;
            

                Broker.SelectedFogDevice = FogBrokerUtility.GetValidFogDeviceForMRR(localTuple, fogList);


                if (Broker.SelectedFogDevice != null)
                {

                    Broker.Tuple = localTuple;
                    var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = Broker.SelectedFogDevice.Name, TupleName = localTuple.Name };
                    // var actingServer = new ActingServer() { ServerName = Broker.SelectedFogDevice.Name, TupleName = tuple.Name, StartTime = fogTime.TaskArrival };
                    lock (Lock)
                    {
                        result.Link = linksList.Where(x => x.Source == localTuple.Name && x.Destination == Broker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                        result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                        result.FogBroker = Broker;
                    }

                    if (localTuple.BurstTime < Broker.SelectedFogDevice.ProcessorBurstTime)
                        localTuple.BurstTime = 0;
                    else
                    localTuple.BurstTime = 0;
                    //localTuple.BurstTime = localTuple.BurstTime - Broker.SelectedFogDevice.ProcessorBurstTime;
                if (localTuple.BurstTime == 0)
                    {
                        lock (Lock)
                        {
                            localTuple.IsServed = true;

                            localTuple.IsServerFound = true;
                        }
                        string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                        var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = localTuple.Name };
                        // it will execute on policy 4
                        if (FogSimulator.IsCreateCache)
                        {
                            lock (fogCahce)
                            {
                                fogCahce.Add(new FogCache() { DataType = localTuple.DataType, FogServer = Broker.SelectedFogDevice.ID, InternalProcessingTime = localTuple.InternalProcessingTime, TupleGuid = localTuple.ID, link = result.Link });
                            }
                        }
                        #region fog utilization
                        //finding fog utilization
                        Debug.Write("Tuple processig Time " + localTuple.Name + " " + localTuple.InternalProcessingTime);
                        lock (Lock)
                        {
                            fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);
                            fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                            fogTime.FreeTime = tupleTime.TupleDeparture;
                            fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                            FogSimulator.FogTimings.Add(fogTime);
                            FogSimulator.TupleTimings.Add(tupleTime);


                            PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                            ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                            result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                            localTuple.IsJobFinished = true;

                            FogSimulator.resultList.Add(result);
                        }


                        #endregion


                    }
                    else
                    {
                        lock (Lock)
                        {
                            fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);

                            fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                            FogSimulator.FogTimings.Add(fogTime);



                            PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                            ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                            result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                            localTuple.IsJobFinished = false;
                            //NodeLevelQueue.Enqueue(localTuple);
                            //localTuple.BurstNumber++;
                        }

                        //if job still not finished in his time add back to end of queue for next execution


                    }
                }

                else
                {
                    Debug.WriteLine("missed by fog");
                    // because it these values are already set in GetValidFogDevice()
                    // tuple.IsReversed = true;
                    // tuple.IsServerFound = false;
                    if (CommunicationType == 1)
                    {
                        lock (Lock)
                        {
                            // communicate with cloud if fog device is not able to serve.
                            if (CloudSimulator.CloudSimulationForFog(localTuple, false, Service, DataCenter))
                            {
                                localTuple.IsReversed = true;
                                localTuple.IsCloudServed = true;
                                localTuple.IsServedByFC_Cloud = true;
                            }
                        }
                    }
                    else
                    {
                        localTuple.IsCloudServed = false;
                        localTuple.IsReversed = true; //even cloud could not served the request.
                    }

                    result.Tuple = localTuple;
                }

                watch.Stop();
                //only assign it to result object if its burst time is 0 and it is served
                if (localTuple.BurstTime == 0)
                {
                    result.Tuple = localTuple;

                    localTuple.IsJobFinished = true;
                }

            


        }


       

        


        public static void ServeJobsOnNodeLevel(Queue<SFog.Models.Tuple> NodeLevelQueueFinal, FogDevice fogNode, Results result, FogBroker Broker, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter)
        {
            Stopwatch watch = new Stopwatch();
            if (NodeLevelQueueFinal.Count > 0)
            {
                for (int j = 0; j <= NodeLevelQueueFinal.Count; j++)
                {
                    result = new Results();
                    var localTuple = NodeLevelQueueFinal.Dequeue();
                    if (j > 0)
                        j--;

                  
                    
                        Broker.SelectedFogDevice = FogBrokerUtility.SetValidFogDeviceMRR(localTuple, fogNode,fogList);
                    
                    if (Broker.SelectedFogDevice != null)
                    {

                        Broker.Tuple = localTuple;
                        var fogTime = new FogTimes() { TaskArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), FogName = Broker.SelectedFogDevice.Name, TupleName = localTuple.Name };
                        // var actingServer = new ActingServer() { ServerName = Broker.SelectedFogDevice.Name, TupleName = tuple.Name, StartTime = fogTime.TaskArrival };
                        lock (Lock)
                        {
                            result.Link = linksList.Where(x => x.Source == localTuple.Name && x.Destination == Broker.SelectedFogDevice.Name && x != null).FirstOrDefault();
                            result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);
                            result.FogBroker = Broker;
                        }

                        if (localTuple.BurstTime < Broker.SelectedFogDevice.ProcessorBurstTime)
                            localTuple.BurstTime = 0;
                        else
                            localTuple.BurstTime = localTuple.BurstTime - Broker.SelectedFogDevice.ProcessorBurstTime;
                        if (localTuple.BurstTime == 0)
                        {
                            localTuple.IsServed = true;
                            localTuple.IsServerFound = true;
                            string endtime = watch.Elapsed.TotalMilliseconds.ToString();
                            var tupleTime = new TupleTimes() { TupleDeparture = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = localTuple.Name };
                            // it will execute on policy 4
                            if (FogSimulator.IsCreateCache)
                            {
                                lock (fogCahce)
                                {
                                    fogCahce.Add(new FogCache() { DataType = localTuple.DataType, FogServer = Broker.SelectedFogDevice.ID, InternalProcessingTime = localTuple.InternalProcessingTime, TupleGuid = localTuple.ID, link = result.Link });
                                }
                            }
                            #region fog utilization
                            //finding fog utilization
                            Debug.Write("Tuple processig Time " + localTuple.Name + " " + localTuple.InternalProcessingTime);
                            lock (Lock)
                            {
                                fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);
                                fogTime.TimeDifference = Convert.ToDouble(endtime) - Convert.ToDouble(result.InitiatesTime);
                                fogTime.FreeTime = tupleTime.TupleDeparture;
                                fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                FogSimulator.FogTimings.Add(fogTime);
                                FogSimulator.TupleTimings.Add(tupleTime);


                                PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                            }
                            #endregion
                            lock (Lock)
                            {

                                FogSimulator.resultList.Add(result);
                            }
                        }
                        else
                        {
                            lock (Lock)
                            {
                                fogTime.Consumption = PowerUtility.Consumption(Broker.SelectedFogDevice, (watch.Elapsed.TotalMilliseconds - Convert.ToDouble(result.InitiatesTime)), watch.Elapsed.TotalMilliseconds, localTuple);

                                fogTime.ConsumptionPer = PowerUtility.CalConsPercentage(Broker.SelectedFogDevice, fogList);
                                FogSimulator.FogTimings.Add(fogTime);



                                PowerUtility.ReleasePower(Broker.SelectedFogDevice, localTuple);
                                ResourceUtility.ResourceReleaseChanged(Broker.SelectedFogDevice, localTuple, fogList);
                                result.ElapsedTime = (result.Link.Propagationtime + localTuple.InternalProcessingTime) - Convert.ToDouble(result.InitiatesTime);
                              
                            }


                            //if job still not finished in his time add back to end of queue for next execution
                            NodeLevelQueueFinal.Enqueue(localTuple);

                        }
                    }

                    else
                    {
                        Debug.WriteLine("missed by fog");
                        // because it these values are already set in GetValidFogDevice()
                        // tuple.IsReversed = true;
                        // tuple.IsServerFound = false;
                        if (CommunicationType == 1)
                        {
                            lock (Lock)
                            {
                                // communicate with cloud if fog device is not able to serve.
                                if (CloudSimulator.CloudSimulationForFog(localTuple, false, Service, DataCenter))
                                {
                                    localTuple.IsReversed = true;
                                    localTuple.IsCloudServed = true;
                                    localTuple.IsServedByFC_Cloud = true;
                                }
                            }
                        }
                        else
                        {
                            localTuple.IsCloudServed = false;
                            localTuple.IsReversed = true; //even cloud could not served the request.
                        }
                        lock (Lock)
                        {

                            result.Tuple = localTuple;
                        }
                    }

                    watch.Stop();
                    //only assign it to result object if its burst time is 0 and it is served
                    lock (Lock)
                    {
                        if (localTuple.BurstTime == 0)
                            result.Tuple = localTuple;
                    }

                }
                //}));



            }

        }
    }


    public class VisitedNodes
    {
        private List<int> nodes;
        public List<int> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }
    }
    public class Connections
    {
        private List<int> nodes;
        public List<int> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }
    }
}