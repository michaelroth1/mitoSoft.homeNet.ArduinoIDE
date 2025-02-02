namespace mitoSoft.homeNet.ArduinoIDE
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public void ShowDialog(string text)
        {
            this.richTextBox1.Text = text;
            this.CleanStatusStrip();
            this.CopyToClipboard();
            base.ShowDialog();
        }

        private void CopyButton_Clicked(object sender, EventArgs e)
        {
            this.CopyToClipboard();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();

            this.CleanStatusStrip();
        }

        private void CopyToClipboard()
        {
            Clipboard.SetText(this.richTextBox1.Text);

            this.toolStripStatusLabel1.Text = "Successfully copied...";

            this.timer1.Start();
        }

        private void CleanStatusStrip()
        {
            this.toolStripStatusLabel1.Text = "";
        }
    }
}
