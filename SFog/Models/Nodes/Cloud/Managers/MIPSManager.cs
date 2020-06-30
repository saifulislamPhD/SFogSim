using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Nodes.Cloud.Managers
{
    public class MIPSManager
    {
        private double mips;

        public double MIPS
        {
            get { return mips; }
            set { mips = value; }
        }

        private double availableMIPS;

        public double AvailableMIPS
        {
            get { return availableMIPS; }
            set { availableMIPS = value; }
        }
    }
}