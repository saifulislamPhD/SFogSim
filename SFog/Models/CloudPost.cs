using SFog.Models.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models
{
    public class CloudPost
    {
        private int cloudSize;

        public int CloudSize
        {
            get { return cloudSize; }
            set { cloudSize = value; }
        }

        private CloudDevice cloudDevice;

        public CloudDevice CloudDevice
        {
            get { return cloudDevice; }
            set { cloudDevice = value; }
        }
    }
}