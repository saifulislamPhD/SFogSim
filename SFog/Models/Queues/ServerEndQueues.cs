using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Models.Queues
{
    public class AllocatedServer
    {
        private string serverID;
        public string ServerID
        {
            get { return serverID; }
            set { serverID = value; }
        }
        private Tuple tuple;
        public Tuple Tuple
        {
            get { return tuple; }
            set { tuple = value; }
        }
        public AllocatedServer(string serverId, Tuple tuple)
        {
            ServerID = serverId;
            Tuple = tuple;
        }
    }
    public class ServerQueue
    {
        private string serverId;
        public string ServerId
        {
            get { return serverId; }
            set { serverId = value; }
        }
        private Queue<List<Tuple>> tupleQueue;
        public Queue<List<Tuple>> TupleQueue
        {
            get { return tupleQueue; }
            set { tupleQueue = value; }
        }
        public ServerQueue(string serverId, List<Tuple> Tuple)
        {

            ServerId = serverId;
            TupleQueue.Enqueue(Tuple);
        }

    }
}