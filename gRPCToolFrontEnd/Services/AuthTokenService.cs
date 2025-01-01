namespace gRPCToolFrontEnd.Services
{
    public class AuthTokenService
    {

        private readonly AuthTokens.AuthTokensClient _authTokensClient;
        public AuthTokenService(AuthTokens.AuthTokensClient authTokensClient)
        {
            _authTokensClient = authTokensClient;
        }

        public async Task<GenerateAuthTokenResponse> GenerateAuthTokenAsync(GenerateAuthTokenRequest authTokenRequest)
        {
            return await _authTokensClient.GenerateTokenAsync(authTokenRequest);
        }

    }
}
