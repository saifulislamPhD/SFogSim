using SFog.Models.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Business.Utilities.Cloud
{
    public static class CloudRersourceUtility
    {
        private static Object Lock = new Object();
        public static void ResourceConsumption(Models.Tuple tuple, CloudDevice cloud)
        {
            lock (Lock)
            {
                if (cloud.CurrentAllocatedBw <= cloud.UpBW &&
                    cloud.CurrentAllocatedMips <= cloud.MIPS &&
                    cloud.CurrentAllocatedRam <= cloud.RAM &&
                    cloud.CurrentAllocatedSize <= cloud.Size)
                {
                    cloud.CurrentAllocatedBw = cloud.CurrentAllocatedBw + tuple.BW;
                    cloud.CurrentAllocatedMips = cloud.CurrentAllocatedMips + tuple.MIPS;
                    cloud.CurrentAllocatedRam = cloud.CurrentAllocatedRam + tuple.RAM;
                    cloud.CurrentAllocatedSize = cloud.CurrentAllocatedSize + tuple.Size;
                }
                else
                {
                    lock (Lock)
                    {
                        tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                        tuple.IsServerFound = true;
                    }
                }
            }
        }

        public static void ResourceReleased(Models.Tuple tuple, CloudDevice cloud)
        {
            if (cloud != null && tuple.IsServerFound == true && tuple.IsReversed == false)
            {
                lock (Lock)
                {
                    cloud.CurrentAllocatedBw = cloud.CurrentAllocatedBw - tuple.BW;
                    cloud.CurrentAllocatedMips = cloud.CurrentAllocatedMips - tuple.MIPS;
                    cloud.CurrentAllocatedRam = cloud.CurrentAllocatedRam - tuple.RAM;
                    cloud.CurrentAllocatedSize = cloud.CurrentAllocatedSize - tuple.Size;
                }
            }
        }

        public static bool IsFullyConsumed(CloudDevice cloud)
        {
            if (cloud.CurrentAllocatedBw >= cloud.UpBW ||
                    cloud.CurrentAllocatedMips >= cloud.MIPS ||
                    cloud.CurrentAllocatedRam >= cloud.RAM ||
                    cloud.CurrentAllocatedSize >= cloud.Size)
            {
                return true;
            }
            return false;
        }
    }
}