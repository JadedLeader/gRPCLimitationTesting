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

        /// <summary>
        /// this class is used for the sending of client unary requests
        /// handles everything do to with unary sequential requests
        /// </summary>

        private readonly IClientHelper _clientHelper;
        public UnaryClientRequest(IClientHelper helper)
        {
            _clientHelper = helper;
        }

        //multiple clients multiple sequential requests

        /// <summary>
        /// Creates multiple clients with mutliple sequential requests
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="instances"></param>
        /// <param name="fileSize"></param>
        /// <param name="amountOfClients"></param>
        /// <returns></returns>
        public async Task MultipleClientsUnaryRequest(GrpcChannel channel, int instances, string fileSize, int amountOfClients)
        {

            var listOfClients = await _clientHelper.CreatingClients(channel, amountOfClients);

            foreach (var client in listOfClients)
            {
                string clientIdentifier = Guid.NewGuid().ToString();

                await ClientUnaryRequestBatch(channel, instances, "small");
            }
        }

        /// <summary>
        /// simple single request from a single client
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public async Task ClientUnaryRequest(GrpcChannel channel, string fileSize)
        {

            var freshClient = _clientHelper.CreatingSingularClient(channel);

            string clientIdentifier = Guid.NewGuid().ToString();    

            await GeneratingRequest(freshClient, fileSize , clientIdentifier);
        }

        /// <summary>
        /// determines what file is being used for the requests
        /// alongside ghetting the time, creating metadata and then sending the request 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="fileSize"></param>
        /// <param name="clientIdentifier"></param>
        /// <returns></returns>
        private async Task GeneratingRequest(Unary.UnaryClient client, string fileSize, string clientIdentifier)
        {
            string filePath = _clientHelper.FileSize(fileSize);

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
        /// <summary>
        /// singular client sending multiple requests sequentially
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="instances"></param>
        /// <param name="fileSize"></param>
        /// <returns></returns>
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

        

        

    }
}
