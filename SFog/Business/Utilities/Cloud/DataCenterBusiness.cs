using System.Collections.Generic;
using SFog.Models.Nodes.Cloud;
using SFog.Models.Utility;

namespace SFog.Business.Utilities.Cloud
{
    public static class DataCenterBusiness
    {
        public struct DataCenterLocations
        {
            //USA Location
            public const double USALong = 37.422421;

            public const double USALat = -122.0866703;

            //Singapore Location
            public const double SingaporeLong = 1.277911;

            public const double SingaporeLat = 103.849662;
        }

        private enum LocationNames
        {
            USA,
            Singapore
        }

        public static DataCenter CreateDatacenter(string locationNames)
        {
            DatacenterCharacteristics characteristics = new DatacenterCharacteristics();
            List<Host> hostList = new List<Host>();
            int hostId = 1;
            int ram = 51200; // host memory (MB)
            long storage = 1000000; // host storage
            int bw = 50000;
            string name = locationNames + "_DataCenter";
            long mips = 500000;
            int pe = 6;

            hostList.Add(new Host()
            {
                ID = hostId,
                Storage = storage,
                Failed = false,
                BW = bw,
                Ram = ram,
                DataCenter = name,
                MIPS = mips,
                PE = pe,
                ResourceManager = ResourceUtility.SetResources(ram, storage, bw, mips, pe),
                VMList = new List<VM>()
            });

            string arch = "x86"; // system architecture
            string os = "Linux"; // operating system

            if (locationNames.IndexOf(LocationNames.USA.ToString()) > -1)
            {
                characteristics = new DatacenterCharacteristics(
                                            arch, os, 1, 1, new GeoLocation(DataCenterLocations.USALong, DataCenterLocations.USALat), hostList);
            }
            else if (locationNames.IndexOf(LocationNames.Singapore.ToString()) > -1)
            {
                characteristics = new DatacenterCharacteristics(
                                             arch, os, 1, 1, new GeoLocation(DataCenterLocations.SingaporeLong, DataCenterLocations.SingaporeLat), hostList);
            }
            return new DataCenter(name, characteristics);
        }

        public static List<DataCenter> GetDataCenterList()
        {
            var DataCenterList = new List<DataCenter>();
            DataCenterList.Add(CreateDatacenter(LocationNames.USA.ToString()));
            DataCenterList.Add(CreateDatacenter(LocationNames.Singapore.ToString()));

            return DataCenterList;
        }
    }
}