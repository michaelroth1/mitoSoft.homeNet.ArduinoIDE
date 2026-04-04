using ICSharpCode.AvalonEdit;
using mitoSoft.homeNet.ArduinoIDE.Contracts;
using mitoSoft.homeNet.ArduinoIDE.Services;
using System.Windows;
using System.Windows.Controls;
using Res = mitoSoft.homeNet.ArduinoIDE.Properties.Resources;

namespace mitoSoft.homeNet.ArduinoIDE.Views;

public partial class OutputView : UserControl, IEditorView, ISaveable
{
    private readonly TextEditorService _textEditorService = new();
    private readonly FileService _fileService = new();
    private readonly string _controllerName;

    public OutputView(string controllerName)
    {
        InitializeComponent();

        _controllerName = controllerName;

        FindBar.Visibility = Visibility.Collapsed;
        TextEditor.ManipulationBoundaryFeedback += (s, e) => { e.Handled = true; };
        TextEditor.PreviewMouseWheel += (s, e) =>
        {
            TextEditor.ScrollToVerticalOffset(TextEditor.VerticalOffset - e.Delta / 3.0);
            e.Handled = true;
        };

        FindBar.FindNextRequested += FindBar_FindNextRequested;
        FindBar.CloseRequested += FindBar_CloseRequested; ;
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

    private void FindBar_CloseRequested(object? sender, EventArgs e) =>
        FindBar.Visibility = Visibility.Collapsed;

    public void SetContent(string content) =>
        this.TextEditor.Text = content;

    public void SetZoomFactor(double zoomFactor) =>
        this.TextEditor.FontSize = 12 * zoomFactor;

    public TextEditor GetTextEditor() =>
        this.TextEditor;

    public string? Save(string? fileName) =>
        this.SaveAs(fileName);

    public string? SaveAs(string? fileName)
    {
        var filePath = _fileService.ShowSaveDialog(
            filter: Res.Filter_Arduino,
            defaultFileName: $"{_controllerName}.ino");

        if (filePath != null)
        {
            _fileService.WriteFile(filePath, this.GetTextEditor().Text);
        }

        return filePath;
    }
}