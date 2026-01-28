using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace JournalApp.Converters
{
    public class XamlToPlainTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string input)
            {
                if (string.IsNullOrWhiteSpace(input)) return string.Empty;

                // If it looks like XAML, strip tags
                if (input.Contains("<FlowDocument") || input.Contains("<Section"))
                {
                    // Basic regex to strip tags. Not perfect but good for snippets.
                    string plainText = Regex.Replace(input, "<.*?>", string.Empty);
                    // Decode some basics if needed
                    plainText = plainText.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">");
                    return plainText.Trim();
                }
                
                // If it's Markdown, ideally we'd strip that too, but for snippets it's often okay
                return input;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
