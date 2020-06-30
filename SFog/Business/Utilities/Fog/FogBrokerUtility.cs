using Newtonsoft.Json;
using SFog.Models.Cache;
using SFog.Models.Links;
using SFog.Models.Nodes;
using SFog.Models.Nodes.Cloud;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;

namespace SFog.Business.Utilities.Fog
{
    public static class FogBrokerUtility
    {
        public static double cooperSpeed = 197863.022;
        public static double transmitionRate = 2 * Math.Pow(10, 6);
        private static Link link = new Link();
        private static Object Lock = new Object();

        public static ConcurrentBag<FogCache> fogCahce = new ConcurrentBag<FogCache>();

        public static FogDevice GetValidFogDevice(Models.Tuple tuple, List<FogDevice> fogList)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                //  List<FogDevice> fogList = FogSimulator.getList().ToList();
                //faizan changes getting fog for a specific job #change
               
                    fogList = PowerUtility.GetRequiredFogs(tuple, fogList);
                
                if (fogList != null)
                {
                    foreach (var item in fogList)
                    {
                        var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                            item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                            GeoCodeCalcMeasurement.Kilometers);
                        lock (Lock)
                        { item.DistanceFromTuple = dis; }
                        var link = new Link()
                        {
                            Source = tuple.Name,
                            Destination = item.Name,
                            SDDistance = dis,
                            Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                        };
                        lock (Lock)
                            FogUtility.linksList.Add(link);
                    }
                    FogDevice nearestFogDevice, nearestFogDeviceUpdated = null;
                    lock (Lock)
                    {
                        nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                        if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                        {
                            //if do some thing arrange new server which is free
                            nearestFogDeviceUpdated = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                        }
                    }
                    Debug.WriteLine("nearestFogDevice " + nearestFogDevice.CurrentAllocatedMips);

