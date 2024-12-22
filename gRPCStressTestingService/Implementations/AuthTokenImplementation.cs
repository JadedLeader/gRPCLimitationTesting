using DbManagerWorkerService.DbModels;
using DbManagerWorkerService.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using gRPCStressTestingService.proto;
using Serilog;

namespace gRPCStressTestingService.Implementations
{
    public class AuthTokenImplementation : AuthTokens.AuthTokensBase
    {

        private readonly IAuthTokenService _authTokenService;
        public AuthTokenImplementation(IAuthTokenService authTokenService)
        {
            _authTokenService = authTokenService;
        }

        public override async Task<GenerateAuthTokenResponse> GenerateToken(GenerateAuthTokenRequest request, ServerCallContext context)
        {
            GenerateAuthTokenResponse? tokenGeneration = await _authTokenService.GenerateToken(request, context);

            if(tokenGeneration == null)
            {
                Log.Warning($"Token implementation could not generate a token");
            }

            return tokenGeneration;
        }

    }
}
