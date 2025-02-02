using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Helpers;
using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;

namespace mitoSoft.homeNet.ArduinoIDE;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        this.richTextBox1.Text = Properties.Settings.Default.YamlContent;
    }

    private void Inspect_Clicked(object sender, EventArgs e)
    {
        this.SaveYaml();

        var controllers = new YamlParser(richTextBox1.Text)
            .ParseHomeNetControllers();

        this.comboBox1.Items.Clear();
        foreach (var controller in controllers)
        {
            this.comboBox1.Items.Add(controller);
        }
        this.comboBox1.SelectedIndex = 0;
    }

    private void SaveYaml()
    {
        Properties.Settings.Default.YamlContent = this.richTextBox1.Text;
        Properties.Settings.Default.Save();
    }

    private void Convert_Clicked(object sender, EventArgs e)
    {
        this.SaveYaml();

        this.Check();

        var controllerName = ((HomeNetController)this.comboBox1.SelectedItem!).Name;
        var id = ((HomeNetController)this.comboBox1.SelectedItem!).UniqueId;
        var ip = ((HomeNetController)this.comboBox1.SelectedItem!).IPAddress;
        var mac = ((HomeNetController)this.comboBox1.SelectedItem!).MacAddress;
        var subscribedTopic = ((HomeNetController)this.comboBox1.SelectedItem!).SubscribedTopic;

        var mqtt = new YamlParser(richTextBox1.Text)
            .Parse(id);

        var program = new ProgramTextBuilder(
           controllerName,
           ip,
           mac,
           Properties.Settings.Default.BrokerAddress,
           Properties.Settings.Default.GpioMode,
           subscribedTopic)
           .Build(mqtt);

        var f = new Form2();
        f.ShowDialog(program);
    }

    private void CheckYAML_Clicked(object sender, EventArgs e)
    {
        this.SaveYaml();

        new YamlParser(richTextBox1.Text)
            .CheckYaml();

        MessageBox.Show("No errors in YAML file.");
    }

    private void createHomeNetElementsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        this.SaveYaml();

        var newConfig = new YamlParser(richTextBox1.Text)
            .AddHomeNetElements();

        var f = new Form2();
        f.ShowDialog(newConfig);
    }

    private void Check()
    {
        if (string.IsNullOrEmpty(Properties.Settings.Default.BrokerAddress)
         || string.IsNullOrEmpty(Properties.Settings.Default.GpioMode))
        {
            throw new InvalidOperationException("Complete Settings.");
        }

        if (string.IsNullOrEmpty(this.richTextBox1.Text))
        {
            throw new InvalidOperationException("Add a YAML file.");
        }

        if (this.comboBox1.SelectedItem == null)
        {
            throw new InvalidOperationException("Inspect and choose a controller.");
        }
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var settings = new Settings();
        settings.ShowDialog();
    }
}