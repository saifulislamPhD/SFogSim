using SFog.Models.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Cache
{
    public class CloudCache
    {
        private string dataType;
        private string cloud;
        private Guid tupleId;
        private double internalProcessingTime;
        private Link _link;
        public string DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }
        public string Cloud
        {
            get { return cloud; }
            set { cloud = value; }
        }
        public Guid TupleGuid
        {
            get { return tupleId; }
            set { tupleId = value; }
        }
        public double InternalProcessingTime
        {

            get { return internalProcessingTime; }
            set { internalProcessingTime = value; }
        }
        public Link link
        {
            get { return _link; }
            set { _link = value; }
        }

    }


}