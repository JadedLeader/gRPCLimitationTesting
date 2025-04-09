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


        public async Task StreamingClientToChannelAllocation(int ammountOfClientsPerChannel, int amountOfRequests, string fileSize)
        {
            var channelsCurrentlyAvailable = GetCurrentAvailableChannels();

            CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

            var tasks = new List<Task>();

            foreach(var channel in channelsCurrentlyAvailable)
            {
                int i = 0;

                while (i < ammountOfClientsPerChannel)
                {
                    tasks.Add(Task.Run(async () =>
                    {

                        StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(channel.Value);

                        Log.Information($"Streaming client connected to channel {channel.Key}");

                        await _streamingLatencyService.GeneratingeManySingleStreamingRequests(streamingClient, amountOfRequests, newlyCreatedClient.ClientUnique, fileSize);

                        _clientStorage.IncrementStreamingClients();

                    }));

                    i++;
                    
                }
            }
        }

        public async Task StreamingBatchClientToChannelAllocation(int amountOfClientsPerChannel, int amountOfRequests, string fileSize)
        {
            var channelsCurrentlyAvailable = GetCurrentAvailableChannels();

            CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

            var tasks = new List<Task>();

            foreach (var channel in channelsCurrentlyAvailable)
            {
                int i = 0;


                while(i < amountOfClientsPerChannel)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(channel.Value);

                        await _streamingLatencyService.GeneratingSingularBatchStreamingRequest(streamingClient, amountOfRequests, newlyCreatedClient.ClientUnique, fileSize);

                        _clientStorage.IncrementStreamingClients();
                    }));

                  
                    i++;
                }
            }
        }

        public async Task UnaryClientToChannelAllocation(int amountOfUnaryClientsPerChannel, string fileSize, int amountOfRequests)
        {
            var channelsCurrentlyAvailable = GetCurrentAvailableChannels();

            var tasks = new List<Task>();

            foreach (var channel in channelsCurrentlyAvailable)
            {
                int i = 0;

                while(i < amountOfUnaryClientsPerChannel)
                {

                    tasks.Add(Task.Run(async () =>
                    {
                        Unary.UnaryClient unaryClient = new Unary.UnaryClient(channel.Value);

                        await _unaryRequestService.UnaryResponseIterativeAsync(null, fileSize, amountOfRequests);
                    }));

                    i++;
                }
            }
        }

        public async Task UnaryBatchClientToChannelAllocation(int amountOfUnaryClientsPerChannel, string fileSize, int amountOfRequests)
        {
            var channelsCurrentlyAvailable = GetCurrentAvailableChannels();

            var tasks = new List<Task>();

            foreach (var channel in channelsCurrentlyAvailable)
            {
                int i = 0;

                while(i <  amountOfUnaryClientsPerChannel)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        Unary.UnaryClient unaryClient = new Unary.UnaryClient(channel.Value);

                        await _unaryRequestService.UnaryBatchIterativeAsync(null, amountOfRequests, fileSize);
                    }));

                   
                    i++;
                }
            }

            
        }

        private ConcurrentDictionary<Guid, GrpcChannel> GetCurrentAvailableChannels()
        {
            return _accountDetailsStore.GetChannels();
        }
    }
}
