using System.ComponentModel.DataAnnotations;

namespace ConfigurationStuff.DbModels;

public class SessionRuns
{
    [Key]
    public Guid SessionsRunId { get; set;  }
    
    [Required]
    public Guid SessionUnique { get; set; }
    
    [Required]
    public string PresetName { get; set;  }
    
    [Required]
    public string OverarchingPresetName { get; set; }

    [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}