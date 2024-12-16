using Grpc.Core;
using GrpcTestingLimitationsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcTestingLimitationsClient.Services
{
    public class AdminService
    {

        public AdminService()
        {
            
        }


        public async Task ResettingDatabase(Admin.AdminClient adminClient)
        {

            var resettingDatabaseRequest = await adminClient.DatabaseResetAsync(new DatabaseWipeRequest
            {

            });

            Console.WriteLine($"reset database?");

        }

        public async Task RetrievingAllClients(Admin.AdminClient adminClient)
        {
            var retrievingAllClients = await adminClient.RetrievingAllClientsAsync(new RetrievingAllClientsRequest
            {

            }); 
        }
    }
}
