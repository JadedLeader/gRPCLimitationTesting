using Grpc.Net.Client;
using GrpcTestingLimitationsClient.proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCommonalities.UsefulFeatures;
using GrpcTestingLimitationsClient.Interfaces;
using SharedCommonalities.TimeStorage;
using SharedCommonalities.Storage;
using SharedCommonalities.ReturnModels.ReturnTypes;

namespace GrpcTestingLimitationsClient.Helpers
{
    public class ClientHelper : IClientHelper
    {

        /// <summary>
        /// Simple class that is going to be utilised by the other services within the client 
        /// Contains general functionality
        /// </summary>

        public ClientHelper()
        {
            
        }

        /// <summary>
        /// Generates multiple channels by taking in a parameter for how many channels you want created
        /// Adds all the channels to a list
        /// Incrementation of the active channels from the client side 
        /// </summary>
        /// <param name="amountOfChannels"></param>
        /// <returns>A list of channels that were created</returns>
        public async Task<List<GrpcChannel>> GeneratingMutlipleChannels(int amountOfChannels)
        {
            int i = 0;

            List<GrpcChannel> channels = new List<GrpcChannel>();
           
            while(amountOfChannels > i)
            {
                var newChannel = GrpcChannel.ForAddress("http://localhost:5000",  new GrpcChannelOptions
                {
                    MaxSendMessageSize = 100 * 1024 * 1024, 
                    MaxReceiveMessageSize = 100 * 1024 * 1024,
                });

                channels.Add(newChannel);
                

                Settings.IncrementActiveChannels();

                i++;
            }

            Console.WriteLine($"amount of Channels open : {Settings.GetNumberOfActiveChannels()}");

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

            Settings.IncrementActiveClients();

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

                numberOfClients = Settings.IncrementActiveClients();

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
