using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfigurationStuff.DbModels;

public class LatencyMeasurements
{
    
    [Key]
    public Guid MeasurementUnique { get; set; }
    
    [ForeignKey("SessionUnique")]
    public Guid SessionUnique { get; set; }
    
    [Required]
    public string TestType { get; set;  }
    
    [Required]
    double Latency { get; set; }
    
    public SessionRuns? SessionRuns { get; set; }
}