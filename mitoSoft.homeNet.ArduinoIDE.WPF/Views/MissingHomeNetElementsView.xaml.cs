using ICSharpCode.AvalonEdit;
using mitoSoft.homeNet.ArduinoIDE.WPF.Services;
using System.Windows;
using System.Windows.Controls;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Views;

public partial class MissingHomeNetElementsView : UserControl
{
    private readonly TextEditorService _textEditorService = new();

    public MissingHomeNetElementsView()
    {
        InitializeComponent();
        FindBar.Visibility = Visibility.Collapsed;
        TextEditor.ManipulationBoundaryFeedback += (s, e) => { e.Handled = true; };
        TextEditor.PreviewMouseWheel += (s, e) =>
        {
            TextEditor.ScrollToVerticalOffset(TextEditor.VerticalOffset - e.Delta);
            e.Handled = true;
        };

        FindBar.FindNextRequested += FindBar_FindNextRequested;
        FindBar.CloseRequested += (s, e) => FindBar.Visibility = Visibility.Collapsed;
    }

    public void ShowFindBar(string searchText)
    {
        FindBar.Show(searchText);
    }

    private void FindBar_FindNextRequested(object? sender, string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return;

        if (!_textEditorService.FindNext(TextEditor, searchText))
        {
            var savedStart = TextEditor.SelectionStart;
            TextEditor.SelectionStart = 0;
            TextEditor.SelectionLength = 0;
            if (_textEditorService.FindNext(TextEditor, searchText))
            {
                FindBar.SetNotFoundState(false);
            }
            else
            {
                TextEditor.SelectionStart = savedStart;
                FindBar.SetNotFoundState(true);
            }
        }
        else
        {
            FindBar.SetNotFoundState(false);
        }
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
