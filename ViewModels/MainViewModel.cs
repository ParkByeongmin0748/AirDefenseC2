using System.Collections.ObjectModel;
using System.Windows.Threading;
using AirDefenseC2.Infrastructure;
using AirDefenseC2.Models;
using AirDefenseC2.Services;
namespace AirDefenseC2.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly DispatcherTimer _timer;
    private readonly TargetSimulator _simulator = new();
    private readonly ThreatEvaluator _evaluator = new();
    private int _ticks;
    private int _eventCount;
    public ObservableCollection<TargetViewModel> Targets { get; } = [];
    public ObservableCollection<Weapon> Weapons { get; } = [];
    public ObservableCollection<string> Logs { get; } = [];
    public RelayCommand StartCommand { get; }
    public RelayCommand PauseCommand { get; }
    public RelayCommand ResetCommand { get; }
    public RelayCommand EngageCommand { get; }
    public RelayCommand<TargetViewModel> SelectTargetCommand { get; }

    private TargetViewModel? _selectedTarget;
    public TargetViewModel? SelectedTarget
    {
        get => _selectedTarget;
        set
        {
            if (ReferenceEquals(_selectedTarget, value)) return;
            if (_selectedTarget != null) _selectedTarget.IsSelected = false;
            if (SetProperty(ref _selectedTarget, value) && value != null) value.IsSelected = true;
            UpdateEngage();
        }
    }
    private Weapon? _selectedWeapon;
    public Weapon? SelectedWeapon
    {
        get => _selectedWeapon;
        set { if (SetProperty(ref _selectedWeapon, value)) UpdateEngage(); }
    }
    private bool _isRunning;
    public bool IsRunning
    {
        get => _isRunning;
        private set
        {
            if (SetProperty(ref _isRunning, value))
            {
                OnPropertyChanged(nameof(RunState));
                StartCommand.RaiseCanExecuteChanged();
                PauseCommand.RaiseCanExecuteChanged();
            }
        }
    }
    public string RunState => IsRunning ? "RUNNING" : "PAUSED";
    public string TargetCount => $"TRACKS {Targets.Count:00}";
    public string SelectedSummary => SelectedTarget is null ? "표적을 선택하세요" :
        $"{SelectedTarget.Id}  ·  {SelectedTarget.Distance:0.0} u  ·  {SelectedTarget.ThreatText}";

    public MainViewModel()
    {
        StartCommand = new RelayCommand(Start, () => !IsRunning);
        PauseCommand = new RelayCommand(Pause, () => IsRunning);
        ResetCommand = new RelayCommand(Reset);
        EngageCommand = new RelayCommand(Engage, CanEngage);
        SelectTargetCommand = new RelayCommand<TargetViewModel>(target => SelectedTarget = target);
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => Tick();
        Reset();
    }
    private void Log(string message)
    {
        Logs.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {message}");
        if (Logs.Count > 100) Logs.RemoveAt(Logs.Count - 1);
    }
    private void AddTarget()
    {
        var model = _simulator.Create();
        model.Threat = _evaluator.Evaluate(model.Distance);
        Targets.Add(new TargetViewModel(model));
        Log($"DETECT  {model.Id} / {model.Kind}  (가상 신호)");
        OnPropertyChanged(nameof(TargetCount));
    }
    private void Tick()
    {
        _ticks++;
        foreach (var vm in Targets.ToArray())
        {
            _simulator.Move(vm.Model);
            var oldThreat = vm.Model.Threat;
            vm.Model.Threat = _evaluator.Evaluate(vm.Model.Distance);
            vm.Refresh();
            if (oldThreat != vm.Model.Threat)
                Log($"STATUS  {vm.Id} → {vm.ThreatText}");
            if (vm.Distance <= 0)
            {
                if (ReferenceEquals(SelectedTarget, vm)) SelectedTarget = null;
                Targets.Remove(vm);
                Log($"EXIT  {vm.Id} / 시뮬레이션 종료");
            }
        }
        if (_ticks % 5 == 0 && Targets.Count < 10) AddTarget();
        OnPropertyChanged(nameof(TargetCount));
        OnPropertyChanged(nameof(SelectedSummary));
        UpdateEngage();
    }
    private void Start()
    {
        _timer.Start();
        IsRunning = true;
        Log("SIMULATION START");
    }
    private void Pause()
    {
        _timer.Stop();
        IsRunning = false;
        Log("SIMULATION PAUSE");
    }
    private void Reset()
    {
        _timer.Stop();
        IsRunning = false;
        _ticks = 0;
        _eventCount = 0;
        SelectedTarget = null;
        Targets.Clear();
        Weapons.Clear();
        Logs.Clear();
        Weapons.Add(new Weapon { Name = "UNIT-ALPHA", Coverage = 40, Ammo = 5 });
        Weapons.Add(new Weapon { Name = "UNIT-BRAVO", Coverage = 65, Ammo = 4 });
        Weapons.Add(new Weapon { Name = "UNIT-CHARLIE", Coverage = 90, Ammo = 3 });
        foreach (var weapon in Weapons) weapon.PropertyChanged += (_, _) => UpdateEngage();
        SelectedWeapon = Weapons[0];
        for (int i = 0; i < 5; i++) AddTarget();
        Log("SIMULATION READY / 수치는 모두 가상 단위");
        OnPropertyChanged(nameof(TargetCount));
        OnPropertyChanged(nameof(SelectedSummary));
        UpdateEngage();
    }
    private bool CanEngage() => SelectedTarget != null && SelectedWeapon != null &&
        SelectedWeapon.Ammo > 0 && SelectedTarget.Distance <= SelectedWeapon.Coverage;
    private void UpdateEngage() => EngageCommand.RaiseCanExecuteChanged();
    private void Engage()
    {
        if (!CanEngage()) return;
        var target = SelectedTarget!;
        var weapon = SelectedWeapon!;
        weapon.Ammo--;
        _eventCount++;
        Log($"SIM-EVENT #{_eventCount:000}  {weapon.Name} → {target.Id} / 모의 처리 완료");
        Targets.Remove(target);
        SelectedTarget = null;
        OnPropertyChanged(nameof(TargetCount));
        OnPropertyChanged(nameof(SelectedSummary));
        UpdateEngage();
    }
}
