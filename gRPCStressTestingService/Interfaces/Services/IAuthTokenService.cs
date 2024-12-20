using Grpc.Core;
using gRPCStressTestingService.proto;

namespace gRPCStressTestingService.Interfaces.Services
{
    public interface IAuthTokenService
    {

        public Task<GenerateAuthTokenResponse> GenerateToken(GenerateAuthTokenRequest request, ServerCallContext context);

    }
}
