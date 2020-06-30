using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Nodes.Cloud.Managers
{
    public class PeManager
    {
        private int pe;

        public int PE
        {
            get { return pe; }
            set { pe = value; }
        }

        private int availablepe;

        public int AvailablePE
        {
            get { return availablepe; }
            set { availablepe = value; }
        }
    }
}