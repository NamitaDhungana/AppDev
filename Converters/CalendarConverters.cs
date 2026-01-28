using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace JournalApp.Converters
{
    public class DateHighlightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Brushes.Transparent;

            if (values[0] is DateTime date && values[1] is IEnumerable<DateTime> entryDates)
            {
                if (entryDates.Any(d => d.Date == date.Date))
                {
                    // Return a subtle color to indicate an entry exists
                    // We can use a theme resource if possible, but let's start with a solid light blue/indigo
                    return new SolidColorBrush(Color.FromArgb(40, 63, 81, 181)); // Very light indigo
                }
            }

            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateToBorderBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return Brushes.Transparent;

            if (values[0] is DateTime date && values[1] is IEnumerable<DateTime> entryDates)
            {
                if (entryDates.Any(d => d.Date == date.Date))
                {
                    return Brushes.Indigo;
                }
            }

            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
