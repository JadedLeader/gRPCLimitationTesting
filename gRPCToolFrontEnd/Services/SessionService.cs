namespace gRPCToolFrontEnd.Services
{
    public class SessionService
    {
        private readonly Sessions.SessionsClient _sessionsClient; 
        public SessionService(Sessions.SessionsClient sessionsClient)
        {
            _sessionsClient = sessionsClient;
        }

        public async Task<CreateSessionResponse> CreateSessionAsync(CreateSessionRequest createSessionRequest)
        {
           return await _sessionsClient.CreateSessionAsync(createSessionRequest);
        }

    }
}
