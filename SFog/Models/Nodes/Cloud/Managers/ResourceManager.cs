using System.Collections.Generic;

namespace SFog.Models.Nodes.Cloud.Managers
{
    public class ResourceManager
    {
        private List<VM> vmList;

        public List<VM> VMList
        {
            get { return vmList; }
            set { vmList = value; }
        }

        private List<Host> hostList;

        public List<Host> HostList
        {
            get { return hostList; }
            set { hostList = value; }
        }

        private List<DataCenter> datacenterList;

        public List<DataCenter> DataCenterList
        {
            get { return datacenterList; }
            set { datacenterList = value; }
        }

        private RamManager ramManager;

        public RamManager RamManager
        {
            get { return ramManager; }
            set { ramManager = value; }
        }

        private PeManager peManager;

        public PeManager PeManager
        {
            get { return peManager; }
            set { peManager = value; }
        }

        private StorageManager storageManager;

        public StorageManager StorageManager
        {
            get { return storageManager; }
            set { storageManager = value; }
        }

        private MIPSManager mipsManager;

        public MIPSManager MIPSManager
        {
            get { return mipsManager; }
            set { mipsManager = value; }
        }

        private BWManager bwManager;

        public BWManager BWManager
        {
            get { return bwManager; }
            set { bwManager = value; }
        }

        public ResourceManager()
        {
        }

        public ResourceManager(RamManager rm, PeManager pem, StorageManager sm, MIPSManager mipsm, BWManager bwm)
        {
            RamManager = rm;
            PeManager = pem;
            StorageManager = sm;
            MIPSManager = mipsm;
            BWManager = bwm;
        }
    }
}