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

    }
}
