using gRPCToolFrontEnd.Interfaces;
using gRPCToolFrontEnd.Services;
using Serilog;
using System.Runtime.CompilerServices;

namespace gRPCToolFrontEnd.Helpers
{
    public class RequestTypeHandlingFactory : IRequestTypeHandlingFactory
    {
        private readonly StreamingLatencyService _streamingLatencyService;

        private readonly UnaryRequestService _unaryService;
        public RequestTypeHandlingFactory(StreamingLatencyService streamingLatencyService, UnaryRequestService unaryService)
        {
            _streamingLatencyService = streamingLatencyService;
            _unaryService = unaryService;
        }

        public IRequestReceiver RequestReceiver(bool isStreaming, string dropDownState, bool unaryOrBatch, Guid? channelUnique, string clientUnique, string fileSize, 
            int iterationsInbatch, int amountOfRequests)
        {

            Log.Information("Factory has been reached");

            if (string.IsNullOrEmpty(dropDownState))
            {
                Log.Warning("dropDownState parameter is null or empty.");
            }

            if (!channelUnique.HasValue)
            {
                Log.Warning("channelUnique parameter is null.");
            }

            if (string.IsNullOrEmpty(clientUnique))
            {
                Log.Warning("clientUnique parameter is null or empty.");
            }

            if (string.IsNullOrEmpty(fileSize))
            {
                Log.Warning("fileSize parameter is null or empty.");
            }

            if (!isStreaming)
            {

                return dropDownState switch
                {
                    "one : one" => new OneToOneUnaryHandler(_unaryService, unaryOrBatch, channelUnique, fileSize, clientUnique, iterationsInbatch),
                    "one : many" => new OneToManyUnaryHandler(_unaryService, unaryOrBatch, channelUnique, fileSize, iterationsInbatch),
                    "many : many" => new ManyToManyUnaryHandler(_unaryService, unaryOrBatch, fileSize, iterationsInbatch),
                    "many : one" => new ManyToOneUnaryHandler(_unaryService, unaryOrBatch, channelUnique, fileSize, iterationsInbatch)
                };

            }
            else
            {

                return dropDownState switch
                {
                    "one : one" => new OneToOneStreamingHandler(_streamingLatencyService, unaryOrBatch, channelUnique, fileSize, iterationsInbatch),
                    "one : many" => new OneToManyStreamingHandler(_streamingLatencyService, unaryOrBatch, iterationsInbatch, fileSize,  channelUnique, amountOfRequests),
                    "many : many" => new ManyToManyStreamingHandler(_streamingLatencyService, unaryOrBatch, amountOfRequests, fileSize),
                    "many : one" => new ManyToOneStreamingHandler(_streamingLatencyService, unaryOrBatch, channelUnique, amountOfRequests, fileSize),
                };

            }

           
        }

     
    }
}
