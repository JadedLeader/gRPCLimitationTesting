using Grpc.Core;
using gRPCStressTestingService.proto;

namespace gRPCStressTestingService.Interfaces.Services
{
    public interface IClientInstanceService
    {

        public Task<CreateClientInstanceResponse> CreateClientInstance(CreateClientInstanceRequest request, ServerCallContext context);

        public Task<StreamClientInstanceResponse> StreamClientInstances(IAsyncStreamReader<StreamClientInstanceRequest> requestStream, ServerCallContext context);

        public Task<ClearClientInstancesResponse> StreamClientClears(IAsyncStreamReader<ClearClientInstancesRequest> requestStream, ServerCallContext context);

        public Task GetClientInstances(GetClientInstancesFromSessionUniqueRequest request, IServerStreamWriter<GetClientInstancesFromSessionUniqueResponse> responseStream, ServerCallContext context);

    }
}
