using DbManagerWorkerService.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService;
using gRPCStressTestingService.Interfaces.Services;
using SharedCommonalities.Storage;
using gRPCStressTestingService.protos;

namespace gRPCStressTestingService.Services
{
    public class AdminService : IAdminService
    {
        private readonly ICommunicationDelayRepo _communicationDelayRepo;
        private readonly ClientStorage _clientStorage;
        public AdminService(ICommunicationDelayRepo communicationDelayRepo, ClientStorage clientStorage)
        {
            _communicationDelayRepo = communicationDelayRepo;
            _clientStorage = clientStorage;
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

            await _communicationDelayRepo.EmptyTable();

            return databaseWipe;


        }

        public async Task<RetrievingAllClientsResponse> RetrievingAllClients(RetrievingAllClientsRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

    }
}
