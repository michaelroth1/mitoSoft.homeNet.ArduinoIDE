using ICSharpCode.AvalonEdit;
using mitoSoft.homeNet.ArduinoIDE.WPF.Services;
using System.Windows;
using System.Windows.Controls;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Views;

public partial class YamlView : UserControl, IEditorView
{
    private readonly TextEditorService _textEditorService = new();

    public event EventHandler? TextChanged;

    public YamlView()
    {
        InitializeComponent();
        FindBar.Visibility = Visibility.Collapsed;
        YamlTextEditor.TextChanged += (s, e) => TextChanged?.Invoke(this, EventArgs.Empty);
        YamlTextEditor.ManipulationBoundaryFeedback += (s, e) => { e.Handled = true; };
        YamlTextEditor.PreviewMouseWheel += (s, e) =>
        {
            YamlTextEditor.ScrollToVerticalOffset(YamlTextEditor.VerticalOffset - e.Delta / 3.0);
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

        if (!_textEditorService.FindNext(YamlTextEditor, searchText))
        {
            var savedStart = YamlTextEditor.SelectionStart;
            YamlTextEditor.SelectionStart = 0;
            YamlTextEditor.SelectionLength = 0;
            if (_textEditorService.FindNext(YamlTextEditor, searchText))
            {
                FindBar.SetNotFoundState(false);
            }
            else
            {
                YamlTextEditor.SelectionStart = savedStart;
                FindBar.SetNotFoundState(true);
            }
        }
        else
        {
            FindBar.SetNotFoundState(false);
        }
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

    public TextEditor GetTextEditor()
    {
        return YamlTextEditor;
    }

    private void CommentButton_Click(object sender, RoutedEventArgs e)
    {
        _textEditorService.CommentLines(YamlTextEditor);
    }

    private void UncommentButton_Click(object sender, RoutedEventArgs e)
    {
        _textEditorService.UncommentLines(YamlTextEditor);
    }

    private void SelectHomeNetNodeButton_Click(object sender, RoutedEventArgs e)
    {
        _textEditorService.SelectYamlNode(YamlTextEditor, "homeNet:");
    }
}