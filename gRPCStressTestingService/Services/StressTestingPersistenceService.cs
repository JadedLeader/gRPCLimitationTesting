using ConfigurationStuff.DbModels;
using ConfigurationStuff.DTO;
using ConfigurationStuff.Interfaces.Repos;
using Grpc.Core;
using gRPCStressTestingService.Interfaces.Services;
using Serilog;

namespace gRPCStressTestingService.Services;

public class StressTestingPersistenceService : IStressTestingPersistenceService
{
    
    private readonly ISessionRunsRepo _sessionRunsRepo;
    private readonly ILatencyMeasurementsRepo _latencyMeasurementsRepo;
    
    public StressTestingPersistenceService(ISessionRunsRepo sessionRunsRepo, ILatencyMeasurementsRepo latencyMeasurementsRepo)
    {
        _sessionRunsRepo = sessionRunsRepo;
        _latencyMeasurementsRepo = latencyMeasurementsRepo;
    }

    /// <summary>
    /// In charge of saving the entirety of the gRPC testing session and moving the records into the respective database tables
    /// </summary>
    /// <param name="requestStream"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<SaveSessionPointResponse> SaveSession(IAsyncStreamReader<SaveSessionPointRequest> requestStream, ServerCallContext context)
    {
        SaveSessionPointResponse serverResponse = new SaveSessionPointResponse();

        SessionRuns sessionRun = null;
        
       
            await foreach (var response in requestStream.ReadAllAsync())
            {

                if (sessionRun == null )
                {
                    sessionRun = await AddToSessionsRuns(response);
                }

                LatencyMeasurements addToLatencyMeasurements = await AddToLatencyMeasurements(response, sessionRun);
                
            }
            
        
        return serverResponse;
        
    }

    public async Task StreamSessionRuns(StreamSessionRunRequest request, IServerStreamWriter<StreamSessionRunResponse> responseStream, ServerCallContext context)
    {

       List<SessionRunInformation> sessionRuns = await _sessionRunsRepo.GetSessionRunsViaSesionUnique(Guid.Parse(request.SessionUnique));
       
       StreamSessionRunResponse serverResponse = new StreamSessionRunResponse();

       if (sessionRuns.Count == 0)
       {
           serverResponse.Success = false;
       }

       foreach (var sessionRun in sessionRuns)
       {
           serverResponse.PresetName = sessionRun.PresetName;
           serverResponse.SessionRunId = sessionRun.SessionsRunId.ToString();
           serverResponse.Success = true;
           serverResponse.Message = "";
           serverResponse.OverarchingPresetName = sessionRun.OverarchingPresetName;
           
           await responseStream.WriteAsync(serverResponse);
       }
       
    }

    public async Task StreamLatencyMeasurements(StreamLatencyMeasurementsRequest request, IServerStreamWriter<StreamLatencyMeasurementsResponse> responseStream, ServerCallContext context)
    {
        List<LatencyMeasurementInformation> latencyMeasurements =
            await _latencyMeasurementsRepo.GetLatencyMeasurementsViaSessionRunId(request.SessionRunUnique);
        
        StreamLatencyMeasurementsResponse serverResponse = new StreamLatencyMeasurementsResponse();

        if (latencyMeasurements.Count == 0)
        {
            serverResponse.Success = false;
            serverResponse.Message = "No latency measurements found";
        }
        
        
        foreach (var latency in latencyMeasurements)
        {
            
            Log.Information($"Sending type {latency.TestType} with latency: {latency.Latency} to the front end ");
            
           serverResponse.TestType = latency.TestType;
           serverResponse.Latency = latency.Latency;
           serverResponse.Success = true;
           serverResponse.Message = $"Successfully transmitted latency measurement {latency.Latency}";
           
           await responseStream.WriteAsync(serverResponse); 
        }
    }

    public async Task StreamMultipleLatencies(StreamSessionRunIdsRequest request, IServerStreamWriter<StreamSessionRunIdsResponse> responseStream, ServerCallContext context)
    {
        List<string> sessionRunIds = await _sessionRunsRepo.GetSessionRunIds(request.OverarchingPresetName);

        List<LatencyMeasurementInformation> latencies = await _latencyMeasurementsRepo.GetLatencyMeasurementsViaSessionRunId(sessionRunIds);

        foreach (var sessionRun in latencies)
        {
            StreamSessionRunIdsResponse serverResponse = new StreamSessionRunIdsResponse()
            {
                TestType = sessionRun.TestType,
                Latency = sessionRun.Latency, 
                ClientType = sessionRun.ClientType, 
                StressLevel = sessionRun.StressLevel,
               
            }; 
            
            await responseStream.WriteAsync(serverResponse);
        }
    }

    private async Task<SessionRuns> AddToSessionsRuns(SaveSessionPointRequest saveSessionPoint)
    {
        
        SessionRuns sessionRun = new SessionRuns()
        {
            SessionsRunId = Guid.NewGuid(),
            SessionUnique = Guid.Parse(saveSessionPoint.SessionUnique),
            PresetName = saveSessionPoint.PresetName,
            CreatedAt = DateTime.UtcNow,
            OverarchingPresetName = saveSessionPoint.OverarchingPresetName
        };
        
        await _sessionRunsRepo.AddToDbAsync(sessionRun);
        
        await _sessionRunsRepo.SaveAsync();
        
        return sessionRun;
    }

    private async Task<LatencyMeasurements> AddToLatencyMeasurements(SaveSessionPointRequest saveSessionPoint, SessionRuns sessionRun)
    {
        
        LatencyMeasurements newLatencyMeasurement = new LatencyMeasurements()
        {
            MeasurementUnique = Guid.NewGuid(),
            SessionUnique = Guid.Parse(saveSessionPoint.SessionUnique),
            TestType = saveSessionPoint.TestType.ToString(),
            Latency = saveSessionPoint.LatencyValue,
            SessionRuns = sessionRun, 
            StressLevel = saveSessionPoint.StressLevel.ToString(),
            ClientType = saveSessionPoint.ClientType.ToString()
        };

        await _latencyMeasurementsRepo.AddToDbAsync(newLatencyMeasurement);
        await _latencyMeasurementsRepo.SaveAsync();

        return newLatencyMeasurement;

    }
    
    
}