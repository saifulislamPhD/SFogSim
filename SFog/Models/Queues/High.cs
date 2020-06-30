using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Queues
{
    public class High
    {
        private Tuple hpTuple;
        public Tuple HP_Tuple
        {
            get { return hpTuple; }
            set { hpTuple = value; }
        }
    }
}