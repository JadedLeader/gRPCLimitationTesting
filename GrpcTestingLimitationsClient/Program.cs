﻿global using GrpcTestingLimitationsClient.proto;
using Grpc.Net.Client;
using SharedCommonalities.Interfaces.TimeStorage;
using SharedCommonalities.TimeStorage;
using GrpcTestingLimitationsClient.Services;
using GrpcTestingLimitationsClient.Interfaces;
using GrpcTestingLimitationsClient.Helpers;
using SharedCommonalities;

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

            //this is currently only using the same client, we would have to generate these with the requests 
            var client = new Unary.UnaryClient(channel);

            var client2 = new Unary.UnaryClient(channel);

           // var client3 = new Unary.UnaryClient(channelList[0]);

            UnaryClientRequest clientRequest = new UnaryClientRequest(helper);
            UnaryClientRequestBatch clientRequestBatch = new UnaryClientRequestBatch(helper);

            //this is a singular request

            //await clientRequest.ClientUnaryRequest(channel, "small");
            //await clientRequest.ClientUnaryRequest(channel, "large");
            //await clientRequest.ClientUnaryRequest(channel, "large");

            //one client with multiple sequential messages
            //await clientRequest.ClientUnaryRequestBatch(channel, 15, "small");

            //await clientRequest.ClientUnaryRequestBatch(client2, 100, "small");

            //multiple clients unary requests

            //await clientRequest.MultipleClientsUnaryRequest(channel, 10, "large", 15);

            //await clientRequestBatch.RequestBatchAsync(channel, "large", 10);

            //await clientRequestBatch.RequestBatchSingleClient(channel, "small", 10);

            //await clientRequestBatch.MutlipleClientsRequestBatch(channel, "small", 2, 7);
            //await clientRequestBatch.MutlipleClientsRequestBatch(channel, "medium", 2, 5);

            // await clientRequest.ClientUnaryRequest(channel, "medium");
            //await clientRequest.ClientUnaryRequest(channel, "medium"); 
            Admin.AdminClient adminclient = new Admin.AdminClient(channel);

            AdminService adminservice = new AdminService();

            //await adminservice.ResettingDatabase(adminclient);

           // await adminservice.GetAccountViaId( adminclient, "E3CC23A7-F35A-4D0D-886C-56DE2828A0D1");

            Accounts.AccountsClient accountClient = new Accounts.AccountsClient(channel);

            AccountService account = new AccountService();

           // account.CreateAccount(accountClient, "jaded", "123");

           // await account.AccountLogin(accountClient, "jaded", "123");

            Console.WriteLine($"hello");

            Console.ReadKey();


        }
    }
}
