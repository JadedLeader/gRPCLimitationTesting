﻿
using Grpc.Core;
using gRPCStressTestingService.proto;
using gRPCStressTestingService.Interfaces;
using gRPCStressTestingService.Interfaces.Services;
using Serilog;

namespace gRPCStressTestingService.Implementations
{
    public class ClientInstanceImplementation : ClientInstances.ClientInstancesBase
    {
        private readonly IClientInstanceService _clientInstanceService;
        public ClientInstanceImplementation(IClientInstanceService clientInstanceService)
        {
            _clientInstanceService = clientInstanceService;
        }

        public override async Task<CreateClientInstanceResponse> CreateClientInstance(CreateClientInstanceRequest request, ServerCallContext context)
        {
            CreateClientInstanceResponse createClientInstance = await _clientInstanceService.CreateClientInstance(request, context);

            if(createClientInstance == null)
            {
                Log.Error($"Something went wrong when creating a client instance");

                throw new RpcException(new Status(StatusCode.Internal, $"Could not create client instance")); 
            }

            return createClientInstance;
        }

        public override async Task<StreamClientInstanceResponse> StreamClientInstances(IAsyncStreamReader<StreamClientInstanceRequest> requestStream, ServerCallContext context)
        {
            StreamClientInstanceResponse streamCreateClientInstance = await _clientInstanceService.StreamClientInstances(requestStream, context);

            if (streamCreateClientInstance == null)
            {
                Log.Error($"Something went wrong when creating a streamed client instance");

                throw new RpcException(new Status(StatusCode.Internal, $"could not create streamed client instances"));
            }

            return streamCreateClientInstance;
        }

        public override async Task<ClearClientInstancesResponse> StreamClientClears(IAsyncStreamReader<ClearClientInstancesRequest> requestStream, ServerCallContext context)
        {
            ClearClientInstancesResponse streamClearClientInstances = await _clientInstanceService.StreamClientClears(requestStream, context);

            if(streamClearClientInstances == null)
            {
                Log.Error($"Something went wrong when clearing client instances");

                throw new RpcException(new Status(StatusCode.Internal, $"Could not stream clear client instances"));
            }

            return streamClearClientInstances;
        }

        public override async Task GetClientInstances(GetClientInstancesFromSessionUniqueRequest request, IServerStreamWriter<GetClientInstancesFromSessionUniqueResponse> responseStream, ServerCallContext context)
        {
             await _clientInstanceService.GetClientInstances(request, responseStream, context);
        }

        public override async Task<GetClientInstancesFromSessionUniqueResponse> GetClientInstancesUnary(GetClientInstancesFromSessionUniqueRequest request, ServerCallContext context)
        {
            GetClientInstancesFromSessionUniqueResponse thing = await _clientInstanceService.GetClientInstancesUnary(request, context);

            if(thing == null)
            {
                Log.Error($"Something went wrong when clearing client instances unary");

                throw new RpcException(new Status(StatusCode.Internal, $"Could not stream clear client instances"));
            }

            return thing;
        }

    }
}
