using SFog.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Business.Utilities.Cloud
{
    public static class ServicesUtility
    {
        public static List<Services> GetServices()
        {
            var list = new List<Services>();

            var service1 = new Services()
            {
                ID = Guid.NewGuid(),
                Name = "Service 1",
                NumberOfPes = 1,
                RAM = 8192, // 8 GB
                Size = 5000,
                MIPS = 9000,
                BW = 500
            };
            list.Add(service1);

            var service2 = new Services()
            {
                ID = Guid.NewGuid(),
                Name = "Service 2",
                NumberOfPes = 2,
                RAM = 16384, // 16 GB
                Size = 10000,
                MIPS = 18000,
                BW = 500
            };
            list.Add(service2);

            var service3 = new Services()
            {
                ID = Guid.NewGuid(),
                Name = "Service 3",
                NumberOfPes = 4,
                RAM = 32768, // 32 GB
                Size = 15000,
                MIPS = 27000,
                BW = 500
            };
            list.Add(service3);

            return list;
        }
    }
}