using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;
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

    private void ToolStripButton1_Clicked(object sender, EventArgs e)
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
}