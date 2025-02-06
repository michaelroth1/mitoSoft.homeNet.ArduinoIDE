using mitoSoft.homeNet.ArduinoIDE.ProgramParser.Models;

namespace mitoSoft.homeNet.ArduinoIDE
{
    public partial class Form2 : Form
    {
        private HomeNetController _controller;

        public Form2(HomeNetController controller)
        {
            InitializeComponent();

            _controller = controller;

            if (Properties.Settings.Default.TextZoom > 0)
            {
                this.TextBox.ZoomFactor = Properties.Settings.Default.TextZoom;
            }
        }

        public void ShowDialog(string text)
        {
            this.TextBox.Text = text;
            this.CleanStatusStrip();
            this.CopyToClipboard();
            base.ShowDialog();
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
            var controllerName = _controller?.Name ?? "TestController";

            SaveFileDialog.InitialDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Arduino");

            SaveFileDialog.FileName = $"{controllerName}.ino";

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
            this.toolStripStatusLabel1.Text = status;

            this.timer1.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();

            this.CleanStatusStrip();
        }

        private void CleanStatusStrip()
        {
            this.toolStripStatusLabel1.Text = "";
        }
    }
}