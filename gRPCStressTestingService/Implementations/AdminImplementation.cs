using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using gRPCStressTestingService;
using Serilog;


namespace gRPCStressTestingService.Implementations
{
    public class AdminImplementation : Admin.AdminBase
    {
        private readonly IAdminService _adminService;
        public AdminImplementation(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public override Task<RetrievingClientMessagesViaIdResponse> ClientMessages(RetrievingClientMessagesViaIdRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public override async Task<DatabaseWipeResponse> DatabaseReset(DatabaseWipeRequest request, ServerCallContext context)
        {
            var databaseReset = await _adminService.DatabaseReset(request, context); 

            if (databaseReset == null)
            {
                Log.Warning($"Database reset did not work or returned null, please check");
            }

            return databaseReset;
        }
        public override async Task<GetAcountViaIdResponse> GetAccountViaId(GetAcountViaIdRequest request, ServerCallContext context)
        {
            var calling = await _adminService.GetAccountViaId(request, context);

            if(calling == null)
            {
                Log.Warning($"couldn't get an account via the id");
            }

            return calling;

        }

        public override Task<RetrievingAllClientsResponse> RetrievingAllClients(RetrievingAllClientsRequest request, ServerCallContext context)
        {
             throw new NotFiniteNumberException();
        }

        public override async Task<RevokeTokenResponse> RevokeToken(RevokeTokenRequest request, ServerCallContext context)
        {
            var callingTokenRevoke = await _adminService.RevokeToken(request, context);

            if(callingTokenRevoke == null)
            {
                Log.Error($"{callingTokenRevoke} did not return as expected");
                throw new RpcException(new Status(new StatusCode(), $"The revoke token endpoint did not function as expected {StatusCode.Internal}"));
            }

            return callingTokenRevoke;
        }

        public override async Task<RevokeSessionResponse> RevokeSession(RevokeSessionRequest request, ServerCallContext context)
        {
            RevokeSessionResponse? revokeSession = await _adminService.RevokeSession(request, context);

            if(revokeSession == null)
            {
                Log.Error($"The revoke session endpoint has failed");

                throw new RpcException(new Status(StatusCode.Internal, $"Session could not be revoked, an error occurred within the admin service"));
            }

            return revokeSession;
        }

        public override async Task<RevokeClientInstanceResponse> RevokeClientInstances(RevokeClientInstanceRequest request, ServerCallContext context)
        {
            RevokeClientInstanceResponse revokeClientInstances = await _adminService.RevokeClientInstances(request, context);

            if(revokeClientInstances == null)
            {
                Log.Error($"The revoke client instances endpoint has failed");

                throw new RpcException(new Status(StatusCode.Internal, $"Client instances could not be revoked, an error occureed within the admin service")); 
            }

            return revokeClientInstances;
        }

        public override async Task<WipeDelayCalcResponse> ClearDelayCalc(WipeDelayCalcRequest request, ServerCallContext context)
        {
            WipeDelayCalcResponse wipeDelayCalc = await _adminService.ClearDelayCalc(request, context);

            if(wipeDelayCalc == null)
            {
                Log.Error($"The wiping of the delay calc table could not be performed");

                throw new RpcException(new Status(StatusCode.Internal, $"could not empty database table")); 
            }

            return wipeDelayCalc;
        }

    }
}
