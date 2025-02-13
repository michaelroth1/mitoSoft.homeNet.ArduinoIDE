using mitoSoft.homeNet.ArduinoIDE.Extensions;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;

namespace mitoSoft.homeNet.ArduinoIDE;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        if (Properties.Settings.Default.FormSize.Width > 0
            && Properties.Settings.Default.FormSize.Height > 0)
        {
            this.Size = Properties.Settings.Default.FormSize;
        }

        if (Properties.Settings.Default.TextZoom > 0)
        {
            this.YamlTextBox.ZoomFactor = Properties.Settings.Default.TextZoom;
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        this.YamlTextBox.Text = Properties.Settings.Default.YamlContent;
    }

    private void RichTextBox1_TextChanged(object sender, EventArgs e)
    {
        this.SaveYamlInProps();

        var controllers = new YamlParser(YamlTextBox.Text)
            .ParseHomeNetControllers();

        var selected = this.toolStripComboBox.SelectedItem as HomeNetController;

        this.toolStripComboBox.Items.Clear();
        this.toolStripComboBox.SelectedIndex = -1;
        this.toolStripComboBox.Text = "";
        foreach (var controller in controllers)
        {
            this.toolStripComboBox.Items.Add(controller);
        }

        foreach (HomeNetController item in this.toolStripComboBox.Items)
        {
            if (item.Name == selected?.Name)
            {
                toolStripComboBox.SelectedItem = item;
                break;
            }
        }
    }

    private void SaveYamlInProps()
    {
        Properties.Settings.Default.YamlContent = this.YamlTextBox.Text;
        Properties.Settings.Default.Save();
    }

    private void BuildToolStripButton_Click(object sender, EventArgs e)
    {
        this.SaveYamlInProps();
        this.CheckEntries();

        this.CheckYaml();

        var controller = (HomeNetController)toolStripComboBox.SelectedItem!;

        var mqtt = new YamlParser(YamlTextBox.Text)
            .Parse(controller!.UniqueId);

        var program = new ProgramTextBuilder(
           controller!.Name,
           controller!.IPAddress.GetArduinoIPFormat(),
           controller!.MacAddress.GetArduinoSignaturFormat(),
           controller!.BrokerIPAddress.GetArduinoIPFormat(),
           controller!.GpioMode,
           controller!.SubscribedTopic,
           controller!.AdditionalDeclaration, 
           controller!.AdditionalSetup,
           controller!.AdditionalCode)
           .Build(mqtt);

        var f = new Form2((HomeNetController)this.toolStripComboBox.SelectedItem!);
        f.ShowDialog(program);
    }

    private void CheckYAMLToolStripMenuItem_Clicked(object sender, EventArgs e)
    {
        this.CheckYaml();
    }

    private void CreateHomeNetElementsToolStripMenuItem_Clicked(object sender, EventArgs e)
    {
        var newConfig = new YamlParser(YamlTextBox.Text)
            .AddHomeNetElements();

        var f = new Form2((HomeNetController)this.toolStripComboBox.SelectedItem!);
        f.ShowDialog(newConfig);
    }

    private void CheckEntries()
    {
        if (string.IsNullOrEmpty(this.YamlTextBox.Text))
        {
            throw new InvalidOperationException("Add a YAML file.");
        }

        if (this.toolStripComboBox.SelectedItem == null)
        {
            throw new InvalidOperationException("Inspect and choose a controller.");
        }
    }

    private void CheckYaml()
    {
        var warnings = new YamlParser(YamlTextBox.Text)
            .CheckYaml();

        this.WarningTextBox.Text = warnings.ToString();
    }

    private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (OpenFileDialog.ShowDialog() == DialogResult.OK)
        {
            var yaml = File.ReadAllText(OpenFileDialog.FileName);
            this.YamlTextBox.Text = yaml;
        }
    }

    private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (SaveFileDialog.ShowDialog() == DialogResult.OK)
        {
            File.WriteAllText(SaveFileDialog.FileName, this.YamlTextBox.Text);
        }
    }

    private void CommentToolStripButton_Clicked(object sender, EventArgs e)
    {
        this.CommentSelectedLines();
    }

    private void UncommentToolStripButton_Clicked(object sender, EventArgs e)
    {
        this.UncommentSelectedLines();
    }

    private void CommentSelectedLines()
    {
        // Speichert die aktuelle Cursorposition
        int selectionStart = this.YamlTextBox.SelectionStart;
        int selectionEnd = selectionStart + this.YamlTextBox.SelectionLength;

        // Bestimme den Start der ersten Zeile und das Ende der letzten Zeile in der Auswahl
        int lineStart = this.YamlTextBox.GetFirstCharIndexOfCurrentLine();
        int lineEnd = selectionEnd;

        while (lineEnd < this.YamlTextBox.Text.Length && this.YamlTextBox.Text[lineEnd] != '\n')
        {
            lineEnd++; // Gehe zum Ende der letzten Zeile
        }

        // Extrahiere die betroffenen Zeilen
        string selectedText = this.YamlTextBox.Text.Substring(lineStart, lineEnd - lineStart);

        // Jede Zeile mit "#" auskommentieren
        string commentedText = string.Join("\n", selectedText
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .Select(line => "#" + line));

        // Ersetze den ursprünglichen Text
        this.YamlTextBox.Select(lineStart, lineEnd - lineStart);
        this.YamlTextBox.SelectedText = commentedText;

        // Setzt den Cursor zurück
        this.YamlTextBox.SelectionStart = lineStart;
        this.YamlTextBox.SelectionLength = commentedText.Length;
    }

    private void UncommentSelectedLines()
    {
        int selectionStart = this.YamlTextBox.SelectionStart;
        int selectionEnd = selectionStart + this.YamlTextBox.SelectionLength;

        // Bestimme den Start der ersten Zeile und das Ende der letzten Zeile
        int lineStart = this.YamlTextBox.GetFirstCharIndexOfCurrentLine();
        int lineEnd = selectionEnd;

        while (lineEnd < this.YamlTextBox.Text.Length && this.YamlTextBox.Text[lineEnd] != '\n')
        {
            lineEnd++; // Gehe zum Ende der letzten Zeile
        }

        // Extrahiere die betroffenen Zeilen
        string selectedText = this.YamlTextBox.Text.Substring(lineStart, lineEnd - lineStart);

        // Entfernt das "#" am Zeilenanfang, falls vorhanden
        string uncommentedText = string.Join("\n", selectedText
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .Select(line => line.StartsWith("#") ? line.Substring(1) : line));

        // Ersetze den ursprünglichen Text
        this.YamlTextBox.Select(lineStart, lineEnd - lineStart);
        this.YamlTextBox.SelectedText = uncommentedText;

        // Setzt den Cursor zurück
        this.YamlTextBox.SelectionStart = lineStart;
        this.YamlTextBox.SelectionLength = uncommentedText.Length;
    }

    private void SelectHomeNetNodeToolStripMenuItem_Clicked(object sender, EventArgs e)
    {
        var key = "homeNet";

        var edit = new YamlEditor(this.YamlTextBox);

        this.YamlTextBox.Focus();

        edit.SelectYamlNode(key);
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        Properties.Settings.Default.FormSize = this.Size;
        Properties.Settings.Default.TextZoom = this.YamlTextBox.ZoomFactor;
        Properties.Settings.Default.Save();
    }
}