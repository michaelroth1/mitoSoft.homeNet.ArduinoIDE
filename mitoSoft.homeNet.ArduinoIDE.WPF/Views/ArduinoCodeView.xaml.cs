using ICSharpCode.AvalonEdit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Views;

public partial class OutputView : UserControl
{
    public OutputView()
    {
        InitializeComponent();
        SetupTextEditor();
    }

    public void SetContent(string content)
    {
        TextEditor.Text = content;
    }

    public void SetZoomFactor(double zoomFactor)
    {
        TextEditor.FontSize = 11 * zoomFactor;
    }

    public TextEditor GetTextEditor()
    {
        return TextEditor;
    }

    private void SetupTextEditor()
    {
        // Enable touch and manipulation
        TextEditor.ManipulationBoundaryFeedback += (s, e) => { e.Handled = true; };

        // Improve scrolling with mouse wheel
        TextEditor.PreviewMouseWheel += (s, e) =>
        {
            var editor = s as TextEditor;
            if (editor != null)
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
        TextEditor.Loaded += (s, e) =>
        {
            var scrollViewer = FindScrollViewer(TextEditor);
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
}
