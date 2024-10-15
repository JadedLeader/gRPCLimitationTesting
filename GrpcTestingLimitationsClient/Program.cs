using Grpc.Net.Client;
using GrpcTestingLimitationsClient.proto;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.TimeStorage;
using GrpcTestingLimitationsClient.Services;

namespace GrpcTestingLimitationsClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions
            {
                MaxSendMessageSize = 100 * 1024 * 1024, 
                MaxReceiveMessageSize = 100 * 1024 * 1024,  
            });
                

            //this is currently only using the same client, we would have to generate these with the requests 
            var client = new Unary.UnaryClient(channel);

            var client2 = new Unary.UnaryClient(channel);

            UnaryClientRequest clientRequest = new UnaryClientRequest();
            UnaryClientRequestBatch clientRequestBatch = new UnaryClientRequestBatch();

            //this is a singular request
            await clientRequest.ClientUnaryRequest(client, "large");

            //multiple instances of multiple clients using the same channel
            // await clientRequest.ClientUnaryRequestBatch(client, 100, "large");

            //await clientRequest.ClientUnaryRequestBatch(client2, 100, "small");

            //multiple clients unary requests

            await clientRequest.MultipleClientsUnaryRequest(channel, 30, "large", 50);

            //await clientRequestBatch.RequestBatchAsync(client, "large", 10);

            Console.WriteLine($"hello");

            Console.ReadKey();


        }
    }
}
