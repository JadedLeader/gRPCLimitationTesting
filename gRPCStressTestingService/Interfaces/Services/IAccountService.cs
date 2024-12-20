using Grpc.Core;
using gRPCStressTestingService.proto;

namespace gRPCStressTestingService.Interfaces.Services
{
    public interface IAccountService
    {

        public Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request, ServerCallContext context);

        public Task<AccountLoginResponse> AccountLogin(AccountLoginRequest request, ServerCallContext context);
    }
}
