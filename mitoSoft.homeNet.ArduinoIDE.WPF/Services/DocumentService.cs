using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Services;

public class DocumentService
{
    private readonly LayoutDocumentPane _documentPane;
    private readonly double _zoomFactor;

    public DocumentService(LayoutDocumentPane documentPane, double zoomFactor)
    {
        _documentPane = documentPane;
        _zoomFactor = zoomFactor;
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
        var textBox = dockPanel?.Children.OfType<TextBox>().FirstOrDefault();
        if (textBox != null)
        {
            textBox.Text = content;
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

        var textBox = new TextBox
        {
            Text = content,
            IsReadOnly = true,
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            FontFamily = new FontFamily("Consolas"),
            FontSize = 11 * _zoomFactor,
            Background = Brushes.White
        };

        dockPanel.Children.Add(toolBar);
        dockPanel.Children.Add(textBox);

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
                var textBox = dockPanel?.Children.OfType<TextBox>().FirstOrDefault();
                if (textBox != null)
                {
                    textBox.FontSize = 11 * zoomFactor;
                }
            }
        }
    }
}
