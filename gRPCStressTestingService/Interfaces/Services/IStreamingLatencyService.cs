using Grpc.Core;

namespace gRPCStressTestingService.Interfaces.Services
{
    public interface IStreamingLatencyService
    {
        public Task StreamingSingleRequest(IAsyncStreamReader<StreamingSingleLatencyRequest> requestStream, IServerStreamWriter<StreamingSingleLatencyResponse> responseStream, ServerCallContext context);

        public Task StreamingManySingleRequest(IAsyncStreamReader<StreamingManySingleLatencyRequest> requestStream, IServerStreamWriter<StreamingManySingleLatencyResponse> responseStream, ServerCallContext context);

        public Task StreamingSingleBatchRequest(IAsyncStreamReader<StreamingBatchLatencyRequest> requestStream, IServerStreamWriter<StreamingBatchLatencyResponse> responseStream, ServerCallContext context);
    }
}
