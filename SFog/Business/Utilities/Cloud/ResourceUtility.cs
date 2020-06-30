using SFog.Models.Nodes.Cloud;
using SFog.Models.Nodes.Cloud.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFog.Business.Utilities.Cloud
{
    public static class ResourceUtility
    {
        private static Object Lock = new Object();
        public static bool ConsumeResources(Host host, VM vm)
        {
            lock (Lock)
                host.ResourceManager.RamManager.AvailableRam = host.ResourceManager.RamManager.Ram - vm.RAM;
            lock (Lock)
                host.ResourceManager.PeManager.AvailablePE = host.ResourceManager.PeManager.PE - vm.NumberOfPes;
            lock (Lock)
                host.ResourceManager.StorageManager.AvailableStorage = host.ResourceManager.StorageManager.Storage - vm.Size;
            lock (Lock)
                host.ResourceManager.MIPSManager.AvailableMIPS = host.ResourceManager.MIPSManager.MIPS - vm.MIPS;
            lock (Lock)
                host.ResourceManager.BWManager.AvailableBW = host.ResourceManager.BWManager.BW - vm.BW;

            var test = new ResourceManager(new RamManager(), new PeManager(), new StorageManager(), new MIPSManager(), new BWManager());
            test.RamManager.AvailableRam = host.ResourceManager.RamManager.AvailableRam;
            test.PeManager.AvailablePE = host.ResourceManager.PeManager.AvailablePE;
            test.StorageManager.AvailableStorage = host.ResourceManager.StorageManager.AvailableStorage;
            test.MIPSManager.AvailableMIPS = host.ResourceManager.MIPSManager.AvailableMIPS;
            test.BWManager.AvailableBW = host.ResourceManager.BWManager.AvailableBW;
            lock (Lock)
                CloudSimulator.ResourceMonitoring.Add(test);
            return true;
        }

        public static bool ReleaseResources(Host host, VM vm)
        {
            lock (Lock)
                host.ResourceManager.RamManager.AvailableRam = host.ResourceManager.RamManager.AvailableRam + vm.RAM;
            lock (Lock)
                host.ResourceManager.PeManager.AvailablePE = host.ResourceManager.PeManager.AvailablePE + vm.NumberOfPes;
            lock (Lock)
                host.ResourceManager.StorageManager.AvailableStorage = host.ResourceManager.StorageManager.AvailableStorage + vm.Size;
            lock (Lock)
                host.ResourceManager.MIPSManager.AvailableMIPS = host.ResourceManager.MIPSManager.AvailableMIPS + vm.MIPS;
            lock (Lock)
                host.ResourceManager.BWManager.AvailableBW = host.ResourceManager.BWManager.AvailableBW + vm.BW;

            var test = new ResourceManager(new RamManager(), new PeManager(), new StorageManager(), new MIPSManager(), new BWManager());
            test.RamManager.AvailableRam = host.ResourceManager.RamManager.AvailableRam;
            test.PeManager.AvailablePE = host.ResourceManager.PeManager.AvailablePE;
            test.StorageManager.AvailableStorage = host.ResourceManager.StorageManager.AvailableStorage;
            test.MIPSManager.AvailableMIPS = host.ResourceManager.MIPSManager.AvailableMIPS;
            test.BWManager.AvailableBW = host.ResourceManager.BWManager.AvailableBW;
            lock (Lock)
                CloudSimulator.ResourceMonitoring.Add(test);

            return true;
        }

        public static ResourceManager SetResources(int ram, long storage, int bw, long mips, int pe)
        {
            var obj = new ResourceManager()
            {
                BWManager = new BWManager() { BW = bw, AvailableBW = bw },
                RamManager = new RamManager() { Ram = ram, AvailableRam = ram },
                StorageManager = new StorageManager() { Storage = storage, AvailableStorage = storage },
                MIPSManager = new MIPSManager() { MIPS = bw, AvailableMIPS = mips },
                PeManager = new PeManager() { PE = pe, AvailablePE = pe },
            };
            return obj;
        }
    }
}