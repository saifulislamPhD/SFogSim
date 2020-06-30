using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Nodes
{
    public class Device
    {
        private List<Models.Tuple> tuple;

        public List<Models.Tuple> Tuple
        {
            get { return tuple; }
            set { tuple = value; }
        }

        private List<Models.Application> application;

        public List<Models.Application> Application
        {
            get { return application; }
            set { application = value; }
        }

        public Device(List<Models.Tuple> tuple, List<Models.Application> application )
        {
            Tuple = tuple;
            Application = application;
        }
    }
}