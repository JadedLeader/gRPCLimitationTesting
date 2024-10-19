using Grpc.Core;
using Grpc.Net.Client;
using GrpcTestingLimitationsClient.Interfaces;
using GrpcTestingLimitationsClient.proto;
using SharedCommonalities.TimeStorage;
using SharedCommonalities.UsefulFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcTestingLimitationsClient.Services
{
    public class UnaryClientRequestBatch
    {
        private readonly IClientHelper _clientHelper;
        public UnaryClientRequestBatch(IClientHelper helper)
        {
            _clientHelper = helper;
        }

        public async Task MutlipleClientsRequestBatch(GrpcChannel channel, string fileSize, int amountOfClients, int amountOfRequests)
        {
            var clientGeneration = await _clientHelper.CreatingClients(channel,amountOfClients);

            foreach (var client in clientGeneration)
            {
                await RequestBatchAsync(channel, fileSize, amountOfRequests, client);
            }

            Console.WriteLine($"client request batch : {channel.State} ");
        }

        public async Task RequestBatchSingleClient(GrpcChannel channel, string fileSize, int amountToSend)
        {

            var freshClient = _clientHelper.CreatingSingularClient(channel);

            await SendingBatchOfRequests(amountToSend, freshClient);
        }

        public async Task RequestBatchAsync(GrpcChannel channel, string fileSize, int amountOfRequests, Unary.UnaryClient freshClient)
        {
            await SendingBatchOfRequests(amountOfRequests, freshClient);

        }

        private async Task SendingBatchOfRequests(int amountOfRequests, Unary.UnaryClient freshClient)
        {
            List<DataRequest> requests = GeneratingBatchOfRequests(amountOfRequests);

            var firstRequest = requests[0];

            string metaDataId = firstRequest.RequestId;
            string metaDataTimestamp = firstRequest.RequestTimestamp;
            string requestTypeMetaData = firstRequest.RequestType;

            var metaData = new Metadata();

            metaData.Add("batch-request-id", metaDataId);
            metaData.Add("batch-request-timestamp", metaDataTimestamp);
            metaData.Add("batch-request-count", requests.Count.ToString());
            metaData.Add("request-type", requestTypeMetaData);
            metaData.Add("active-clients", Settings.GetNumberOfActiveClients().ToString());

            var clientRequest = await freshClient.BatchUnaryResponseAsync(new BatchDataRequest
            {
                BatchDataRequest_ = { requests }
            }, headers: metaData);

            Console.WriteLine($"Client count -> client side ---- {Settings.GetNumberOfActiveClients()}");
        }

        private List<DataRequest> GeneratingBatchOfRequests(int amountOfRequests)
        {
            int i = 0;

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            string requestId = Guid.NewGuid().ToString();

            List<DataRequest> requests = new List<DataRequest>();

            while(amountOfRequests > i)
            {
                var newDataRequest = new DataRequest()
                {
                    RequestId = requestId,
                    ConnectionAlive = true,
                    DataSize = "0",
                    RequestTimestamp = preciseTime,
                    RequestType = "UnaryBatch"
                    
                };

                Console.WriteLine($"New request added to the batch request pool {newDataRequest.RequestId} : {newDataRequest.RequestTimestamp}");

                requests.Add(newDataRequest);

                i++;
            }

            return requests;
        }


    }
}
