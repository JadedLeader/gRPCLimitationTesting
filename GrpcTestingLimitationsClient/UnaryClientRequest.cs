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

namespace GrpcTestingLimitationsClient
{
    public class UnaryClientRequest
    {


        public UnaryClientRequest()
        {
            
        }

        private async Task<List<Unary.UnaryClient>> CreatingClients(GrpcChannel channel, int amountOfClients)
        {
            Console.WriteLine($"generating clients... ");

            int i = 0;

            List<Unary.UnaryClient> clients = new List<Unary.UnaryClient>();

            var numberOfClients = 0;

            while (amountOfClients > i)
            {
                var client = new Unary.UnaryClient(channel);

                clients.Add(client);

                numberOfClients = Settings.IncrementActiveClients();

                i++;
            }

            Console.WriteLine($"amount of client channels open: {numberOfClients} ");

            return clients;
        }

        public async Task MultipleClientsUnaryRequest(GrpcChannel channel,int instances, string fileSize, int amountOfClients)
        {
            

            var listOfClients = await CreatingClients(channel, amountOfClients);

            foreach(var client in listOfClients)
            { 
                await ClientUnaryRequestBatch(client, instances, "small");
            }

            
        }

        public async Task ClientUnaryRequest(Unary.UnaryClient client, string fileSize)
        {

            string filePath = FileSize(fileSize);

            var content = File.ReadAllText(filePath);

            var now = DateTime.UtcNow;
            long ticks = now.Ticks;
            string preciseTime = now.ToString("HH:mm:ss.ffffff");

            string guid = Guid.NewGuid().ToString();

            var metaData = new Metadata();

            metaData.Add("request-id", guid);
            metaData.Add("timestamp", preciseTime);

            
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

        public async Task ClientUnaryRequestBatch(Unary.UnaryClient client, int instances, string fileSize)
        {

            int i = 0;
            while(instances >= i)
            {
                await ClientUnaryRequest(client, fileSize);

                Console.WriteLine($"{i}");
                i++;
            }

            Console.WriteLine($"client request batch is finished, releasing client");

            DisposeClient(client);
            
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

        private void DisposeClient(Unary.UnaryClient client)
        {
            client = null;
        }

    }
}
