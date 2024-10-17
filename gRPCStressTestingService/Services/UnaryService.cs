using Grpc.Core;
using gRPCStressTestingService;
using gRPCStressTestingService.Interfaces;
using gRPCStressTestingService.proto;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.ReturnModels;
using SharedCommonalities.TimeStorage;
using System.Data;

namespace gRPCStressTestingService.Services
{
    public class UnaryService  : IUnaryService
    {

        
        public UnaryService()
        {
            
        }


        public async Task<DataResponse> UnaryResponse(DataRequest request, ServerCallContext context)
        {

            var now = DateTime.UtcNow;         
            long ticks = now.Ticks;             
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            var dataReturn = new DataResponse()
            {
                ConnectionAlive = false,
                RequestId = request.RequestId,
                RequestType = request.RequestType,
                ResponseTimestamp = preciseTime
            };

            string? guidFromMetaData = context.RequestHeaders.GetValue("request-id");
            string? responseTimeFromMetaData = context.RequestHeaders.GetValue("timestamp");
            string? typeOfDataFromMetaData = context.RequestHeaders.GetValue("request-type");

            if(guidFromMetaData == string.Empty || responseTimeFromMetaData == string.Empty)
            {
                Console.WriteLine($"the guid or the response time from the meta data was null here, please check again");
            }

            Console.WriteLine($"meta data: {guidFromMetaData} : {responseTimeFromMetaData}");

            var requestUnaryInfo = new UnaryInfo()
            {
                Delay = null, 
                LengthOfData = LengthOfRequest(request), 
                TimeOfRequest = Convert.ToDateTime(responseTimeFromMetaData),
                TypeOfData = typeOfDataFromMetaData,
            };

            var responseUnaryInfo = new UnaryInfo()
            {
                Delay = null,
                LengthOfData = LengthOfRequest(request),
                TypeOfData = typeOfDataFromMetaData,
                TimeOfRequest = Convert.ToDateTime(dataReturn.ResponseTimestamp)
            };

            RequestResponseTimeStorage.AddRequestToList(guidFromMetaData, requestUnaryInfo);

            RequestResponseTimeStorage.AddResponseToList(dataReturn.RequestId, responseUnaryInfo);

            Console.WriteLine($"This is the client request");
            Console.WriteLine($"{dataReturn.RequestId} : {request.RequestTimestamp}");

            var numOfResponses = RequestResponseTimeStorage.ReturnServerResponse();

            return dataReturn;
               
        }

        public async Task<BatchDataResponse> BatchUnaryResponse(BatchDataRequest request, ServerCallContext context)
        {

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            var batchIdFromMetaData = context.RequestHeaders.GetValue("batch-request-id");
            var batchTimestampFromMetaData = context.RequestHeaders.GetValue("batch-request-timestamp");
            var typeOfDataFromMetaData = context.RequestHeaders.GetValue("request-type");
            var batchFromMetaData = Convert.ToInt32(context.RequestHeaders.GetValue("batch-request-count"));
            

            var batchDataResponse = new BatchDataResponse()
            {
                BatchResponseId = batchIdFromMetaData,
                NumberOfRequestsInBatch = batchFromMetaData,
                ResponseTimestamp = preciseTime,
                RequestType = typeOfDataFromMetaData,
                
            };

            var responseUnaryInfo = new UnaryInfo()
            {
                Delay = null,
                LengthOfData = batchDataResponse.NumberOfRequestsInBatch,
                TimeOfRequest = Convert.ToDateTime(batchDataResponse.ResponseTimestamp),
                TypeOfData = typeOfDataFromMetaData,
                
            };

            var requestUnaryInfo = new UnaryInfo()
            {
                Delay = null,
                LengthOfData = batchFromMetaData,
                TimeOfRequest = Convert.ToDateTime(batchTimestampFromMetaData),
                TypeOfData = typeOfDataFromMetaData

            };

            RequestResponseTimeStorage.AddRequestToList(batchIdFromMetaData, requestUnaryInfo);

            RequestResponseTimeStorage.AddResponseToList(batchDataResponse.BatchResponseId, responseUnaryInfo);


            Console.WriteLine($"this is the client request");
            Console.WriteLine($"{batchDataResponse.BatchResponseId} : {batchTimestampFromMetaData}");

            return batchDataResponse;
        }

        private int LengthOfRequest(DataRequest dataRequest)
        {

            if(dataRequest.DataSize == string.Empty)
            {
                Console.WriteLine($"There was no string data passed along with this request");
            }

            return dataRequest.DataSize.Length;
        }

        

    }
}
