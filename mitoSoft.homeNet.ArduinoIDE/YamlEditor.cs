using YamlDotNet.Core.Events;

namespace mitoSoft.homeNet.ArduinoIDE;

public class YamlEditor
{
    private readonly RichTextBox _textBox;
    public YamlEditor(RichTextBox textBox) => _textBox = textBox;

    // YAML-Knoten + Unterknoten selektieren
    public void SelectYamlNode(string key)
    {
        string[] lines = _textBox.Text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        int startIndex = -1;
        int endIndex = _textBox.Text.Length - 1;
        bool found = false;
        bool topNode;
        bool comment;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (!string.IsNullOrWhiteSpace(line)
                && !line.StartsWith(" "))
            {
                topNode = true;
            }
            else
            {
                topNode = false;
            }

            if (line.TrimStart().StartsWith("#"))
            {
                comment = true;
            }
            else
            {
                comment = false;
            }

            if (found && topNode && !comment) // next topnode
            {
                endIndex = _textBox.Text.IndexOf(lines[i]);
                break;
            }

            if (found == false && topNode
                && line.StartsWith($"{key}:")) // first topnode
            {
                found = true;
                startIndex = _textBox.Text.IndexOf(lines[i]);
            }

            if (found == false && topNode
                && line.Replace(" ", "")
                       .StartsWith($"#{key}:")) // commented topnode
            {
                found = true;
                startIndex = _textBox.Text.IndexOf(lines[i]);
            }
        }

        if (found)
        {
            _textBox.SelectionStart = startIndex;
            _textBox.SelectionLength = endIndex - startIndex;
            _textBox.ScrollToCaret();
        }
        else
        {
            MessageBox.Show($"Yaml node '{key}' not found.", "ArduinoIDE", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}