                    if (nearestFogDeviceUpdated != null)
                    {
                        nearestFogDevice = nearestFogDeviceUpdated;
                        if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                        {
                            PowerUtility.ConsumePower(nearestFogDevice, tuple);
                            ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                            // check the power required by tuple and available power by fog.
                            //if(tuple.BurstNumber==0)
                            tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                            //else
                            //tuple.InternalProcessingTime = tuple.InternalProcessingTime+ FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                            tuple.IsServerFound = true;
                            tuple.FogLevelServed = 1;

                        }
                        else
                        { }
                    }
                    else
                    {
                        var flag = FogSimulator.WithCoo; // for without Cooperation of fog.
                        if (flag)
                        {
                            // queue for fog device can be implemented here. but it requires to send these tuples back to the fog server once the server is free.
                            // for this, it requires to remove null check on nearestFogDevice.
                            foreach (var item in fogList)
                            {
                                var dis = GeoDistance.CalcDistance(nearestFogDevice.GeoLocation.getLongitude(), nearestFogDevice.GeoLocation.getLatitude(),
                                                                    item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                                    GeoCodeCalcMeasurement.Kilometers);
                                lock (item)
                                { item.DistanceFromFogServer = dis; }
                                var link = new Link()
                                {
                                    Source = nearestFogDevice.Name,
                                    Destination = item.Name,
                                    SDDistance = dis,
                                    Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                                };
                                lock (Lock)
                                    FogUtility.linksList.Add(link);
                            }
                            FogDevice nearestFogDeviceUpdated2 = null;
                            nearestFogDevice = fogList.Where(x => (x.MIPS - x.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromFogServer).FirstOrDefault();
                            if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                            {
                                //if do some thing arrange new server which is free
                                nearestFogDeviceUpdated2 = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                            }
                            if (nearestFogDeviceUpdated2 != null)
                            {
                                nearestFogDevice = nearestFogDeviceUpdated2;

                                if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                                {
                                    //check the power required by tuple and available power by fog.
                                    if (tuple.BurstNumber == 0)
                                        tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                                    else
                                        //this code is for MRR if second burst tuple internalprocesisg time must add to previous burst time
                                        tuple.InternalProcessingTime= tuple.InternalProcessingTime+ FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                                    tuple.IsServerFound = true;
                                    tuple.FogLevelServed = 2;
                                    PowerUtility.ConsumePower(nearestFogDevice, tuple);
                                    ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                                }
                                else
                                { }
                            }
                            else
                            {
                                lock (Lock)
                                {
                                    tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                    tuple.IsServerFound = false;
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            lock (Lock)
                            {
                                tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                tuple.IsServerFound = false;
                                return null;
                            }
                        }
                    }
                    return nearestFogDevice;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }


        public static FogDevice GetValidFogDeviceForMRR(Models.Tuple tuple, List<FogDevice> fogList)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                //  List<FogDevice> fogList = FogSimulator.getList().ToList();
                //faizan changes getting fog for a specific job #change

                fogList = PowerUtility.GetRequiredFogs(tuple, fogList);

                if (fogList != null)
                {
                    foreach (var item in fogList)
                    {
                        var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                            item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                            GeoCodeCalcMeasurement.Kilometers);
                        lock (Lock)
                        { item.DistanceFromTuple = dis; }
                        var link = new Link()
                        {
                            Source = tuple.Name,
                            Destination = item.Name,
                            SDDistance = dis,
                            Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                        };
                        lock (Lock)
                            FogUtility.linksList.Add(link);
                    }
                    FogDevice nearestFogDevice, nearestFogDeviceUpdated = null;
                    lock (Lock)
                    {
                        nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                        if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                        {
                            //if do some thing arrange new server which is free
                            nearestFogDeviceUpdated = IsResourcesAvailableChangedforMRR(nearestFogDevice, tuple, fogList);
                        }
                    }
                    Debug.WriteLine("nearestFogDevice " + nearestFogDevice.CurrentAllocatedMips);

                    if (nearestFogDeviceUpdated != null)
                    {
                        nearestFogDevice = nearestFogDeviceUpdated;
                        if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                        {
                            PowerUtility.ConsumePower(nearestFogDevice, tuple);
                            ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                            // check the power required by tuple and available power by fog.
                            //if(tuple.BurstNumber==0)
                            tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                            //else
                            //tuple.InternalProcessingTime = tuple.InternalProcessingTime+ FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                            tuple.IsServerFound = true;
                            tuple.FogLevelServed = 1;

                        }
                        else
                        { }
                    }
                    else
                    {
                        var flag = FogSimulator.WithCoo; // for without Cooperation of fog.
                        if (flag)
                        {
                            // queue for fog device can be implemented here. but it requires to send these tuples back to the fog server once the server is free.
                            // for this, it requires to remove null check on nearestFogDevice.
                            foreach (var item in fogList)
                            {
                                var dis = GeoDistance.CalcDistance(nearestFogDevice.GeoLocation.getLongitude(), nearestFogDevice.GeoLocation.getLatitude(),
                                                                    item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                                    GeoCodeCalcMeasurement.Kilometers);
                                lock (item)
                                { item.DistanceFromFogServer = dis; }
                                var link = new Link()
                                {
                                    Source = nearestFogDevice.Name,
                                    Destination = item.Name,
                                    SDDistance = dis,
                                    Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                                };
                                lock (Lock)
                                    FogUtility.linksList.Add(link);
                            }
                            FogDevice nearestFogDeviceUpdated2 = null;
                            nearestFogDevice = fogList.Where(x => (x.MIPS - x.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromFogServer).FirstOrDefault();
                            if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                            {
                                //if do some thing arrange new server which is free
                                nearestFogDeviceUpdated2 = IsResourcesAvailableChangedforMRR(nearestFogDevice, tuple, fogList);
                            }
                            if (nearestFogDeviceUpdated2 != null)
                            {
                                nearestFogDevice = nearestFogDeviceUpdated2;

                                if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                                {
                                    //check the power required by tuple and available power by fog.
                                    if (tuple.BurstNumber == 0)
                                        tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                                    else
                                        //this code is for MRR if second burst tuple internalprocesisg time must add to previous burst time
                                        tuple.InternalProcessingTime = tuple.InternalProcessingTime + FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                                    tuple.IsServerFound = true;
                                    tuple.FogLevelServed = 2;
                                    PowerUtility.ConsumePower(nearestFogDevice, tuple);
                                    ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                                }
                                else
                                { }
                            }
                            else
                            {
                                lock (Lock)
                                {
                                    tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                    tuple.IsServerFound = false;
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            lock (Lock)
                            {
                                tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                tuple.IsServerFound = false;
                                return null;
                            }
                        }
                    }
                    return nearestFogDevice;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }


        public static FogDevice SetValidFogDeviceMRR(Models.Tuple tuple, FogDevice fogList, List<FogDevice> fogDevicesList)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                //  List<FogDevice> fogList = FogSimulator.getList().ToList();
                //faizan changes getting fog for a specific job #change
                fogDevicesList = PowerUtility.GetRequiredFogsMRR(tuple, fogDevicesList);

                if (fogDevicesList != null)
                {
                    foreach (var item in fogDevicesList)
                    {
                        var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                            item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                            GeoCodeCalcMeasurement.Kilometers);
                        lock (Lock)
                        {
                            item.DistanceFromTuple = dis;
                        }
                        var link = new Link()
                        {
                            Source = tuple.Name,
                            Destination = item.Name,
                            SDDistance = dis,
                            Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                        };
                        lock (Lock)
                            FogUtility.linksList.Add(link);
                    }
                }
                    FogDevice nearestFogDevice, nearestFogDeviceUpdated = null;
                    lock (Lock)
                    {
                        nearestFogDevice = fogList;
                        if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                        {
                            //if do some thing arrange new server which is free
                            nearestFogDeviceUpdated = IsResourcesAvailableChangedMRR(nearestFogDevice, tuple);
                        }
                    }
                    Debug.WriteLine("nearestFogDevice " + nearestFogDevice.CurrentAllocatedMips);

                    if (nearestFogDeviceUpdated != null)
                    {
                        nearestFogDevice = nearestFogDeviceUpdated;
                        if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                        {
                            PowerUtility.ConsumePower(nearestFogDevice, tuple);
                            ResourceUtility.ResourceConsumptionChangedMRR(nearestFogDevice, tuple);
                            // check the power required by tuple and available power by fog.

                            tuple.InternalProcessingTime = FogInternalProcessingChangedMRR(tuple, nearestFogDevice);
                            tuple.IsServerFound = true;
                            tuple.FogLevelServed = 1;

                        }
                        else
                        { }
                    }
                    else
                    {
                        var flag = FogSimulator.WithCoo; // for without Cooperation of fog.
                        if (flag)
                        {
                            // queue for fog device can be implemented here. but it requires to send these tuples back to the fog server once the server is free.
                            // for this, it requires to remove null check on nearestFogDevice.
                           
                            FogDevice nearestFogDeviceUpdated2 = null;
                            nearestFogDevice = fogList;
                            if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                            {
                                //if do some thing arrange new server which is free
                                nearestFogDeviceUpdated2 = IsResourcesAvailableChangedMRR(nearestFogDevice, tuple);
                            }
                            if (nearestFogDeviceUpdated2 != null)
                            {
                                nearestFogDevice = nearestFogDeviceUpdated2;

                                if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                                {
                                    //check the power required by tuple and available power by fog.
                                    tuple.InternalProcessingTime = FogInternalProcessingChangedMRR(tuple, nearestFogDevice);
                                    tuple.IsServerFound = true;
                                    tuple.FogLevelServed = 2;
                                    PowerUtility.ConsumePower(nearestFogDevice, tuple);
                                    ResourceUtility.ResourceConsumptionChangedMRR(nearestFogDevice, tuple);
                                }
                                else
                                { }
                            }
                            else
                            {
                                lock (Lock)
                                {
                                    tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                    tuple.IsServerFound = false;
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            lock (Lock)
                            {
                                tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                tuple.IsServerFound = false;
                                return null;
                            }
                        }
                    }
                    return nearestFogDevice;
               
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }


        public static FogDevice GetValidFogDeviceMRR(Models.Tuple tuple, List<FogDevice> fogList)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                //  List<FogDevice> fogList = FogSimulator.getList().ToList();
                //faizan changes getting fog for a specific job #change
                fogList = PowerUtility.GetRequiredFogsMRR(tuple, fogList);
                if (fogList != null)
                {
                    foreach (var item in fogList)
                    {
                        var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                            item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                            GeoCodeCalcMeasurement.Kilometers);
                        lock (Lock)
                        { item.DistanceFromTuple = dis; }
                        var link = new Link()
                        {
                            Source = tuple.Name,
                            Destination = item.Name,
                            SDDistance = dis,
                            Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                        };
                        lock (Lock)
                            FogUtility.linksList.Add(link);
                    }
                    FogDevice nearestFogDevice, nearestFogDeviceUpdated = null;
                    lock (Lock)
                    {
                        nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                         //nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).OrderByDescending(x => x.MIPS).FirstOrDefault();
                        //nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderByDescending(x => x.MIPS).ThenBy(x => x.DistanceFromTuple).FirstOrDefault();
                        if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                        {
                            //if do some thing arrange new server which is free
                            nearestFogDeviceUpdated = nearestFogDevice;
                        }
                    }
                    Debug.WriteLine("nearestFogDevice " + nearestFogDevice.CurrentAllocatedMips);

                    
                   
                    return nearestFogDevice;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public static FogDevice GetValidFogDevice_LR(Models.Tuple tuple, List<FogDevice> fogList, string Server_ID)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                fogList = PowerUtility.GetRequiredFogs(tuple, fogList);
                if (fogList != null)
                {
                    FogDevice nearestFogDevice, nearestFogDeviceUpdated = null;
                    foreach (var item in fogList)
                    {
                        var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                        item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                        GeoCodeCalcMeasurement.Kilometers);
                        var link = new Link()
                        {
                            Source = tuple.Name,
                            Destination = item.Name,
                            SDDistance = dis,
                            Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                        };
                        lock (Lock)
                            item.DistanceFromTuple = dis;
                        lock (Lock)
                            FogUtility.linksList.Add(link);

                    }
                    lock (Lock)
                    {
                        nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS && a.ID.ToString() == Server_ID).OrderBy(x => x.DistanceFromTuple).FirstOrDefault() == null ? fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault() : fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS && a.ID.ToString() == Server_ID).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                        //Server_ID
                        // nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();

                        //do some thing arrange new server which is free
                        nearestFogDeviceUpdated = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                    }
                    Debug.WriteLine("nearestFogDevice " + nearestFogDevice.CurrentAllocatedMips);

                    if (nearestFogDeviceUpdated != null)
                    {
                        nearestFogDevice = nearestFogDeviceUpdated;
                        if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                        {
                            PowerUtility.ConsumePower(nearestFogDevice, tuple);
                            ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                            // check the power required by tuple and available power by fog.
                            tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                            tuple.IsServerFound = true;
                            tuple.FogLevelServed = 1;

                        }
                        else
                        { }
                    }
                    else
                    {
                        var flag = FogSimulator.WithCoo; // for without Cooperation of fog.
                        if (flag)
                        {
                            // queue for fog device can be implemented here. but it requires to send these tuples back to the fog server once the server is free.
                            // for this, it requires to remove null check on nearestFogDevice.
                            foreach (var item in fogList)
                            {
                                var dis = GeoDistance.CalcDistance(nearestFogDevice.GeoLocation.getLongitude(), nearestFogDevice.GeoLocation.getLatitude(),
                                                                    item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                                    GeoCodeCalcMeasurement.Kilometers);
                                lock (Lock)
                                    item.DistanceFromFogServer = dis;
                                var link = new Link()
                                {
                                    Source = nearestFogDevice.Name,
                                    Destination = item.Name,
                                    SDDistance = dis,
                                    Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                                };
                                lock (Lock)
                                    FogUtility.linksList.Add(link);
                            }
                            FogDevice nearestFogDeviceUpdated2 = null;
                            // nearestFogDevice = fogList.Where(x => (x.MIPS - x.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromFogServer).FirstOrDefault();
                            lock (Lock)
                            {
                                nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS && a.ID.ToString() == Server_ID).OrderBy(x => x.DistanceFromTuple).FirstOrDefault() == null ? fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault() : fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS && a.ID.ToString() == Server_ID).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();

                                // if do some thing arrange new server which is free
                                nearestFogDeviceUpdated2 = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                            }

                            if (nearestFogDeviceUpdated2 != null)
                            {
                                nearestFogDevice = nearestFogDeviceUpdated2;

                                if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                                {
                                    //check the power required by tuple and available power by fog.
                                    tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                                    tuple.IsServerFound = true;
                                    tuple.FogLevelServed = 2;
                                    PowerUtility.ConsumePower(nearestFogDevice, tuple);
                                    ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                                }
                                else
                                { }
                            }
                            else
                            {
                                lock (Lock)
                                {
                                    tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                    tuple.IsServerFound = false;
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            lock (Lock)
                            {
                                tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                tuple.IsServerFound = false;
                                return null;
                            }
                        }
                    }
                    return nearestFogDevice;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        #region for memory 
        public static FogDevice _GetValidMemoryDevice(Models.Tuple tuple, List<FogDevice> fogList)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                //  List<FogDevice> fogList = FogSimulator.getList().ToList();
                //faizan changes getting fog for a specific job #change
                fogList = PowerUtility.GetRequiredFogs(tuple, fogList);
                FogDevice nearestFogDevice = null, nearestFogDeviceUpdated = null;
                if (fogList != null)
                {
                    try
                    {
                        //  if (fogCahce.Count > 0)
                        //   {
                        Debug.WriteLine("fogcache " + fogCahce.Count);
                        FogCache F_cache = fogCahce.Where(x => x.DataType == tuple.DataType).OrderBy(x => x.InternalProcessingTime).OrderBy(x => x.link.Propagationtime).FirstOrDefault();
                        if (F_cache != null)
                        {
                            FogDevice usedServer = null;
                            lock (Lock)
                                usedServer = fogList.Where(a => a.ID == F_cache.FogServer).FirstOrDefault();
                            Debug.WriteLine("used server = " + usedServer.ID);
                            if ((usedServer.MIPS - usedServer.CurrentAllocatedMips) > tuple.MIPS)
                            {
                                nearestFogDevice = usedServer;

                                lock (fogCahce)
                                {
                                    fogCahce.Add(new FogCache() { DataType = tuple.DataType, FogServer = usedServer.ID, InternalProcessingTime = tuple.InternalProcessingTime, TupleGuid = tuple.ID, link = F_cache.link });
                                }
                            }
                            else
                            {
                                foreach (var item in fogList)
                                {
                                    var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                                        item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                                        GeoCodeCalcMeasurement.Kilometers);
                                    lock (Lock)
                                    { item.DistanceFromTuple = dis; }
                                    var link = new Link()
                                    {
                                        Source = tuple.Name,
                                        Destination = item.Name,
                                        SDDistance = dis,
                                        Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                                    };
                                    lock (Lock)
                                    {
                                        FogUtility.linksList.Add(link);
                                        nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                                    }
                                    lock (fogCahce)
                                    {
                                        fogCahce.Add(new FogCache() { DataType = tuple.DataType, FogServer = usedServer.ID, InternalProcessingTime = tuple.InternalProcessingTime, TupleGuid = tuple.ID, link = link });
                                    }
                                }
                            }


                        }
                        else
                        {
                            foreach (var item in fogList)
                            {
                                var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                                    item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                                    GeoCodeCalcMeasurement.Kilometers);
                                lock (Lock)
                                { item.DistanceFromTuple = dis; }
                                var link = new Link()
                                {
                                    Source = tuple.Name,
                                    Destination = item.Name,
                                    SDDistance = dis,
                                    Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                                };
                                lock (Lock)
                                {
                                    FogUtility.linksList.Add(link);
                                    nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                                }
                                lock (fogCahce)
                                {
                                    fogCahce.Add(new FogCache() { DataType = tuple.DataType, FogServer = nearestFogDevice.ID, InternalProcessingTime = tuple.InternalProcessingTime, TupleGuid = tuple.ID, link = link });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        throw new ArgumentException(ex.Message);
                    }

                    lock (Lock)
                    {

                        if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                        {
                            //if do some thing arrange new server which is free
                            nearestFogDeviceUpdated = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                        }
                    }
                    Debug.WriteLine("nearestFogDevice " + nearestFogDevice.CurrentAllocatedMips);

                    if (nearestFogDeviceUpdated != null)
                    {
                        nearestFogDevice = nearestFogDeviceUpdated;
                        if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                        {
                            PowerUtility.ConsumePower(nearestFogDevice, tuple);
                            ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                            // check the power required by tuple and available power by fog.

                            tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                            tuple.IsServerFound = true;
                            tuple.FogLevelServed = 1;

                        }
                        else
                        { }
                    }
                    else
                    {
                        var flag = FogSimulator.WithCoo; // for without Cooperation of fog.
                        if (flag)
                        {
                            // queue for fog device can be implemented here. but it requires to send these tuples back to the fog server once the server is free.
                            // for this, it requires to remove null check on nearestFogDevice.
                            foreach (var item in fogList)
                            {
                                var dis = GeoDistance.CalcDistance(nearestFogDevice.GeoLocation.getLongitude(), nearestFogDevice.GeoLocation.getLatitude(),
                                                                    item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                                    GeoCodeCalcMeasurement.Kilometers);
                                lock (item)
                                { item.DistanceFromFogServer = dis; }
                                var link = new Link()
                                {
                                    Source = nearestFogDevice.Name,
                                    Destination = item.Name,
                                    SDDistance = dis,
                                    Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                                };
                                lock (Lock)
                                    FogUtility.linksList.Add(link);
                            }
                            FogDevice nearestFogDeviceUpdated2 = null;
                            nearestFogDevice = fogList.Where(x => (x.MIPS - x.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromFogServer).FirstOrDefault();
                            if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                            {
                                //if do some thing arrange new server which is free
                                nearestFogDeviceUpdated2 = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                            }
                            if (nearestFogDeviceUpdated2 != null)
                            {
                                nearestFogDevice = nearestFogDeviceUpdated2;

                                if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                                {
                                    //check the power required by tuple and available power by fog.
                                    tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                                    tuple.IsServerFound = true;
                                    tuple.FogLevelServed = 2;
                                    PowerUtility.ConsumePower(nearestFogDevice, tuple);
                                    ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                                }
                                else
                                { }
                            }
                            else
                            {
                                lock (Lock)
                                {
                                    tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                    tuple.IsServerFound = false;
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            lock (Lock)
                            {
                                tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                tuple.IsServerFound = false;
                                return null;
                            }
                        }
                    }
                    return nearestFogDevice;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public static FogDevice GetValidMemoryDevice(Models.Tuple tuple, List<FogDevice> fogList)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                //  List<FogDevice> fogList = FogSimulator.getList().ToList();
                //faizan changes getting fog for a specific job #change
                fogList = PowerUtility.GetRequiredFogs(tuple, fogList);
                if (fogList != null)
                {
                    foreach (var item in fogList)
                    {
                        var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                            item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                            GeoCodeCalcMeasurement.Kilometers);
                        lock (Lock)
                        { item.DistanceFromTuple = dis; }
                        var link = new Link()
                        {
                            Source = tuple.Name,
                            Destination = item.Name,
                            SDDistance = dis,
                            Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                        };
                        lock (Lock)
                            FogUtility.linksList.Add(link);
                    }
                    FogDevice nearestFogDevice, nearestFogDeviceUpdated = null;
                    lock (Lock)
                    {
                        nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                        if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                        {
                            //if do some thing arrange new server which is free
                            nearestFogDeviceUpdated = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                        }
                    }
                    Debug.WriteLine("nearestFogDevice " + nearestFogDevice.CurrentAllocatedMips);

                    if (nearestFogDeviceUpdated != null)
                    {
                        nearestFogDevice = nearestFogDeviceUpdated;
                        if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                        {
                            PowerUtility.ConsumePower(nearestFogDevice, tuple);
                            ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                            // check the power required by tuple and available power by fog.

                            tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                            tuple.IsServerFound = true;
                            tuple.FogLevelServed = 1;

                        }
                        else
                        { }
                    }
                    else
                    {
                        var flag = FogSimulator.WithCoo; // for without Cooperation of fog.
                        if (flag)
                        {
                            // queue for fog device can be implemented here. but it requires to send these tuples back to the fog server once the server is free.
                            // for this, it requires to remove null check on nearestFogDevice.
                            foreach (var item in fogList)
                            {
                                var dis = GeoDistance.CalcDistance(nearestFogDevice.GeoLocation.getLongitude(), nearestFogDevice.GeoLocation.getLatitude(),
                                                                    item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                                    GeoCodeCalcMeasurement.Kilometers);
                                lock (item)
                                { item.DistanceFromFogServer = dis; }
                                var link = new Link()
                                {
                                    Source = nearestFogDevice.Name,
                                    Destination = item.Name,
                                    SDDistance = dis,
                                    Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                                };
                                lock (Lock)
                                    FogUtility.linksList.Add(link);
                            }
                            FogDevice nearestFogDeviceUpdated2 = null;
                            nearestFogDevice = fogList.Where(x => (x.MIPS - x.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromFogServer).FirstOrDefault();
                            if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                            {
                                //if do some thing arrange new server which is free
                                nearestFogDeviceUpdated2 = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                            }
                            if (nearestFogDeviceUpdated2 != null)
                            {
                                nearestFogDevice = nearestFogDeviceUpdated2;

                                if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                                {
                                    //check the power required by tuple and available power by fog.
                                    tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                                    tuple.IsServerFound = true;
                                    tuple.FogLevelServed = 2;
                                    PowerUtility.ConsumePower(nearestFogDevice, tuple);
                                    ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                                }
                                else
                                { }
                            }
                            else
                            {
                                lock (Lock)
                                {
                                    tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                    tuple.IsServerFound = false;
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            lock (Lock)
                            {
                                tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                tuple.IsServerFound = false;
                                return null;
                            }
                        }
                    }
                    return nearestFogDevice;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public static FogDevice ValidMemoryDevice(Models.Tuple tuple, List<FogDevice> fogList, FogCache CacheDevice = null)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                //List<FogDevice> fogList = FogSimulator.getList().ToList();
                //faizan changes getting fog for a specific job #change
                fogList = PowerUtility.GetRequiredFogs(tuple, fogList);
                FogDevice nearestFogDevice, nearestFogDeviceUpdated = null;
                if (fogList != null)
                {
                    nearestFogDevice = fogList.Where(a => a.ID == CacheDevice.FogServer).FirstOrDefault(); ;

                    // foreach (var item in fogList)
                    // {
                    if (nearestFogDevice.MIPS > nearestFogDevice.CurrentAllocatedMips)
                    {
                        var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                            nearestFogDevice.GeoLocation.getLongitude(), nearestFogDevice.GeoLocation.getLatitude(),
                                                            GeoCodeCalcMeasurement.Kilometers);
                        lock (Lock)
                        { nearestFogDevice.DistanceFromTuple = dis; }
                        var link = new Link()
                        {
                            Source = tuple.Name,
                            Destination = nearestFogDevice.Name,
                            SDDistance = dis,
                            Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                        };
                        lock (Lock)
                            FogUtility.linksList.Add(link);
                        // }
                        nearestFogDeviceUpdated = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                    }
                    else
                    {

                        lock (Lock)
                        {
                            nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                            if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                            {
                                //if do some thing arrange new server which is free
                                nearestFogDeviceUpdated = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                            }
                        }
                        Debug.WriteLine("nearestFogDevice " + nearestFogDevice.CurrentAllocatedMips);
                    }
                    if (nearestFogDeviceUpdated != null)
                    {
                        nearestFogDevice = nearestFogDeviceUpdated;
                        if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                        {
                            PowerUtility.ConsumePower(nearestFogDevice, tuple);
                            ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                            // check the power required by tuple and available power by fog.

                            tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                            tuple.IsServerFound = true;
                            tuple.FogLevelServed = 1;

                        }
                        else
                        { }
                    }
                    else
                    {
                        var flag = FogSimulator.WithCoo; // for without Cooperation of fog.
                        if (flag)
                        {
                            // queue for fog device can be implemented here. but it requires to send these tuples back to the fog server once the server is free.
                            // for this, it requires to remove null check on nearestFogDevice.
                            foreach (var item in fogList)
                            {
                                var dis = GeoDistance.CalcDistance(nearestFogDevice.GeoLocation.getLongitude(), nearestFogDevice.GeoLocation.getLatitude(),
                                                                    item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                                    GeoCodeCalcMeasurement.Kilometers);
                                lock (item)
                                { item.DistanceFromFogServer = dis; }
                                var link = new Link()
                                {
                                    Source = nearestFogDevice.Name,
                                    Destination = item.Name,
                                    SDDistance = dis,
                                    Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                                };
                                lock (Lock)
                                    FogUtility.linksList.Add(link);
                            }
                            FogDevice nearestFogDeviceUpdated2 = null;
                            nearestFogDevice = fogList.Where(x => (x.MIPS - x.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromFogServer).FirstOrDefault();
                            if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                            {
                                //if do some thing arrange new server which is free
                                nearestFogDeviceUpdated2 = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                            }
                            if (nearestFogDeviceUpdated2 != null)
                            {
                                nearestFogDevice = nearestFogDeviceUpdated2;

                                if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                                {
                                    //check the power required by tuple and available power by fog.
                                    tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                                    tuple.IsServerFound = true;
                                    tuple.FogLevelServed = 2;
                                    PowerUtility.ConsumePower(nearestFogDevice, tuple);
                                    ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                                }
                                else
                                { }
                            }
                            else
                            {
                                lock (Lock)
                                {
                                    tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                    tuple.IsServerFound = false;
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            lock (Lock)
                            {
                                tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                tuple.IsServerFound = false;
                                return null;
                            }
                        }
                    }
                    return nearestFogDevice;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        #endregion
        public static FogDevice GetValidEdgeDevice(Models.Tuple tuple, List<FogDevice> edgeList)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                edgeList = PowerUtility.GetRequiredFogs(tuple, edgeList);
                if (edgeList != null)
                {
                    FogDevice nearestEdgeDevice, nearestEdgeDeviceUpdated = null;

                    foreach (var item in edgeList)
                    {
                        var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                            item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                            GeoCodeCalcMeasurement.Kilometers);
                        dis = dis > 0 ? (dis / 2) : 0;
                        item.DistanceFromTuple = dis;
                        var link = new Link()
                        {
                            Source = tuple.Name,
                            Destination = item.Name,
                            SDDistance = dis,
                            Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                        };
                        lock (Lock)
                        {
                            FogUtility.linksList.Add(link);
                        }
                    }
                    nearestEdgeDevice = edgeList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.CurrentAllocatedMips).FirstOrDefault();
                    // .OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                    // && (a.RAM - a.CurrentAllocatedRam) > tuple.RAM
                    // .OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                    // .OrderBy(x => x.CurrentAllocatedMips)
                    // if ((nearestEdgeDevice.MIPS - nearestEdgeDevice.CurrentAllocatedMips) > tuple.MIPS)
                    // {
                    // if do some thing arrange new server which is free
                    nearestEdgeDeviceUpdated = IsResourcesAvailableChanged(nearestEdgeDevice, tuple, edgeList);
                    // }

                    Debug.WriteLine("nearestEdgeDevice " + nearestEdgeDevice.CurrentAllocatedMips + "nearestEdgeDevice Name = " + nearestEdgeDevice.Name);

                    if (nearestEdgeDeviceUpdated != null)
                    {
                        lock (Lock)
                        {
                            nearestEdgeDevice = nearestEdgeDeviceUpdated;
                            if (nearestEdgeDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestEdgeDevice))
                            {
                                PowerUtility.ConsumePower(nearestEdgeDevice, tuple);
                                ResourceUtility.ResourceConsumptionChanged(nearestEdgeDevice, tuple, edgeList);
                                // check the power required by tuple and available power by fog.
                                tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestEdgeDevice, edgeList);
                                tuple.IsServerFound = true;
                                tuple.FogLevelServed = 1;

                            }
                        }
                    }
                    else
                    {
                        lock (Lock)
                        {
                            tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                            tuple.IsServerFound = false;
                            return null;
                        }
                    }
                    return nearestEdgeDevice;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public static FogDevice GetValidEdgeDeviceBuffer(Models.Tuple tuple, List<FogDevice> edgeList)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                //faizan changes getting fog for a specific job #change
                edgeList = PowerUtility.GetRequiredFogs(tuple, edgeList);
                if (edgeList != null)
                {
                    foreach (var item in edgeList)
                    {
                        var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                            item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                            GeoCodeCalcMeasurement.Kilometers);
                        dis = dis > 0 ? (dis / 2) : 0;
                        lock (Lock)
                        { item.DistanceFromTuple = dis; }
                        var link = new Link()
                        {
                            Source = tuple.Name,
                            Destination = item.Name,
                            SDDistance = dis,
                            Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                        };
                        lock (Lock)
                            FogUtility.linksList.Add(link);
                    }
                    FogDevice nearestEdgeDevice, nearestEdgeDeviceUpdated = null;
                    lock (Lock)
                    {
                        nearestEdgeDevice = edgeList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS && (a.RAM - a.CurrentAllocatedRam) > tuple.RAM).FirstOrDefault();
                        //.OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                        //.OrderBy(x => x.CurrentAllocatedMips)
                        if ((nearestEdgeDevice.MIPS - nearestEdgeDevice.CurrentAllocatedMips) > tuple.MIPS)
                        {
                            //if do some thing arrange new server which is free
                            nearestEdgeDeviceUpdated = IsResourcesAvailableChanged(nearestEdgeDevice, tuple, edgeList);
                        }
                    }
                    Debug.WriteLine("nearestEdgeDevice " + nearestEdgeDevice.CurrentAllocatedMips);

                    if (nearestEdgeDeviceUpdated != null)
                    {
                        nearestEdgeDevice = nearestEdgeDeviceUpdated;
                        if (nearestEdgeDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestEdgeDevice))
                        {
                            PowerUtility.ConsumePower(nearestEdgeDevice, tuple);
                            ResourceUtility.ResourceConsumptionChanged(nearestEdgeDevice, tuple, edgeList);
                            // check the power required by tuple and available power by fog.
                            tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestEdgeDevice, edgeList);
                            tuple.IsServerFound = true;
                            tuple.FogLevelServed = 1;
                        }
                        else
                        {
                            EdgeSimulator.DropedtupleList.Add(tuple);
                        }
                    }
                    else
                    {
                        lock (Lock)
                        {
                            tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                            tuple.IsServerFound = false;
                            return null;
                        }
                    }
                    return nearestEdgeDevice;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public static FogDevice GetValidFogDeviceFoEdgeFog(Models.Tuple tuple, List<FogDevice> fogList)
        {
            try
            {
                var rnd = new Random();
                double[] arrLatency = { 4, 5, 6 };
                // List<FogDevice> fogList = FogSimulator.getList().ToList();
                // faizan changes getting fog for a specific job #change
                fogList = PowerUtility.GetRequiredFogs(tuple, fogList);
                if (fogList != null)
                {
                    foreach (var item in fogList)
                    {
                        var dis = GeoDistance.CalcDistance(tuple.GeoLocation.getLongitude(), tuple.GeoLocation.getLatitude(),
                                                            item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                            GeoCodeCalcMeasurement.Kilometers);
                        lock (Lock)
                        { item.DistanceFromTuple = dis; }
                        var link = new Link()
                        {
                            Source = tuple.Name,
                            Destination = item.Name,
                            SDDistance = dis,
                            Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                        };
                        lock (Lock)
                            FogUtility.linksList.Add(link);
                    }
                    FogDevice nearestFogDevice, nearestFogDeviceUpdated = null;
                    lock (Lock)
                    {
                        nearestFogDevice = fogList.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromTuple).FirstOrDefault();
                        if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                        {
                            //if do some thing arrange new server which is free
                            nearestFogDeviceUpdated = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                        }
                    }
                    Debug.WriteLine("nearestFogDevice " + nearestFogDevice.CurrentAllocatedMips);

                    if (nearestFogDeviceUpdated != null)
                    {
                        nearestFogDevice = nearestFogDeviceUpdated;
                        if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                        {
                            PowerUtility.ConsumePower(nearestFogDevice, tuple);
                            ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                            // check the power required by tuple and available power by fog.

                            tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                            tuple.IsServerFound = true;
                            tuple.FogLevelServed = 1;

                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        var flag = EdgeSimulator.WithCoo; // for without Cooperation of fog.
                        if (flag)
                        {
                            // queue for fog device can be implemented here. but it requires to send these tuples back to the fog server once the server is free.
                            // for this, it requires to remove null check on nearestFogDevice.
                            foreach (var item in fogList)
                            {
                                var dis = GeoDistance.CalcDistance(nearestFogDevice.GeoLocation.getLongitude(), nearestFogDevice.GeoLocation.getLatitude(),
                                                                    item.GeoLocation.getLongitude(), item.GeoLocation.getLatitude(),
                                                                    GeoCodeCalcMeasurement.Kilometers);
                                lock (item)
                                { item.DistanceFromFogServer = dis; }
                                var link = new Link()
                                {
                                    Source = nearestFogDevice.Name,
                                    Destination = item.Name,
                                    SDDistance = dis,
                                    Propagationtime = LinkUtility.CalculateLatency(dis, cooperSpeed, tuple.Size, transmitionRate, arrLatency[rnd.Next(arrLatency.Length)])
                                };
                                lock (Lock)
                                    FogUtility.linksList.Add(link);
                            }
                            FogDevice nearestFogDeviceUpdated2 = null;
                            nearestFogDevice = fogList.Where(x => (x.MIPS - x.CurrentAllocatedMips) > tuple.MIPS).OrderBy(x => x.DistanceFromFogServer).FirstOrDefault();
                            if ((nearestFogDevice.MIPS - nearestFogDevice.CurrentAllocatedMips) > tuple.MIPS)
                            {
                                //if do some thing arrange new server which is free
                                nearestFogDeviceUpdated2 = IsResourcesAvailableChanged(nearestFogDevice, tuple, fogList);
                            }
                            if (nearestFogDeviceUpdated2 != null)
                            {
                                nearestFogDevice = nearestFogDeviceUpdated2;

                                if (nearestFogDevice.AvailablePower > PowerUtility.GetRequiredPowerPercentage(tuple, nearestFogDevice))
                                {
                                    //check the power required by tuple and available power by fog.
                                    tuple.InternalProcessingTime = FogInternalProcessingChanged(tuple, nearestFogDevice, fogList);
                                    tuple.IsServerFound = true;
                                    tuple.FogLevelServed = 2;
                                    PowerUtility.ConsumePower(nearestFogDevice, tuple);
                                    ResourceUtility.ResourceConsumptionChanged(nearestFogDevice, tuple, fogList);
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                lock (Lock)
                                {
                                    tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                    tuple.IsServerFound = false;
                                    return null;
                                }
                            }
                        }
                        else
                        {
                            lock (Lock)
                            {
                                tuple.IsReversed = true; //return IsReversed when server is fully occupied.
                                tuple.IsServerFound = false;
                                return null;
                            }
                        }
                    }
                    return nearestFogDevice;
                }
                else return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
        public static bool IsResourcesAvailable(FogDevice fogDevice, SFog.Models.Tuple tuple)
        {
            if (fogDevice.CurrentAllocatedBw <= fogDevice.UpBW &&
               fogDevice.CurrentAllocatedMips <= fogDevice.MIPS &&
               fogDevice.CurrentAllocatedRam <= fogDevice.RAM &&
               fogDevice.CurrentAllocatedSize <= fogDevice.Size)
            {
                return true;
            }
            return false;
        }
        public static FogDevice IsResourcesAvailableChanged(FogDevice fogDev, SFog.Models.Tuple tuple, List<FogDevice> devcieList)
        {
            lock (Lock)
            {
                var fogDevice = devcieList.FirstOrDefault(x => x.ID.Equals(fogDev.ID));

               


                if (fogDevice != null)
                    {

                        if ((fogDevice.MIPS - fogDevice.CurrentAllocatedMips) >= tuple.MIPS &&
                           (fogDevice.RAM - fogDevice.CurrentAllocatedRam) >= tuple.RAM)
                        {
                            if (fogDevice.CurrentAllocatedMips < 0)
                                return null;
                            else return fogDevice;
                        }
                        else
                            return null;
                    }
                    else { return null; }
                }
            

        }

        public static FogDevice IsResourcesAvailableChangedforMRR(FogDevice fogDev, SFog.Models.Tuple tuple, List<FogDevice> devcieList)
        {
            lock (Lock)
            {
                var fogDevice = devcieList.FirstOrDefault(x => x.ID.Equals(fogDev.ID));

                while (fogDevice.MIPS - fogDevice.CurrentAllocatedMips < tuple.MIPS ||
                      (fogDevice.RAM - fogDevice.CurrentAllocatedRam) < tuple.RAM)
                {
                    System.Threading.Thread.Sleep(50);
                }


                if (fogDevice != null)
                {

                    if ((fogDevice.MIPS - fogDevice.CurrentAllocatedMips) >= tuple.MIPS &&
                       (fogDevice.RAM - fogDevice.CurrentAllocatedRam) >= tuple.RAM)
                    {
                        if (fogDevice.CurrentAllocatedMips < 0)
                            return null;
                        else return fogDevice;
                    }
                    else
                        return null;
                }
                else { return null; }
            }


        }

        public static FogDevice IsResourcesAvailableChangedMRR(FogDevice fogDev, SFog.Models.Tuple tuple)
        {
            lock (Lock)
            {
                var fogDevice = fogDev;
                if (fogDevice != null)
                {   // fogDevice.CurrentAllocatedSize <= fogDevice.Size
                    // fogDevice.CurrentAllocatedBw <= fogDevice.UpBW &&
                    // if (fogDevice.CurrentAllocatedMips <= fogDevice.MIPS &&
                    // fogDevice.CurrentAllocatedRam <= fogDevice.RAM) { return fogDevice; }

                    if ((fogDevice.MIPS - fogDevice.CurrentAllocatedMips) >= tuple.MIPS &&
                       (fogDevice.RAM - fogDevice.CurrentAllocatedRam) >= tuple.RAM)
                    {
                        if (fogDevice.CurrentAllocatedMips < 0)
                            return null;
                        else return fogDevice;
                    }
                    else
                        return null;
                }
                else { return null; }
            }
        }

        public static bool ResourceConsumption(FogDevice fogDevice, SFog.Models.Tuple tuple)
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
                    return true;
                }
                return false;
            }
        }

        public static void ResourceRelease(FogDevice fogDevice, SFog.Models.Tuple tuple)
        {
            lock (Lock)
            {
                fogDevice.CurrentAllocatedBw = fogDevice.CurrentAllocatedBw - tuple.BW;
                fogDevice.CurrentAllocatedMips = fogDevice.CurrentAllocatedMips - tuple.MIPS;
                fogDevice.CurrentAllocatedRam = fogDevice.CurrentAllocatedRam - tuple.RAM;
                fogDevice.CurrentAllocatedSize = fogDevice.CurrentAllocatedSize - tuple.Size;
            }
        }

        public static double FogInternalProcessing(Models.Tuple tuple, FogDevice fogDevice)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            lock (Lock)
            {
                if (PowerUtility.GetPercentageOfServerFreePower(fogDevice) != 0)
                {
                    for (int i = 0; i < int.Parse(Math.Ceiling((decimal)PowerUtility.GetRequiredPowerPercentage(tuple, fogDevice) / PowerUtility.GetPercentageOfServerFreePower(fogDevice)).ToString()); i++)
                    {
                        //Internal Processing time.
                    }
                }
            }
            watch.Stop();
            return watch.Elapsed.TotalMilliseconds;
        }
        public static double FogInternalProcessingChanged(Models.Tuple tuple, FogDevice fogDev, List<FogDevice> deviceList)
        {

            Stopwatch watch = new Stopwatch();
            watch.Start();
            FogDevice fogDevice = new FogDevice();
            lock (Lock)
            {
                fogDevice = deviceList.FirstOrDefault(x => x.ID.Equals(fogDev.ID));
            }

            if (PowerUtility.GetPercentageOfServerFreePower(fogDevice) != 0)
            {
                int executionTime = Convert.ToInt32(Math.Round(tuple.MIPS / (fogDevice.MIPS / 1000)));
                for (int i = 0; i < int.Parse(Math.Ceiling((decimal)PowerUtility.GetRequiredPowerPercentage(tuple, fogDevice) / PowerUtility.GetPercentageOfServerFreePower(fogDevice)).ToString()); i++)
                {
                    System.Threading.Thread.Sleep(executionTime);
                }
            }
            watch.Stop();
            return watch.Elapsed.TotalMilliseconds;

        }

        public static double FogInternalProcessingChangedMRR(Models.Tuple tuple, FogDevice fogDev)
        {

            Stopwatch watch = new Stopwatch();
            watch.Start();
            FogDevice fogDevice = new FogDevice();
            lock (Lock)
            {
                fogDevice = fogDev;
            }

            if (PowerUtility.GetPercentageOfServerFreePower(fogDevice) != 0)
            {
                int executionTime = Convert.ToInt32(Math.Round(tuple.MIPS / (fogDevice.MIPS / 1000)));
                for (int i = 0; i < int.Parse(Math.Ceiling((decimal)PowerUtility.GetRequiredPowerPercentage(tuple, fogDevice) / PowerUtility.GetPercentageOfServerFreePower(fogDevice)).ToString()); i++)
                {
                    System.Threading.Thread.Sleep(executionTime);
                }
            }
            watch.Stop();
            return watch.Elapsed.TotalMilliseconds;

        }

    }
}

