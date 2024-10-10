using Grpc.Net.Client;
using GrpcTestingLimitationsClient.proto;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.TimeStorage;

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
                

            var client = new Unary.UnaryClient(channel);

            RequestResponseTimeStorage requestResponse = new RequestResponseTimeStorage();

            UnaryClientRequest clientRequest = new UnaryClientRequest(requestResponse);

            //this is a singular request
            //await clientRequest.ClientUnaryRequest(client, "large");

            //multiple instances of multiple clients using the same channel
            await clientRequest.ClientUnaryRequestBatch(client, 4, "large");

            Console.WriteLine($"hello");

            Console.ReadKey();


        }
    }
}
