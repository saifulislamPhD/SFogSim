using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models
{
    public class TupleTimes
    {
        public string name;
        public string tupleDeparture;
        public string tupleArrival;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string TupleArrival
        {
            get { return tupleArrival; }
            set { tupleArrival = value; }
        }

        public string TupleDeparture
        {
            get { return tupleDeparture; }
            set { tupleDeparture = value; }
        }
    }
}