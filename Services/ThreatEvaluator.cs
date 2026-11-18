using AirDefenseC2.Models;
namespace AirDefenseC2.Services;
// 교육용 색상 분류: 임의의 화면상 거리 기준이며 실제 위협평가 로직이 아님.
public sealed class ThreatEvaluator
{
    public ThreatLevel Evaluate(double displayDistance) => displayDistance switch
    {
        < 30 => ThreatLevel.Critical,
        < 60 => ThreatLevel.Warning,
        _ => ThreatLevel.Normal
    };
}
