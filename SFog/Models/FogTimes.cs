using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models
{
    public class FogTimes
    {
        private string freeTime;

        private string fogName;

        private string taskArrival;

        private double consumption;

        private double timedifference;

        private string tupleName;

        public string FogName
        {
            get { return fogName; }
            set { fogName = value; }
        }

        public string TupleName
        {
            get { return tupleName; }
            set { tupleName = value; }
        }

        public string FreeTime
        {
            get { return freeTime; }
            set { freeTime = value; }
        }


        public string TaskArrival
        {
            get { return taskArrival; }
            set { taskArrival = value; }
        }
        public double Consumption
        {
            get { return consumption; }
            set { consumption = value; }
        }

        public double TimeDifference
        {
            get { return timedifference; }
            set { timedifference = value; }
        }
        public double ConsumptionPer { get; set; }
    }
}