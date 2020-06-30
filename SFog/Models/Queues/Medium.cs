using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Queues
{
    public class Medium
    {
        private Tuple mpTuple;
        public Tuple MP_Tuple
        {
            get { return mpTuple; }
            set { mpTuple = value; }
        }
    }
}