using ConfigurationStuff.Interfaces.Repos;
using ConfigurationStuff.DbModels;
using Grpc.Core;
using gRPCStressTestingService.proto;
using Microsoft.EntityFrameworkCore.Query.Internal;
using gRPCStressTestingService.Interfaces;
using gRPCStressTestingService.Interfaces.Services;
using Azure.Core.Pipeline;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DbManagerWorkerService.Interfaces.DataContext;
using ConfigurationStuff.DbContexts;

namespace gRPCStressTestingService.Services
{
    public class ClientInstanceService : IClientInstanceService
    { 
        private readonly ISessionRepo _sessionRepo; 
        private readonly IClientInstanceRepo _clientInstanceRepo;
        private readonly IAccountRepo _accountRepo;
        private readonly DataContexts _dataContext;

        public ClientInstanceService(ISessionRepo sessionRepo, IClientInstanceRepo clientInstanceRepo, IAccountRepo accountRepo, DataContexts context)
        {
            _sessionRepo = sessionRepo;
            _clientInstanceRepo = clientInstanceRepo;
            _accountRepo = accountRepo;
            _dataContext = context;

            Console.WriteLine($"ClientInstanceService is using SessionRepo instance: {_sessionRepo.GetHashCode()}");
            Console.WriteLine($"ClientInstanceService is using ClientInstanceRepo instance: {_clientInstanceRepo.GetHashCode()}");
        }

