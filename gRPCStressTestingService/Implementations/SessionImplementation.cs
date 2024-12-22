using Grpc.Core;
using gRPCStressTestingService.Interfaces;
using gRPCStressTestingService.proto;
using Serilog;

namespace gRPCStressTestingService.Implementations
{
    public class SessionImplementation : Sessions.SessionsBase
    {
        private readonly ISessionService _sessionService; 

        public SessionImplementation(ISessionService sessionService)
        {
            _sessionService = sessionService;   
        }

        public override async Task<CreateSessionResponse> CreateSession(CreateSessionRequest request, ServerCallContext context)
        {
            var creatingSession = await _sessionService.CreateSession(request, context);

            if(creatingSession == null)
            {
                Log.Error($"Something went wrong when trying to create a session");
            }

            return creatingSession;
        }
    }
}
