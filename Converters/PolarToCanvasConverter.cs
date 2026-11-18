using System.Globalization;
using System.Windows.Data;
namespace AirDefenseC2.Converters;
// 반경 100(가상 단위)을 500x500 Canvas의 중심(250,250), 표시 반경 215로 투영.
public sealed class PolarToCanvasConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || values[0] is not double distance || values[1] is not double bearing)
            return 250.0;
        double radians = bearing * Math.PI / 180.0;
        double radius = Math.Clamp(distance, 0, 100) / 100.0 * 215.0;
        bool x = string.Equals(parameter?.ToString(), "X", StringComparison.OrdinalIgnoreCase);
        // N=0°, E=90°, 중심 기준 좌표. 원형 마커(12px)의 반을 빼서 중심 보정.
        return x ? 250 + Math.Sin(radians) * radius - 6
                 : 250 - Math.Cos(radians) * radius - 6;
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
        [Binding.DoNothing, Binding.DoNothing];
}
