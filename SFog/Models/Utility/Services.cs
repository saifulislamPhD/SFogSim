using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Utility
{
    public class Services
    {
        private Guid Id;

        public Guid ID
        {
            get { return Id; }
            set { Id = value; }
        }

        private long size;

        public long Size
        {
            get { return size; }
            set { size = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private double mips;
        /** The MIPS capacity of each VM's PE. */

        public double MIPS
        {
            get { return mips; }
            set { mips = value; }
        }

        private int numerofpes;
        /** The number of PEs required by the VM. */

        public int NumberOfPes
        {
            get { return numerofpes; }
            set { numerofpes = value; }
        }

        private int ram;
        /** The required ram. */

        public int RAM
        {
            get { return ram; }
            set { ram = value; }
        }

        private long bw;
        /** The required bw. */

        public long BW
        {
            get { return bw; }
            set { bw = value; }
        }
    }
}