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

        /// <summary>
        /// this class handles all the unary batch operations 
        /// this is where the requests are all sent at once, not sequentially
        /// </summary>

        private readonly IClientHelper _clientHelper;
        public UnaryClientRequestBatch(IClientHelper helper)
        {
            _clientHelper = helper;
        }

        /// <summary>
        /// Generates multiple clients and then interates over the list of clients, sending a request batch
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="fileSize"></param>
        /// <param name="amountOfClients"></param>
        /// <param name="amountOfRequests"></param>
        /// <returns></returns>
        public async Task MutlipleClientsRequestBatch(GrpcChannel channel, string fileSize, int amountOfClients, int amountOfRequests)
        {
            var clientGeneration = await _clientHelper.CreatingClients(channel, amountOfClients);

            foreach (var client in clientGeneration)
            {
                string overarchingBatchId = Guid.NewGuid().ToString();

                await SendingBatchOfRequests(amountOfRequests, client, fileSize, overarchingBatchId, amountOfRequests);
            }

            Console.WriteLine($"client request batch : {channel.State} ");
        }

        /// <summary>
        /// Generates a singular client
        /// Sends a single batch of requests 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="fileSize"></param>
        /// <param name="amountToSend"></param>
        /// <returns></returns>
        public async Task RequestBatchSingleClient(GrpcChannel channel, string fileSize, int amountToSend)
        {

            var freshClient = _clientHelper.CreatingSingularClient(channel);

            string overarchingBatchId = Guid.NewGuid().ToString();  

            await SendingBatchOfRequests(amountToSend, freshClient, fileSize, overarchingBatchId, amountToSend);

        }

        private async Task SendingBatchOfRequests(int? amountOfRequests, Unary.UnaryClient freshClient, string fileSize, string overarchingGuid, int iterations)
        {
            List<BatchDataRequestDetails> requests = GeneratingBatchOfRequests(amountOfRequests, fileSize, overarchingGuid, iterations);

            var firstRequest = requests[0];

            string metaDataTimestamp = firstRequest.RequestTimestamp;
            string requestTypeMetaData = firstRequest.RequestType;

            string metaDataRequestId = Guid.NewGuid().ToString();

            var metaData = new Metadata();

            metaData.Add("batch-iterations", iterations.ToString());
            metaData.Add("overarching-id", overarchingGuid);
            metaData.Add("batch-request-id", metaDataRequestId);
            metaData.Add("batch-request-timestamp", metaDataTimestamp);
            metaData.Add("batch-request-count", requests.Count.ToString());
            metaData.Add("request-type", requestTypeMetaData);
            metaData.Add("active-clients", Settings.GetNumberOfActiveClients().ToString());
            metaData.Add("data-size", _clientHelper.FileSize(fileSize));

            var clientRequest = await freshClient.BatchUnaryResponseAsync(new BatchDataRequest
            {
                BatchDataRequest_ = { requests }
            }, headers: metaData);

            Console.WriteLine($"Client count -> client side ---- {Settings.GetNumberOfActiveClients()}");
        }

        private int CheckingMaxRequestLimit(List<DataRequest> requestList)
        {
            int maxRequestLimit = 0;

            foreach(var clientRequest in  requestList)
            {
                maxRequestLimit += Convert.ToInt32(clientRequest.DataSize);
            }

            return maxRequestLimit;
        }

        private List<BatchDataRequestDetails> GeneratingBatchOfRequests(int? amountOfRequests, string fileSize, string overarchingRequestId, int iterations)
        {
            int i = 0;

            string filePath = _clientHelper.FileSize(fileSize);

            var content = File.ReadAllText(filePath);

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            string dataContent = _clientHelper.DataContentCalc(fileSize);

            List<BatchDataRequestDetails> requests = new List<BatchDataRequestDetails>();

            while(amountOfRequests > i)
            {
                string requestId = Guid.NewGuid().ToString();
                var newBatchDataRequests = new BatchDataRequestDetails()
                {
                    OverarchingRequestId = overarchingRequestId,
                    RequestId = requestId,
                    ConnectionAlive = true,
                    DataSize = fileSize,
                    DataContent = dataContent,
                    RequestTimestamp = preciseTime,
                    RequestType = "UnaryBatch"
                };

                Console.WriteLine($"Overarching ID : {newBatchDataRequests.OverarchingRequestId} new message added to pool with ID : {newBatchDataRequests.RequestId}");

                Console.WriteLine($"DATA CONTENT : {newBatchDataRequests.DataContent}");

                requests.Add(newBatchDataRequests);

                i++;
            }

            return requests;
        }

        /* public async Task RequestBatchAsync(GrpcChannel channel, string fileSize, int amountOfRequests, Unary.UnaryClient freshClient)
        {
            await SendingBatchOfRequests(amountOfRequests, freshClient, fileSize);

        } */


    }
}
