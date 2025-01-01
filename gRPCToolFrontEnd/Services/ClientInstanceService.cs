using Grpc.Core;
using Grpc.Net.Client;
using Serilog;
using System.Runtime.InteropServices;

namespace gRPCToolFrontEnd.Services
{
    public class ClientInstanceService
    {
        private readonly ClientInstances.ClientInstancesClient _clientInstanceClient;

        public ClientInstanceService(ClientInstances.ClientInstancesClient clientInstancesClient)
        {
            _clientInstanceClient = clientInstancesClient;
        }

        public async Task<CreateClientInstanceResponse> CreateClientInstanceAsync(CreateClientInstanceRequest clientInstanceRequest)
        {
            return await _clientInstanceClient.CreateClientInstanceAsync(clientInstanceRequest);
        }

        public async Task GetClientInstancesViaSessionUnique(GetClientInstancesFromSessionUniqueRequest getClientInstanceRequest, List<Guid> clientInstanceFromDb)
        {
            using(var call = _clientInstanceClient.GetClientInstances(getClientInstanceRequest))
            {
                await foreach(var response in call.ResponseStream.ReadAllAsync())
                {
                    clientInstanceFromDb.Add(Guid.Parse(response.ClientUnique));

                    Log.Information($"Client instance has received Client instance {response.ClientUnique}");
                }
            }
        }

        public async Task<ClearClientInstancesResponse> ClearingClientInstancesStreamedAsync(string sessionUnique, List<Guid> clientsToClear)
        {

           var metadata = new Metadata();

            metadata.Add("session-unique", sessionUnique);

            using(var call = _clientInstanceClient.StreamClientClears(new CallOptions(metadata)))
            {
                foreach(Guid client in clientsToClear)
                {
                    var request = new ClearClientInstancesRequest
                    {
                        SessionUnique = sessionUnique,
                        ClientUnique = client.ToString()
                    };

                    Log.Information($"Sending client instance with ID {client} to be deleted");

                    await call.RequestStream.WriteAsync(request);
                }

                await call.RequestStream.CompleteAsync();

                ClearClientInstancesResponse response = await call.ResponseAsync;

                return response;

            } 
        }

        public async Task<StreamClientInstanceResponse> CreatingClientInstancesStreamedAsync(string sessionUnique, string clientUnique, List<Guid> clientList)
        {

            try
            {
                var metadata = new Metadata();

                metadata.Add("session-unique", sessionUnique);

                using (var call = _clientInstanceClient.StreamClientInstances(new CallOptions(metadata)))
                {
                   
                    foreach (Guid message in clientList)
                    {
                        var request = new StreamClientInstanceRequest
                        {
                            ClientUnique = message.ToString()
                        };

                        Log.Information($"sending message over with GUID: {message}");

                        await call.RequestStream.WriteAsync(request);
                    }

                    await call.RequestStream.CompleteAsync();

                    StreamClientInstanceResponse response = await call.ResponseAsync;

                    return response;

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"exception thrown: {ex.Message}");
            }

            return null;
       
        }

    }
}
