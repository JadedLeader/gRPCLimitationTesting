using Grpc.Core;
using gRPCStressTestingService.proto;

namespace gRPCStressTestingService.Interfaces.Services
{
    public interface ISessionService
    {

        public Task<CreateSessionResponse> CreateSession(CreateSessionRequest request, ServerCallContext context);

    }
}
