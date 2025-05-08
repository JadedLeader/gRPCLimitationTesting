using System.Collections.Concurrent;
using Grpc.Net.Client;
using gRPCToolFrontEnd.Helpers;
using gRPCToolFrontEnd.LocalStorage;

namespace gRPCToolFrontEnd.Services;

public class StressTestingPersistenceService
{
    
    private readonly ClientHelper _clientHelper;
    private readonly GlobalSettings _globalSettings;
    
    public StressTestingPersistenceService(ClientHelper clientHelper, GlobalSettings globalSettings)
    {
        _clientHelper = clientHelper;
        _globalSettings = globalSettings;
    }

    protected async Task DelayPersistenceTransporation(string presetName, List<double> delayValues, TestType testType, StressLevel stressLevel, ClientType clientType)
    {
        
        var newChannel = GrpcChannel.ForAddress(_globalSettings.CurrentLocalHost,  new GrpcChannelOptions
        {
            MaxSendMessageSize = 100 * 1024 * 1024, 
            MaxReceiveMessageSize = 100 * 1024 * 1024,
        });
        
        StressTestingPersistence.StressTestingPersistenceClient newStressTestingClient = new StressTestingPersistence.StressTestingPersistenceClient(newChannel);

        string SessionUnique = await _clientHelper.GetStringFromStringFromLocalStorage("session-unique");
        
        var call = newStressTestingClient.SaveSession(); 

        foreach (var delay in delayValues)
        {
            SaveSessionPointRequest newSessionPointRequest = new SaveSessionPointRequest()
            {
                SessionUnique = SessionUnique,
                PresetName = presetName,
                TestType = testType,
                LatencyValue = delay,
                StressLevel = stressLevel,
                ClientType = clientType

            };
            
            await call.RequestStream.WriteAsync(newSessionPointRequest);
            
        }

        await call.RequestStream.CompleteAsync();

    }

    public async Task DelayPersistenceUnarySingle(string presetName, List<double> delayValues, StressLevel stressLevel, ClientType clientType) 
        => await DelayPersistenceTransporation(presetName, delayValues, TestType.UnarySingle, stressLevel, clientType);
    
    public async Task DelayPersistenceUnaryBatch(string presetName, List<double> delayValues, StressLevel stressLevel, ClientType clientType) 
        => await DelayPersistenceTransporation(presetName, delayValues, TestType.UnaryBatch, stressLevel, clientType);
    
    public async Task DelayPersistenceStreamingSingle(string presetName, List<double> delayValues, StressLevel stressLevel, ClientType clientType) 
        => await DelayPersistenceTransporation(presetName, delayValues, TestType.StreamingSingle, stressLevel, clientType);  
    
    public async Task DelayPersistenceStreamingBatch(string presetName, List<double> delayValues, StressLevel stressLevel, ClientType clientType) 
        => await DelayPersistenceTransporation(presetName, delayValues, TestType.StreamingBatch, stressLevel, clientType); 
    
    
    
}