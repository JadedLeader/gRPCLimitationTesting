
using Grpc.Core;
using gRPCStressTestingService.proto;
using gRPCStressTestingService.Interfaces;
using gRPCStressTestingService.Interfaces.Services;
using Serilog;

namespace gRPCStressTestingService.Implementations
{
    public class ClientInstanceImplementation : ClientInstances.ClientInstancesBase
    {
        private readonly IClientInstanceService _clientInstanceService;
        public ClientInstanceImplementation(IClientInstanceService clientInstanceService)
        {
            _clientInstanceService = clientInstanceService;
        }

        public override async Task<CreateClientInstanceResponse> CreateClientInstance(CreateClientInstanceRequest request, ServerCallContext context)
        {
            CreateClientInstanceResponse createClientInstance = await _clientInstanceService.CreateClientInstance(request, context);

            if(createClientInstance == null)
            {
                Log.Error($"Something went wrong when creating a client instance");

                throw new RpcException(new Status(StatusCode.Internal, $"Could not create client instance")); 
            }

            return createClientInstance;
        }

        public override Task<CreateClientInstanceResponse> StreamClientInstances(IAsyncStreamReader<StreamClientInstanceRequest> requestStream, ServerCallContext context)
        {
            return base.StreamClientInstances(requestStream, context);
        }

    }
}
