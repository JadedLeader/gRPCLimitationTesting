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
                SessionUnique = account.Session.SessionUnique.ToString(),
                TimeOfClientCreation= DateTime.Now.ToString()
            };

        }
        
        //this is for what happens when there are no current client instances 
        private async Task UpdatingClientInstance(string sessionUnique, ClientInstance clientInstance, Session session)
        {

            try
            {
                //it means there are no client instances
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

        public async Task<CreateClientInstanceResponse> StreamClientInstances(IAsyncStreamReader<StreamClientInstanceRequest> requestStream, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

    }
}
