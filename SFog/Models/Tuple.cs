using SFog.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models
{
    public class Tuple
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

        private bool isJobFinished;
        public bool IsJobFinished
        {

            get { return isJobFinished; }
            set { isJobFinished = value; }
        }

        private int numerofpes;
        /** The number of PEs required by the VM. */

        public int NumberOfPes
        {
            get { return numerofpes; }
            set { numerofpes = value; }
        }

        private int burstNumber;
        public int BurstNumber
        {
            get { return burstNumber; }
            set { burstNumber = value; }

        }

        private int ram;
        /** The required ram. */

        public int RAM
        {
            get { return ram; }
            set { ram = value; }
        }

        private long bw;
        /** The required bw. */

        public long BW
        {
            get { return bw; }
            set { bw = value; }
        }
        private string source;

        public string Source
        {
            get { return source; }
            set { source = value; }
        }
        private string destination;

        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }
        private double delay;
        public double Delay
        {
            get { return delay; }
            set { delay = Delay; }
        }
        private string priority;
        public string Priority
        {
            get { return priority; }
            set { priority = value; }
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

        private List<double> currentAllocatedMips;
        /** The current allocated mips for each VM's PE. */

        public List<double> CurrentAllocatedMips
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

        private string dataType;

        public string DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        private int dataPercentage;

        public int DataPercentage
        {
            get { return dataPercentage; }
            set { dataPercentage = value; }
        }

        private bool isReversed;

        public bool IsReversed
        {
            get { return isReversed; }
            set { isReversed = value; }
        }

        private bool isServerFound;

        public bool IsServerFound
        {
            get { return isServerFound; }
            set { isServerFound = value; }
        }

        private bool isCloudServed;

        public bool IsCloudServed
        {
            get { return isCloudServed; }
            set { isCloudServed = value; }
        }

        private bool isServed;

        public bool IsServed
        {
            get { return isServed; }
            set { isServed = value; }
        }

        private string deviceType;

        public string DeviceType
        {
            get { return deviceType; }
            set { deviceType = value; }
        }

        private Services service;

        public Services Service
        {
            get { return service; }
            set { service = value; }
        }

        private double queueDelay;

        public double QueueDelay
        {
            get { return queueDelay; }
            set { queueDelay = value; }
        }

        private double internalProcessingTime;

        public double InternalProcessingTime
        {
            get { return internalProcessingTime; }
            set { internalProcessingTime = value; }
        }

        private List<TupleTimes> tupleTimes;

        public List<TupleTimes> TupleTimes
        {
            get { return tupleTimes; }
            set { tupleTimes = value; }
        }

        private int fogLevelServed;

        public int FogLevelServed
        {
            get { return fogLevelServed; }
            set { fogLevelServed = value; }
        }

        private bool isServedByFC_Cloud;

        public bool IsServedByFC_Cloud
        {
            get { return isServedByFC_Cloud; }
            set { isServedByFC_Cloud = value; }
        }
         private double  burstTime;
        public double BurstTime
        {
            get { return burstTime; }
            set { burstTime = value; }

        }

        private double? burstTimeDiffernce;
        public double ? BurstTimeDifference
        {

            get { return burstTimeDiffernce; }
            set { burstTimeDiffernce = value; }
        }

        public Tuple(Guid id, int userId, double mips, int numberOfPes, int ram, long bw, long size, string name, string dataType, int dataPercentage, double delay, string priority,
                        CloudletScheduler cloudletScheduler, GeoLocation geoLocation, bool isReversed, string deviceType, double  BTime, double? BTimeDifference)
        {
            ID = id;
            MIPS = mips;
            NumberOfPes = numberOfPes;
            RAM = ram;
            BW = bw;
            Size = size;
            Name = name;
            DataType = dataType;
            Delay = delay;
            Priority = priority;
            CloudletScheduler = cloudletScheduler;
            BeingInstantiated = true;
            CurrentAllocatedBw = 0;
            CurrentAllocatedMips = null;
            CurrentAllocatedRam = 0;
            CurrentAllocatedSize = 0;
            GeoLocation = geoLocation;
            DataPercentage = dataPercentage;
            IsReversed = isReversed;
            DeviceType = deviceType;
            InternalProcessingTime = 0;
            TupleTimes = new List<TupleTimes>();
            burstTime = BTime;
            burstTimeDiffernce = BTimeDifference;
            IsJobFinished = false;
            BurstNumber = 0;
        }
    }
}