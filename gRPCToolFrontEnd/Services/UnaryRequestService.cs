using Grpc.Core;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace gRPCToolFrontEnd.Services
{
    public class UnaryRequestService
    {
        private readonly Unary.UnaryClient _unaryClient;
        public UnaryRequestService(Unary.UnaryClient unaryClient)
        {
            _unaryClient = unaryClient;
        }


        public async Task<DataResponse> UnaryResponseAsync(DataRequest dataRequest)
        {
            Metadata metaData = new Metadata();

            metaData.Add("request-type", "Unary");
            metaData.Add("open-channels", 0.ToString());
            metaData.Add("active-clients", 0.ToString());
            metaData.Add("data-iterations", "1");

           return await _unaryClient.UnaryResponseAsync(dataRequest, metaData);
        }

        public async Task<BatchDataResponse> UnaryBatchResponseAsync(BatchDataRequest batchDataRequest, int batchIterations)
        {
            Metadata metaData = new Metadata();

            string batchRequestId = Guid.NewGuid().ToString();

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            metaData.Add("batch-iteration", batchIterations.ToString());
            metaData.Add("batch-request-id", batchRequestId);
            metaData.Add("batch-request-timestamp", preciseTime);
            metaData.Add("request-type", "BatchUnary");
            metaData.Add("batch-request-count", batchDataRequest.BatchDataRequest_.Count.ToString());
            metaData.Add("active-clients", "0");

            return await _unaryClient.BatchUnaryResponseAsync(batchDataRequest, metaData);
        }

    }
}
