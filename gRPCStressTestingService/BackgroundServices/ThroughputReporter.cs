
using Serilog;
using SharedCommonalities.Storage;

namespace gRPCStressTestingService.BackgroundServices
{
    public class ThroughputReporter : BackgroundService
    {
        private readonly ThroughputStorage _throughputStorage;

        public ThroughputReporter(ThroughputStorage throughputStorage)
        {
            _throughputStorage = throughputStorage;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

                if(_throughputStorage.SingleStreamingThroughput == 0)
                {
                    Log.Information($"Nothing to add to bag, current count: {_throughputStorage.ThroughputCount}");
                }
                else
                {
                    _throughputStorage.AddBatchStreamingThroughputToBag();
                    _throughputStorage.AddBatchUnaryThroughputToBag();
                    _throughputStorage.AddSingleUnaryThroughputToBag();
                    _throughputStorage.AddSingleStreamingThroughputToBag();

                    Log.Information($"Throughput has been added to the bag, current bag count {_throughputStorage.SingleStreamingThroughputBag.Count}");

                    Log.Information($"throughput count: {_throughputStorage.SingleStreamingThroughput}");
                }
            }
        }
    }
}
