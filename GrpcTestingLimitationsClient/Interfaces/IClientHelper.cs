using Grpc.Net.Client;
using GrpcTestingLimitationsClient.proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrpcTestingLimitationsClient.Interfaces
{
    public interface IClientHelper
    {

        public Task<List<Unary.UnaryClient>> CreatingClients(GrpcChannel channel, int amountOfClients);

        public Task<List<GrpcChannel>> GeneratingMutlipleChannels(int amountOfChannels);

        public Unary.UnaryClient CreatingSingularClient(GrpcChannel channel);

        public string FileSize(string fileSize);

        public string DataContentCalc(string fileSize);


    }
}
