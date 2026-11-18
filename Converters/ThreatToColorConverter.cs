using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using AirDefenseC2.Models;
namespace AirDefenseC2.Converters;
public sealed class ThreatToColorConverter : IValueConverter
{
    private static readonly Brush Green = Freeze("#39D98A");
    private static readonly Brush Orange = Freeze("#FFB44C");
    private static readonly Brush Red = Freeze("#FF6374");
    private static Brush Freeze(string hex)
    {
        var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        brush.Freeze();
        return brush;
    }
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is ThreatLevel level ? level switch
        {
            ThreatLevel.Warning => Orange,
            ThreatLevel.Critical => Red,
            _ => Green
        } : Green;
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        Binding.DoNothing;
}
