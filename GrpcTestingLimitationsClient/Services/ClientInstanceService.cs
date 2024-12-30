using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcTestingLimitationsClient.Services
{
    public class ClientInstanceService
    {

        public ClientInstanceService()
        {
            
        }


        public async Task<CreateClientInstanceResponse> GenerateClientIntsance(ClientInstances.ClientInstancesClient client, string username, string sessionUnique)
        {
            CreateClientInstanceResponse generatingClientInstance = await client.CreateClientInstanceAsync(new CreateClientInstanceRequest
            {
                SessionUnique = sessionUnique,
                Username = username,

            });

            return generatingClientInstance;
        }

    }
}
