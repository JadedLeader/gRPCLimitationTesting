using DbManagerWorkerService.DbModels;
using DbManagerWorkerService.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using gRPCStressTestingService.proto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Serilog;
using Serilog.Parsing;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
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
        /// This is the main function for triggering the gRPC endpoint
        /// Generates token(s) (both short lived and long depending on the current account) and adds them to the database
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>The token and the creation time of the token</returns>
        public async Task<GenerateAuthTokenResponse> GenerateToken(GenerateAuthTokenRequest request, ServerCallContext context)
        {
            Account? getUserAccount = await _accountRepo.GetAccountViaUsername(request.Username);

            if(getUserAccount == null)
            {
                Log.Warning($"No account could be found via the username of {request.Username}");

                throw new RpcException(new Status(StatusCode.NotFound, $"Account not found"));
            }

            string? token = await HasToken(getUserAccount.AccountUnique, request.Username, getUserAccount.Role, getUserAccount);

            GenerateAuthTokenResponse serverResponse = new GenerateAuthTokenResponse()
            {
                Token = token,
                TokenCreationTime = DateTime.Now.ToString()
            };

            return serverResponse;
        }


        /// <summary>
        /// This method is in charge of the operations that occur if an account has a token or not
        /// If the account doesn't have a token, we generate both a long lived and a short lived token and add them to the database
        /// If the account does have a current long lifetime token, we ensure it's in date, if the token is not in date, we refresh the long lived token
        /// If the long lived token is in date, the short lived token is regenerated
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="username"></param>
        /// <param name="role"></param>
        /// <param name="account"></param>
        /// <returns>The short lived token</returns>
        private async Task<string> HasToken(Guid accountId, string username, string role, Account account)
        {
            Account? getAccountWithToken = await _accountRepo.LinkAccountWithTokenViaId(accountId);

            if(getAccountWithToken == null)
            {
                Log.Warning($"Could not find an account with the ID {accountId}");

                throw new RpcException(new Status(StatusCode.NotFound, $"Account not found"));
            }

            string shortLiveToken = GenerateDynamicToken(username, role, 120);
            string longLivedToken = GenerateDynamicToken(username, role, 43800);

            AuthToken newToken = new AuthToken()
            {
                AuthUnique = Guid.NewGuid(),
                CurrentToken = shortLiveToken,
                RefreshToken = longLivedToken
            };

            if (getAccountWithToken.AuthUnique == null || getAccountWithToken.AuthToken == null)
            {
                await UpdateAccountAddAuthToken(getAccountWithToken, newToken.AuthUnique, newToken);
            }
            
            if (getAccountWithToken.AuthUnique != null && getAccountWithToken.AuthToken.CurrentToken == null && getAccountWithToken.AuthToken.RefreshToken == null)
            {

                getAccountWithToken.AuthToken.CurrentToken = shortLiveToken; 
                getAccountWithToken.AuthToken.RefreshToken = longLivedToken;

                await _accountRepo.UpdateAuthUnique(getAccountWithToken, getAccountWithToken.AuthUnique, getAccountWithToken.AuthToken);
            }

            if (getAccountWithToken.AuthUnique != null && getAccountWithToken.AuthToken.CurrentToken != null && getAccountWithToken.AuthToken.RefreshToken != null)
            {
                string longLifeToken = getAccountWithToken.AuthToken.RefreshToken;

                JwtSecurityTokenHandler jwtHandler = new JwtSecurityTokenHandler();

                SecurityToken readingToken = jwtHandler.ReadToken(longLifeToken); 

                DateTime timeValid = readingToken.ValidTo;

                if(timeValid < DateTime.Now)
                {
                    getAccountWithToken.AuthToken.RefreshToken = longLifeToken;
                }

                getAccountWithToken.AuthToken.CurrentToken = shortLiveToken;

                await _accountRepo.UpdateAuthUnique(getAccountWithToken, getAccountWithToken.AuthUnique, getAccountWithToken.AuthToken);
            }

            return shortLiveToken;

        }

        /// <summary>
        /// Adds a token to the authTokens table in the database 
        /// Also allows for updating fields in the account table, such as the authUnique and the navigation property, pushing those changes into the database
        /// </summary>
        /// <param name="account"></param>
        /// <param name="tokenUnique"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task UpdateAccountAddAuthToken(Account account, Guid? tokenUnique, AuthToken token)
        {
            await _authTokenRepo.AddToDbAsync(token);
            await _accountRepo.UpdateAuthUnique(account, tokenUnique, token);
        }

        /// <summary>
        /// Generates a JWT token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="role"></param>
        /// <param name="tokenDuration"></param>
        /// <returns></returns>
        private string GenerateDynamicToken(string username, string role, int tokenDuration)
        {
            Claim[] tokenClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Role", role),
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            JwtSecurityToken token = new JwtSecurityToken(
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
