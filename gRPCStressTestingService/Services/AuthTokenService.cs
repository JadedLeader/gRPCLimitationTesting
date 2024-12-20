using DbManagerWorkerService.DbModels;
using DbManagerWorkerService.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using gRPCStressTestingService.proto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace gRPCStressTestingService.Services
{
    public class AuthTokenService : IAuthTokenService
    {

        private readonly IAuthTokenRepo _authTokenRepo;

        private readonly IConfiguration _configuration;

        private readonly IAccountRepo _accountRepo;
        public AuthTokenService(IAuthTokenRepo authTokenRepo, IConfiguration configuration, IAccountRepo accountRepo)
        {
            _authTokenRepo = authTokenRepo;
            _configuration = configuration;
            _accountRepo = accountRepo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<GenerateAuthTokenResponse> GenerateToken(GenerateAuthTokenRequest request, ServerCallContext context)
        {
            Account? getUserAccount = await _accountRepo.GetAccountViaUsername(request.Username);

            if(getUserAccount == null)
            {
                Log.Error($"Could not find an existing account when trying to generate a token for this user");
            }

            string? token = await HasToken(getUserAccount.AccountUnique, request.Username, getUserAccount.Role, getUserAccount);

            GenerateAuthTokenResponse serverResponse = new GenerateAuthTokenResponse()
            {
                Token = token,
                TokenCreationTime = DateTime.Now.ToString()
            };

            return serverResponse;
        }

        public async Task<RevokeAuthTokenResponse> RevokeToken(RevokeAuthTokenRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="username"></param>
        /// <param name="role"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        private async Task<string> HasToken(Guid accountId, string username, string role, Account account)
        {
            Account? getAccountWithToken = await _accountRepo.LinkAccountWithTokenViaId(accountId);

            string? shortLiveToken = GenerateDynamicToken(username, role, 120);

            string? longLivedToken = GenerateDynamicToken(username, role, 43800);

            AuthToken newToken = new AuthToken()
            {
                AuthUnique = Guid.NewGuid(),
                CurrentToken = shortLiveToken,
                RefreshToken = longLivedToken
            };

            //if they dont have a token, we'll generate them a token and add it to the database for both the account and the auth 
            if (getAccountWithToken.AuthUnique == null || getAccountWithToken.AuthToken == null)
            {
                await UpdateAccountAddAuthToken(getAccountWithToken, newToken.AuthUnique, newToken);
            }

            //if they do have a token, we will check the long life token to ensure it is valid
            //if it is not valid, we issue another long life token
            //if it is valid we generate a short lived token and update the auth repo to reflect this change

            string? verifyingLongLifeToken = getAccountWithToken.AuthToken.RefreshToken;

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler(); 

            JwtSecurityToken? longLifeToken = tokenHandler.ReadJwtToken(verifyingLongLifeToken);

            DateTime expiration = longLifeToken.ValidTo;

            if(expiration < DateTime.Now)
            {
                await UpdateAccountAddAuthToken(getAccountWithToken, newToken.AuthUnique, newToken);
            }

            //if the long life token is still active we just want to edit the

            AuthToken getToken = await _authTokenRepo.GetRecordViaId(account.AuthUnique);

            getToken.RefreshToken = longLivedToken;

            await _authTokenRepo.UpdateDbAsync(getToken);

            return shortLiveToken;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="tokenUnique"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task UpdateAccountAddAuthToken(Account account, Guid tokenUnique, AuthToken token)
        {
            await _authTokenRepo.AddToDbAsync(token);
            await _accountRepo.UpdateAuthUnique(account, tokenUnique, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="role"></param>
        /// <param name="tokenDuration"></param>
        /// <returns></returns>
        private string GenerateDynamicToken(string username, string role, int tokenDuration)
        {
            var tokenClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Role", role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
            issuer: _configuration["Tokens:Issuer"],
            audience: _configuration["Tokens:Audience"],
            claims: tokenClaims,
            expires: DateTime.Now.AddMinutes(tokenDuration),
            signingCredentials: credentials);

            string writingToken = new JwtSecurityTokenHandler().WriteToken(token);

            return writingToken;
        }

    }
}
