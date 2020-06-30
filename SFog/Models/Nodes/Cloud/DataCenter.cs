using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Nodes.Cloud
{
    public class DataCenter
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private DatacenterCharacteristics datacenterCharacteristics;

        public DatacenterCharacteristics DatacenterCharacteristics
        {
            get { return datacenterCharacteristics; }
            set { datacenterCharacteristics = value; }
        }

        public DataCenter(string name, DatacenterCharacteristics datacenterCharacteristics)
        {
            Name = name;
            DatacenterCharacteristics = datacenterCharacteristics;
        }

        public DataCenter()
        {
        }
    }
}