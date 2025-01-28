using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCommonalities.Storage
{
    public class ThroughputStorage
    {

        public ConcurrentBag<int> Throughput = new ConcurrentBag<int>();

        public int ThroughputCount { get; set; } 

        public void IncrementThroughputCount()
        {
            ThroughputCount += 1;
        }

        public void AddThroughputToBag()
        {
            
            Throughput.Add(ThroughputCount);

            ThroughputCount = 0;
        }

        public async Task<ConcurrentBag<int>> ReturnThroughputBag()
        {
            return Throughput;
        }
        

    }
}
