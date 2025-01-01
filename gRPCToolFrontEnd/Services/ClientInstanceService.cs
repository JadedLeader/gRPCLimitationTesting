using Grpc.Core;
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

        public async Task<StreamClientInstanceResponse> CreatingClientInstancesStreamedAsync(string sessionUnique, string clientUnique, List<Guid> clientList)
        {
            using(var call = _clientInstanceClient.StreamClientInstances())
            {
                var metadata = new Metadata();

                metadata.Add("session-unique", sessionUnique);

                await call.RequestStream.WriteAsync(new StreamClientInstanceRequest 
                { 
                    ClientUnique = clientUnique 
                });

                foreach(Guid message in clientList)
                {
                    var request = new StreamClientInstanceRequest
                    {
                        ClientUnique = message.ToString()
                    };

                    await call.RequestStream.WriteAsync(request);
                }

                await call.RequestStream.CompleteAsync();

                StreamClientInstanceResponse response = await call.ResponseAsync;

                return response;

            }

       
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
