using ICSharpCode.AvalonEdit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Views;

public partial class YamlDocumentView : UserControl
{
    public event EventHandler? TextChanged;

    public YamlDocumentView()
    {
        InitializeComponent();
        SetupTextEditor();
    }

    public string Text
    {
        get => YamlTextEditor.Text;
        set => YamlTextEditor.Text = value;
    }

    public void SetZoomFactor(double zoomFactor)
    {
        YamlTextEditor.FontSize = 12 * zoomFactor;
    }

    private void SetupTextEditor()
    {
        // Subscribe to TextChanged event
        YamlTextEditor.TextChanged += (s, e) => TextChanged?.Invoke(this, EventArgs.Empty);

        // Enable touch and manipulation
        YamlTextEditor.ManipulationBoundaryFeedback += (s, e) => { e.Handled = true; };

        // Improve scrolling with mouse wheel
        YamlTextEditor.PreviewMouseWheel += (s, e) =>
        {
            var editor = s as TextEditor;
            if (editor != null && editor.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                var scrollViewer = FindScrollViewer(editor);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3.0);
                    e.Handled = true;
                }
            }
        };

        // Enable touch scrolling
        YamlTextEditor.Loaded += (s, e) =>
        {
            var scrollViewer = FindScrollViewer(YamlTextEditor);
            if (scrollViewer != null)
            {
                scrollViewer.PanningMode = PanningMode.VerticalOnly;
                scrollViewer.PanningDeceleration = 0.001;
                scrollViewer.PanningRatio = 1.0;
            }
        };
    }

    private ScrollViewer? FindScrollViewer(DependencyObject obj)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is ScrollViewer scrollViewer)
                return scrollViewer;

            var result = FindScrollViewer(child);
            if (result != null)
                return result;
        }
        return null;
    }

    public TextEditor GetTextEditor()
    {
        return YamlTextEditor;
    }
}
