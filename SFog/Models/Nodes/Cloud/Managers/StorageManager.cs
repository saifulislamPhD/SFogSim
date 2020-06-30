using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Nodes.Cloud.Managers
{
    public class StorageManager
    {
        private long storage;

        public long Storage
        {
            get { return storage; }
            set { storage = value; }
        }

        private long availablestorage;

        public long AvailableStorage
        {
            get { return availablestorage; }
            set { availablestorage = value; }
        }
    }
}