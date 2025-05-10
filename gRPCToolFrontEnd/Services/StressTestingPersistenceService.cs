using System.Collections.Concurrent;
using Grpc.Core;
using Grpc.Net.Client;
using gRPCToolFrontEnd.Helpers;
using gRPCToolFrontEnd.LocalStorage;
using Serilog;

namespace gRPCToolFrontEnd.Services;

public class StressTestingPersistenceService
{
    
    private readonly ClientHelper _clientHelper;
    private readonly GlobalSettings _globalSettings;

    public List<SaveSessionPointRequest> CollatedDelays = new();
    
    public event Action<StreamSessionRunResponse> OnSessionRunResponse;
    
    public StressTestingPersistenceService(ClientHelper clientHelper, GlobalSettings globalSettings)
    {
        _clientHelper = clientHelper;
        _globalSettings = globalSettings;
    }

    public async Task DelayPersistenceTransporation()
    {
        
        var newChannel = GrpcChannel.ForAddress(_globalSettings.CurrentLocalHost,  new GrpcChannelOptions
        {
            MaxSendMessageSize = 100 * 1024 * 1024, 
            MaxReceiveMessageSize = 100 * 1024 * 1024,
        });
        
        StressTestingPersistence.StressTestingPersistenceClient newStressTestingClient = new StressTestingPersistence.StressTestingPersistenceClient(newChannel);
        
        var call = newStressTestingClient.SaveSession();

        foreach (var delay in CollatedDelays)
        {
            Log.Information($"SENDING DELAY VALUE {delay} to the backend of type {delay.TestType}");
            
            await call.RequestStream.WriteAsync(delay);
        }

        await call.RequestStream.CompleteAsync();

    }

    public async Task CollateAllDelayTypes(TestType testType, List<double> delayValues, string presetName, StressLevel stressLevel, ClientType clientType)
    {
        string sessionUnique = await _clientHelper.GetStringFromStringFromLocalStorage("session-unique");
        
        foreach (var delay in delayValues)
        {
            SaveSessionPointRequest newSavePoint = new SaveSessionPointRequest()
            {
                SessionUnique = sessionUnique,
                PresetName = presetName,
                TestType = testType,
                LatencyValue = delay,
                StressLevel = stressLevel,
                ClientType = clientType
            }; 
            
            CollatedDelays.Add(newSavePoint);
        }
    }

    public void StartReceivingSessionRuns()
    {
        Task.Run(() => ReceivingSessionRunPresetNames());
    }

    private async Task ReceivingSessionRunPresetNames()
    {
        var newChannel = GrpcChannel.ForAddress(_globalSettings.CurrentLocalHost,  new GrpcChannelOptions
        {
            MaxSendMessageSize = 100 * 1024 * 1024, 
            MaxReceiveMessageSize = 100 * 1024 * 1024,
        });
        
        CancellationToken newCalCancellationToken = new CancellationToken();

        StressTestingPersistence.StressTestingPersistenceClient newClient =
            new StressTestingPersistence.StressTestingPersistenceClient(newChannel);
        
        string sessionUnique = await _clientHelper.GetStringFromStringFromLocalStorage("session-unique");

        StreamSessionRunRequest newRunRequest = new StreamSessionRunRequest()
        {
            SessionUnique = sessionUnique,
        };
        
        var call = newClient.StreamSessionRuns(newRunRequest); 
        
        
        while(await call.ResponseStream.MoveNext())
        {
            var response = call.ResponseStream.Current;

            Log.Information($"Received session run {response.PresetName} from the backend");
            
            OnSessionRunResponse?.Invoke(response);
        }
    }

    
}