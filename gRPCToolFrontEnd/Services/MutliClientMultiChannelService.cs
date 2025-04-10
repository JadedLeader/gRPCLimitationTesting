using Grpc.Net.Client;
using gRPCToolFrontEnd.LocalStorage;
using MudBlazor;
using Serilog;
using System.Collections.Concurrent;

namespace gRPCToolFrontEnd.Services
{
    public class MutliClientMultiChannelService
    {

        private readonly AccountDetailsStore _accountDetailsStore;
        private readonly StreamingLatencyService _streamingLatencyService;
        private readonly UnaryRequestService _unaryRequestService;
        private readonly ClientInstanceService _clientInstanceService;
        private readonly ClientStorage _clientStorage;
        

        public MutliClientMultiChannelService(AccountDetailsStore accountDetailsStore, StreamingLatencyService streamingLatencyService, UnaryRequestService unaryRequestService,
            ClientInstanceService clientInstanceService, ClientStorage clientStorage)
        {
            _accountDetailsStore = accountDetailsStore;
            _streamingLatencyService = streamingLatencyService;
            _unaryRequestService = unaryRequestService;
            _clientInstanceService = clientInstanceService;
            _clientStorage = clientStorage;
        }


        public async Task StreamingClientToChannelAllocation(int ammountOfClientsPerChannel, int amountOfRequests, string fileSize, int amountOfChannels)
        {
            

            CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

            var tasks = new List<Task>();

            
            int i = 0;

            while (i < ammountOfClientsPerChannel)
            {
                tasks.Add(Task.Run(async () =>
                {

                    await _streamingLatencyService.CreateManySingleStreamingRequests(null, false, amountOfRequests, fileSize, amountOfChannels);

                    _clientStorage.IncrementStreamingClients();

                }));

                 i++;
                    
            }
            
        }

        public async Task StreamingBatchClientToChannelAllocation(int amountOfClientsPerChannel, int amountOfRequests, string fileSize, int amountOfChannels)
        {
            

            CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

            var tasks = new List<Task>();
            
            int i = 0;

            while(i < amountOfClientsPerChannel)
            {
                tasks.Add(Task.Run(async () =>
                {


                    await _streamingLatencyService.CreateManyStreamingBatchRequest(false, amountOfRequests, fileSize, amountOfChannels);

                    _clientStorage.IncrementStreamingClients();
                }));

                i++;

              
            }
        }

        public async Task UnaryClientToChannelAllocation(int amountOfUnaryClientsPerChannel, string fileSize, int amountOfRequests, int amountOfChannels)
        {
         
            var tasks = new List<Task>();

            
            int i = 0;

            while(i < amountOfUnaryClientsPerChannel)
            {

                 tasks.Add(Task.Run(async () =>
                 {
                        await _unaryRequestService.UnaryResponseIterativeAsync(false,  fileSize, amountOfRequests, amountOfChannels);
                 }));

                 i++;
                
            }
        }

        public async Task UnaryBatchClientToChannelAllocation(int amountOfUnaryClientsPerChannel, string fileSize, int amountOfRequests, int amountOfChannels)
        {
         
            var tasks = new List<Task>();

            
            int i = 0;

            while (i < amountOfUnaryClientsPerChannel)
            {
                tasks.Add(Task.Run(async () =>
                {
                        await _unaryRequestService.UnaryBatchIterativeAsync(false, amountOfRequests, fileSize, amountOfChannels);
                }));

                i++;
            }
                
            
        }

        private ConcurrentDictionary<Guid, GrpcChannel> GetCurrentAvailableChannels()
        {
            return _accountDetailsStore.GetChannels();
        }
    }
}
