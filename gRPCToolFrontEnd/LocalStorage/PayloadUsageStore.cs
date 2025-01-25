using Serilog;

namespace gRPCToolFrontEnd.LocalStorage
{
    public class PayloadUsageStore
    {

        public int SmallPayloadTotal {get; set;}

        public int MediumPayloadTotal { get; set;}

        public int LargePayloadTotal { get; set; }

        public event Action<int> OnSmallPayloadReceived;

        public event Action<int> OnMediumPayloadReceived;

        public event Action<int> OnLargePayloadReceived;

        public PayloadUsageStore()
        {
            
        }

        public void IncrementSmallPayloadTotal()
        {
            Log.Information($"small payload received, incrementing small payload usage");

            SmallPayloadTotal += 1;

            OnSmallPayloadReceived?.Invoke(SmallPayloadTotal);
        }

        public void IncrementMediumPayloadTotal()
        {
            Log.Information($"medium payload received, incrementing medium payload usage");

            MediumPayloadTotal += 1;
            OnMediumPayloadReceived?.Invoke(MediumPayloadTotal);
        }

        public void IncrementLargePayloadTotal()
        {
            Log.Information($"large payload received, increment large payload usage");

            LargePayloadTotal += 1;

            OnLargePayloadReceived?.Invoke(LargePayloadTotal);
        }

    }
}
