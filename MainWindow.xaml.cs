using System.Windows;
using AirDefenseC2.ViewModels;
namespace AirDefenseC2;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
