using ICSharpCode.AvalonEdit;
using mitoSoft.homeNet.ArduinoIDE.Contracts;
using mitoSoft.homeNet.ArduinoIDE.Services;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Res = mitoSoft.homeNet.ArduinoIDE.Properties.Resources;

namespace mitoSoft.homeNet.ArduinoIDE.Views;

public partial class YamlView : UserControl, IEditorView, ISaveable
{
    private readonly TextEditorService _textEditorService = new();
    private readonly FileService _fileService = new();

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
        FindBar.CloseRequested += FindBar_CloseRequested;
    }

    public void ShowFindBar(string searchText) =>
        FindBar.Show(searchText);

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

    private void FindBar_CloseRequested(object? sender, EventArgs e) =>
        FindBar.Visibility = Visibility.Collapsed;

    public string Text
    {
        get => this.YamlTextEditor.Text;
        set => this.YamlTextEditor.Text = value;
    }

    public void SetZoomFactor(double zoomFactor) =>
        this.YamlTextEditor.FontSize = 12 * zoomFactor;

    public TextEditor GetTextEditor() =>
        this.YamlTextEditor;

    private void CommentButton_Click(object sender, RoutedEventArgs e) =>
        _textEditorService.CommentLines(YamlTextEditor);

    private void UncommentButton_Click(object sender, RoutedEventArgs e) =>
        _textEditorService.UncommentLines(YamlTextEditor);

    private void SelectHomeNetNodeButton_Click(object sender, RoutedEventArgs e) =>
        _textEditorService.SelectYamlNode(YamlTextEditor, "homeNet:");

    public string? Save(string? fileName)
    {
        if (!string.IsNullOrEmpty(fileName) &&
            File.Exists(fileName))
        {
            _fileService.WriteFile(fileName, this.GetTextEditor().Text);
            return fileName;
        }
        else
        {
            return this.SaveAs(fileName);
        }
    }

    public string? SaveAs(string? fileName)
    {
        var filePath = _fileService.ShowSaveDialog(
            filter: Res.Filter_Yaml,
            defaultFileName: "config.yaml",
            currentFilePath: fileName);

        if (filePath != null)
        {
            _fileService.WriteFile(filePath, this.GetTextEditor().Text);
        }

        return filePath;
    }
}