using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using gRPCStressTestingService.proto;

namespace gRPCStressTestingService.Implementations
{
    public class ThroughputImplementation : Throughput.ThroughputBase
    {

        private readonly IThroughputService _throughputService;

        public ThroughputImplementation(IThroughputService throughputService)
        {
            _throughputService = throughputService;
        }

        public override async Task GetStreamingSingleThroughput(GetStreamingSingleThroughputRequest request, IServerStreamWriter<GetStreamingSingleThroughputResponse> responseStream, ServerCallContext context)
        {
            await _throughputService.GetStreamingSingleThroughput(request, responseStream, context);
        }

        public override async Task GetStreamingBatchThroughput(GetStreamingBatchThroughputRequest request, IServerStreamWriter<GetStreamingBatchThroughputResponse> responseStream, ServerCallContext context)
        {
            await _throughputService.GetStreamingBatchThroughput(request, responseStream, context);
        }

        public override async Task GetUnarySingleThroughput(GetUnarySingleThroughputRequest request, IServerStreamWriter<GetUnarySingleThroughputResponse> responseStream, ServerCallContext context)
        {
            await _throughputService.GetUnarySingleThroughput(request,responseStream, context);
        }

        public override async Task GetUnaryBatchThroughput(GetUnaryBatchThroughputRequest request, IServerStreamWriter<GetUnaryBatchThroughputResponse> responseStream, ServerCallContext context)
        {
            await _throughputService.GetUnaryBatchThroughput(request, responseStream, context);
        }

    }
}
