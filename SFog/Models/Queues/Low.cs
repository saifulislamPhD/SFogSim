using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFog.Models.Utility;

namespace SFog.Models.Queues
{
    public class Low
    {
        private Tuple lpTuple;
        public Tuple LP_Tuple
        {
            get { return lpTuple; }
            set { lpTuple = value; }
        }
    }
}