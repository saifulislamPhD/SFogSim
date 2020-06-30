using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Nodes.Cloud.Managers
{
    public class RamManager
    {
        private int ram;

        public int Ram
        {
            get { return ram; }
            set { ram = value; }
        }

        private int availableRam;

        public int AvailableRam
        {
            get { return availableRam; }
            set { availableRam = value; }
        }
    }
}