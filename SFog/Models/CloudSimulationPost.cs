using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models
{
    public class CloudSimulationPost
    {
        private CloudPost cloudPost;

        public CloudPost CloudPost
        {
            get { return cloudPost; }
            set { cloudPost = value; }
        }

        private TuplePost tuplePost;

        public TuplePost TuplePost
        {
            get { return tuplePost; }
            set { tuplePost = value; }
        }

        private string policyType;

        public string PolicyType
        {
            get { return policyType; }
            set { policyType = value; }
        }

        private int service;

        public int Service
        {
            get { return service; }
            set { service = value; }
        }

        private List<string> datacenter;

        public List<string> DataCenter
        {
            get { return datacenter; }
            set { datacenter = value; }
        }
    }
}