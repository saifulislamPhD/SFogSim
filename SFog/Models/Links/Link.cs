using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Links
{
    public class Link
    {
        private double propagationtime;

        public double Propagationtime
        {
            get { return propagationtime; }
            set { propagationtime = value; }
        }

        private string source;

        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        private string destination;

        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        //source and destination distance.
        private double sdDistance;

        public double SDDistance
        {
            get { return sdDistance; }
            set { sdDistance = value; }
        }
        private double weight;
        public double Weight
        {
            get { return weight; }
            set { weight = value; }
        }
        private double bandWidth;
        public double BandWidth
        {
            get { return bandWidth; }
            set { bandWidth = 100; }// value 100 mbps (mega bit per second)
        }
        private double availabilty;
        public double Availabilty
        {
            get { return availabilty; }
            set { availabilty = value; }
        }
        private double reliability;
        public double Reliability
        {
            get { return reliability; }
            set { reliability = value; }
        }
        public Link()
        {
        }

        public Link(double propagationtime, string source, string destination, double sdDistance, double weight, double bandwidth, double availability = 0, double reliablity = 0)
        {
            Propagationtime = propagationtime;
            Source = source;
            Destination = destination;
            SDDistance = sdDistance;
            Weight = weight;
            BandWidth = bandWidth;
            Availabilty = availability;
            Reliability = reliability;
        }
    }
}