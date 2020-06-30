using SFog.Models.Links;
using SFog.Models.Nodes.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Business.Utilities.Cloud
{
    public static class BrokerUtility
    {
        public static double cooperSpeed = 197863.022;
        public static double transmitionRate = 2 * Math.Pow(10, 6);
        private static Link link = new Link();

        public static DataCenter GetValidDataCenter(Models.Tuple tuple, List<DataCenter> datacenterList)
        {
            var rnd = new Random();
            double[] arrLatency = { 40, 50, 60 };
            foreach (var item in datacenterList)
            {
                var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                    item.DatacenterCharacteristics.GeoLocation.getLongitude(), item.DatacenterCharacteristics.GeoLocation.getLatitude(),
                                                    GeoCodeCalcMeasurement.Kilometers);

                lock (item)
                { item.DatacenterCharacteristics.DistanceFromTuple = dis; }
                var link = new Link()
                {
                    Source = tuple.Name,
                    Destination = item.Name,
                    SDDistance = dis,
                    Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                };
                lock (CloudUtility.linksList)
                    CloudUtility.linksList.Add(link);
            }
            return datacenterList.OrderBy(x => x.DatacenterCharacteristics.DistanceFromTuple).FirstOrDefault();
        }

        public static Host GetHostFromDataCenter(DataCenter dataCenter, VM vm)
        {
            foreach (var item in dataCenter.DatacenterCharacteristics.HostList)
            {
                if (item.ResourceManager.RamManager.AvailableRam > vm.RAM &&
                    item.ResourceManager.PeManager.AvailablePE > vm.NumberOfPes &&
                    item.ResourceManager.StorageManager.AvailableStorage > vm.Size &&
                    item.ResourceManager.MIPSManager.AvailableMIPS > vm.MIPS &&
                    item.ResourceManager.BWManager.AvailableBW > vm.BW)
                {
                    lock (item.VMList)
                        item.VMList.Add(vm);
                    ResourceUtility.ConsumeResources(item, vm);
                    return item;
                }
            }
            return null;
        }
    }
}