        public async Task<CreateClientInstanceResponse> CreateClientInstance(CreateClientInstanceRequest request, ServerCallContext context)
        {

            var account = await _accountRepo.GetAccountWithSessionClientInstance(request.Username);

            if (account == null || account.Session == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Account or Session not found."));
            }

            if (account.Session.ClientInstance.Any(ci => ci.ClientUnique == Guid.Parse(request.SessionUnique)))
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, "Client instance already exists."));
            }

        
            ClientInstance newClientInstance = new ClientInstance
            {
                ClientUnique = Guid.NewGuid(),
                SessionUnique = account.Session.SessionUnique
            };

            account.Session.ClientInstance.Add(newClientInstance);

           
            await _sessionRepo.SaveAsync();

            return new CreateClientInstanceResponse
            {
                ClientUnique = newClientInstance.ClientUnique.ToString(),
                SessionUnique = account.Session.SessionUnique.ToString(),
                TimeOfClientCreation= DateTime.Now.ToString()
            };

        }
        
        //this is for what happens when there are no current client instances 
        private async Task UpdatingClientInstance(string sessionUnique, ClientInstance clientInstance, Session session)
        {

            try
            {
                
                if (session.ClientInstance.Count == 0)
                {
                    session.ClientInstance.Add(clientInstance);
                }

                await _sessionRepo.UpdateDbAsync(session);
            } 
            catch(DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var currentValues = entry.CurrentValues;
                var databaseValues = entry.GetDatabaseValues();

                if(databaseValues != null)
                {
                    currentValues.SetValues(databaseValues);

                    await _sessionRepo.UpdateDbAsync(session);
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.Aborted, "Session has been deleted or updated by another process."));
                }
            }

             
        }

        public async Task<StreamClientInstanceResponse> StreamClientInstances(IAsyncStreamReader<StreamClientInstanceRequest> requestStream, ServerCallContext context)
        {

            List<ClientInstance> clientInstancesList = new List<ClientInstance>();

            string? sessionUnique = context.RequestHeaders.GetValue("session-unique");

            if(sessionUnique == null)
            {
                Log.Warning($"No session unique was passed in the metadata when creating client instances");
            }

            if(requestStream.ReadAllAsync() == null)
            {
                Log.Warning($"Nothing received in the request stream on the server side");
            }

            StreamClientInstanceResponse serverResponse = new StreamClientInstanceResponse
            {
                State = true
            }; 

            await foreach(var message in requestStream.ReadAllAsync())
            {

                ClientInstance clientInstance = new ClientInstance
                {
                    SessionUnique = Guid.Parse(sessionUnique),
                    ClientUnique = Guid.Parse(message.ClientUnique),
                    DelayCalcs = null
                };

                Log.Information($"Received message with GUID {message.ClientUnique}");

                clientInstancesList.Add(clientInstance);
            }

            await _clientInstanceRepo.AddBatchToDbAsync(clientInstancesList);

            await _clientInstanceRepo.SaveAsync();

            return serverResponse;
        }

        public async Task<ClearClientInstancesResponse> StreamClientClears(IAsyncStreamReader<ClearClientInstancesRequest> requestStream, ServerCallContext context)
        {
            List<ClientInstance> clientInstancesToRemove = new List<ClientInstance>();

            string? sessionUnique = context.RequestHeaders.GetValue("session-unique");

            if(sessionUnique == null)
            {
                Log.Warning($"Session unique is null on the stream client clears");
            }

            ClearClientInstancesResponse serverResponse = new ClearClientInstancesResponse
            {
                SessionUnique = sessionUnique,
                InstancesCleared = 0,
                ResponseState = true
            };

            await foreach (ClearClientInstancesRequest instanceClear in requestStream.ReadAllAsync())
            {

                ClientInstance newClientInstance = new ClientInstance
                {
                    ClientUnique = Guid.Empty,
                    DelayCalcs = null,
                    SessionUnique = null
                };

                serverResponse.InstancesCleared += 1;

                clientInstancesToRemove.Add(newClientInstance);

            }

            await _clientInstanceRepo.RemoveRange(clientInstancesToRemove);

            await _clientInstanceRepo.SaveAsync();

            return serverResponse;

        }

        public async Task GetClientInstances(GetClientInstancesFromSessionUniqueRequest request, IServerStreamWriter<GetClientInstancesFromSessionUniqueResponse> responseStream, ServerCallContext context)
        {

            try
            {
                List<ClientInstance> clientInstances = await _clientInstanceRepo.GetClientInstancesViaSessionId(Guid.Parse(request.SessionUnique));

                if (clientInstances.Count == 0)
                {
                    Log.Warning($"No client instances can be found");
                }

                List<ClientInstance> clientsToDelete = new List<ClientInstance>();

                foreach (ClientInstance client in clientInstances)
                {
                    GetClientInstancesFromSessionUniqueResponse newResponse = new GetClientInstancesFromSessionUniqueResponse
                    {
                        SessionUnique = client.SessionUnique.ToString(),
                        ClientUnique = client.ClientUnique.ToString(),
                    };

                    Log.Information($"Found client instance {client.ClientUnique}");

                    await responseStream.WriteAsync(newResponse);

                    clientsToDelete.Add(client);

                }

                await _clientInstanceRepo.RemoveRangeAsync(clientsToDelete);

                await _clientInstanceRepo.SaveAsync();
            } 
            catch(DbUpdateConcurrencyException ex)
            {
                Log.Error($"Concurrency thing happened again {ex.Message}");
            }

            
        }

        public async Task<GetClientInstancesFromSessionUniqueResponse> GetClientInstancesUnary(GetClientInstancesFromSessionUniqueRequest request, ServerCallContext context)
        {
            List<ClientInstance> clientInstances = await _clientInstanceRepo.GetClientInstancesViaSessionId(Guid.Parse(request.SessionUnique));

            if (clientInstances.Count == 0)
            {
                Log.Warning($"No client instances can be found");
            }

            await _clientInstanceRepo.RemoveRange(clientInstances);

            await _clientInstanceRepo.SaveAsync();

            GetClientInstancesFromSessionUniqueResponse serverResponse = new GetClientInstancesFromSessionUniqueResponse
            {
                SessionUnique = request.SessionUnique
            };

            return serverResponse;
        }

    }
}
