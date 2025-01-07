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

    }
}
