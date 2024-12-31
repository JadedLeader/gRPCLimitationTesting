namespace gRPCToolFrontEnd.Services
{
   

    public class AccountsService
    {
        private readonly Accounts.AccountsClient _grpcAccountClient;
        public AccountsService(Accounts.AccountsClient grpcAccountClient)
        {
            _grpcAccountClient = grpcAccountClient;
        }

        public async Task<AccountLoginResponse> LoginAsync(AccountLoginRequest loginRequest)
        {
            return await _grpcAccountClient.AccountLoginAsync(loginRequest);
        }

        public async Task<CreateAccountResponse> CreateAccount(CreateAccountRequest createAccountRequest)
        {
            var metadata = new Grpc.Core.Metadata();

            metadata.Add("role", "User");
            metadata.Add("creation-time", DateTime.Now.ToString());

            return await _grpcAccountClient.CreateAccountAsync(createAccountRequest, metadata);
        }



    }
}
