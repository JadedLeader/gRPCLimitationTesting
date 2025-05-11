namespace ConfigurationStuff.DTO;

public class LatencyMeasurementInformation
{
    public string TestType { get; set; }
    
    public double Latency { get; set; }
    
    public string ClientType { get; set; }
    
    public string StressLevel { get; set; }
}