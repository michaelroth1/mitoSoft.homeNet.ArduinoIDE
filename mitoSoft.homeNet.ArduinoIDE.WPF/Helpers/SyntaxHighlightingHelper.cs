using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.Windows;
using System.Xml;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Helpers;

public static class SyntaxHighlightingHelper
{
    public static readonly DependencyProperty HighlightingFileProperty =
        DependencyProperty.RegisterAttached(
            "HighlightingFile",
            typeof(string),
            typeof(SyntaxHighlightingHelper),
            new PropertyMetadata(null, OnHighlightingFileChanged));

    public static string? GetHighlightingFile(DependencyObject obj)
    {
        return (string?)obj.GetValue(HighlightingFileProperty);
    }

    public static void SetHighlightingFile(DependencyObject obj, string? value)
    {
        obj.SetValue(HighlightingFileProperty, value);
    }

    private static void OnHighlightingFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextEditor editor && e.NewValue is string resourcePath && !string.IsNullOrEmpty(resourcePath))
        {
            try
            {
                var uri = new Uri(resourcePath, UriKind.Relative);
                using var stream = Application.GetResourceStream(uri)?.Stream;
                
                if (stream != null)
                {
                    using var reader = new XmlTextReader(stream);
                    editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                }
            }
            catch
            {
                // Silently fail if highlighting cannot be loaded
            }
        }
    }
}
