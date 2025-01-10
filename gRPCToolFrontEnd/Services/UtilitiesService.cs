using Grpc.Core;
using gRPCToolFrontEnd.DictionaryModel;
using gRPCToolFrontEnd.LocalStorage;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace gRPCToolFrontEnd.Services
{
    public class UtilitiesService : IDisposable
    {

  
        private readonly Utilities.UtilitiesClient _utilitiesClient;
        private CancellationTokenSource _cancellationToken;

        public event Action<GetClientsWithMessagesResponse> OnUpdateReceived;

        public UtilitiesService(Utilities.UtilitiesClient utilitiesClient)
        {
            _utilitiesClient = utilitiesClient;
        }

        public void StartReceivingMessages(GetClientsWithMessagesRequest request)
        {
            _cancellationToken = new CancellationTokenSource();

            Task.Run(() => ReceivingMessageStream(request, _cancellationToken.Token));
        }

        public void StopReceivingMessages()
        {
            if(_cancellationToken != null && !_cancellationToken.IsCancellationRequested)
            {
                _cancellationToken.Cancel();
            }
        }

        public async Task ReceivingMessageStream(GetClientsWithMessagesRequest messageRequest, CancellationToken cancellationToken)
        {
            Log.Information("Started receiving messages from gRPC stream.");
            try
            {
                using var call = _utilitiesClient.GetClientsWithMessages(messageRequest);

                while (await call.ResponseStream.MoveNext(cancellationToken))
                {
                    var response = call.ResponseStream.Current;

                    Log.Debug($"Received update for ClientUnique: {response.ClientUnique}");
                    OnUpdateReceived?.Invoke(response);
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while reading the gRPC stream.");
                throw;
            }
        }
        public void Dispose()
        {
            StopReceivingMessages();
            _cancellationToken?.Dispose();
        }

    }

}
    

