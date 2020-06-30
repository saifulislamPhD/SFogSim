using SFog.Models.Archives;
using SFog.Models.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SFog.Business.Utilities
{
    public static class PowerUtility
    {
        public static List<Range> NRange = new List<Range>();

        //0%, 10%, 20%, 30%, 40%, 50%, 60%, 70%, 80%, 90%, 100%
        private static double[] PowerConsumption = { 86, 89.4, 92.6, 96, 99.5, 102, 106, 108, 112, 114, 117 };
        private static Object Lock = new Object();
        public static double GetPowerConsumption(int percentage)
        {
            switch (percentage)
            {
                case 0:
                    return PowerConsumption[0];

                case 10:
                    return PowerConsumption[1];

                case 20:
                    return PowerConsumption[2];

                case 30:
                    return PowerConsumption[3];

                case 40:
                    return PowerConsumption[4];

                case 50:
                    return PowerConsumption[5];

                case 60:
                    return PowerConsumption[6];

                case 70:
                    return PowerConsumption[7];

                case 80:
                    return PowerConsumption[8];

                case 90:
                    return PowerConsumption[9];

                case 100:
                    return PowerConsumption[10];

                default:
                    return ExactPower(percentage);
            }
        }

        public static int GetPercentageOfServerFreePower(FogDevice fogDevice)
        {
            return int.Parse(Math.Ceiling((decimal)((fogDevice.MIPS - fogDevice.CurrentAllocatedMips) / fogDevice.MIPS) * 100).ToString());
        }

        public static double SetIdlePower()
        {
            return PowerConsumption[0];
        }

        public static double GetMaxPowerConsumption()
        {
            return PowerConsumption[PowerConsumption.Length - 1];
        }

        public static int GetRequiredPowerPercentage(Models.Tuple tuple, FogDevice fogdevice)
        {
            lock (Lock)
            {

                return int.Parse(Math.Ceiling((decimal)(tuple.MIPS / fogdevice.MIPS) * 100).ToString());
            }
        }

        private static double ExactPower(int percentage)
        {
            int j = 0;
            if (percentage > 100)
            {
                return 117;
            }
            for (int i = 0; i <= 100; i = i + 10)
            {
                if (percentage >= i && percentage <= i + 10)
                {
                    var perPercent = (PowerConsumption[j + 1] - PowerConsumption[j]) / 10;
                    return PowerConsumption[j] + ((percentage - i) * perPercent);
                }
                j++;
            }
            return 0.0;
        }

        //public static List<double> testlist = new List<double>();

        public static void ConsumePower(FogDevice nearestFogDevice, Models.Tuple tuple)
        {
            lock (Lock)
            {
                Debug.WriteLine("RequiredPowerByTuple-" + GetRequiredPowerPercentage(tuple, nearestFogDevice));
                Debug.WriteLine("ConsumePower-" + nearestFogDevice.BusyPower);

                nearestFogDevice.BusyPower = nearestFogDevice.BusyPower + (GetRequiredPowerPercentage(tuple, nearestFogDevice));
                nearestFogDevice.AvailablePower = 100 - nearestFogDevice.BusyPower;
                var obj = new PowerConsumption()
                {
                    PowerValue = (nearestFogDevice.BusyPower > 100) ?
                    CalcPowerValues("100") : (nearestFogDevice.BusyPower < 1) ? CalcPowerValues("1") : CalcPowerValues(nearestFogDevice.BusyPower.ToString()),
                    Time = DateTime.Now.ToString("hh:mm:ss.fff tt")
                };
                nearestFogDevice.PowerConsumption.Add(obj); //log busy percentage of fog device

                //GetRequiredPowerPercentage(tuple, nearestFogDevice)
            }
        }

        public static void ReleasePower(FogDevice nearestFogDevice, Models.Tuple tuple)
        {
            lock (Lock)
            {
                nearestFogDevice.BusyPower = nearestFogDevice.BusyPower - (GetRequiredPowerPercentage(tuple, nearestFogDevice));
                nearestFogDevice.AvailablePower = 100 - nearestFogDevice.BusyPower;
                /// PowerUtility.CalcPowerValues

                //var obj = new PowerConsumption()
                //{
                //    PowerValue = nearestFogDevice.BusyPower < 86 ? GetPowerConsumption(86) : GetPowerConsumption(int.Parse(nearestFogDevice.BusyPower.ToString())),
                //    Time = DateTime.Now.ToString("hh:mm:ss.fff tt")
                //};
                Debug.WriteLine("ReleasePower-" + nearestFogDevice.BusyPower);

                var obj = new PowerConsumption()
                {
                    PowerValue = (nearestFogDevice.BusyPower < 1) ? CalcPowerValues("1") : (nearestFogDevice.BusyPower > 100) ? CalcPowerValues("100") : CalcPowerValues(nearestFogDevice.BusyPower.ToString()),
                    Time = DateTime.Now.ToString("hh:mm:ss.fff tt")
                };

                nearestFogDevice.PowerConsumption.Add(obj);

                //GetRequiredPowerPercentage(tuple, nearestFogDevice)
            }
        }

        public static double Consumption(FogDevice currentFog1, double DelTime, double TimeMax, Models.Tuple tuple)
        {
            try
            {
                lock (Lock)
                {
                    var currentFog = FogSimulator.getList().FirstOrDefault(x => x.ID.Equals(currentFog1.ID));
                    if (currentFog != null)
                    {
                        return (Math.Round((tuple.MIPS * DelTime) / (currentFog.MIPS * TimeMax), 4));
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return 0;
        }

        public static double JobLoad(Models.Tuple tuple, double timeDiff)
        {
            return (tuple.MIPS * timeDiff);
        }

        public static double getFogCapacity(FogDevice currentFog)
        {
            lock (Lock)
            {
                //Fog 500 MIPS;Processors=2; Job MIPS=75;Job Prcessors=1;
                //return currentFog.NumberOfPes > 1 ? Math.Ceiling((currentFog.MIPS / currentFog.NumberOfPes)) : currentFog.MIPS;
                return currentFog.MIPS;
            }
        }

        public static double getTimeDiff(string FreeTime, string StartTime)
        {
            return (Convert.ToDouble(FreeTime) - Convert.ToDouble(StartTime));
        }

        public static List<FogDevice> GetRequiredFogs(Models.Tuple tuple, IEnumerable<FogDevice> devices)
        {
            return devices.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS).ToList();
        }


        public static List<FogDevice> GetRequiredFogsMRR(Models.Tuple tuple, IEnumerable<FogDevice> devices)
        {
            //return devices.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS && a.MaxCapacity > 0).ToList();
            return devices.Where(a => (a.MIPS - a.CurrentAllocatedMips) > tuple.MIPS && a.RAM-a.CurrentAllocatedRam>tuple.RAM ).ToList();
        }

        public static double CalcPowerValues(string InputNumber)
        {
            try
            {

                string[] NumRange = { "0-10", "11-20", "21-30", "31-40", "41-50", "51-60", "61-70", "71-80", "81-90", "91-100" };
                double[] NRmultiplier = { 0.34, 0.32, 0.34, 0.35, 0.25, 0.4, 0.2, 0.4, 0.2, 0.3 };
                double[] PowerConsumption = { 86, 89.4, 92.6, 96, 99.5, 102, 106, 108, 112, 114 };//117
                string[] splitInputNumber = InputNumber.Split('.');
                string FirstPartOfNumber = splitInputNumber[0];
                char Firstdgt = FirstPartOfNumber[0];

                if (Firstdgt == '+' || Firstdgt == '-')
                {
                    FirstPartOfNumber = "0"; ;
                }

                int _index = GetRangeIndex(Convert.ToInt32(FirstPartOfNumber));
                List<string> getfromList = NumRange[_index].Split('-').ToList();
                List<int> AllrangeNumber = Enumerable.Range(Convert.ToInt32(getfromList[0]) == 0 ? 1 : Convert.ToInt32(getfromList[0]), 10).ToList();
                int numFrom = AllrangeNumber.IndexOf(Convert.ToInt32(FirstPartOfNumber));
                double value = (numFrom * NRmultiplier[_index]) + PowerConsumption[_index];
                if (FirstPartOfNumber == "100") { value = 117; }

                return value;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void FillNumRange()
        {
            Range _range;
            int n = 0;
            for (int j = 0; j < 10; j++)
            {
                _range = new Range();
                _range._range = Enumerable.Range(n + 1, 10).ToList();
                NRange.Add(_range);
                n = _range._range[9];
            }
        }
        public static int GetRangeIndex(int InputNumber)
        {
            int n = 0;
            foreach (var item in NRange)
            {
                bool gotit = item._range.Contains(InputNumber);
                if (gotit)
                {
                    int RangeNumberIndex = item._range.IndexOf(InputNumber);
                    return n;

                }
                n++;
            }
            return 0;
        }
        public static double CalConsPercentage(FogDevice fogDev, List<FogDevice> deviceList)
        {
            lock (Lock)
            {
                var fogDevice = deviceList.FirstOrDefault(x => x.ID.Equals(fogDev.ID));
                if (fogDevice != null)
                {
                    double Consumption = Math.Round((fogDevice.CurrentAllocatedMips / fogDev.MIPS) * 100, 3);
                    if (Consumption < 0)
                    {

                    }
                    if (Consumption > 100)
                    {

                    }
                    return Consumption;
                }
                else { return 0; }
            }
        }

    }
}