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
        Properties.Settings.Default.YamlContent = this.richTextBox1.Text;
        Properties.Settings.Default.Save();

        var controllers = new YamlParser(richTextBox1.Text)
            .ParseHomeNetControllers();

        this.comboBox1.Items.Clear();
        foreach (var controller in controllers)
        {
            this.comboBox1.Items.Add(controller);
        }
        this.comboBox1.SelectedIndex = 0;
    }

    private void Convert_Clicked(object sender, EventArgs e)
    {
        this.Check();

        var controllerName = ((HomeNetController)this.comboBox1.SelectedItem!).Name;
        var id = ((HomeNetController)this.comboBox1.SelectedItem!).UniqueId;
        var ip = ((HomeNetController)this.comboBox1.SelectedItem!).IPAddress;
        var mac = ((HomeNetController)this.comboBox1.SelectedItem!).MacAddress;

        var mqtt = new YamlParser(richTextBox1.Text)
            .Parse(id);

        var program = new ProgramTextBuilder(
           controllerName,
           ip,
           mac,
           Properties.Settings.Default.BrokerAddress,
           Properties.Settings.Default.GpioMode)
           .Build(mqtt);

        var f = new Form2();
        f.ShowDialog(program);
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