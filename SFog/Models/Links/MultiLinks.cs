using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Links
{
    public class MultiLinks
    {
        private string head;
        public string Head
        {
            get { return head; }
            set { head = value; }
        }
        private List<Link> links;
        public List<Link> Links
        {
            get { return links; }
            set { links = value; }
        }
        private List<string> directnodes;
        public List<string> DirectNodes
        {
            get { return directnodes; }
            set { directnodes = value; }
        }
        private string alphanodes;
        public string AlphaNodes
        {
            get { return alphanodes; }
            set { alphanodes = value; }
        }
    }
    public class NodeLinkList
    {
        private string head;
        public string Head
        {
            get { return head; }
            set { head = value; }
        }
        private List<int> nodes;
        public List<int> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }
        private List<CurrentPaths> paths;
        public List<CurrentPaths> Paths
        {
            get { return paths; }
            set { paths = value; }
        }
    }
    public class CurrentPaths
    {
        private string node;
        public string Node
        {
            get { return node; }
            set { node = value; }
        }
        private Link link;
        public Link PLink
        {
            get { return link; }
            set { link = value; }
        }
    }
    public class CollectionLinks
    {
        private List<CurrentPaths> paths;
        public List<CurrentPaths> Paths
        {
            get { return paths; }
            set { paths = value; }
        }
    }

}