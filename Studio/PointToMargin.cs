namespace Studio
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows;
    using Point = System.Drawing.Point;

    public class PointToMargin : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var val = (Point) value;
            return new Thickness(val.X, val.Y, 0, 0);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}