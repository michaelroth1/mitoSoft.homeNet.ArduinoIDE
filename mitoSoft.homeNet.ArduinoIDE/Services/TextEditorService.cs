using ICSharpCode.AvalonEdit;

namespace mitoSoft.homeNet.ArduinoIDE.Services;

public class TextEditorService
{
    // AvalonEdit TextEditor overloads
    public void CommentLines(TextEditor textEditor)
    {
        int selectionStart = textEditor.SelectionStart;
        int selectionLength = textEditor.SelectionLength;

        string text = textEditor.Text;
        int lineStart = text.LastIndexOf('\n', selectionStart) + 1;
        int lineEnd = text.IndexOf('\n', selectionStart + selectionLength);
        if (lineEnd == -1) lineEnd = text.Length;

        string selectedText = text[lineStart..lineEnd];
        string commentedText = string.Join("\n", selectedText
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(line => "#" + line));

        textEditor.Text = text[..lineStart] + commentedText + text[lineEnd..];
        textEditor.SelectionStart = lineStart;
        textEditor.SelectionLength = commentedText.Length;
    }

    public void UncommentLines(TextEditor textEditor)
    {
        int selectionStart = textEditor.SelectionStart;
        int selectionLength = textEditor.SelectionLength;

        string text = textEditor.Text;
        int lineStart = text.LastIndexOf('\n', selectionStart) + 1;
        int lineEnd = text.IndexOf('\n', selectionStart + selectionLength);
        if (lineEnd == -1) lineEnd = text.Length;

        string selectedText = text[lineStart..lineEnd];
        string uncommentedText = string.Join("\n", selectedText
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(line => line.StartsWith('#') ? line[1..] : line));

        textEditor.Text = text[..lineStart] + uncommentedText + text[lineEnd..];
        textEditor.SelectionStart = lineStart;
        textEditor.SelectionLength = uncommentedText.Length;
    }

    public void SelectYamlNode(TextEditor textEditor, string key)
    {
        string text = textEditor.Text;
        int index = text.IndexOf(key, StringComparison.OrdinalIgnoreCase);

        if (index >= 0)
        {
            textEditor.Focus();
            textEditor.SelectionStart = index;
            textEditor.SelectionLength = key.Length;
            textEditor.ScrollToLine(this.GetLineNumber(text, index) + 1);
        }
    }

    public bool FindNext(TextEditor textEditor, string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return false;

        int startPosition = textEditor.SelectionStart + textEditor.SelectionLength;
        int foundIndex = textEditor.Text.IndexOf(searchText, startPosition, StringComparison.OrdinalIgnoreCase);

        if (foundIndex != -1)
        {
            textEditor.Focus();
            textEditor.SelectionStart = foundIndex;
            textEditor.SelectionLength = searchText.Length;
            textEditor.ScrollToLine(this.GetLineNumber(textEditor.Text, foundIndex) + 1);
            return true;
        }

        return false;
    }

    private int GetLineNumber(string text, int index) =>
        text[..index].Count(c => c == '\n');
}