using AirDefenseC2.Infrastructure;
namespace AirDefenseC2.Models;
public sealed class Weapon : ObservableObject
{
    public required string Name { get; init; }
    // 임의의 화면 단위: 실물 무기 제원과 무관
    public double Coverage { get; init; }
    private int _ammo;
    public int Ammo { get => _ammo; set { if (SetProperty(ref _ammo, value)) OnPropertyChanged(nameof(Status)); } }
    public string Status => Ammo > 0 ? "READY" : "EMPTY";
}
