using ConfigurationStuff.DbModels;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace gRPCStressTestingService.Storage
{
    public class DelayCalcStorage
    {

        public ConcurrentBag<DelayCalc> UnarySingleStorage = new ConcurrentBag<DelayCalc>();

        public ConcurrentBag<DelayCalc> UnaryBatchStorage = new ConcurrentBag<DelayCalc>();

        public ConcurrentBag<DelayCalc> StreamingSingleStorage = new ConcurrentBag<DelayCalc>();

        public ConcurrentBag<DelayCalc> StreamingBatchStorage = new ConcurrentBag<DelayCalc>();

    }
}
