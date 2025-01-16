using Grpc.Core;

namespace gRPCStressTestingService.Interfaces.Services
{
    public interface IUtilitiesService
    {

        public Task GetClientsWithMessages(GetClientsWithMessagesRequest request, IServerStreamWriter<GetClientsWithMessagesResponse> responseStream, ServerCallContext context);

        public Task GetstreamingBatchDelays(GetStreamingBatchDelaysRequest request, IServerStreamWriter<GetStreamingBatchDelaysResponse> responseStream, ServerCallContext context);

    }
}
