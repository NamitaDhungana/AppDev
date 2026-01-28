using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace JournalApp.Helpers
{
    public static class RichTextHelper
    {
        public static readonly DependencyProperty DocumentXamlProperty =
            DependencyProperty.RegisterAttached("DocumentXaml", typeof(string), typeof(RichTextHelper), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDocumentXamlChanged));

        public static string GetDocumentXaml(DependencyObject obj) => (string)obj.GetValue(DocumentXamlProperty);
        public static void SetDocumentXaml(DependencyObject obj, string value) => obj.SetValue(DocumentXamlProperty, value);

        private static bool _isUpdating = false;

        private static void OnDocumentXamlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (_isUpdating) return;
            if (d is RichTextBox rtb)
            {
                string xaml = e.NewValue as string ?? "";
                _isUpdating = true;
                try
                {
                    var doc = new FlowDocument();
                    if (!string.IsNullOrEmpty(xaml))
                    {
                        var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xaml)))
                        {
                            range.Load(stream, DataFormats.Xaml);
                        }
                    }
                    rtb.Document = doc;
                }
                catch
                {
                    rtb.Document = new FlowDocument(new Paragraph(new Run(xaml)));
                }
                finally
                {
                    _isUpdating = false;
                }

                // Attach to text changed to update the property back
                rtb.TextChanged -= Rtb_TextChanged;
                rtb.TextChanged += Rtb_TextChanged;
            }
        }

        private static void Rtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdating) return;
            if (sender is RichTextBox rtb)
            {
                _isUpdating = true;
                try
                {
                    var range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                    using (var stream = new MemoryStream())
                    {
                        range.Save(stream, DataFormats.Xaml);
                        SetDocumentXaml(rtb, Encoding.UTF8.GetString(stream.ToArray()));
                    }
                }
                finally
                {
                    _isUpdating = false;
                }
            }
        }
    }
}
