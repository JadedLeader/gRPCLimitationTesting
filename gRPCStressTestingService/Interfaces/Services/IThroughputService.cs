using Grpc.Core;
using gRPCStressTestingService.proto;

namespace gRPCStressTestingService.Interfaces.Services
{
    public interface IThroughputService
    {

        public Task GetStreamingSingleThroughput(GetStreamingSingleThroughputRequest request, IServerStreamWriter<GetStreamingSingleThroughputResponse> responseStream, ServerCallContext context);
     
        public Task GetStreamingBatchThroughput(GetStreamingBatchThroughputRequest request, IServerStreamWriter<GetStreamingBatchThroughputResponse> responseStream, ServerCallContext context);

        public Task GetUnarySingleThroughput(GetUnarySingleThroughputRequest request, IServerStreamWriter<GetUnarySingleThroughputResponse> responseStream, ServerCallContext context);

        public Task GetUnaryBatchThroughput(GetUnaryBatchThroughputRequest request, IServerStreamWriter<GetUnaryBatchThroughputResponse> responseStream, ServerCallContext context);
    }
}
