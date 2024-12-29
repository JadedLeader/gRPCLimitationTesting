using Grpc.Core;
using gRPCStressTestingService;

namespace gRPCStressTestingService.Interfaces.Services
{
    public interface IAdminService
    {

        public Task<RetrievingClientMessagesViaIdResponse> ClientMessages(RetrievingClientMessagesViaIdRequest request, ServerCallContext context);

        public Task<DatabaseWipeResponse> DatabaseReset(DatabaseWipeRequest request, ServerCallContext context);

        public Task<GetAcountViaIdResponse> GetAccountViaId(GetAcountViaIdRequest request, ServerCallContext context);

        public Task<RevokeTokenResponse> RevokeToken(RevokeTokenRequest request, ServerCallContext context);

        public Task<RevokeSessionResponse> RevokeSession(RevokeSessionRequest request, ServerCallContext context);

        public Task<RevokeClientInstanceResponse> RevokeClientInstances(RevokeClientInstanceRequest request, ServerCallContext context);


    }
}
