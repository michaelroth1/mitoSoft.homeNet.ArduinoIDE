using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace mitoSoft.homeNet.ArduinoIDE;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
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

        var selected = this.toolStripComboBox1.SelectedItem as HomeNetController;

        this.toolStripComboBox1.Items.Clear();
        foreach (var controller in controllers)
        {
            this.toolStripComboBox1.Items.Add(controller);
        }

        foreach (HomeNetController item in this.toolStripComboBox1.Items)
        {
            if (item.Name == selected?.Name)
            {
                toolStripComboBox1.SelectedItem = item;
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

        var controller = (HomeNetController)toolStripComboBox1.SelectedItem!;

        var controllerName = controller?.Name;
        var id = controller!.UniqueId;
        var ip = controller!.IPAddress;
        var mac = controller!.MacAddress;
        var subscribedTopic = controller!.SubscribedTopic;

        var mqtt = new YamlParser(YamlTextBox.Text)
            .Parse(id);

        var program = new ProgramTextBuilder(
           controllerName!,
           ip,
           mac,
           Properties.Settings.Default.BrokerAddress,
           Properties.Settings.Default.GpioMode,
           subscribedTopic)
           .Build(mqtt);

        var f = new Form2((HomeNetController)this.toolStripComboBox1.SelectedItem);
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

        var f = new Form2((HomeNetController)this.toolStripComboBox1.SelectedItem);
        f.ShowDialog(newConfig);
    }

    private void CheckEntries()
    {
        if (string.IsNullOrEmpty(Properties.Settings.Default.BrokerAddress)
         || string.IsNullOrEmpty(Properties.Settings.Default.GpioMode))
        {
            throw new InvalidOperationException("Complete Settings.");
        }

        if (string.IsNullOrEmpty(this.YamlTextBox.Text))
        {
            throw new InvalidOperationException("Add a YAML file.");
        }

        if (this.toolStripComboBox1.SelectedItem == null)
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

    private void SettingsToolStripMenuItem_Clicked(object sender, EventArgs e)
    {
        var settings = new Settings();
        settings.ShowDialog();
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

        edit.SelectYamlNode(key);
    }
}