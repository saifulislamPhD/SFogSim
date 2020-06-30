using SFog.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Nodes
{
    public class FogDevice
    {
        private Guid Id;
        /** The FogDevice unique id. */

        public Guid ID
        {
            get { return Id; }
            set { Id = value; }
        }

        private long size;
        /** The size the VM image size (the amount of storage it will use, at least initially). */

        public long Size
        {
            get { return size; }
            set { size = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private double mips;
        /** The MIPS capacity of each VM's PE. */

        public double MIPS
        {
            get { return mips; }
            set { mips = value; }
        }

        private int numerofpes;
        /** The number of PEs required by the VM. */

        public int NumberOfPes
        {
            get { return numerofpes; }
            set { numerofpes = value; }
        }

        private int ram;
        /** The required ram. */

        public int RAM
        {
            get { return ram; }
            set { ram = value; }
        }

        private long upBw;
        /** The required bw. */

        public long UpBW
        {
            get { return upBw; }
            set { upBw = value; }
        }

        private long downBw;
        /** The required bw. */

        public long DownBW
        {
            get { return downBw; }
            set { downBw = value; }
        }
        private double availability;

        public double Availability
        {
            get { return availability; }
            set { availability = value; }
        }
        private double relibility;

        public double Relibility
        {
            get { return relibility; }
            set { relibility = value; }
        }
        private CloudletScheduler cloudletScheduler;
        /** The Cloudlet scheduler the VM uses to schedule cloudlets execution. */

        public CloudletScheduler CloudletScheduler
        {
            get { return cloudletScheduler; }
            set { cloudletScheduler = value; }
        }

        private long currentAllocatedSize;
        /** The current allocated storage size. */

        public long CurrentAllocatedSize
        {
            get { return currentAllocatedSize; }
            set { currentAllocatedSize = value; }
        }

        private int currentAllocatedRam;
        /** The current allocated ram. */

        public int CurrentAllocatedRam
        {
            get { return currentAllocatedRam; }
            set { currentAllocatedRam = value; }
        }

        private long currentAllocatedBw;
        /** The current allocated bw. */

        public long CurrentAllocatedBw
        {
            get { return currentAllocatedBw; }
            set { currentAllocatedBw = value; }
        }

        private double currentAllocatedMips;
        /** The current allocated mips for each VM's PE. */

        public double CurrentAllocatedMips
        {
            get { return currentAllocatedMips; }
            set { currentAllocatedMips = value; }
        }

        private bool beingInstantiated;
        /** Indicates if the VM is being instantiated. */

        public bool BeingInstantiated
        {
            get { return beingInstantiated; }
            set { beingInstantiated = value; }
        }

        private GeoLocation geoLocation;

        public GeoLocation GeoLocation
        {
            get { return geoLocation; }
            set { geoLocation = value; }
        }

        private int storage;

        public int Storage
        {
            get { return storage; }
            set { storage = value; }
        }

        private string dataType;

        public string DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        private double distanceFromTuple;

        public double DistanceFromTuple
        {
            get { return distanceFromTuple; }
            set { distanceFromTuple = value; }
        }

        private double distanceFromFogServer;

        public double DistanceFromFogServer
        {
            get { return distanceFromFogServer; }
            set { distanceFromFogServer = value; }
        }

        private bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        private double busyPower;

        public double BusyPower
        {
            get { return busyPower; }
            set { busyPower = value; }
        }

       

        private double idlePower;

        public double IdlePower
        {
            get { return idlePower; }
            set { idlePower = value; }
        }

        private double availablePower;

        public double AvailablePower
        {
            get { return availablePower; }
            set { availablePower = value; }
        }

        private double processorBurstTime;
        public double ProcessorBurstTime
        {
            get { return processorBurstTime; }
            set { processorBurstTime = value; }
        }

        private int maxCapacity;
        public int MaxCapacity
        {

            get { return maxCapacity; }
            set { maxCapacity = value; }
        }

        private List<PowerConsumption> powerConsumption;

        public List<PowerConsumption> PowerConsumption
        {
            get { return powerConsumption; }
            set { powerConsumption = value; }
        }

        private List<FogTimes> fogTimes;

        public List<FogTimes> FogTimes
        {
            get { return fogTimes; }
            set { fogTimes = value; }
        }

        public FogDevice()
        { }

        public FogDevice(Guid id, int userId, double mips, int numberOfPes, int ram, long upBw, long downBw, long size, int Storage, string name, string dataType,
            CloudletScheduler cloudletScheduler, GeoLocation geoLocation, bool isActive, double busyPower, double idlepower,double ProcessorBurstTime)
        {
            ID = id;
            MIPS = mips;
            NumberOfPes = numberOfPes;
            RAM = ram;
            UpBW = upBw;
            DownBW = downBw;
            Size = size;
            Storage = storage;
            Name = name;
            DataType = dataType;
            CloudletScheduler = cloudletScheduler;
            BeingInstantiated = true;
            CurrentAllocatedBw = 0;
            CurrentAllocatedMips = 0;
            CurrentAllocatedRam = 0;
            CurrentAllocatedSize = 0;
            GeoLocation = geoLocation;
            DistanceFromTuple = 0.0;
            IsActive = isActive;
            BusyPower = busyPower;
            IdlePower = idlepower;
            PowerConsumption = new List<PowerConsumption>();
            AvailablePower = 100;
            processorBurstTime = ProcessorBurstTime;
            


        }
    }

    public class PowerConsumption
    {
        private double powerValue;

        public double PowerValue
        {
            get { return powerValue; }
            set { powerValue = value; }
        }

        private string time;

        public string Time
        {
            get { return time; }
            set { time = value; }
        }
    }
}