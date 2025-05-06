namespace gRPCToolFrontEnd.Services
{
    public class StatisticsService
    {

        
        public async Task<double> CalculateMinValue(IEnumerable<double> doubleDataStructure)
        {
            double quickestDelayValue = Math.Round(doubleDataStructure.Min(), 2);

            return Math.Round(quickestDelayValue / 1000.0, 2);
        }

        public async Task<double> CalculateMaxValue(IEnumerable<double> doubleDataStructure)
        {
            double slowestDelayValue = Math.Round(doubleDataStructure.Max(), 2);

            return Math.Round(slowestDelayValue / 1000.0, 2);
        }

        public async Task<double> CalculateRangeValue(IEnumerable<double> doubleDataStructure)
        {
            double lowestValue = await CalculateMinValue(doubleDataStructure);

            double highestValue = await CalculateMaxValue(doubleDataStructure);

            double millisecondRange = Math.Round(highestValue - lowestValue, 2);

            return Math.Round(millisecondRange / 1000.0, 2);
        }

        public async Task<double> CalculateStandardDeviation(IEnumerable<double> doubleDataStructure)
        {
            
            int length = doubleDataStructure.Count();

            double mean = doubleDataStructure.Average();

            double sumSq = doubleDataStructure.Select(x => (x - mean) * (x - mean)).Sum();

            return Math.Round(Math.Sqrt(sumSq / length), 2);

        }

    }
}
