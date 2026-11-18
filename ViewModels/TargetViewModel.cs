using AirDefenseC2.Infrastructure;
using AirDefenseC2.Models;
namespace AirDefenseC2.ViewModels;
public sealed class TargetViewModel : ObservableObject
{
    public Target Model { get; }
    public TargetViewModel(Target model) => Model = model;
    public string Id => Model.Id;
    public string Kind => Model.Kind;
    public double Distance => Model.Distance;
    public double Bearing => Model.Bearing;
    public ThreatLevel Threat => Model.Threat;
    public string ThreatText => Threat switch
    {
        ThreatLevel.Normal => "NORMAL",
        ThreatLevel.Warning => "WARNING",
        _ => "CRITICAL"
    };
    private bool _isSelected;
    public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
    public void Refresh()
    {
        OnPropertyChanged(nameof(Distance));
        OnPropertyChanged(nameof(Bearing));
        OnPropertyChanged(nameof(Threat));
        OnPropertyChanged(nameof(ThreatText));
    }
}
