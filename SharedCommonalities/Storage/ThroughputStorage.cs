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

        public ConcurrentBag<int> SingleStreamingThroughputBag = new ConcurrentBag<int>();

        public ConcurrentBag<int> BatchStreamingThroughputBag  = new ConcurrentBag<int>();

        public ConcurrentBag<int> SingleUnaryThroughputBag = new ConcurrentBag<int>();

        public ConcurrentBag<int> BatchUnaryThroughputBag = new ConcurrentBag<int>();

        public int ThroughputCount { get; set; } 

        public int SingleStreamingThroughput { get; set; }

        public int BatchStreamingThroughput { get; set; }

        public int SingleUnaryThroughput { get; set; }

        public int BatchUnaryThroughput { get; set; }

        public void IncrementThroughputCount()
        {
            ThroughputCount += 1;
        }

        public void IncrementSingleStreamingThroughput()
        {

             SingleStreamingThroughput += 1; 
        }

        public void IncrementBatchStreamingThroughput()
        {
            BatchStreamingThroughput += 1;
        }

        public void IncrementSingleUnaryThroughput()
        {

            SingleUnaryThroughput += 1; 
        }

        public void IncrementBatchUnaryThroughput()
        {
            BatchUnaryThroughput += 1;
        }

        public void AddThroughputToBag()
        {
            
            Throughput.Add(ThroughputCount);

            ThroughputCount = 0;
        }

        public void AddSingleStreamingThroughputToBag()
        {
            SingleStreamingThroughputBag.Add(SingleStreamingThroughput);

            SingleStreamingThroughput = 0;
        }

        public void AddBatchStreamingThroughputToBag()
        {
            BatchStreamingThroughputBag.Add(BatchStreamingThroughput);

            BatchStreamingThroughput = 0;
        }

        public void AddSingleUnaryThroughputToBag()
        {
            SingleUnaryThroughputBag.Add(SingleUnaryThroughput);

            SingleUnaryThroughput = 0;
        }

        public void AddBatchUnaryThroughputToBag()
        {
            BatchUnaryThroughputBag.Add(BatchUnaryThroughput);

            BatchUnaryThroughput = 0;
        }

        public async Task<ConcurrentBag<int>> ReturnThroughputBag()
        {
            return Throughput;
        }

        public async Task<ConcurrentBag<int>> ReturnSingleStreamingBag()
        {
            return SingleStreamingThroughputBag;
        }

        public async Task<ConcurrentBag<int>> ReturnBatchStreamingBag()
        {
            return BatchStreamingThroughputBag;
        }

        public async Task<ConcurrentBag<int>> ReturnSingleUnaryBag()
        {
            return SingleUnaryThroughputBag;
        }

        public async Task<ConcurrentBag<int>> ReturnBatchUnaryBag()
        {
            return BatchUnaryThroughputBag;
        }
        

    }
}
