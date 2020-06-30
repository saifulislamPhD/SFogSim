using SFog.Models.Brokers;
using SFog.Models.Links;
using SFog.Models.Nodes;
using SFog.Models.Nodes.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models
{
    public class Results
    {
        private double elapsedTime;

        public double ElapsedTime
        {
            get { return elapsedTime; }
            set { elapsedTime = value; }
        }

        private Models.Tuple tuple;

        public Models.Tuple Tuple
        {
            get { return tuple; }
            set { tuple = value; }
        }

        private FogDevice fogDevice;

        public FogDevice FogDevice
        {
            get { return fogDevice; }
            set { fogDevice = value; }
        }

        private CloudDevice cloudDevice;

        public CloudDevice CloudDevice
        {
            get { return cloudDevice; }
            set { cloudDevice = value; }
        }

        private string initiatesTime;

        public string InitiatesTime
        {
            get { return initiatesTime; }
            set { initiatesTime = value; }
        }

        private Link link;

        public Link Link
        {
            get { return link; }
            set { link = value; }
        }

        private List<FogDevice> inActiveFogDecives;

        public List<FogDevice> InActiveFogDecives
        {
            get { return inActiveFogDecives; }
            set { inActiveFogDecives = value; }
        }

        private CloudBroker cloudBroker;

        public CloudBroker CloudBroker
        {
            get { return cloudBroker; }
            set { cloudBroker = value; }
        }

        private FogBroker fogBroker;

        public FogBroker FogBroker
        {
            get { return fogBroker; }
            set { fogBroker = value; }
        }
    }
}