using DbManagerWorkerService.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using gRPCStressTestingService.proto;
using Serilog;

namespace gRPCStressTestingService.Implementations
{
    public class AccountImplementation : Accounts.AccountsBase
    {

        private readonly IAccountService _accountService;

        public AccountImplementation(IAccountService accountService)
        {
            _accountService = accountService;
        }
        public override async Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request, ServerCallContext context)
        {
            var calling = await _accountService.CreateAccount(request, context);

            if(calling == null)
            {
                Log.Error($"{@calling} was null in the account creation response implementation");
            }

            return calling;
        }

        public override async Task<DeleteAccountResponse> DeleteAccount(DeleteAccountRequest request, ServerCallContext context)
        {
            return await base.DeleteAccount(request, context);
        }

        public override async Task<AccountLoginResponse> AccountLogin(AccountLoginRequest request, ServerCallContext context)
        {
            var accountLogin = await _accountService.AccountLogin(request, context);

            if (accountLogin == null)
            {
                Log.Error($"{accountLogin} something went wrong when trying to login to the account");
            }

            return accountLogin;
        }

    }
}
