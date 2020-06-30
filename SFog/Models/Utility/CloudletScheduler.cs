using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Utility
{
    public class CloudletScheduler
    {
        private double previousTime;

        public double PreviousTime
        {
            get { return previousTime; }
            set { previousTime = value; }
        }

        private List<double> currentMipsShare;

        public List<double> CurrentMipsShare
        {
            get { return currentMipsShare; }
            set { currentMipsShare = value; }
        }

    }
}