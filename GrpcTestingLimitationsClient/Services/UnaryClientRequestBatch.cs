using Grpc.Core;
using GrpcTestingLimitationsClient.proto;
using SharedCommonalities.TimeStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcTestingLimitationsClient.Services
{
    public class UnaryClientRequestBatch
    {

        public UnaryClientRequestBatch()
        {
            
        }

        public async Task RequestBatchAsync(Unary.UnaryClient client, string fileSize, int amountOfRequests)
        {
            List<DataRequest> requests = GeneratingBatchOfRequests(amountOfRequests);

            var firstRequest = requests[0];

            string metaDataId = firstRequest.RequestId;
            string metaDataTimestamp = firstRequest.RequestTimestamp;

            var metaData = new Metadata();

            metaData.Add("batch-request-id", metaDataId); 
            metaData.Add("batch-request-timestamp", metaDataTimestamp);
            metaData.Add("batch-request-count", requests.Count.ToString());

            var clientRequest = await client.BatchUnaryResponseAsync(new BatchDataRequest
            {
                BatchDataRequest_ = { requests }
            }, headers: metaData); 

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
                    RequestType = "Batch"

                };

                Console.WriteLine($"New request added to the batch request pool {newDataRequest.RequestId} : {newDataRequest.RequestTimestamp}");

                requests.Add(newDataRequest);

                i++;
            }

            return requests;
        }


    }
}
