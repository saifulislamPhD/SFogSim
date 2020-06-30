using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Nodes.Cloud.Managers
{
    public class BWManager
    {
        private long bw;

        public long BW
        {
            get { return bw; }
            set { bw = value; }
        }

        private long availablebw;

        public long AvailableBW
        {
            get { return availablebw; }
            set { availablebw = value; }
        }
    }
}