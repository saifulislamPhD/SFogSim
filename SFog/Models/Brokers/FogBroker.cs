using SFog.Models.Nodes;
using SFog.Models.Nodes.Cloud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Brokers
{
    public class FogBroker
    {
        private Tuple tuple;

        public Tuple Tuple
        {
            get { return tuple; }
            set { tuple = value; }
        }

        private List<FogDevice> fogList;

        public List<FogDevice> FogList
        {
            get { return fogList; }
            set { fogList = value; }
        }

        private FogDevice selectedFogDevice;

        public FogDevice SelectedFogDevice
        {
            get { return selectedFogDevice; }
            set { selectedFogDevice = value; }
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