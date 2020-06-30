using SFog.Models.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models
{
    public class FogPost
    {
        private int fogSize;

        public int FogSize
        {
            get { return fogSize; }
            set { fogSize = value; }
        }

        private FogDevice fogDevice;

        public FogDevice FogDevice
        {
            get { return fogDevice; }
            set { fogDevice = value; }
        }
    }
}