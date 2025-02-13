using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;

namespace mitoSoft.homeNet.ArduinoIDE
{
    public partial class Form2 : Form
    {
        private readonly string _controllerName;

        public Form2(string controllerName)
        {
            InitializeComponent();

            _controllerName = controllerName;

            if (Properties.Settings.Default.TextZoom > 0)
            {
                this.TextBox.ZoomFactor = Properties.Settings.Default.TextZoom;
            }
        }

        public void ShowDialog(string text)
        {
            this.Text = $"Arduino IDE: {_controllerName}";
            this.TextBox.Text = text;
            this.CleanStatusStrip();
            this.CopyToClipboard();
            base.ShowDialog();
        }

        public void Show(string text)
        {
            this.Text = $"Arduino IDE: {_controllerName}";
            this.TextBox.Text = text;
            this.CleanStatusStrip();
            this.CopyToClipboard();
            base.Show();
        }

        private void CopyButton_Clicked(object sender, EventArgs e)
        {
            this.CopyToClipboard();
        }

        private void CopyToClipboard()
        {
            Clipboard.SetText(this.TextBox.Text);

            this.ShowInStatusBar("Successfully copied...");
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            SaveFileDialog.InitialDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Arduino");

            SaveFileDialog.FileName = $"{this._controllerName}.ino";

            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var fileInfo = new FileInfo(SaveFileDialog.FileName);

                var file = Path.Combine(fileInfo.DirectoryName!,
                                        fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length),
                                        fileInfo.Name);

                var path = new FileInfo(file).DirectoryName!;

                Directory.CreateDirectory(path);

                File.WriteAllText(file, this.TextBox.Text);

                this.ShowInStatusBar("Successfully saved...");
            }
        }

        private void ShowInStatusBar(string status)
        {
            this.StatusLabel.Text = status;

            this.timer1.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();

            this.CleanStatusStrip();
        }

        private void CleanStatusStrip()
        {
            this.StatusLabel.Text = "";
        }
    }
}