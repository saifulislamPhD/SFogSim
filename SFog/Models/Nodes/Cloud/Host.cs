using SFog.Models.Nodes.Cloud.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Nodes.Cloud
{
    public class Host
    {
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private long storage;

        public long Storage
        {
            get { return storage; }
            set { storage = value; }
        }

        private long mips;

        public long MIPS
        {
            get { return mips; }
            set { mips = value; }
        }

        private int pr;

        public int PE
        {
            get { return pr; }
            set { pr = value; }
        }

        private bool failed;

        public bool Failed
        {
            get { return failed; }
            set { failed = value; }
        }

        private string dataCenter;

        public string DataCenter
        {
            get { return dataCenter; }
            set { dataCenter = value; }
        }

        private List<VM> vmList;

        public List<VM> VMList
        {
            get { return vmList; }
            set { vmList = value; }
        }

        private double bw;

        public double BW
        {
            get { return bw; }
            set { bw = value; }
        }

        private double ram;

        public double Ram
        {
            get { return ram; }
            set { ram = value; }
        }

        private ResourceManager resourceManager;

        public ResourceManager ResourceManager
        {
            get { return resourceManager; }
            set { resourceManager = value; }
        }
    }
}