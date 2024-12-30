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

        public async Task GetAccountViaId(Admin.AdminClient client, string unique)
        {
            var sendingRequest = await client.GetAccountViaIdAsync(new GetAcountViaIdRequest
            {
                AccountUnique = unique
            });
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

        public async Task<RevokeSessionResponse> RevokeSession(Admin.AdminClient adminClient, string sessionUnique)
        {
            RevokeSessionResponse revokingSession = await adminClient.RevokeSessionAsync(new RevokeSessionRequest
            {
                SessionUnique = sessionUnique
            });


            return revokingSession;
        }

        public async Task<RevokeClientInstanceResponse>  RevokeClientInstance(Admin.AdminClient adminClient, string sessionUnique)
        {
            RevokeClientInstanceResponse revokeClientInstance = await adminClient.RevokeClientInstancesAsync(new RevokeClientInstanceRequest
            {
                SessionUnique = sessionUnique
            });

            return revokeClientInstance;
        }

        public async Task<RevokeTokenResponse> RevokeToken(Admin.AdminClient adminClient, string tokenUnique)
        {
            RevokeTokenResponse revokeToken = await adminClient.RevokeTokenAsync(new RevokeTokenRequest
            {
                TokenUnique = tokenUnique
            });

            return revokeToken;
        }
    }
}
