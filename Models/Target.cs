namespace AirDefenseC2.Models;
public sealed class Target
{
    public required string Id { get; init; }
    public required string Kind { get; init; }
    public double Distance { get; set; }
    public double Bearing { get; set; }
    public double Step { get; init; }
    public ThreatLevel Threat { get; set; }
}
