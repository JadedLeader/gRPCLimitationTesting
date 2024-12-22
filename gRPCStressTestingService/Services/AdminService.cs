using DbManagerWorkerService.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService;
using gRPCStressTestingService.Interfaces.Services;
using SharedCommonalities.Storage;
using gRPCStressTestingService.protos;
using Serilog;
using DbManagerWorkerService.DbModels;
using Microsoft.AspNetCore.Mvc;

namespace gRPCStressTestingService.Services
{
    public class AdminService : IAdminService
    {
        private readonly ICommunicationDelayRepo _communicationDelayRepo;
        private readonly ClientStorage _clientStorage;
        private readonly IAccountRepo _accountRepo;
        private readonly IAuthTokenRepo _authTokenRepo;
        public AdminService(ICommunicationDelayRepo communicationDelayRepo, ClientStorage clientStorage, IAccountRepo accountRepo, IAuthTokenRepo authTokenRepo)
        {
            _communicationDelayRepo = communicationDelayRepo;
            _clientStorage = clientStorage;
            _accountRepo = accountRepo;
            _authTokenRepo = authTokenRepo;
        }

        public async Task<RetrievingClientMessagesViaIdResponse> ClientMessages(RetrievingClientMessagesViaIdRequest request, ServerCallContext context)
        {
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
            Account? getAccount = await _accountRepo.GetAccountViaId(Guid.Parse(request.AccountUnique));

            GetAcountViaIdResponse serverResponse = new GetAcountViaIdResponse()
            {
                AccountUnique = request.AccountUnique,
                Success = true,

            };

            if (getAccount == null)
            {
                Log.Warning($"Could not find an account when trying to get it via the ID {request.AccountUnique}");
                throw new RpcException(new Status(StatusCode.NotFound, $"Account not found"));
            }

            return serverResponse;
        }

        public async Task<RevokeTokenResponse> RevokeToken(RevokeTokenRequest request, ServerCallContext context)
        {
            Account? retrieveAccount = await _accountRepo.GetAccountViaToken(Guid.Parse(request.TokenUnique));

            if(retrieveAccount == null)
            {
                Log.Warning($"Could not find an account when trying to get it via the token ID of {request.TokenUnique}");
                throw new RpcException(new Status(StatusCode.NotFound, $"Account not found"));
            }

            RevokeTokenResponse serverResponse = new RevokeTokenResponse()
            {
                TokenUnique = request.TokenUnique,
                State = true
            };

            if (retrieveAccount.AuthUnique == null || retrieveAccount.AuthToken == null)
            {
                serverResponse.TokenUnique = request.TokenUnique;
                serverResponse.State = false;

                return serverResponse;
            
            }

            retrieveAccount.AuthToken.RefreshToken = null;
            retrieveAccount.AuthToken.CurrentToken = null; 

            await _accountRepo.UpdateDbAsync(retrieveAccount);


            return serverResponse;
        }

    }
}
