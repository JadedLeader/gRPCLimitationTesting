using DbManagerWorkerService.Interfaces.DataContext;
using DevOne.Security.Cryptography.BCrypt;
using Grpc.Core;
using gRPCStressTestingService.proto;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Identity.Client;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using gRPCStressTestingService.Interfaces;
using gRPCStressTestingService.Interfaces.Services;
using Microsoft.VisualBasic;
using Serilog;
using ConfigurationStuff.Interfaces.Repos;
using ConfigurationStuff.DbModels;


namespace gRPCStressTestingService.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;

        public AccountService(IAccountRepo accountRepo)
        {
            _accountRepo = accountRepo;
        }

        public async Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request, ServerCallContext context)
        {
            //we're literally just going to want to read the request, get the username and password and boom

            string? accountRole = context.RequestHeaders.GetValue("role");
            string? accountCreationTime = context.RequestHeaders.GetValue("creation-time");

            //if it does not exist it returns false
            bool usernameExist = await VerifyingNonExistingUsername(request.Username);

            if(usernameExist == true)
            {
                Log.Information($"User will not be created with name {request.Username} as this user already exists in the database");
                CreateAccountResponse invalidServerResponse = new CreateAccountResponse()
                {
                    Username = request.Username,
                    Password = "N/A",
                    Role = "N/A",
                    TimeOfCreation = "N/A"
                };

                return invalidServerResponse;
            }

            CreateAccountResponse serverResponse = new CreateAccountResponse()
            {
                Username = request.Username,
                Password = request.Password,
                Role = accountRole,
                TimeOfCreation = accountCreationTime, 
 
            };

            //then we just need to add to the db - we want to serialise the password first before we add it to the database

            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password, 13);

            Account newAccount = new Account
            {
                AccountUnique = Guid.NewGuid(),
                Username = request.Username,
                Password = passwordHash,
                TimeOfLogin = "",
                TimeOfAccountCreation = DateTime.Now.ToString(),
                Role = accountRole,
            };

            await _accountRepo.AddToDbAsync(newAccount);

            await _accountRepo.SaveAsync();

            return serverResponse;

        }

        /// <summary>
        /// Account login functionality
        /// Takes the username and password from the request 
        /// Passwords are hashed by default, required to de-hash and check the password is equal
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>Returns an account login response, containing the username and the state (state should always be true unless there's an rpc exception</returns>
        public async Task<AccountLoginResponse> AccountLogin(AccountLoginRequest request, ServerCallContext context)
        {
            Account? userAccount =  await _accountRepo.GetAccountViaUsername(request.Username);

            if(userAccount == null)
            {
                Log.Warning($"No user account can be found via the username {request.Username}");

                throw new RpcException(new Status(StatusCode.NotFound, $"Account not found"));
            }

            string hashedPassword = userAccount.Password;

            bool match = BCrypt.Net.BCrypt.EnhancedVerify(request.Password, hashedPassword);

            if(!match)
            {
                Log.Warning($"The password from the request was not a match to the password retrieved from the database");

                throw new RpcException(new Status(StatusCode.Unauthenticated, $"The stored password and the password entered don't match"));
            }

            AccountLoginResponse serverResponse = new AccountLoginResponse()
            {
                Username = request.Username,
                State = true,
                Role = userAccount.Role,
                AccountUnique = userAccount.AccountUnique.ToString(),
                SessionUnique = userAccount.Session.SessionUnique.ToString()
            };

            Log.Information($"Account login successfull for user : {serverResponse.Username}");

            return serverResponse;

        }

        public async Task<DeleteAccountResponse> DeleteAccount(DeleteAccountRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> VerifyingNonExistingUsername(string username)
        {
            Account? getAccount = await _accountRepo.GetAccountViaUsername(username);

            if (getAccount == null)
            {
                return false;
            }

            return true;

        }

    }
}
