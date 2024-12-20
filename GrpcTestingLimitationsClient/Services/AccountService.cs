using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcTestingLimitationsClient.Services
{
    public class AccountService
    {

        public AccountService()
        {
            
        }

        public void CreateAccount(Accounts.AccountsClient client, string username, string password)
        {

            Metadata metadata = new Metadata();

            string accountCreation = DateTime.Now.ToString();

            metadata.Add("role", "User"); 
            metadata.Add("creation-time", accountCreation);

            var creatingRequest = client.CreateAccountAsync(new CreateAccountRequest
            {
                Username = username,
                Password = password

            }, metadata);
        }

        public void AccountLogin(Accounts.AccountsClient accountClient, string username, string password)
        {
            Metadata metadata = new Metadata();

            //metadata.Add("login-timestamp", DateTime.Now.ToString());

            var creatingRequest = accountClient.AccountLoginAsync(new AccountLoginRequest
            {
                Username = username,
                Password = password,
            });

        }

        public void DeleteAccount(Accounts.AccountsClient client, string accountId)
        {

            Metadata metadata = new Metadata();

            var creatingRequest = client.DeleteAccountAsync(new DeleteAccountRequest
            {
                AccountId = accountId
            });


        }

    }
}
