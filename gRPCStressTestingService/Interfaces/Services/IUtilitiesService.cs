using Grpc.Core;

namespace gRPCStressTestingService.Interfaces.Services
{
    public interface IUtilitiesService
    {
        

        public Task GetstreamingBatchDelays(GetStreamingBatchDelaysRequest request, IServerStreamWriter<GetStreamingBatchDelaysResponse> responseStream, ServerCallContext context);

        public Task GetStreamingDelays(GetStreamingDelaysRequest request, IServerStreamWriter<GetStreamingDelaysResponse> responseStream, ServerCallContext context);

        public Task GetUnaryDelays(GetUnaryDelaysRequest request, IServerStreamWriter<GetUnaryDelaysResponse> responseStream, ServerCallContext context);

        public Task GetUnaryBatchDelays(GetUnaryBatchDelaysRequest request, IServerStreamWriter<GetUnaryBatchDelaysResponse> responseStream, ServerCallContext context);
    }
}
