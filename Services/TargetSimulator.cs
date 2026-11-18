using AirDefenseC2.Models;
namespace AirDefenseC2.Services;
public sealed class TargetSimulator
{
    private readonly Random _random = new();
    private int _nextId;
    private static readonly string[] Kinds = ["SIM-A", "SIM-B", "SIM-C"];
    public Target Create()
    {
        var target = new Target
        {
            Id = $"T-{++_nextId:000}",
            Kind = Kinds[_random.Next(Kinds.Length)],
            Distance = _random.Next(50, 96),
            Bearing = _random.Next(0, 360),
            Step = _random.Next(1, 5) * 0.7
        };
        return target;
    }
    public void Move(Target target)
    {
        target.Distance = Math.Max(0, target.Distance - target.Step);
        target.Bearing = (target.Bearing + 1.8) % 360;
    }
}
