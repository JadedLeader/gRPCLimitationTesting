using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using gRPCStressTestingService.proto;
using SharedCommonalities.Storage;
using System.Collections.Concurrent;

namespace gRPCStressTestingService.Services
{
    public class ThroughputService : IThroughputService
    {

        private readonly ThroughputStorage _throughputStorage;

        public ThroughputService(ThroughputStorage throughputStorage)
        {
            _throughputStorage = throughputStorage;
        }


        public async Task GetStreamingSingleThroughput(GetStreamingSingleThroughputRequest request, IServerStreamWriter<GetStreamingSingleThroughputResponse> responseStream, ServerCallContext context)
        {


            while(!context.CancellationToken.IsCancellationRequested)
            {
                ConcurrentBag<int> getStreamingSingleBag = await _throughputStorage.ReturnSingleStreamingBag();

                if(getStreamingSingleBag.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.3));
                }
                else
                {

                    int bestThroughput = getStreamingSingleBag.Max();

                    GetStreamingSingleThroughputResponse serverResponse = new GetStreamingSingleThroughputResponse()
                    {
                        CurrentThroughput = bestThroughput,
                    };

                    await responseStream.WriteAsync(serverResponse);

                }

            }

        }

        public async Task GetStreamingBatchThroughput(GetStreamingBatchThroughputRequest request, IServerStreamWriter<GetStreamingBatchThroughputResponse> responseStream, ServerCallContext context)
        {
            
            while(!context.CancellationToken.IsCancellationRequested)
            {
                ConcurrentBag<int> getStreamingbatchBag = await _throughputStorage.ReturnBatchStreamingBag();

                if( getStreamingbatchBag.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.3));
                }
                else
                {
                    int bestBatchThroughput = getStreamingbatchBag.Max();

                    GetStreamingBatchThroughputResponse serverResponse = new GetStreamingBatchThroughputResponse()
                    {
                        CurrentThroughput = bestBatchThroughput,
                    };

                    await responseStream.WriteAsync(serverResponse);
                }

            }
            
        }

        public async Task GetUnarySingleThroughput(GetUnarySingleThroughputRequest request, IServerStreamWriter<GetUnarySingleThroughputResponse> responseStream, ServerCallContext context)
        {
            while(!context.CancellationToken.IsCancellationRequested)
            {
                ConcurrentBag<int> getUnarySingleBag = await _throughputStorage.ReturnSingleUnaryBag();

                if(getUnarySingleBag.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.3));
                }
                else
                {
                    int bestSingleThroughput = getUnarySingleBag.Max();

                    GetUnarySingleThroughputResponse serverResponse = new GetUnarySingleThroughputResponse()
                    {
                        CurrentThroughput = bestSingleThroughput
                    };

                    await responseStream.WriteAsync(serverResponse);
                }
            }
        }

        public async Task GetUnaryBatchThroughput(GetUnaryBatchThroughputRequest request, IServerStreamWriter<GetUnaryBatchThroughputResponse> responseStream, ServerCallContext context)
        {

            while(!context .CancellationToken.IsCancellationRequested)
            {

                ConcurrentBag<int> getUnaryBatchBag = await _throughputStorage.ReturnBatchUnaryBag();

                if(getUnaryBatchBag.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.3));
                }
                else
                {
                    int bestBatchThroughput = getUnaryBatchBag.Max();

                    GetUnaryBatchThroughputResponse serverResponse = new GetUnaryBatchThroughputResponse()
                    {
                        CurrentThroughput = bestBatchThroughput
                    };

                    await responseStream.WriteAsync(serverResponse);
                }

            }

        }

    }
}
