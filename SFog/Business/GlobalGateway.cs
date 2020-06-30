using SFog.Business.Utilities.Fog;
using SFog.Models.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using SFog.Business.Utilities;
using System.Threading.Tasks;
using System.Diagnostics;
using SFog.Models;
using Amib.Threading;

namespace SFog.Business
{
    public static class GlobalGateway
    {
        private static Object Lock = new Object();
        public static bool SendToCloud(Models.Tuple tuple)
        {
            if (tuple.DataType == FogSimulator.EnumDataType.Bulk.ToString() || tuple.DataType == FogSimulator.EnumDataType.Large.ToString())
            {
                return true;
            }
            else
                return false;
        }

        public static void GatewayPathDecider(List<Models.Tuple> tupleList, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter, bool fcfs)
        {
            SmartThreadPool s = new SmartThreadPool();
            s.MaxThreads = 1000;
            s.MinThreads = 1000;
            List<Task> myTaskList = new List<Task>();
            if (fcfs)
            {
                foreach (var item in tupleList)
                {
                    if (SendToCloud(item) && CommunicationType == 1)
                    {
                        var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                        lock (Lock)
                            FogSimulator.TupleTimings.Add(tupleTime);
                        CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter);
                        item.IsServedByFC_Cloud = false;
                    }
                    else
                    {
                        var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                        lock (Lock)
                            FogSimulator.TupleTimings.Add(tupleTime);
                        s.QueueWorkItem(o => FogUtility.FogSim(item, fogList, CommunicationType, Service, DataCenter), new object());

                    }
                }
                try
                {
                    s.WaitForIdle();
                    s.Shutdown();
                }
                catch { };
            }
            else
            {
                var split = LinqExtensions.Split(tupleList, 16).ToList();

                for (int j = 0; j < split.Count(); j++)
                {
                    foreach (var item in split[j])
                    {
                        if (SendToCloud(item) && CommunicationType == 1)
                        {
                            var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                            lock (Lock)
                                FogSimulator.TupleTimings.Add(tupleTime);
                            CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter);
                            item.IsServedByFC_Cloud = false;
                        }
                        else
                        {
                            var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                            lock (Lock)
                                FogSimulator.TupleTimings.Add(tupleTime);
                            s.QueueWorkItem(o => FogUtility.FogSim(item, fogList, CommunicationType, Service, DataCenter), new object());
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
        }

        /// <summary>
        /// Gateway used for edge
        /// </summary>
        /// <param name="tupleList"></param>
        /// <param name="edgeList same class used as fog"></param>
        /// <param name="CommunicationType"></param>
        /// <param name="Service"></param>
        /// <param name="DataCenter"></param>
        /// <param name="fcfs"></param>
        public static void EGatewayPathDecider(List<Models.Tuple> tupleList, List<FogDevice> edgeList, int CommunicationType, int Service, List<string> DataCenter, bool fcfs)
        {
            SmartThreadPool s = new SmartThreadPool();
            s.MaxThreads = 1000;
            s.MinThreads = 1000;
            List<Task> myTaskList = new List<Task>();
            if (fcfs)
            {
                foreach (var item in tupleList)
                {
                    if (SendToCloud(item) && CommunicationType == 1)
                    {
                        var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                        lock (Lock)
                            FogSimulator.TupleTimings.Add(tupleTime);
                        CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter);
                        item.IsServedByFC_Cloud = false;
                    }
                    else
                    {
                        var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                        FogSimulator.TupleTimings.Add(tupleTime);
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
            else
            {
                var split = LinqExtensions.Split(tupleList, 16).ToList();

                for (int j = 0; j < split.Count(); j++)
                {
                    foreach (var item in split[j])
                    {
                        if (SendToCloud(item) && CommunicationType == 1)
                        {
                            var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                            lock (Lock)
                                FogSimulator.TupleTimings.Add(tupleTime);
                            CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter);
                            item.IsServedByFC_Cloud = false;
                        }
                        else
                        {
                            var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                            FogSimulator.TupleTimings.Add(tupleTime);
                            s.QueueWorkItem(o => FogUtility.EdgeSim(item, edgeList, CommunicationType), new object());
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
        }

        public static void MemoryGatewayPathDecider(List<Models.Tuple> tupleList, List<FogDevice> fogList, int CommunicationType, int Service, List<string> DataCenter, bool fcfs)
        {
            SmartThreadPool s = new SmartThreadPool();
            s.MaxThreads = 1000;
            s.MinThreads = 1000;
            List<Task> myTaskList = new List<Task>();
            if (fcfs)
            {
                foreach (var item in tupleList)
                {
                    if (SendToCloud(item) && CommunicationType == 1)
                    {
                        var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                        lock (Lock)
                            FogSimulator.TupleTimings.Add(tupleTime);
                        CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter);
                        item.IsServedByFC_Cloud = false;
                    }
                    else
                    {
                        var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                        lock (Lock)
                            FogSimulator.TupleTimings.Add(tupleTime);
                        //commenting code by ali for testing
                        //s.QueueWorkItem(o => FogUtility.Memory(item, fogList, CommunicationType, Service, DataCenter), new object());
                        FogUtility.Memory(item, fogList, CommunicationType, Service, DataCenter);
                    }
                }
                try
                {
                    s.WaitForIdle();
                    s.Shutdown();
                }
                catch { };
            }
            else
            {
                var split = LinqExtensions.Split(tupleList, 16).ToList();

                for (int j = 0; j < split.Count(); j++)
                {
                    foreach (var item in split[j])
                    {
                        if (SendToCloud(item) && CommunicationType == 1)
                        {
                            var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                            lock (Lock)
                                FogSimulator.TupleTimings.Add(tupleTime);
                            CloudSimulator.CloudSimulationForFog(item, false, Service, DataCenter);
                            item.IsServedByFC_Cloud = false;
                        }
                        else
                        {
                            var tupleTime = new TupleTimes() { TupleArrival = DateTime.Now.ToString("hh:mm:ss.fff tt"), Name = item.Name };
                            lock (Lock)
                                FogSimulator.TupleTimings.Add(tupleTime);
                            s.QueueWorkItem(o => FogUtility.FogSim(item, fogList, CommunicationType, Service, DataCenter), new object());
                            // FogUtility.Memory(item, fogList, CommunicationType, Service, DataCenter);
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
        }

    }
}