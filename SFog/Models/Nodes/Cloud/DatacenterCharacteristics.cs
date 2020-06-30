using SFog.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Nodes.Cloud
{
    public class DatacenterCharacteristics
    {
        private string architecture;

        public string Architecture
        {
            get { return architecture; }
            set { architecture = value; }
        }

        private string os;

        public string OS
        {
            get { return os; }
            set { os = value; }
        }

        private int allocationPolicy;

        public int AllocationPolicy
        {
            get { return allocationPolicy; }
            set { allocationPolicy = value; }
        }

        private double costPerSecond;

        public double CostPerSecond
        {
            get { return costPerSecond; }
            set { costPerSecond = value; }
        }

        private GeoLocation geoLocation;

        public GeoLocation GeoLocation
        {
            get { return geoLocation; }
            set { geoLocation = value; }
        }

        private List<Host> hostList;

        public List<Host> HostList
        {
            get { return hostList; }
            set { hostList = value; }
        }

        private double distanceFromTuple;

        public double DistanceFromTuple
        {
            get { return distanceFromTuple; }
            set { distanceFromTuple = value; }
        }

        public DatacenterCharacteristics()
        {
        }

        public DatacenterCharacteristics(string architecture, string os, int allocationPolicy, double costPerSecond, GeoLocation geoLocation, List<Host> hostList)
        {
            Architecture = architecture;
            OS = os;
            AllocationPolicy = allocationPolicy;
            CostPerSecond = costPerSecond;
            GeoLocation = geoLocation;
            HostList = hostList;
        }
    }
}