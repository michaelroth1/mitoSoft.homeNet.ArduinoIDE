using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;

namespace mitoSoft.homeNet.ArduinoIDE.WPF.Services;

public class TextEditorService
{
    // AvalonEdit TextEditor overloads
    public void CommentLines(TextEditor textEditor)
    {
        int selectionStart = textEditor.SelectionStart;
        int selectionLength = textEditor.SelectionLength;

        if (selectionLength == 0) return;

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

        if (selectionLength == 0) return;

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
            textEditor.ScrollToLine(GetLineNumber(text, index) + 1);
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
            textEditor.ScrollToLine(GetLineNumber(textEditor.Text, foundIndex) + 1);
            return true;
        }

        return false;
    }

    // TextBox overloads (for backward compatibility)
    public void CommentLines(TextBox textBox)
    {
        int selectionStart = textBox.SelectionStart;
        int selectionLength = textBox.SelectionLength;

        if (selectionLength == 0) return;

        string text = textBox.Text;
        int lineStart = text.LastIndexOf('\n', selectionStart) + 1;
        int lineEnd = text.IndexOf('\n', selectionStart + selectionLength);
        if (lineEnd == -1) lineEnd = text.Length;

        string selectedText = text[lineStart..lineEnd];
        string commentedText = string.Join("\n", selectedText
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(line => "#" + line));

        textBox.Text = text[..lineStart] + commentedText + text[lineEnd..];
        textBox.SelectionStart = lineStart;
        textBox.SelectionLength = commentedText.Length;
    }

    public void UncommentLines(TextBox textBox)
    {
        int selectionStart = textBox.SelectionStart;
        int selectionLength = textBox.SelectionLength;

        if (selectionLength == 0) return;

        string text = textBox.Text;
        int lineStart = text.LastIndexOf('\n', selectionStart) + 1;
        int lineEnd = text.IndexOf('\n', selectionStart + selectionLength);
        if (lineEnd == -1) lineEnd = text.Length;

        string selectedText = text[lineStart..lineEnd];
        string uncommentedText = string.Join("\n", selectedText
            .Split(["\r\n", "\n"], StringSplitOptions.None)
            .Select(line => line.StartsWith('#') ? line[1..] : line));

        textBox.Text = text[..lineStart] + uncommentedText + text[lineEnd..];
        textBox.SelectionStart = lineStart;
        textBox.SelectionLength = uncommentedText.Length;
    }

    public void SelectYamlNode(TextBox textBox, string key)
    {
        string text = textBox.Text;
        int index = text.IndexOf(key, StringComparison.OrdinalIgnoreCase);

        if (index >= 0)
        {
            textBox.Focus();
            textBox.SelectionStart = index;
            textBox.SelectionLength = key.Length;
            textBox.ScrollToLine(GetLineNumber(text, index));
        }
    }

    public bool FindNext(TextBox textBox, string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return false;

        int startPosition = textBox.SelectionStart + textBox.SelectionLength;
        int foundIndex = textBox.Text.IndexOf(searchText, startPosition, StringComparison.OrdinalIgnoreCase);

        if (foundIndex != -1)
        {
            textBox.Focus();
            textBox.SelectionStart = foundIndex;
            textBox.SelectionLength = searchText.Length;
            textBox.ScrollToLine(GetLineNumber(textBox.Text, foundIndex));
            return true;
        }

        return false;
    }

    private int GetLineNumber(string text, int index)
    {
        return text[..index].Count(c => c == '\n');
    }
}
