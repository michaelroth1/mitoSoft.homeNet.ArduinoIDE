namespace mitoSoft.homeNet.ArduinoIDE
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            button1 = new Button();
            label1 = new Label();
            TextBox = new RichTextBox();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            timer1 = new System.Windows.Forms.Timer(components);
            button2 = new Button();
            SaveFileDialog = new SaveFileDialog();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(1353, 20);
            button1.Name = "button1";
            button1.Size = new Size(150, 46);
            button1.TabIndex = 5;
            button1.Text = "Clipboard";
            button1.UseVisualStyleBackColor = true;
            button1.Click += CopyButton_Clicked;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 20);
            label1.Name = "label1";
            label1.Size = new Size(167, 32);
            label1.TabIndex = 4;
            label1.Text = "Arduino Code:";
            // 
            // TextBox
            // 
            TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TextBox.Location = new Point(12, 72);
            TextBox.Name = "TextBox";
            TextBox.Size = new Size(1491, 680);
            TextBox.TabIndex = 3;
            TextBox.Text = "";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(32, 32);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 765);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1515, 42);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(237, 32);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // timer1
            // 
            timer1.Interval = 2500;
            timer1.Tick += Timer_Tick;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Location = new Point(1197, 20);
            button2.Name = "button2";
            button2.Size = new Size(150, 46);
            button2.TabIndex = 7;
            button2.Text = "Save";
            button2.UseVisualStyleBackColor = true;
            button2.Click += SaveButton_Clicked;
            // 
            // SaveFileDialog
            // 
            SaveFileDialog.Filter = "Arduino |*.ino";
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1515, 807);
            Controls.Add(button2);
            Controls.Add(statusStrip1);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(TextBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form2";
            Text = "Arduino IDE";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private RichTextBox TextBox;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Timer timer1;
        private Button button2;
        private SaveFileDialog SaveFileDialog;
    }
}