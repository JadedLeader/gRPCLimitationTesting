using Grpc.Net.Client;
using gRPCToolFrontEnd.LocalStorage;
using MudBlazor;
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
            ClientInstanceService clientInstanceService)
        {
            _accountDetailsStore = accountDetailsStore;
            _streamingLatencyService = streamingLatencyService;
            _unaryRequestService = unaryRequestService;
            _clientInstanceService = clientInstanceService;
        }


        public async Task StreamingClientToChannelAllocation(int ammountOfClientsPerChannel, int amountOfRequests, string fileSize)
        {
            var channelsCurrentlyAvailable = GetCurrentAvailableChannels();

            CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

            foreach(var channel in channelsCurrentlyAvailable)
            {
                int i = 0;

                while (i < ammountOfClientsPerChannel)
                {
                    StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(channel.Value);

                    await _streamingLatencyService.GeneratingeManySingleStreamingRequests(streamingClient, amountOfRequests, newlyCreatedClient.ClientUnique, fileSize);

                    _clientStorage.IncrementStreamingClients();

                    i++;
                }
            }

        }

        public async Task StreamingBatchClientToChannelAllocation(int amountOfClientsPerChannel, int amountOfRequests, string fileSize)
        {
            var channelsCurrentlyAvailable = GetCurrentAvailableChannels();

            CreateClientInstanceResponse newlyCreatedClient = await _clientInstanceService.CreateClientInstanceAsync();

            foreach(var channel in channelsCurrentlyAvailable)
            {
                int i = 0;

                while(i < amountOfClientsPerChannel)
                {
                    StreamingLatency.StreamingLatencyClient streamingClient = new StreamingLatency.StreamingLatencyClient(channel.Value);

                    await _streamingLatencyService.GeneratingSingularBatchStreamingRequest(streamingClient, 1, newlyCreatedClient.ClientUnique, fileSize);

                    _clientStorage.IncrementStreamingClients();

                    i++;
                }
            }
        }

        public async Task UnaryClientToChannelAllocation(int amountOfUnaryClientsPerChannel, string fileSize)
        {
            var channelsCurrentlyAvailable = GetCurrentAvailableChannels();

            foreach(var channel in channelsCurrentlyAvailable)
            {
                int i = 0;

                while(i < amountOfUnaryClientsPerChannel)
                {
                    Unary.UnaryClient unaryClient = new Unary.UnaryClient(channel.Value);

                    await _unaryRequestService.UnaryBatchIterativeAsync(null, 1, fileSize);

                    i++;
                }
            }
        }

        public async Task UnaryBatchClientToChannelAllocation(int amountOfUnaryClientsPerChannel, string fileSize)
        {
            var channelsCurrentlyAvailable = GetCurrentAvailableChannels();

            foreach(var channel in channelsCurrentlyAvailable)
            {
                int i = 0;

                while(i <  amountOfUnaryClientsPerChannel)
                {
                    Unary.UnaryClient unaryClient = new Unary.UnaryClient(channel.Value);

                    await _unaryRequestService.UnaryBatchResponseAsync(1, channel.Key, fileSize);

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
