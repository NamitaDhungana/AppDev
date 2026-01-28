using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace JournalApp.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void FormatBold_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EditingCommands.ToggleBold.Execute(null, QuickEditor);
            QuickEditor.Focus();
        }

        private void FormatItalic_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EditingCommands.ToggleItalic.Execute(null, QuickEditor);
            QuickEditor.Focus();
        }

        private void FormatBullets_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EditingCommands.ToggleBullets.Execute(null, QuickEditor);
            QuickEditor.Focus();
        }

        private void FormatHeading_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (QuickEditor.Selection.IsEmpty) return;
            var currentSize = QuickEditor.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            if (currentSize is double size && size > 20)
                QuickEditor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, 16.0);
            else
                QuickEditor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, 24.0);
            QuickEditor.Focus();
        }

        private void FormatLink_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (QuickEditor.Selection.IsEmpty) return;
            var link = new Hyperlink(QuickEditor.Selection.Start, QuickEditor.Selection.End);
            link.NavigateUri = new System.Uri("https://google.com"); // Placeholder
            QuickEditor.Focus();
        }
    }
}
