using ICSharpCode.AvalonEdit;
using System.Windows.Controls;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Views;

public partial class MissingHomeNetElementsView : UserControl
{
    public MissingHomeNetElementsView()
    {
        InitializeComponent();
        TextEditor.ManipulationBoundaryFeedback += (s, e) => { e.Handled = true; };
        TextEditor.PreviewMouseWheel += (s, e) =>
        {
            TextEditor.ScrollToVerticalOffset(TextEditor.VerticalOffset - e.Delta);
            e.Handled = true;
        };
    }

    public void SetContent(string content)
    {
        TextEditor.Text = content;
    }

    public void SetZoomFactor(double zoomFactor)
    {
        TextEditor.FontSize = 12 * zoomFactor;
    }

    public TextEditor GetTextEditor()
    {
        return TextEditor;
    }
}
