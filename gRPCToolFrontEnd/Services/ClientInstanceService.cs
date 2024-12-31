using Grpc.Net.Client;

namespace gRPCToolFrontEnd.Services
{
    public class ClientInstanceService
    {
        private readonly ClientInstances.ClientInstancesClient _clientInstanceClient;

        private List<Guid> ClientList = new List<Guid>();
        public ClientInstanceService(ClientInstances.ClientInstancesClient clientInstancesClient)
        {
            _clientInstanceClient = clientInstancesClient;
        }

        public async Task<CreateClientInstanceResponse> CreateClientInstanceAsync(CreateClientInstanceRequest clientInstanceRequest)
        {
            return await _clientInstanceClient.CreateClientInstanceAsync(clientInstanceRequest);
        }

        public async Task GeneratingClientInstances(int numberOfClientInstances)
        {
            int i = 0; 

            while(numberOfClientInstances < i)
            {
                ClientList.Add(Guid.NewGuid());
            }
        }

        public async Task<List<Guid>> GetClientList()
        {
            return ClientList;
        }

    }
}
