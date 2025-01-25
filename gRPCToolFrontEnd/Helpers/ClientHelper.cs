using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using gRPCToolFrontEnd;
using Grpc.Net.Client.Balancer;
using Serilog;
using Blazored.LocalStorage;
using Microsoft.EntityFrameworkCore.Metadata;
using gRPCToolFrontEnd.LocalStorage;

namespace gRPCToolFrontEnd.Helpers
{
    public class ClientHelper 
    {

        /// <summary>
        /// Simple class that is going to be utilised by the other services within the client 
        /// Contains general functionality
        /// </summary>
        private readonly ILocalStorageService _localStorageService;

        private readonly PayloadUsageStore _payloadUsageStorage;
        public ClientHelper(ILocalStorageService LocalStorageService, PayloadUsageStore payloadUsageStore)
        {
            _localStorageService = LocalStorageService;
            _payloadUsageStorage = payloadUsageStore;
        }

        /// <summary>
        /// Generates multiple channels by taking in a parameter for how many channels you want created
        /// Adds all the channels to a list
        /// Incrementation of the active channels from the client side 
        /// </summary>
        /// <param name="amountOfChannels"></param>
        /// <returns>A list of channels that were created</returns>
        public async Task<Dictionary<Guid, GrpcChannel>> GeneratingMutlipleChannels(int amountOfChannels, string forAddress)
        {
            int i = 0;

            Dictionary<Guid, GrpcChannel> channels = new Dictionary<Guid, GrpcChannel>();
           
            while(amountOfChannels > i)
            {
                var newChannel = GrpcChannel.ForAddress(forAddress,  new GrpcChannelOptions
                {
                    MaxSendMessageSize = 100 * 1024 * 1024, 
                    MaxReceiveMessageSize = 100 * 1024 * 1024,
                });

                Guid channelUnique = Guid.NewGuid();

                channels.Add(channelUnique, newChannel);

                Log.Information($"Channel created with ID {channelUnique}");

                i++;
            }

         
            return channels;
        }

        /// <summary>
        /// Creates a single client for unary requests
        /// </summary>
        /// <param name="channel"></param>
        /// <returns>The client object that was created</returns>
        public Unary.UnaryClient CreatingSingularClient(GrpcChannel channel)
        {
            var freshClient = new Unary.UnaryClient(channel);

            //Settings.IncrementActiveClients();

            return freshClient;
        }


        /// <summary>
        /// Used for creating multiple clients, how many clients are created is determined by the int parameter
        /// All clients generated get added to the clients list
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="amountOfClients"></param>
        /// <returns>a list of unary clients</returns>
        public async Task<List<Unary.UnaryClient>> CreatingClients(GrpcChannel channel, int amountOfClients)
        {
            Console.WriteLine($"generating clients... ");

            int i = 0;

            List<Unary.UnaryClient> clients = new List<Unary.UnaryClient>();

            var numberOfClients = 0;

            while (amountOfClients > i)
            {
                
                var client = new Unary.UnaryClient(channel);

                clients.Add(client);

               // numberOfClients = Settings.IncrementActiveClients();

                i++;
            }

            Console.WriteLine($"amount of clients open: {numberOfClients} ");

            return clients;
        }

        /// <summary>
        /// Simple switch case to return the path to the what size file is being used 
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public string FileSize(string fileSize)
        {

            string fileReturn = "";

            switch (fileSize)
            {
                case "small":
                    fileReturn = "C:\\Users\\joshy\\source\\repos\\gRPCLimitationTesting\\gRPCToolFrontEnd\\DataSizes\\text_1MB.txt";
                    break;
                case "medium":
                    fileReturn = "C:\\Users\\joshy\\source\\repos\\gRPCLimitationTesting\\gRPCToolFrontEnd\\DataSizes\\text_30MB.txt";      
                    break;
                case "large":
                    fileReturn = "C:\\Users\\joshy\\source\\repos\\gRPCLimitationTesting\\gRPCToolFrontEnd\\DataSizes\\text_100MB.txt";
                    break;
            }

            return fileReturn;
        }

        public string DataContentCalc(string fileSize)
        {
            string dataAmount = "";

            switch(fileSize)
            {
                case "small":
                    dataAmount = "1MB";
                    break;
                case "medium":
                    dataAmount = "30MB";
                    break;
                case "large":
                    dataAmount = "100MB";
                    break;
            }

            return dataAmount;
        }

        /// <summary>
        /// Allows for the retrieval of a string from the local storage
        /// </summary>
        /// <param name="storagekey">the key identifier of the item in local storage</param>
        /// <returns>The string value of they key specified</returns>
        public async Task<string> GetStringFromStringFromLocalStorage(string storagekey)
        {
            string? localStorageUsername = await _localStorageService.GetItemAsync<string>(storagekey);

            if (string.IsNullOrEmpty(localStorageUsername))
            {
                Log.Information($"key {storagekey} cannot be found in the local storage");
                return "";
            }

            return localStorageUsername;
        }


        public async Task PayloadUsage(string fileSize)
        {
            if (fileSize == "small")
            {

                Log.Information($"Small payload being used");
                _payloadUsageStorage.IncrementSmallPayloadTotal();
            }
            else if (fileSize == "medium")
            {

                Log.Information($"Medium payload being used");
                _payloadUsageStorage.IncrementMediumPayloadTotal();
            }
            else if (fileSize == "large")
            {
                Log.Information($"Large payload being used");
                _payloadUsageStorage.IncrementLargePayloadTotal();
            }
        }


    }
}
