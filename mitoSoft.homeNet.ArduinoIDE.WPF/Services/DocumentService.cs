using System.Linq;
using System.Reflection;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Xceed.Wpf.AvalonDock.Layout;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Services;

public class DocumentService
{
    private readonly LayoutDocumentPane _documentPane;
    private readonly Func<double> _getCurrentZoomFactor;
    private IHighlightingDefinition? _cppHighlighting;

    public DocumentService(LayoutDocumentPane documentPane, Func<double> getCurrentZoomFactor)
    {
        _documentPane = documentPane;
        _getCurrentZoomFactor = getCurrentZoomFactor;
        LoadCppSyntaxHighlighting();
    }

    private void LoadCppSyntaxHighlighting()
    {
        try
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "mitoSoft.homeNet.ArduinoIDE.WPF.Editor.Arduino.xshd";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new XmlTextReader(stream))
                    {
                        _cppHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }
        }
        catch
        {
            // Silently fail if highlighting cannot be loaded
            _cppHighlighting = null;
        }
    }

    public void CreateOrUpdateOutputDocument(string controllerName, string content)
    {
        var existingDoc = _documentPane.Children.OfType<LayoutDocument>()
            .FirstOrDefault(d => d.Title == $"Output: {controllerName}");

        if (existingDoc != null)
        {
            UpdateExistingDocument(existingDoc, content);
            return;
        }

        CreateNewDocument(controllerName, content);
    }

    private void UpdateExistingDocument(LayoutDocument document, string content)
    {
        var dockPanel = document.Content as DockPanel;
        var textEditor = dockPanel?.Children.OfType<TextEditor>().FirstOrDefault();
        if (textEditor != null)
        {
            textEditor.Text = content;
        }
        document.IsSelected = true;
        document.IsActive = true;
    }

    private void CreateNewDocument(string controllerName, string content)
    {
        var outputDocument = new LayoutDocument
        {
            Title = $"Output: {controllerName}",
            CanClose = true,
            Content = CreateOutputContent(controllerName, content)
        };

        _documentPane.Children.Add(outputDocument);
        outputDocument.IsSelected = true;
        outputDocument.IsActive = true;
    }

    private DockPanel CreateOutputContent(string controllerName, string content)
    {
        var dockPanel = new DockPanel();

        var toolBar = new ToolBar();
        DockPanel.SetDock(toolBar, Dock.Top);

        var copyButton = new Button
        {
            Content = "Copy to Clipboard",
            Padding = new Thickness(10, 2, 10, 2)
        };
        copyButton.Click += (s, e) =>
        {
            if (!string.IsNullOrEmpty(content))
            {
                Clipboard.SetText(content);
                MessageBox.Show("Content copied to clipboard!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        };

        toolBar.Items.Add(copyButton);

        var textEditor = new TextEditor
        {
            Text = content,
            IsReadOnly = true,
            ShowLineNumbers = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            FontFamily = new FontFamily("Consolas"),
            FontSize = 11 * _getCurrentZoomFactor(),
            Background = Brushes.White
        };

        if (_cppHighlighting != null)
        {
            textEditor.SyntaxHighlighting = _cppHighlighting;
        }

        dockPanel.Children.Add(toolBar);
        dockPanel.Children.Add(textEditor);

        return dockPanel;
    }

    public LayoutDocument? GetActiveDocument()
    {
        return _documentPane.Children.OfType<LayoutDocument>().FirstOrDefault(d => d.IsActive);
    }

    public void UpdateZoomForOutputDocuments(double zoomFactor)
    {
        foreach (var document in _documentPane.Children.OfType<LayoutDocument>())
        {
            if (document.Title.StartsWith("Output:"))
            {
                var dockPanel = document.Content as DockPanel;
                var textEditor = dockPanel?.Children.OfType<TextEditor>().FirstOrDefault();
                if (textEditor != null)
                {
                    textEditor.FontSize = 11 * zoomFactor;
                }
            }
        }
    }
}
