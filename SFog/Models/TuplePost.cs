using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models
{
    public class TuplePost
    {
        private int tupleSize;

        public int TupleSize
        {
            get { return tupleSize; }
            set { tupleSize = value; }
        }

        private Tuple tupleData;

        public Tuple TupleData
        {
            get { return tupleData; }
            set { tupleData = value; }
        }
    }
}