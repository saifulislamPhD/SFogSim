using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace SFog.Models
{
    public class FogSimulationPost
    {
        private FogPost fogPost;

        public FogPost FogPost
        {
            get { return fogPost; }
            set { fogPost = value; }
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

        private string gatewayPolicyType;

        public string GatewayPolicyType
        {
            get { return gatewayPolicyType; }
            set { gatewayPolicyType = value; }
        }

        private string nodeLevelPolicyTypes;

        public string NodeLevelPolicyTypes
        {
            get { return nodeLevelPolicyTypes; }
            set { nodeLevelPolicyTypes = value; }
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

        private int communicationType;

        public int CommunicationType
        {
            get { return communicationType; }
            set { communicationType = value; }
        }

        private string gateway;

        public string Gateway
        {
            get { return gateway; }
            set { gateway = value; }
        }

        private string cooperation;

        public string Cooperation
        {
            get { return cooperation; }
            set { cooperation = value; }
        }

        private string fogServerType;
        public string FogType
        {
            get { return fogServerType; }
            set { fogServerType = value; }
        }
    }
    public class MemoryParams
    {
        private FogPost fogPost;

        public FogPost FogPost
        {
            get { return fogPost; }
            set { fogPost = value; }
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

        private int communicationType;

        public int CommunicationType
        {
            get { return communicationType; }
            set { communicationType = value; }
        }

        private string gateway;

        public string Gateway
        {
            get { return gateway; }
            set { gateway = value; }
        }

        private string cooperation;

        public string Cooperation
        {
            get { return cooperation; }
            set { cooperation = value; }
        }

        private string fogServerType;
        public string FogType
        {
            get { return fogServerType; }
            set { fogServerType = value; }
        }
    }

}