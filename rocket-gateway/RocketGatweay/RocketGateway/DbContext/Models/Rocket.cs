using Microsoft.EntityFrameworkCore;

namespace RocketGateway.DbContext.Models;

[PrimaryKey(nameof(Id))]
public class RocketDAO
{
    public Guid Id { get; set; }

    public string Type { get; set; }

    public int LaunchSpeed { get; set; }

    public string Mission { get; set; }

    public string ExplodedReason { get; set; }

    public bool WasExploded { get; set; }
    
}