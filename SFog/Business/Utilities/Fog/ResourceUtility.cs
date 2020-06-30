using SFog.Models.Nodes;
using SFog.Models.Nodes.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Business.Utilities.Fog
{
    public static class ResourceUtility
    {
        private static Object Lock = new Object();
        public static void ResourceConsumption(Models.Tuple tuple, FogDevice fogDevice)
        {
            lock (Lock)
            {
                if (fogDevice.CurrentAllocatedBw <= fogDevice.UpBW &&
                    fogDevice.CurrentAllocatedMips <= fogDevice.MIPS &&
                    fogDevice.CurrentAllocatedRam <= fogDevice.RAM &&
                    fogDevice.CurrentAllocatedSize <= fogDevice.Size)
                {
                    fogDevice.CurrentAllocatedBw = fogDevice.CurrentAllocatedBw + tuple.BW;
                    fogDevice.CurrentAllocatedMips = fogDevice.CurrentAllocatedMips + tuple.MIPS;
                    fogDevice.CurrentAllocatedRam = fogDevice.CurrentAllocatedRam + tuple.RAM;
                    fogDevice.CurrentAllocatedSize = fogDevice.CurrentAllocatedSize + tuple.Size;
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

        public static void ResourceReleased(Models.Tuple tuple, FogDevice fogDevice)
        {
            if (fogDevice != null && tuple.IsServerFound == true && tuple.IsReversed == false)
            {
                lock (Lock)
                {
                    fogDevice.CurrentAllocatedBw = fogDevice.CurrentAllocatedBw - tuple.BW;
                    fogDevice.CurrentAllocatedMips = fogDevice.CurrentAllocatedMips - tuple.MIPS;
                    fogDevice.CurrentAllocatedRam = fogDevice.CurrentAllocatedRam - tuple.RAM;
                    fogDevice.CurrentAllocatedSize = fogDevice.CurrentAllocatedSize - tuple.Size;
                }
            }
        }
        public static void ResourceReleaseChanged(FogDevice fogDev, SFog.Models.Tuple tuple, List<FogDevice> deviceList)
        {
            lock (Lock)
            {
                var fogDevice = deviceList.FirstOrDefault(x => x.ID.Equals(fogDev.ID));
                if (fogDevice != null)
                {
                    fogDevice.CurrentAllocatedBw = fogDevice.CurrentAllocatedBw - tuple.BW;
                    fogDevice.CurrentAllocatedRam = fogDevice.CurrentAllocatedRam - tuple.RAM;
                    fogDevice.CurrentAllocatedSize = fogDevice.CurrentAllocatedSize - tuple.Size;
                    double Mips = fogDevice.CurrentAllocatedMips - tuple.MIPS;
                    if (Mips < 0)
                    {
                        fogDevice.CurrentAllocatedMips = 0;
                    }
                    else
                    {
                        fogDevice.CurrentAllocatedMips = Mips;
                    }
                }
                else { }
            }
        }

        public static bool IsFullyConsumed(FogDevice fogDevice)
        {
            if (fogDevice.CurrentAllocatedBw >= fogDevice.UpBW ||
                    fogDevice.CurrentAllocatedMips >= fogDevice.MIPS ||
                    fogDevice.CurrentAllocatedRam >= fogDevice.RAM ||
                    fogDevice.CurrentAllocatedSize >= fogDevice.Size)
            {
                return true;
            }
            return false;
        }

        public static bool ResourceConsumptionChanged(FogDevice fogDev, SFog.Models.Tuple tuple,List<FogDevice> deviceList)
        {
            if (fogDev == null) return false;
            lock (Lock)
            {
                var fogDevice = deviceList.FirstOrDefault(x => x.ID.Equals(fogDev.ID));
                if (fogDevice != null)
                {
                    // if (fogDevice.CurrentAllocatedBw <= fogDevice.UpBW &&
                    // fogDevice.CurrentAllocatedMips <= fogDevice.MIPS &&
                    // fogDevice.CurrentAllocatedRam <= fogDevice.RAM &&
                    // fogDevice.CurrentAllocatedSize <= fogDevice.Size)
                    if ((fogDevice.MIPS - fogDevice.CurrentAllocatedMips) >= tuple.MIPS &&
                       (fogDevice.RAM - fogDevice.CurrentAllocatedRam) >= tuple.RAM)
                    {
                        fogDevice.CurrentAllocatedBw = fogDevice.CurrentAllocatedBw + tuple.BW;
                        fogDevice.CurrentAllocatedMips = fogDevice.CurrentAllocatedMips + tuple.MIPS;
                        fogDevice.CurrentAllocatedRam = fogDevice.CurrentAllocatedRam + tuple.RAM;
                        fogDevice.CurrentAllocatedSize = fogDevice.CurrentAllocatedSize + tuple.Size;
                        return true;
                    }
                    else return false;

                }
                else { return false; }
            }
        }

        public static bool ResourceConsumptionChangedMRR(FogDevice fogDev, SFog.Models.Tuple tuple)
        {
            if (fogDev == null) return false;
            lock (Lock)
            {
                var fogDevice = fogDev;
                if (fogDevice != null)
                {
                    // if (fogDevice.CurrentAllocatedBw <= fogDevice.UpBW &&
                    // fogDevice.CurrentAllocatedMips <= fogDevice.MIPS &&
                    // fogDevice.CurrentAllocatedRam <= fogDevice.RAM &&
                    // fogDevice.CurrentAllocatedSize <= fogDevice.Size)
                    if ((fogDevice.MIPS - fogDevice.CurrentAllocatedMips) >= tuple.MIPS &&
                       (fogDevice.RAM - fogDevice.CurrentAllocatedRam) >= tuple.RAM)
                    {
                        fogDevice.CurrentAllocatedBw = fogDevice.CurrentAllocatedBw + tuple.BW;
                        fogDevice.CurrentAllocatedMips = fogDevice.CurrentAllocatedMips + tuple.MIPS;
                        fogDevice.CurrentAllocatedRam = fogDevice.CurrentAllocatedRam + tuple.RAM;
                        fogDevice.CurrentAllocatedSize = fogDevice.CurrentAllocatedSize + tuple.Size;
                        return true;
                    }
                    else return false;

                }
                else { return false; }
            }
        }

    }
}