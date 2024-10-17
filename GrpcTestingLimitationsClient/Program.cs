using Grpc.Net.Client;
using GrpcTestingLimitationsClient.proto;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.TimeStorage;
using GrpcTestingLimitationsClient.Services;
using GrpcTestingLimitationsClient.Interfaces;
using GrpcTestingLimitationsClient.Helpers;

namespace GrpcTestingLimitationsClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            IClientHelper helper = new ClientHelper();

            using var channel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions
            {
                MaxSendMessageSize = 100 * 1024 * 1024, 
                MaxReceiveMessageSize = 100 * 1024 * 1024,  
            });

            var channelList = await helper.GeneratingMutlipleChannels(500);
                
            foreach(var channels in channelList)
            {
                Console.WriteLine($"this is the channels {channels.State}");
            }


            //this is currently only using the same client, we would have to generate these with the requests 
            var client = new Unary.UnaryClient(channel);

            var client2 = new Unary.UnaryClient(channel);

            var client3 = new Unary.UnaryClient(channelList[0]);

            UnaryClientRequest clientRequest = new UnaryClientRequest(helper);
            UnaryClientRequestBatch clientRequestBatch = new UnaryClientRequestBatch(helper);

            //this is a singular request
            //await clientRequest.ClientUnaryRequest(client, "large");

            //multiple instances of multiple clients using the same channel
            //await clientRequest.ClientUnaryRequestBatch(client3, 100, "large");

            //await clientRequest.ClientUnaryRequestBatch(client2, 100, "small");

            //multiple clients unary requests

            //await clientRequest.MultipleClientsUnaryRequest(channel, 10, "large", 15);

            //await clientRequestBatch.RequestBatchAsync(client, "large", 10);

            await clientRequestBatch.MutlipleClientsRequestBatch(channel, "large", 10, 10);

            Console.WriteLine($"hello");

            Console.ReadKey();


        }
    }
}
