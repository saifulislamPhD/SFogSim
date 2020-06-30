using SFog.Business.Utilities.Fog;
using SFog.Models;
using SFog.Models.Brokers;
using SFog.Models.Cache;
using SFog.Models.Links;
using SFog.Models.Nodes;
using SFog.Models.Nodes.Cloud;
using SFog.Models.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SFog.Business.Utilities.Cloud
{
    public static class CloudUtility
    {
        public static double cooperSpeed = 197863.022;

        public static double transmitionRate = 100;

        private static Link link = new Link();
        //public static List<Link> linksList = new List<Link>();
        public static ConcurrentBag<Link> linksList = new ConcurrentBag<Link>();

        private static List<Task> myQueueList = new List<Task>();

        public static ConcurrentBag<CloudCache> cloudCahce = new ConcurrentBag<CloudCache>();

        private static Object Lock = new Object();

        public static CloudDevice CloudComputation(Models.Tuple tuple, CloudDevice cloud)
        {
            var rnd = new Random();
            double[] arrLatency = { 40, 50, 60 };
            var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                cloud.GeoLocation.getLongitude(), cloud.GeoLocation.getLatitude(), GeoCodeCalcMeasurement.Kilometers);
            lock (Lock)
            { cloud.DistanceFromTuple = dis; }
            lock (Lock)
            {
                link.Source = tuple.Name;
                link.Destination = cloud.Name;
                link.SDDistance = dis;
                link.Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)]);
            }
            CloudRersourceUtility.ResourceConsumption(tuple, cloud);
            return cloud;
        }

        public static void CloudSim(Models.Tuple tuple, Services service, List<string> datacenter)
        {
            Debug.WriteLine("CloudSim" + tuple.ID);

            try
            {
                Stopwatch watch = new Stopwatch();
                var result = new Results();
                watch.Start();
                result.InitiatesTime = watch.Elapsed.TotalMilliseconds.ToString();

                var Broker = new CloudBroker()
                {
                    Service = service,
                    DatacenterList = DataCenterBusiness.GetDataCenterList().Where(a => datacenter.Any(z => a.Name.Contains(z))).ToList(),
                    Tuple = tuple,
                    // create VM
                    SelectedVM = new VM(service.ID, 1, service.MIPS, service.NumberOfPes, service.RAM, service.BW, service.Size, service.Name,
                    new CloudletScheduler(), new GeoLocation(47.599949, -122.326815))
                };

                Broker.SelectedDataCenter = BrokerUtility.GetValidDataCenter(Broker.Tuple, Broker.DatacenterList);
                if (Broker.SelectedDataCenter != null)
                {
                    lock (Lock)
                    {
                        result.Link = linksList.Where(x => x.Source == tuple.Name && x.Destination == Broker.SelectedDataCenter.Name && x != null).FirstOrDefault();
                        result.Link.Propagationtime = Math.Round(result.Link.Propagationtime, 3);// + tuple.QueueDelay;
                    }
                    // returns the host by finding the appropirate and then add the vm into the vmlist of host object.
                    Broker.SelectedHost = BrokerUtility.GetHostFromDataCenter(Broker.SelectedDataCenter, Broker.SelectedVM);
                    if (Broker.SelectedHost != null)
                    {
                        result.CloudBroker = Broker;
                        tuple.InternalProcessingTime = Math.Round(watch.Elapsed.TotalMilliseconds, 3);
                        tuple.IsServed = true;
                        // it will execute on policy 4
                        if (FogSimulator.IsCreateCache)
                        {
                            lock (cloudCahce)
                            {
                                cloudCahce.Add(new CloudCache() { DataType = tuple.DataType, Cloud = Broker.SelectedDataCenter.Name, InternalProcessingTime = tuple.InternalProcessingTime, link = result.Link });
                            }
                        }
                        ResourceUtility.ReleaseResources(Broker.SelectedHost, Broker.SelectedVM);
                    }
                    else { tuple.IsServerFound = false; tuple.IsServed = false; }

                    result.ElapsedTime = (watch.Elapsed.TotalMilliseconds + result.Link.Propagationtime) - Convert.ToDouble(result.InitiatesTime);
                }
                else tuple.IsReversed = true;
                result.Tuple = tuple;
                watch.Stop();
                // result.InActiveFogDecives = fogList.Where(x => x.IsActive == false).ToList();
                CloudSimulator.resultList.Add(result);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);

            }
        }

        public static void CloudSimForFog(Models.Tuple tuple, Services service, List<string> datacenter)
        {
            Debug.WriteLine("CloudSimForFog" + tuple.ID);

            try
            {
                Stopwatch watch = new Stopwatch();
                var result = new Results();
                watch.Start();
                result.InitiatesTime = watch.Elapsed.Milliseconds.ToString();

                var Broker = new CloudBroker()
                {
                    Service = service,
                     DatacenterList = DataCenterBusiness.GetDataCenterList().Where(a => datacenter.Any(z => a.Name.Contains(z))).ToList(),
                    //DatacenterList = DataCenterBusiness.GetDataCenterList(),
                    Tuple = tuple,
                    // create VM
                    SelectedVM = new VM(service.ID, 1, service.MIPS, service.NumberOfPes, service.RAM, service.BW, service.Size, service.Name,
                    new CloudletScheduler(), new GeoLocation(47.599949, -122.326815))
                };

                Broker.SelectedDataCenter = BrokerUtility.GetValidDataCenter(Broker.Tuple, Broker.DatacenterList);
                if (Broker.SelectedDataCenter != null)
                {
                    lock (Lock)
                    {
                        result.Link = linksList.Where(x => x.Source == tuple.Name && x.Destination == Broker.SelectedDataCenter.Name && x != null).FirstOrDefault();
                        result.Link.Propagationtime = Math.Round(result.Link.Propagationtime + tuple.QueueDelay, 3);
                    }
                    // returns the host by finding the appropirate and then add the vm into the vmlist of host object.
                    Broker.SelectedHost = BrokerUtility.GetHostFromDataCenter(Broker.SelectedDataCenter, Broker.SelectedVM);
                    if (Broker.SelectedHost != null)
                    {
                        result.CloudBroker = Broker;
                        tuple.InternalProcessingTime = Math.Round(watch.Elapsed.TotalMilliseconds, 3);
                        tuple.IsServed = true;
                        // it will execute on policy 4
                        if (FogSimulator.IsCreateCache)
                        {
                            lock (Lock)
                            {
                                cloudCahce.Add(new CloudCache() { DataType = tuple.DataType, Cloud = Broker.SelectedDataCenter.Name, InternalProcessingTime = tuple.InternalProcessingTime, link = result.Link });
                            }
                        }
                        ResourceUtility.ReleaseResources(Broker.SelectedHost, Broker.SelectedVM);
                    }
                    else { tuple.IsServerFound = false; tuple.IsServed = false; }

                    result.ElapsedTime = (watch.Elapsed.Milliseconds + result.Link.Propagationtime) - Convert.ToDouble(result.InitiatesTime);
                }
                else tuple.IsReversed = true;
                result.Tuple = tuple;
                watch.Stop();
                // result.InActiveFogDecives = fogList.Where(x => x.IsActive == false).ToList();
                FogSimulator.resultList.Add(result);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}