using Grpc.Net.Client;
using GrpcTestingLimitationsClient.proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCommonalities.UsefulFeatures;
using GrpcTestingLimitationsClient.Interfaces;

namespace GrpcTestingLimitationsClient.Helpers
{
    public class ClientHelper : IClientHelper
    {


        public ClientHelper()
        {
            
        }

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

    }
}
