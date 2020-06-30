using SFog.Models.Nodes.Cloud;
using SFog.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Brokers
{
    public class CloudBroker
    {
        private Tuple tuple;

        public Tuple Tuple
        {
            get { return tuple; }
            set { tuple = value; }
        }

        private List<DataCenter> datacenterList;

        public List<DataCenter> DatacenterList
        {
            get { return datacenterList; }
            set { datacenterList = value; }
        }

        private Services service;

        public Services Service
        {
            get { return service; }
            set { service = value; }
        }

        private DataCenter selectedDataCenter;

        public DataCenter SelectedDataCenter
        {
            get { return selectedDataCenter; }
            set { selectedDataCenter = value; }
        }

        private Host selectedHost;

        public Host SelectedHost
        {
            get { return selectedHost; }
            set { selectedHost = value; }
        }

        private VM selectedVM;

        public VM SelectedVM
        {
            get { return selectedVM; }
            set { selectedVM = value; }
        }
    }
}