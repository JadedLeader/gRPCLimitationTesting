using DbManagerWorkerService.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService;
using gRPCStressTestingService.Interfaces.Services;
using SharedCommonalities.Storage;
using gRPCStressTestingService.protos;
using Serilog;

namespace gRPCStressTestingService.Services
{
    public class AdminService : IAdminService
    {
        private readonly ICommunicationDelayRepo _communicationDelayRepo;
        private readonly ClientStorage _clientStorage;
        private readonly IAccountRepo _accountRepo;
        public AdminService(ICommunicationDelayRepo communicationDelayRepo, ClientStorage clientStorage, IAccountRepo accountRepo)
        {
            _communicationDelayRepo = communicationDelayRepo;
            _clientStorage = clientStorage;
            _accountRepo = accountRepo;
        }

        public async Task<RetrievingClientMessagesViaIdResponse> ClientMessages(RetrievingClientMessagesViaIdRequest request, ServerCallContext context)
        {
            /* RetrievingClientMessagesViaIdResponse clientMessagesResponse = new RetrievingClientMessagesViaIdResponse()
             {
                 ClientId = "",
                 Finished = true
             };

             //we're going to grab the id from the meta data that will be passed in by the user

             _clientStorage.ReturnClientRequestMessages()

             return clientMessagesResponse; */

            throw new NotImplementedException();
        }

        public async Task<DatabaseWipeResponse> DatabaseReset(DatabaseWipeRequest request, ServerCallContext context)
        {
            DatabaseWipeResponse databaseWipe = new DatabaseWipeResponse()
            {
                Finished = true
            };

           // await _communicationDelayRepo.EmptyTable();

            return databaseWipe;


        }

        public async Task<RetrievingAllClientsResponse> RetrievingAllClients(RetrievingAllClientsRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public async Task<GetAcountViaIdResponse> GetAccountViaId(GetAcountViaIdRequest request, ServerCallContext context)
        {
            var thing = _accountRepo.GetAccountViaId(Guid.Parse(request.AccountUnique));

            if(thing == null)
            {
                Log.Error("thing was null when trying to get an account by the id");
            }

            GetAcountViaIdResponse serverResponse = new GetAcountViaIdResponse()
            {
                AccountUnique = request.AccountUnique,
                Success = true,
            };

            return serverResponse;
        }

    }
}
