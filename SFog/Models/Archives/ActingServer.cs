using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Archives
{
    public class ActingServer
    {
        public string ServerName { get; set; }
        public string TupleName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double TotalMS { get; set; }
    }
}