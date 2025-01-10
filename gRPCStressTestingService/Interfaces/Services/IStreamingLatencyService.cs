using Grpc.Core;

namespace gRPCStressTestingService.Interfaces.Services
{
    public interface IStreamingLatencyService
    {
        public Task StreamingSingleRequest(IAsyncStreamReader<StreamingSingleLatencyRequest> requestStream, IServerStreamWriter<StreamingSingleLatencyResponse> responseStream, ServerCallContext context);
    }
}
