using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcTestingLimitationsClient.Services
{
    public class AuthTokensService
    {

        public AuthTokensService()
        {
            
        }


        public async Task GenerateAuthToken(AuthTokens.AuthTokensClient client, string username)
        {
            
            GenerateAuthTokenResponse generatingToken = await client.GenerateTokenAsync(new GenerateAuthTokenRequest
            {
                Username = username
            });
        }

    }
}
