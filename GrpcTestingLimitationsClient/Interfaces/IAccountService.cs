using SharedCommonalities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcTestingLimitationsClient.Interfaces
{
    public interface IAccountService
    {
        public Task AccountLogin(Accounts.AccountsClient accountClient, string username, string password);

    }
}
