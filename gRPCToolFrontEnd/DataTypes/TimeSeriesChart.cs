namespace gRPCToolFrontEnd.DataTypes
{
    public class TimeSeriesChart
    {

        public string Name { get; set; }
        public List<double> Data { get; set; } = new List<double>();

    }
}
