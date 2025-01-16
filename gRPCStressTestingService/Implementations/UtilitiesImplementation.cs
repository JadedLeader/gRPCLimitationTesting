using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;

namespace gRPCStressTestingService.Implementations
{
    public class UtilitiesImplementation : Utilities.UtilitiesBase
    {

        private readonly IUtilitiesService _utilitiesService;
        public UtilitiesImplementation(IUtilitiesService utilitiesService)
        {
            _utilitiesService = utilitiesService;
        }

        public override async Task GetClientsWithMessages(GetClientsWithMessagesRequest request, IServerStreamWriter<GetClientsWithMessagesResponse> responseStream, ServerCallContext context)
        {
            await _utilitiesService.GetClientsWithMessages(request, responseStream, context);
        }

        public override async Task GetstreamingBatchDelays(GetStreamingBatchDelaysRequest request, IServerStreamWriter<GetStreamingBatchDelaysResponse> responseStream, ServerCallContext context)
        {
            await _utilitiesService.GetstreamingBatchDelays(request, responseStream, context);
        }


    }
}
