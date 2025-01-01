using ConfigurationStuff.Interfaces.Repos;
using ConfigurationStuff.DbModels;
using Grpc.Core;
using gRPCStressTestingService.Interfaces;
using gRPCStressTestingService.proto;
using Serilog;

namespace gRPCStressTestingService.Services
{
    public class SessionService : ISessionService
    {

        private readonly ISessionRepo _sessionRepo;

        private readonly IAccountRepo _accountRepo;

        public SessionService(ISessionRepo sessionRepo, IAccountRepo accountrepo)
        {
            _sessionRepo = sessionRepo;
            _accountRepo = accountrepo;
        }

        /// <summary>
        /// This method handles the creation of sessions based on the different states sessions can be in
        /// 1. The user has no previous sessions, indicating a new account, a new session will be created, filling both the session table and updating the account table
        /// 2. The user has a previous session, indicating prior account usage, both the session and the account table are updated
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>A CreateSessionResponse, detailing the session unique identifier, the username for what user changed and the time of the session creation</returns>
        /// <exception cref="RpcException"></exception>
        public async Task<CreateSessionResponse> CreateSession(CreateSessionRequest request, ServerCallContext context)
        {
            
            Account? getAccountViaUsername = await _accountRepo.LinkAccountWithSessionViaUsername(request.Username);

            if (getAccountViaUsername == null)
            {
                Log.Warning($"No account found for the given username {request.Username}, cannot link this username to a session");

                throw new RpcException(new Status(StatusCode.NotFound, $"No account can be found with this username"));
            }

            Log.Information($"Successfully found an account matching the username {request.Username}");

            Session newSession = new Session()
            {
                AccountUnique = getAccountViaUsername.AccountUnique,
                SessionUnique = Guid.NewGuid(),
                SessionCreated = DateTime.Now.ToString(),
                ClientInstance = new List<ClientInstance>()
            };

            CreateSessionResponse serverResponse = new CreateSessionResponse()
            {
                SessionUnique = newSession.SessionUnique.ToString(),
                Username = request.Username,
                TimeOfSessionCreation = newSession.SessionCreated,
            };

            if (getAccountViaUsername.Session != null)
            {

                serverResponse.SessionUnique = getAccountViaUsername.Session.SessionUnique.ToString();

                getAccountViaUsername.Session.SessionCreated = DateTime.Now.ToString();
                getAccountViaUsername.TimeOfLogin = DateTime.Now.ToString();

                await _accountRepo.UpdateDbAsync(getAccountViaUsername);
                await _sessionRepo.UpdateDbAsync(getAccountViaUsername.Session);

                await _accountRepo.SaveAsync();
                await _sessionRepo.SaveAsync();

            }

            if (getAccountViaUsername.Session == null)
            {
                await _sessionRepo.AddToDbAsync(newSession);

                getAccountViaUsername.TimeOfLogin = newSession.SessionCreated;
                getAccountViaUsername.Session = newSession;

                await _accountRepo.UpdateDbAsync(getAccountViaUsername);

                await _sessionRepo.SaveAsync();
                await _accountRepo.SaveAsync();
            }

            return serverResponse;


        }

    }
}
