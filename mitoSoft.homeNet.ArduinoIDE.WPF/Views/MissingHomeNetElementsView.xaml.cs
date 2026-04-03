using ICSharpCode.AvalonEdit;
using System.Windows.Controls;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Views;

public partial class MissingHomeNetElementsView : UserControl
{
    public MissingHomeNetElementsView()
    {
        InitializeComponent();
        this.SetupTextEditor();
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

    private void SetupTextEditor()
    {
        TextEditor.ManipulationBoundaryFeedback += (s, e) => { e.Handled = true; };

        TextEditor.PreviewMouseWheel += (s, e) =>
        {
            var editor = s as TextEditor;
            if (editor != null)
            {
                e.Handled = true;
                editor.ScrollToVerticalOffset(editor.VerticalOffset - e.Delta);
            }
        };
    }
}
