using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;
using System.Diagnostics;

namespace mitoSoft.homeNet.ArduinoIDE
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = Properties.Settings.Default.BrokerAddress;
            this.comboBox1.Items.Clear();
            var items = Enum.GetValues(typeof(GpioMode));
            var mode = Properties.Settings.Default.GpioMode;
            foreach (var item in items)
            {
                this.comboBox1.Items.Add(item);

                if (mode == item.ToString())
                {
                    this.comboBox1.SelectedItem = item;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.BrokerAddress = this.textBox1.Text;
            Properties.Settings.Default.GpioMode = this.comboBox1.SelectedItem?.ToString();

            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}