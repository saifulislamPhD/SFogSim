using SFog.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models
{
    public class Application
    {
        private String appId;
        public string AppId
        {
            get { return appId; }
            set { appId = value; }
        }

        private int userId;
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        private GeoLocation geoLocation;
        public GeoLocation GeoLocation
        {
            get { return geoLocation; }
            set { geoLocation = value; }
        }

        private Application(string appId, int userId)
        {
            AppId = appId;
            UserId = userId;
        }

        public Application()
        { }
        /**
         * Creates a plain vanilla application with no modules and edges.
         * @param appId
         * @param userId
         * @return
         */
        public static Application createApplication(String appId, int userId)
        {
            return new Application(appId, userId);
        }

        enum Status
        {
            Active = 1,
            Inactive = 2,
            Fail = 3
        }
    }
}