using SFog.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Nodes.Cloud
{
    public class VM
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

        private long bw;
        /** The required bw. */

        public long BW
        {
            get { return bw; }
            set { bw = value; }
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

        public VM(Guid id, int userId, double mips, int numberOfPes, int ram, long bw, long size, string name,
                        CloudletScheduler cloudletScheduler, GeoLocation geoLocation)
        {
            ID = id;
            MIPS = mips;
            NumberOfPes = numberOfPes;
            RAM = ram;
            BW = bw;
            Size = size;
            Name = name;
            DataType = dataType;
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
        }
    }
}