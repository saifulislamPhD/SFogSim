using SFog.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Business.Utilities
{
    public static class GeoDistance
    {
        public const double EarthRadiusInMiles = 3956.0;
        public const double EarthRadiusInKilometers = 6367.0;

        public static GeoLocation RandomGeoLocation(Random rnd)
        {
            var GeoLocations = new List<GeoLocation>();
            GeoLocations.Add(new GeoLocation(33.75918676, 72.89806118));
            GeoLocations.Add(new GeoLocation(33.73367459, 72.94510554));
            GeoLocations.Add(new GeoLocation(33.78799432, 73.03918583));
            GeoLocations.Add(new GeoLocation(33.71149325, 72.98662723));
            GeoLocations.Add(new GeoLocation(33.58553272, 73.05820695));
            GeoLocations.Add(new GeoLocation(33.6694735, 73.11683576));
            GeoLocations.Add(new GeoLocation(33.75774999, 73.09606015));
            GeoLocations.Add(new GeoLocation(33.74430192, 72.94457212));
            GeoLocations.Add(new GeoLocation(33.70381249, 73.01082938));
            GeoLocations.Add(new GeoLocation(33.57105635, 73.11726788));
            GeoLocations.Add(new GeoLocation(33.65591552, 72.85716805));
            GeoLocations.Add(new GeoLocation(33.73420675, 72.95355966));
            GeoLocations.Add(new GeoLocation(33.73518079, 73.14154264));
            GeoLocations.Add(new GeoLocation(33.6382797, 72.83865311));
            GeoLocations.Add(new GeoLocation(33.76693095, 73.05696932));
            GeoLocations.Add(new GeoLocation(33.61259906, 72.97832003));
            GeoLocations.Add(new GeoLocation(33.65388807, 73.09926348));
            GeoLocations.Add(new GeoLocation(33.57278, 72.99649278));
            GeoLocations.Add(new GeoLocation(33.70274783, 73.0236825));
            GeoLocations.Add(new GeoLocation(33.664860, 72.991398)); //33.72553698, 72.9068907
            int r = rnd.Next(GeoLocations.Count);
            return GeoLocations[r];
        }

        public static GeoLocation RandomMPNetwork(Random rnd)
        {
            var GeoLocations = new List<GeoLocation>();
            GeoLocations.Add(new GeoLocation(33.743925, 73.093137));
            GeoLocations.Add(new GeoLocation(33.721316, 73.007993));
            GeoLocations.Add(new GeoLocation(33.652755, 72.949628));
            GeoLocations.Add(new GeoLocation(33.612735, 72.993573));
            GeoLocations.Add(new GeoLocation(33.647039, 73.020353));
            GeoLocations.Add(new GeoLocation(33.671614, 73.060865));
            GeoLocations.Add(new GeoLocation(33.703038, 73.038205));
            GeoLocations.Add(new GeoLocation(33.697897, 73.086271));
            GeoLocations.Add(new GeoLocation(33.721887, 73.135709));
            GeoLocations.Add(new GeoLocation(33.676757, 73.140516));
            GeoLocations.Add(new GeoLocation(33.703038, 73.232526));
            GeoLocations.Add(new GeoLocation(33.639607, 73.202314));
            GeoLocations.Add(new GeoLocation(33.600726, 73.153562));
            GeoLocations.Add(new GeoLocation(33.565260, 73.021726));
            GeoLocations.Add(new GeoLocation(33.621312, 73.032026));
            int r = rnd.Next(GeoLocations.Count);
            return GeoLocations[r];
        }
        public static double ToRadian(double val)
        {
            return val * (Math.PI / 180);
        }

        public static double DiffRadian(double val1, double val2)
        {
            return ToRadian(val2) - ToRadian(val1);
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
        {
            return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
        {
            double radius = GeoDistance.EarthRadiusInMiles;

            if (m == GeoCodeCalcMeasurement.Kilometers) { radius = GeoDistance.EarthRadiusInKilometers; }
            return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
        }
        public static double CalcNodeDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
        {
            double radius = GeoDistance.EarthRadiusInMiles;

            if (m == GeoCodeCalcMeasurement.Kilometers) { radius = GeoDistance.EarthRadiusInKilometers; }
            return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
        }
    }

    public enum GeoCodeCalcMeasurement : int
    {
        Miles = 0,
        Kilometers = 1
    }
}