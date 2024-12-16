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

        public override Task<DatabaseWipeResponse> DatabaseReset(DatabaseWipeRequest request, ServerCallContext context)
        {
            var databaseReset = _adminService.DatabaseReset(request, context); 

            if (databaseReset == null)
            {
                Log.Warning($"Database resset did not work or returned null, please check");
            }

            return databaseReset;
        }

        public override Task<RetrievingAllClientsResponse> RetrievingAllClients(RetrievingAllClientsRequest request, ServerCallContext context)
        {
             throw new NotFiniteNumberException();
        }

    }
}
