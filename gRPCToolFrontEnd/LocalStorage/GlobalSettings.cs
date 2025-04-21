namespace gRPCToolFrontEnd.LocalStorage
{
    public class GlobalSettings
    {

        public string StressTestingPreset { get; set; }

        public string CurrentLocalHost = "https://localhost:5000";

        public int SingleClientChannels { get; set; }

        public int MultiClientChannels { get; set; }

    }
}
