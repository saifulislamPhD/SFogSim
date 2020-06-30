using System;
using System.Collections.Generic;
using System.Linq;
using SFog.Models.Nodes;
using SFog.Models.Utility;
using SFog.Business.Utilities;
using System.Diagnostics;
using SFog.Models;
using System.Threading.Tasks;
using SFog.Business.Utilities.Cloud;
using SFog.Models.Nodes.Cloud;
using SFog.Models.Nodes.Cloud.Managers;
using System.Collections.Concurrent;

namespace SFog.Business
{
    public static class CloudSimulator
    {
        private static Stopwatch watch = new Stopwatch();
        public static ConcurrentBag<Results> resultList = new ConcurrentBag<Results>();
        private static List<Models.Tuple> tupleList = new List<Models.Tuple>();
        public static List<ResourceManager> ResourceMonitoring = new List<ResourceManager>();
        private static Object Lock = new Object();

        public enum EnumDataType
        {
            Multimedia,
            Bulk,
            Large,
            Adrupt,
            SmallTextual,
            Medical,
            LocationBased,
        }

        public enum EnumNodeType
        {
            Sensor,
            Acuator,
            DunbObjects,
            Mobile,
            Nodes
        }

        public static void CloudSimulation(TuplePost tuple, string policy, int _service, List<string> datacenter)
        {
            resultList = new ConcurrentBag<Results>();
            Random rnd = new Random();
            var result = new Results();

            #region Cloud

            bool[] bit = { true };
            var b = rnd.Next(bit.Length);

            // create service
            var service = ServicesUtility.GetServices()[_service];

            #endregion Cloud

            if (FogSimulator.tupleList.Count == 0)
            {
                if (tuple.TupleData.RAM != 0)
                {
                    #region Tuple

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
                            "medium",
                            new CloudletScheduler(),
                            GeoDistance.RandomGeoLocation(rnd),
                            false,
                            randomNodeType.ToString(),0,0)
                            );
                    }

                    #endregion Tuple
                }
                else
                {
                    #region Tuple
                    for (int i = 0; i < tuple.TupleSize; i++)
                    {
                        int[] randomRam = { 10, 25, 50, 100, 150, 200 };
                        var randomRamIndex = rnd.Next(randomRam.Length);

                        int[] randomMips = { 100, 150, 300, 400, 500, 700 };
                        var randomMipsIndex = rnd.Next(randomMips.Length);

                        int[] randomPe = { 1 };
                        var randomPeIndex = rnd.Next(randomPe.Length);

                        int[] randomSize = { 10, 20, 50, 70, 100 };
                        var randomSizeIndex = rnd.Next(randomSize.Length);

                        int[] randomBW = { 10, 30, 50, 80, 100 };
                        var randomBWIndex = rnd.Next(randomBW.Length);

                        string[] _priority = { "medium", "low" };
                        var _priorityIndex = rnd.Next(_priority.Length);

                        Array values = Enum.GetValues(typeof(EnumDataType));
                        EnumDataType randomDataType = (EnumDataType)values.GetValue(rnd.Next(values.Length));
                        bool MedORLB = randomDataType.ToString() == "Medical" || randomDataType.ToString() == "LocationBased";

                        Array NodeTypevalues = Enum.GetValues(typeof(EnumNodeType));
                        EnumNodeType randomNodeType = (EnumNodeType)NodeTypevalues.GetValue(rnd.Next(NodeTypevalues.Length));

                        tupleList.Add(new Models.Tuple(
                            Guid.NewGuid(),
                            1,
                            randomMips[randomMipsIndex],
                            randomPe[randomPeIndex],
                            randomRam[randomRamIndex],
                            randomBW[randomBWIndex],
                            randomSize[randomSizeIndex],
                            "T-" + i,
                            randomDataType.ToString(),
                            100,
                            0.0,
                            MedORLB == true ? "high" : _priority[_priorityIndex],
                            new CloudletScheduler(),
                            GeoDistance.RandomGeoLocation(rnd),
                            false,
                            randomNodeType.ToString(),0,0)
                            );
                    }

                    #endregion Tuple
                }
            }
            else
            {
                tupleList = FogSimulator.tupleList;
            }
            List<Task> myTaskList = new List<Task>();
            if (policy == "1")
            {
                watch.Start();
                foreach (var item in tupleList)
                {
                    item.QueueDelay = watch.Elapsed.Milliseconds;
                    CloudUtility.CloudSim(item, service, datacenter);
                }
                watch.Stop();
            }
            else if (policy == "2")
            {
                var localtupleList = tupleList.OrderBy(x => x.MIPS).ToList();
                watch.Start();
                foreach (var item in localtupleList)
                {
                    item.QueueDelay = watch.Elapsed.Milliseconds;
                    CloudUtility.CloudSim(item, service, datacenter);
                }
                watch.Stop();
            }
            else if (policy == "3")
            {
                var localtupleList = tupleList.OrderByDescending(x => x.MIPS).ToList();
                watch.Start();
                foreach (var item in localtupleList)
                {
                    item.QueueDelay = watch.Elapsed.Milliseconds;
                    CloudUtility.CloudSim(item, service, datacenter);
                }
                watch.Stop();
            }
            else
            {
                var split = LinqExtensions.Split(tupleList, 16).ToList();
                watch.Start();
                for (int j = 0; j < split.Count(); j++)
                {
                    foreach (var item in split[j])
                    {
                        //commented task section by Ali for testing
                        //myTaskList.Add(Task.Factory.StartNew(() =>
                        //{
                            item.QueueDelay = watch.Elapsed.Milliseconds;
                            CloudUtility.CloudSim(item, service, datacenter);
                        //}));
                    }
                }
                watch.Stop();
                Task.WaitAll(myTaskList.ToArray());
            }
            Excel.CreateExcelSheetForCloud(resultList.ToList());

            resultList = new ConcurrentBag<Results>();
            tupleList = new List<Models.Tuple>();
        }

        public static bool CloudSimulationForFog(Models.Tuple tuple, bool fcfs, int _service, List<string> datacenter)
        {//i am here
            if (tuple != null)
            {
                resultList = new ConcurrentBag<Results>();
                Random rnd = new Random();
                var result = new Results();
                var tupleList = new List<Models.Tuple>();

                #region Cloud

                bool[] bit = { true };
                var b = rnd.Next(bit.Length);

                // create service
                var service = ServicesUtility.GetServices()[_service];

                #endregion Cloud
                #region Tuple
                lock (Lock)
                    tupleList.Add(tuple);

                #endregion Tuple

                List<Task> myTaskList = new List<Task>();
                if (fcfs)
                {
                    watch.Start();
                    lock (Lock)
                    {
                        foreach (var item in tupleList)
                        {
                            item.QueueDelay = watch.Elapsed.Milliseconds;
                            CloudUtility.CloudSimForFog(item, service, datacenter);
                        }
                    }
                    watch.Stop();
                }
                else
                {
                    watch.Start();
                    lock (Lock)
                    {
                        foreach (var item in tupleList)
                        {
                            //commented by ali for testing
                            //myTaskList.Add(Task.Factory.StartNew(() =>
                            //{
                                item.QueueDelay = watch.Elapsed.Milliseconds;
                                CloudUtility.CloudSimForFog(item, service, datacenter);
                            //}));
                        }
                    }

                    watch.Stop();
                    Task.WaitAll(myTaskList.ToArray());
                }
                return true;
            }
            return false;
        }
    }
}