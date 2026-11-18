# AirDefenseC2 — WPF + MVVM 학습 프로젝트

실제 레이더/무기 연결이나 현실의 전술 계산이 없는 **가상 관제 UI 학습용** 프로그램입니다.

## 실행
1. Windows에서 Visual Studio 2022 (17.8 이상) 설치, **.NET 데스크톱 개발** 워크로드 선택
2. `AirDefenseC2.csproj` 파일을 열기
3. F5를 눌러 실행 (.NET 8 SDK 필요)
4. **START** 클릭 → 1초마다 표적이 이동하고 5초마다 추가 표적 생성
5. 목록의 행 또는 레이더 점 클릭 → 동일 표적이 선택됨
6. 오른쪽 하단 UNIT 선택 → 범위 및 잔여 횟수 조건이 맞으면 모의 처리 버튼 활성화
7. PAUSE / RESET으로 멈춤 / 초기화

## MVVM 구조
- `Models`: 가상 Target, Weapon, ThreatLevel
- `Services`: TargetSimulator, ThreatEvaluator (실제 위협평가가 아닌 임의의 표시 기준)
- `ViewModels`: MainViewModel, TargetViewModel. `ObservableCollection`, `INotifyPropertyChanged`, `ICommand`
- `Converters`: 위협 색상, 극좌표→화면 좌표
- `Views`: 레이더/목록/유닛 패널 (UserControl)
- `Infrastructure`: NuGet 설치가 필요 없는 기본 ObservableObject 및 RelayCommand

## 코드 읽는 순서
1. `MainWindow.xaml`: 3행 2열 전체 화면
2. `Views/TargetListView.xaml`: DataGrid 목록, SelectedItem 양방향 바인딩
3. `ViewModels/MainViewModel.cs`: 공유 상태, 커맨드, 타이머
4. `Views/RadarView.xaml`: Canvas 위치 컨버터, Storyboard 회전
5. `Views/WeaponPanelView.xaml`: CanExecute에 의한 버튼 비활성화

## 유의사항
- 가상 거리 `u`, 가상 범위, 상태 색상, 잔여 횟수는 교육 목적으로 임의 설정한 값입니다.
- 물리적 교전 모델, 센서 연결, 실전 작전 의사결정, 현실의 위협평가와 무관합니다.
- `CommunityToolkit.Mvvm` 대신 자체 `ObservableObject`/`RelayCommand`로 작성하여 외부 패키지 없이 실행합니다.
