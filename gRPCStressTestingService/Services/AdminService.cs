using ConfigurationStuff.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService;
using gRPCStressTestingService.Interfaces.Services;
using SharedCommonalities.Storage;
using gRPCStressTestingService.protos;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using ConfigurationStuff.DbModels;

namespace gRPCStressTestingService.Services
{
    public class AdminService : IAdminService
    {
       
        private readonly ClientStorage _clientStorage;
        private readonly IAccountRepo _accountRepo;
        private readonly IAuthTokenRepo _authTokenRepo;
        private readonly ISessionRepo _sessionRepo;
        private readonly IClientInstanceRepo _clientInstanceRepo;
        private readonly IDelayCalcRepo _delayCalcRepo;
        public AdminService( ClientStorage clientStorage, IAccountRepo accountRepo, IAuthTokenRepo authTokenRepo, ISessionRepo sessionRepo, 
            IClientInstanceRepo clientRepo, IDelayCalcRepo delayCalcRepo)
        {
            
            _clientStorage = clientStorage;
            _accountRepo = accountRepo;
            _authTokenRepo = authTokenRepo;
            _sessionRepo = sessionRepo;
            _clientInstanceRepo = clientRepo;
            _delayCalcRepo = delayCalcRepo;
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

        /// <summary>
        /// This method handles the revocation of a session
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="RpcException"></exception>
        public async Task<RevokeSessionResponse> RevokeSession(RevokeSessionRequest request, ServerCallContext context)
        {
            Account? getSessionAccount = await _accountRepo.GetSessionAndAccountViaSessionId(Guid.Parse(request.SessionUnique));

            if(getSessionAccount == null)
            {
                Log.Warning($"Could not link a session with an account based off the session ID : {request.SessionUnique}");

                throw new RpcException(new Status(StatusCode.NotFound, $"Session not found"));
            }

            RevokeSessionResponse serverResponse = new RevokeSessionResponse()
            {
                SessionUnique = request.SessionUnique,
                TimeOfSessionRevoke = DateTime.Now.ToString()
            };

            //to revoke a session, it's simple, we're literally just going to want to edit both the sessions table and the account table
            //in the session table we're just going to want  to set the session created = null
            //in the account table we want to set the time of login to null  but we can keep the session unique

            if(getSessionAccount.Session.SessionUnique != null)
            {
                getSessionAccount.Session.SessionCreated = null;

                getSessionAccount.TimeOfLogin = null;

                await _sessionRepo.UpdateDbAsync(getSessionAccount.Session);

                await _accountRepo.UpdateDbAsync(getSessionAccount);

                await _sessionRepo.SaveAsync(); 
            }

            if(getSessionAccount.Session == null)
            {
                Log.Warning($"No session with ID: {request.SessionUnique} belongs to an account, this session cannot be revoked");

                throw new RpcException(new Status(StatusCode.NotFound, $"Session not found"));
            }

            return serverResponse;
        }

        public async Task<RevokeClientInstanceResponse> RevokeClientInstances(RevokeClientInstanceRequest request, ServerCallContext context)
        {
            List<ClientInstance> clientInstancesWithSessions = await _clientInstanceRepo.GetClientInstancesViaSessionId(Guid.Parse(request.SessionUnique));

            if(clientInstancesWithSessions.Count == 0)
            {
                Log.Warning($"No client instances can be found off session ID {request.SessionUnique}");
                throw new RpcException(new Status(StatusCode.NotFound, $"Client instances cannot be found"));
            }

            await _clientInstanceRepo.RemoveRange(clientInstancesWithSessions);

            await _clientInstanceRepo.SaveAsync();

            RevokeClientInstanceResponse serverResponse = new RevokeClientInstanceResponse()
            {
                SessionUnique = request.SessionUnique,
                TimeOfClientRevoke = DateTime.Now.ToString()
            };

            return serverResponse;
        }

        public async Task<WipeDelayCalcResponse> ClearDelayCalc(WipeDelayCalcRequest request, ServerCallContext context)
        {
            await _delayCalcRepo.EmptyDelayCalcTable();


            WipeDelayCalcResponse serverResponse = new WipeDelayCalcResponse
            {
                Finished = true
            };

            return serverResponse;
        }

    }
}
