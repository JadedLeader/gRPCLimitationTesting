using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcTestingLimitationsClient.Services
{
    public class SessionsService
    {

        public SessionsService()
        {
            
        }


        public async Task<CreateSessionResponse> GenerateSession(Sessions.SessionsClient client, string username)
        {
            CreateSessionResponse generateSession = await client.CreateSessionAsync(new CreateSessionRequest
            {
                Username = username,
            });

            return generateSession;
        }

    }
}
