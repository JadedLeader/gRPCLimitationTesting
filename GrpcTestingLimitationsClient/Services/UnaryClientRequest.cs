using GrpcTestingLimitationsClient.proto;
using SharedCommonalities.Interfaces.TimeStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCommonalities.TimeStorage;
using Grpc.Core;
using Grpc.Net.Client;
using System.Net;
using SharedCommonalities.UsefulFeatures;
using System.Net.Http.Headers;
using GrpcTestingLimitationsClient.Interfaces;

namespace GrpcTestingLimitationsClient.Services
{
    public class UnaryClientRequest
    {

        private readonly IClientHelper _clientHelper;
        public UnaryClientRequest(IClientHelper helper)
        {
            _clientHelper = helper;
        }

        //multiple clients multiple sequential requests
        public async Task MultipleClientsUnaryRequest(GrpcChannel channel, int instances, string fileSize, int amountOfClients)
        {

            var listOfClients = await _clientHelper.CreatingClients(channel, amountOfClients);

            foreach (var client in listOfClients)
            {
                string clientIdentifier = Guid.NewGuid().ToString();

                await ClientUnaryRequestBatch(channel, instances, "small");
            }
        }

        //single request
        public async Task ClientUnaryRequest(GrpcChannel channel, string fileSize)
        {

            var freshClient = _clientHelper.CreatingSingularClient(channel);

            string clientIdentifier = Guid.NewGuid().ToString();    

            await GeneratingRequest(freshClient, fileSize , clientIdentifier);
        }

        private async Task GeneratingRequest(Unary.UnaryClient client, string fileSize, string clientIdentifier)
        {
            string filePath = FileSize(fileSize);

            var content = File.ReadAllText(filePath);

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            string guid = Guid.NewGuid().ToString();

            var metaData = new Metadata();

            var amountOfChannelsOpen = Settings.GetNumberOfActiveChannels().ToString();
            var amountOfActiveClients = Settings.GetNumberOfActiveClients().ToString();

            metaData.Add("request-id", guid);
            metaData.Add("timestamp", preciseTime);
            metaData.Add("request-type", "Unary");
            metaData.Add("open-channels", amountOfChannelsOpen);
            metaData.Add("active-clients", amountOfActiveClients);
            metaData.Add("client-identifier", clientIdentifier);   


            var reply = await client.UnaryResponseAsync(

                new DataRequest()
                {
                    RequestId = guid,
                    RequestType = "Unary",
                    RequestTimestamp = preciseTime,
                    ConnectionAlive = true,
                    DataSize = content
                }, headers: metaData);


            Console.WriteLine($"{content.Length}");

            Console.WriteLine($"This is the server response");
            Console.WriteLine($"{reply.RequestId} : {reply.ResponseTimestamp}");
        }

        //same client sending multiple requests sequentially
        public async Task ClientUnaryRequestBatch(GrpcChannel channel, int? instances, string fileSize)
        {
            var freshClient = _clientHelper.CreatingSingularClient(channel);

            string clientIdentifier = Guid.NewGuid().ToString();

            int i = 0;
            while (instances >= i)
            {
                await GeneratingRequest(freshClient, fileSize, clientIdentifier);

                Console.WriteLine($"{i}");
                i++;
            }

            Console.WriteLine($"client request batch is finished, releasing client");

            var numberOfActiveClients = Settings.DecrementActiveClients();

            Console.WriteLine($"Number of active clients now -> {numberOfActiveClients}");


        }

        private string FileSize(string fileSize)
        {

            string fileReturn = "";

            switch (fileSize)
            {
                case "small":
                    fileReturn = "C:\\Users\\joshy\\source\\repos\\gRPCLimitationTesting\\GrpcTestingLimitationsClient\\DataSizes\\text_1MB.txt";
                    break;
                case "medium":
                    fileReturn = "C:\\Users\\joshy\\source\\repos\\gRPCLimitationTesting\\GrpcTestingLimitationsClient\\DataSizes\\text_30MB.txt";
                    break;
                case "large":
                    fileReturn = "C:\\Users\\joshy\\source\\repos\\gRPCLimitationTesting\\GrpcTestingLimitationsClient\\DataSizes\\text_100MB.txt";
                    break;
            }

            return fileReturn;
        }

        

    }
}
