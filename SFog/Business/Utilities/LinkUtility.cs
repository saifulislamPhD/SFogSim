using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Business.Utilities
{
    public static class LinkUtility
    {
        public static double CalculateLatency(double distance, double speed, double size, double transmitionRate, double latency)
        {
            var FrameSerializationTime = size * 1024 * 8 / transmitionRate;
            var LinkMediaDelay = distance / latency;
            //Queueing Delay = Q / R

            var Propagationtime = FrameSerializationTime + LinkMediaDelay; // + QueueingDelay + NodeProcessingDelay
            return Propagationtime;

            //var propagationDelay = distance / speed;
            //var serilizationDelay = size / transmitionRate;
            //return propagationDelay + serilizationDelay;
        }
    }
}