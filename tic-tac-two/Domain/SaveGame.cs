using System.ComponentModel.DataAnnotations;

namespace Domain;

public class SaveGame : BaseEntity
{
    [MaxLength(128)]
    public string GameName { get; set; } = default!;

    [MaxLength(10240)]
    public string State { get; set; } = default!;

    // Expose the Foreign Key   
    // public int ConfigId { get; set; }
    public Configuration? Configuration { get; set; }
}