using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Utility
{
    public class GeoLocation
    {
        public double latitude;
        public double longitude;

        public GeoLocation(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public double getLatitude()
        {
            return latitude;
        }
        public void setLatitude(double latitude)
        {
            this.latitude = latitude;
        }
        public double getLongitude()
        {
            return longitude;
        }
        public void setLongitude(double longitude)
        {
            this.longitude = longitude;
        }
    }
}